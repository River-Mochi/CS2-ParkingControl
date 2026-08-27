// <copyright file="ParkingRelocationSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Gradually hands cars from newly banned curb lanes to vanilla parking relocation.

namespace ParkingControl
{
    using CS2Shared.RiverMochi;
    using Game;
    using Unity.Entities;

    /// <summary>
    /// Queues cars once when PC first bans their curb lane, then feeds small batches to vanilla.
    /// </summary>
    public sealed partial class ParkingRelocationSystem : GameSystemBase
    {
        // Tuning knobs: keep both PC work and vanilla parking searches spread out.
        private const int kUpdateInterval = 4096;
        private const int kLaneRequestsPerPass = 32;
        private const int kCarsPerPass = 64;
#if DEBUG
        private const int kDebugSampleLimit = 10;
#endif

        private Unity.Entities.EntityQuery m_RequestLaneQuery;
        private Unity.Collections.NativeQueue<Unity.Entities.Entity> m_PendingCars;
        private bool m_IsGame;
        private bool m_HadQueuedWork;
        private int m_TotalLaneRequests;
        private int m_TotalQueued;
        private int m_TotalSent;
        private int m_TotalSkipped;

#if DEBUG
        private readonly System.Collections.Generic.List<DebugRelocationSample> m_DebugSamples =
            new(kDebugSampleLimit);
        private int m_DebugSamplesCaptured;
#endif

#if DEBUG
        private readonly struct DebugRelocationSample
        {
            public DebugRelocationSample(
                Unity.Entities.Entity vehicle,
                Unity.Entities.Entity oldLane,
                Unity.Mathematics.float3 oldPosition,
                bool hadTransform)
            {
                Vehicle = vehicle;
                OldLane = oldLane;
                OldPosition = oldPosition;
                HadTransform = hadTransform;
            }

            public Unity.Entities.Entity Vehicle { get; }

            public Unity.Entities.Entity OldLane { get; }

            public Unity.Mathematics.float3 OldPosition { get; }

            public bool HadTransform { get; }
        }
#endif

        /// <inheritdoc/>
        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return kUpdateInterval;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_RequestLaneQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    ParkingRelocationRequest,
                    Game.Net.ParkingLane,
                    Game.Net.LaneObject>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .Build();

            m_PendingCars =
                new Unity.Collections.NativeQueue<Unity.Entities.Entity>(
                    Unity.Collections.Allocator.Persistent);
        }

        /// <inheritdoc/>
        protected override void OnGameLoadingComplete(
            Colossal.Serialization.Entities.Purpose purpose,
            GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            m_IsGame =
                mode == GameMode.Game &&
                (purpose == Colossal.Serialization.Entities.Purpose.NewGame ||
                    purpose == Colossal.Serialization.Entities.Purpose.LoadGame);

            if (m_PendingCars.IsCreated)
            {
                m_PendingCars.Clear();
            }

            ResetCounters();
#if DEBUG
            m_DebugSamples.Clear();
            m_DebugSamplesCaptured = 0;
#endif
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            if (m_PendingCars.IsCreated)
            {
                m_PendingCars.Dispose();
            }

            base.OnDestroy();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!m_IsGame)
            {
                return;
            }

#if DEBUG
            LogDebugRelocationSamples();
#endif

            // Local ECB keeps structural changes batched but plays them back now,
            // so vanilla FixParkingLocationSystem sees this pass later in Modification5.
            Unity.Entities.EntityCommandBuffer commandBuffer =
                new(Unity.Collections.Allocator.Temp);

            try
            {
                int lanesProcessed = CollectSomeLaneRequests(ref commandBuffer);
                int sent = SendNextBatchToVanilla(ref commandBuffer);

                if (lanesProcessed > 0 || sent > 0)
                {
                    commandBuffer.Playback(EntityManager);
                }

                if (sent > 0)
                {
                    ParkingStatusCache.MarkDirty();
                }

#if DEBUG
                if (lanesProcessed > 0 || sent > 0)
                {
                    LogUtils.Info(
                        $"{Mod.ModTag} Auto relocation pass: " +
                        $"lanes={lanesProcessed}, sent={sent}, " +
                        $"pendingCars={m_PendingCars.Count}, " +
                        $"pendingLanes={m_RequestLaneQuery.CalculateEntityCount()}.");
                }
#endif

                if (m_PendingCars.Count == 0 &&
                    m_RequestLaneQuery.IsEmptyIgnoreFilter)
                {
                    FinishLogIfNeeded();
                }
            }
            finally
            {
                commandBuffer.Dispose();
            }
        }

        private int CollectSomeLaneRequests(
            ref Unity.Entities.EntityCommandBuffer commandBuffer)
        {
            if (m_RequestLaneQuery.IsEmptyIgnoreFilter)
            {
                return 0;
            }

            Dependency.Complete();

            Unity.Entities.ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<Game.Net.ParkingLane>(true);

            Unity.Entities.ComponentLookup<StreetParkingState> stateLookup =
                SystemAPI.GetComponentLookup<StreetParkingState>(true);

            Unity.Entities.ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.ParkedCar>(true);

            Unity.Entities.ComponentLookup<Game.Vehicles.FixParkingLocation> fixParkingLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.FixParkingLocation>(true);

            Unity.Entities.BufferLookup<Game.Net.LaneObject> laneObjectLookup =
                SystemAPI.GetBufferLookup<Game.Net.LaneObject>(true);

            using Unity.Collections.NativeArray<Unity.Entities.Entity> requestLanes =
                m_RequestLaneQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            int laneLimit = requestLanes.Length < kLaneRequestsPerPass
                ? requestLanes.Length
                : kLaneRequestsPerPass;

            int added = 0;

            for (int i = 0; i < laneLimit; i++)
            {
                Unity.Entities.Entity lane = requestLanes[i];

                // Consume this one-shot request even if the road changed before our interval.
                QueueRemoveLaneRequest(ref commandBuffer, lane);

                if (!stateLookup.HasComponent(lane) ||
                    !parkingLaneLookup.TryGetComponent(
                        lane,
                        out Game.Net.ParkingLane parkingLane) ||
                    (parkingLane.m_Flags & Game.Net.ParkingLaneFlags.ParkingDisabled) == 0 ||
                    !laneObjectLookup.TryGetBuffer(
                        lane,
                        out Unity.Entities.DynamicBuffer<Game.Net.LaneObject> laneObjects))
                {
                    continue;
                }

                foreach (Game.Net.LaneObject laneObject in laneObjects)
                {
                    Unity.Entities.Entity vehicle = laneObject.m_LaneObject;

                    if (fixParkingLookup.HasComponent(vehicle) ||
                        !parkedCarLookup.TryGetComponent(
                            vehicle,
                            out Game.Vehicles.ParkedCar parkedCar) ||
                        parkedCar.m_Lane != lane)
                    {
                        continue;
                    }

                    m_PendingCars.Enqueue(vehicle);
                    added++;
                }
            }

            if (laneLimit > 0)
            {
                m_HadQueuedWork = true;
                m_TotalLaneRequests += laneLimit;
                m_TotalQueued += added;
            }

            return laneLimit;
        }

        private int SendNextBatchToVanilla(
            ref Unity.Entities.EntityCommandBuffer commandBuffer)
        {
            if (m_PendingCars.Count == 0)
            {
                return 0;
            }

            Unity.Entities.ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.ParkedCar>(true);

            Unity.Entities.ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<Game.Net.ParkingLane>(true);

            Unity.Entities.ComponentLookup<StreetParkingState> stateLookup =
                SystemAPI.GetComponentLookup<StreetParkingState>(true);

            Unity.Entities.ComponentLookup<Game.Vehicles.FixParkingLocation> fixParkingLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.FixParkingLocation>(true);

            Unity.Entities.ComponentLookup<Game.Common.Updated> updatedLookup =
                SystemAPI.GetComponentLookup<Game.Common.Updated>(true);

#if DEBUG
            Unity.Entities.ComponentLookup<Game.Objects.Transform> transformLookup =
                SystemAPI.GetComponentLookup<Game.Objects.Transform>(true);
#endif

            int sent = 0;
            int checkedCount = 0;

            while (checkedCount < kCarsPerPass &&
                m_PendingCars.TryDequeue(out Unity.Entities.Entity vehicle))
            {
                checkedCount++;

                // The owner may have already used the car while it waited in our queue.
                if (!SystemAPI.Exists(vehicle) ||
                    fixParkingLookup.HasComponent(vehicle) ||
                    !parkedCarLookup.TryGetComponent(
                        vehicle,
                        out Game.Vehicles.ParkedCar parkedCar))
                {
                    m_TotalSkipped++;
                    continue;
                }

                Unity.Entities.Entity oldLane = parkedCar.m_Lane;
                if (oldLane == Unity.Entities.Entity.Null ||
                    !SystemAPI.Exists(oldLane) ||
                    !stateLookup.HasComponent(oldLane) ||
                    !parkingLaneLookup.TryGetComponent(
                        oldLane,
                        out Game.Net.ParkingLane parkingLane) ||
                    (parkingLane.m_Flags & Game.Net.ParkingLaneFlags.ParkingDisabled) == 0)
                {
                    // The ban was removed, the road changed, or the car moved elsewhere.
                    m_TotalSkipped++;
                    continue;
                }

#if DEBUG
                CaptureDebugRelocationSample(
                    vehicle,
                    oldLane,
                    ref transformLookup);
#endif

                QueueVanillaRelocation(
                    ref commandBuffer,
                    vehicle,
                    oldLane,
                    parkedCar,
                    !updatedLookup.HasComponent(vehicle));

                sent++;
            }

            m_TotalSent += sent;
            return sent;
        }

        // Keep ECB calls outside SystemAPI-generated methods. This avoids source-generator
        // overload rewriting while still using ECB for the structural changes.
        private static void QueueRemoveLaneRequest(
            ref Unity.Entities.EntityCommandBuffer commandBuffer,
            Unity.Entities.Entity lane)
        {
            commandBuffer.RemoveComponent<ParkingRelocationRequest>(lane);
        }

        private static void QueueVanillaRelocation(
            ref Unity.Entities.EntityCommandBuffer commandBuffer,
            Unity.Entities.Entity vehicle,
            Unity.Entities.Entity oldLane,
            Game.Vehicles.ParkedCar parkedCar,
            bool addUpdated)
        {
            // Vanilla normally allows the current lane to be reconsidered even when disabled.
            // Null the current assignment so its search must choose a new valid lane instead.
            // m_ChangeLane tells vanilla which old LaneObject entry to remove.
            parkedCar.m_Lane = Unity.Entities.Entity.Null;

            commandBuffer.SetComponent<Game.Vehicles.ParkedCar>(
                vehicle,
                parkedCar);

            commandBuffer.AddComponent<Game.Vehicles.FixParkingLocation>(
                vehicle,
                new Game.Vehicles.FixParkingLocation(
                    oldLane,
                    vehicle));

            // FixParkingLocationSystem only queries repair entities that are also Updated.
            if (addUpdated)
            {
                commandBuffer.AddComponent<Game.Common.Updated>(vehicle);
            }
        }

#if DEBUG
        private void CaptureDebugRelocationSample(
            Unity.Entities.Entity vehicle,
            Unity.Entities.Entity oldLane,
            ref Unity.Entities.ComponentLookup<Game.Objects.Transform> transformLookup)
        {
            if (m_DebugSamplesCaptured >= kDebugSampleLimit)
            {
                return;
            }

            bool hadTransform = transformLookup.TryGetComponent(
                vehicle,
                out Game.Objects.Transform transform);

            m_DebugSamples.Add(
                new DebugRelocationSample(
                    vehicle,
                    oldLane,
                    hadTransform ? transform.m_Position : default,
                    hadTransform));

            m_DebugSamplesCaptured++;
        }

        private void LogDebugRelocationSamples()
        {
            if (m_DebugSamples.Count == 0)
            {
                return;
            }

            Unity.Entities.ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.ParkedCar>(true);

            Unity.Entities.ComponentLookup<Game.Objects.Unspawned> unspawnedLookup =
                SystemAPI.GetComponentLookup<Game.Objects.Unspawned>(true);

            Unity.Entities.ComponentLookup<Game.Vehicles.FixParkingLocation> fixParkingLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.FixParkingLocation>(true);

            Unity.Entities.ComponentLookup<Game.Objects.Transform> transformLookup =
                SystemAPI.GetComponentLookup<Game.Objects.Transform>(true);

            LogUtils.Info(
                $"{Mod.ModTag} Vanilla relocation sample results " +
                $"(checked one PC interval after handoff):");

            foreach (DebugRelocationSample sample in m_DebugSamples)
            {
                if (!SystemAPI.Exists(sample.Vehicle))
                {
                    LogUtils.Info(
                        $"{Mod.ModTag}   Entity={FormatEntity(sample.Vehicle)} | " +
                        $"OldLane={FormatEntity(sample.OldLane)} | NoLongerExists=YES");
                    continue;
                }

                bool hasParkedCar = parkedCarLookup.TryGetComponent(
                    sample.Vehicle,
                    out Game.Vehicles.ParkedCar parkedCar);

                bool hasTransform = transformLookup.TryGetComponent(
                    sample.Vehicle,
                    out Game.Objects.Transform transform);

                string parkedLane = hasParkedCar
                    ? FormatEntity(parkedCar.m_Lane)
                    : "<no ParkedCar>";

                string oldPosition = sample.HadTransform
                    ? FormatPosition(sample.OldPosition)
                    : "<none>";

                string position = hasTransform
                    ? FormatPosition(transform.m_Position)
                    : "<none>";

                LogUtils.Info(
                    $"{Mod.ModTag}   Entity={FormatEntity(sample.Vehicle)} | " +
                    $"OldLane={FormatEntity(sample.OldLane)} | " +
                    $"ParkedLane={parkedLane} | " +
                    $"Unspawned={(unspawnedLookup.HasComponent(sample.Vehicle) ? "YES" : "NO")} | " +
                    $"FixPending={(fixParkingLookup.HasComponent(sample.Vehicle) ? "YES" : "NO")} | " +
                    $"OldPos={oldPosition} | Pos={position}");
            }

            m_DebugSamples.Clear();
        }

        private static string FormatEntity(Unity.Entities.Entity entity)
        {
            return entity == Unity.Entities.Entity.Null
                ? "Null"
                : $"{entity.Index}:{entity.Version}";
        }

        private static string FormatPosition(Unity.Mathematics.float3 position)
        {
            return $"({position.x:0.##}, {position.y:0.##}, {position.z:0.##})";
        }
#endif

        private void FinishLogIfNeeded()
        {
            if (!m_HadQueuedWork)
            {
                ResetCounters();
                return;
            }

            LogUtils.Info(
                $"{Mod.ModTag} Auto relocation finished: " +
                $"{m_TotalSent}/{m_TotalQueued} queued car(s) sent to vanilla relocation; " +
                $"{m_TotalSkipped} moved already or no longer eligible; " +
                $"{m_TotalLaneRequests} curb lane request(s) processed.");

            ResetCounters();
        }

        private void ResetCounters()
        {
            m_HadQueuedWork = false;
            m_TotalLaneRequests = 0;
            m_TotalQueued = 0;
            m_TotalSent = 0;
            m_TotalSkipped = 0;
#if DEBUG
            m_DebugSamplesCaptured = 0;
#endif
        }
    }
}

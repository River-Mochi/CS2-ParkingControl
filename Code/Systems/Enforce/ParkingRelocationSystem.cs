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
    using Unity.Collections;
    using Unity.Entities;

    internal struct AutomaticRelocationReport
    {
        public bool HasRun;
        public uint FrameInterval;
        public int LaneRequestsPerPass;
        public int CarsPerPass;
        public bool IsActive;
        public int CyclesStarted;
        public int Passes;
        public int LaneRequestsProcessed;
        public int LaneRequestsPending;
        public int CarsQueued;
        public int CarsSentToVanilla;
        public int CarsSkipped;
        public int CarsPending;
        public uint StartFrame;
        public uint EndFrame;
        public uint ElapsedSimulationFrames;
        public double ElapsedWallSeconds;
        public double MaxPCPassMilliseconds;
        public int VanillaFixParkingPendingAllSources;
    }

    /// <summary>
    /// Queues cars once when PC first bans their curb lane, then feeds small batches to vanilla.
    /// </summary>
    public sealed partial class ParkingRelocationSystem : GameSystemBase
    {
        // Tuning knobs: keep both PC work and vanilla parking searches spread out.
        private const uint kRelocationFrameInterval = 128;
        private const int kLaneRequestsPerPass = 32;
        private const int kCarsPerPass = 64;
#if DEBUG
        private const int kDebugSampleLimit = 10;
#endif

        private Unity.Entities.EntityQuery m_RequestLaneQuery;
        private Unity.Entities.EntityQuery m_FixParkingQuery;
        private Unity.Collections.NativeQueue<Unity.Entities.Entity> m_PendingCars;
        private Game.Simulation.SimulationSystem m_SimulationSystem = null!;
        private bool m_IsGame;
        private bool m_HadQueuedWork;
        private bool m_HasRelocationFrame;
        private bool m_RunActive;
        private bool m_HasCompletedRun;
        private uint m_LastRelocationFrame;
        private int m_CyclesStarted;
        private long m_RunStartTimestamp;
        private int m_TotalLaneRequests;
        private int m_TotalQueued;
        private int m_TotalSent;
        private int m_TotalSkipped;
        private RelocationRunStats m_CurrentRun;
        private RelocationRunStats m_LastCompletedRun;

        private struct RelocationRunStats
        {
            public int Passes;
            public int LaneRequestsProcessed;
            public int CarsQueued;
            public int CarsSentToVanilla;
            public int CarsSkipped;
            public uint StartFrame;
            public uint EndFrame;
            public uint ElapsedSimulationFrames;
            public double ElapsedWallSeconds;
            public double MaxPCPassMilliseconds;
        }

#if DEBUG
        private readonly System.Collections.Generic.List<DebugRelocationSample> m_DebugSamples =
            new System.Collections.Generic.List<DebugRelocationSample>(kDebugSampleLimit);
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
        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem =
                World.GetOrCreateSystemManaged<Game.Simulation.SimulationSystem>();

            m_RequestLaneQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    ParkingRelocationRequest,
                    Game.Net.ParkingLane,
                    Game.Net.LaneObject>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .Build();

            m_FixParkingQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Vehicles.FixParkingLocation>()
                .WithNone<Game.Tools.Temp>()
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

            m_HasRelocationFrame = false;
            m_LastRelocationFrame = 0u;
            ResetCounters();
            ResetRunHistory();
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
            if (IsVerboseLoggingEnabled())
            {
                LogDebugRelocationSamples();
            }
            else if (m_DebugSamples.Count > 0)
            {
                m_DebugSamples.Clear();
                m_DebugSamplesCaptured = 0;
            }
#endif

            uint frameIndex = m_SimulationSystem.frameIndex;

            bool hasWork =
                m_PendingCars.Count > 0 ||
                !m_RequestLaneQuery.IsEmptyIgnoreFilter;

            if (!hasWork)
            {
                m_HasRelocationFrame = false;
                FinishRunIfNeeded(frameIndex);
                FinishLogIfNeeded();
                return;
            }

            if (!m_RunActive)
            {
                BeginRun(frameIndex);
            }

            // Modification5 does not honor GameSystemBase.GetUpdateInterval().
            // Gate on simulation frames so large bans drain gradually on every PC.
            if (m_HasRelocationFrame &&
                unchecked(frameIndex - m_LastRelocationFrame) < kRelocationFrameInterval)
            {
                return;
            }

            m_HasRelocationFrame = true;
            m_LastRelocationFrame = frameIndex;

            // Local ECB keeps structural changes batched but plays them back now,
            // so vanilla FixParkingLocationSystem sees this pass later in Modification5.
            long passStartTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
            Unity.Entities.EntityCommandBuffer commandBuffer =
                new(Unity.Collections.Allocator.Temp);

            m_CurrentRun.Passes++;

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
                if (IsVerboseLoggingEnabled() &&
                    (lanesProcessed > 0 || sent > 0))
                {
                    LogUtils.Info(
                        $"{Mod.ModTag} Auto relocation pass: " +
                        $"frame={frameIndex}, lanes={lanesProcessed}, sent={sent}, " +
                        $"pendingCars={m_PendingCars.Count}, " +
                        $"pendingLanes={m_RequestLaneQuery.CalculateEntityCount()}.");
                }
#endif

                if (m_PendingCars.Count == 0 &&
                    m_RequestLaneQuery.IsEmptyIgnoreFilter)
                {
                    m_HasRelocationFrame = false;
                }
            }
            finally
            {
                commandBuffer.Dispose();
                RecordPassDuration(passStartTimestamp);
            }

            if (m_PendingCars.Count == 0 &&
                m_RequestLaneQuery.IsEmptyIgnoreFilter)
            {
                FinishRunIfNeeded(frameIndex);
                FinishLogIfNeeded();
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
                m_CurrentRun.LaneRequestsProcessed += laneLimit;
                m_CurrentRun.CarsQueued += added;
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
                    m_CurrentRun.CarsSkipped++;
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
                    m_CurrentRun.CarsSkipped++;
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
            m_CurrentRun.CarsSentToVanilla += sent;
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
        private static bool IsVerboseLoggingEnabled()
        {
            return Mod.Settings?.VerboseLog == true;
        }

        private void CaptureDebugRelocationSample(
            Unity.Entities.Entity vehicle,
            Unity.Entities.Entity oldLane,
            ref Unity.Entities.ComponentLookup<Game.Objects.Transform> transformLookup)
        {
            if (!IsVerboseLoggingEnabled() ||
                m_DebugSamplesCaptured >= kDebugSampleLimit)
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
                $"(checked after vanilla handoff):");

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

        internal AutomaticRelocationReport GetReport()
        {
            RelocationRunStats stats =
                m_RunActive ? m_CurrentRun : m_LastCompletedRun;

            uint endFrame = m_RunActive
                ? m_SimulationSystem.frameIndex
                : stats.EndFrame;

            uint elapsedSimulationFrames = m_RunActive
                ? unchecked(endFrame - stats.StartFrame)
                : stats.ElapsedSimulationFrames;

            double elapsedWallSeconds = m_RunActive
                ? GetElapsedWallSeconds(m_RunStartTimestamp)
                : stats.ElapsedWallSeconds;

            return new AutomaticRelocationReport
            {
                HasRun = m_RunActive || m_HasCompletedRun,
                FrameInterval = kRelocationFrameInterval,
                LaneRequestsPerPass = kLaneRequestsPerPass,
                CarsPerPass = kCarsPerPass,
                IsActive = m_RunActive,
                CyclesStarted = m_CyclesStarted,
                Passes = stats.Passes,
                LaneRequestsProcessed = stats.LaneRequestsProcessed,
                LaneRequestsPending = m_RequestLaneQuery.CalculateEntityCount(),
                CarsQueued = stats.CarsQueued,
                CarsSentToVanilla = stats.CarsSentToVanilla,
                CarsSkipped = stats.CarsSkipped,
                CarsPending = m_PendingCars.Count,
                StartFrame = stats.StartFrame,
                EndFrame = endFrame,
                ElapsedSimulationFrames = elapsedSimulationFrames,
                ElapsedWallSeconds = elapsedWallSeconds,
                MaxPCPassMilliseconds = stats.MaxPCPassMilliseconds,
                VanillaFixParkingPendingAllSources = m_FixParkingQuery.CalculateEntityCount(),
            };
        }

        private void BeginRun(uint frameIndex)
        {
            m_RunActive = true;
            m_CyclesStarted++;
            m_RunStartTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
            m_CurrentRun = new RelocationRunStats
            {
                StartFrame = frameIndex,
                EndFrame = frameIndex,
            };
        }

        private void FinishRunIfNeeded(uint frameIndex)
        {
            if (!m_RunActive)
            {
                return;
            }

            m_CurrentRun.EndFrame = frameIndex;
            m_CurrentRun.ElapsedSimulationFrames =
                unchecked(frameIndex - m_CurrentRun.StartFrame);
            m_CurrentRun.ElapsedWallSeconds =
                GetElapsedWallSeconds(m_RunStartTimestamp);
            m_LastCompletedRun = m_CurrentRun;
            m_HasCompletedRun = true;
            m_RunActive = false;
            m_RunStartTimestamp = 0L;
        }

        private void RecordPassDuration(long passStartTimestamp)
        {
            double passMilliseconds =
                (System.Diagnostics.Stopwatch.GetTimestamp() - passStartTimestamp) *
                1000d /
                System.Diagnostics.Stopwatch.Frequency;

            if (passMilliseconds > m_CurrentRun.MaxPCPassMilliseconds)
            {
                m_CurrentRun.MaxPCPassMilliseconds = passMilliseconds;
            }
        }

        private void ResetRunHistory()
        {
            m_RunActive = false;
            m_HasCompletedRun = false;
            m_CyclesStarted = 0;
            m_RunStartTimestamp = 0L;
            m_CurrentRun = default;
            m_LastCompletedRun = default;
        }

        private static double GetElapsedWallSeconds(long startTimestamp)
        {
            return (System.Diagnostics.Stopwatch.GetTimestamp() - startTimestamp) /
                (double)System.Diagnostics.Stopwatch.Frequency;
        }
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

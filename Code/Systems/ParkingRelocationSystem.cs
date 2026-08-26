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
    using Unity.Burst;
    using Unity.Burst.Intrinsics;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// Queues cars once when PC first bans their curb lane, then feeds small batches to vanilla.
    /// </summary>
    public sealed partial class ParkingRelocationSystem : GameSystemBase
    {
        // Roughly a few seconds between passes in normal play. Small batches avoid a relocation spike.
        private const int kUpdateInterval = 512;
        private const int kCarsPerPass = 64;

        private EntityQuery m_RequestLaneQuery;
        private NativeQueue<Entity> m_PendingCars;
        private bool m_IsGame;
        private bool m_HadQueuedWork;
        private int m_TotalSent;
        private int m_TotalSkipped;

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

            m_PendingCars = new Unity.Collections.NativeQueue<Unity.Entities.Entity>(
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

            m_HadQueuedWork = false;
            m_TotalSent = 0;
            m_TotalSkipped = 0;
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

            CollectNewLaneRequests();

            if (m_PendingCars.Count == 0)
            {
                FinishLogIfNeeded();
                return;
            }

            int sent = SendNextBatchToVanilla();
            if (sent > 0)
            {
                ParkingStatusCache.MarkDirty();
            }

            if (m_PendingCars.Count == 0)
            {
                FinishLogIfNeeded();
            }
        }

        private void CollectNewLaneRequests()
        {
            if (m_RequestLaneQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            Dependency.Complete();

            int queueBefore = m_PendingCars.Count;

            CollectCarsJob collectJob = new()
            {
                m_EntityType = SystemAPI.GetEntityTypeHandle(),
                m_LaneObjectType = SystemAPI.GetBufferTypeHandle<Game.Net.LaneObject>(true),
                m_ParkedCarLookup = SystemAPI.GetComponentLookup<Game.Vehicles.ParkedCar>(true),
                m_ParkingLaneLookup = SystemAPI.GetComponentLookup<Game.Net.ParkingLane>(true),
                m_StateLookup = SystemAPI.GetComponentLookup<StreetParkingState>(true),
                m_FixParkingLookup = SystemAPI.GetComponentLookup<Game.Vehicles.FixParkingLocation>(true),
                m_Queue = m_PendingCars.AsParallelWriter(),
            };

            Dependency = collectJob.ScheduleParallel(m_RequestLaneQuery, Dependency);
            Dependency.Complete();

            using Unity.Collections.NativeArray<Unity.Entities.Entity> requestLanes =
                m_RequestLaneQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            int laneCount = requestLanes.Length;
            if (laneCount > 0)
            {
                // Consume each lane request once. The car queue survives across later intervals.
                EntityManager.RemoveComponent<ParkingRelocationRequest>(requestLanes);
            }

            int added = m_PendingCars.Count - queueBefore;
            if (added <= 0)
            {
                return;
            }

            m_HadQueuedWork = true;
            LogUtils.Info(
                $"{Mod.ModTag} Auto relocation queued: {added} parked car(s) from " +
                $"{laneCount} newly banned curb lane(s). Max {kCarsPerPass} every " +
                $"{kUpdateInterval} simulation frames.");
        }

        private int SendNextBatchToVanilla()
        {
            Unity.Entities.ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.ParkedCar>(true);

            Unity.Entities.ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<Game.Net.ParkingLane>(true);

            Unity.Entities.ComponentLookup<StreetParkingState> stateLookup =
                SystemAPI.GetComponentLookup<StreetParkingState>(true);

            Unity.Entities.ComponentLookup<Game.Vehicles.FixParkingLocation> fixParkingLookup =
                SystemAPI.GetComponentLookup<Game.Vehicles.FixParkingLocation>(true);

            using Unity.Entities.EntityCommandBuffer commandBuffer =
                new(Unity.Collections.Allocator.Temp);

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

                Unity.Entities.Entity lane = parkedCar.m_Lane;
                if (lane == Unity.Entities.Entity.Null ||
                    !SystemAPI.Exists(lane) ||
                    !stateLookup.HasComponent(lane) ||
                    !parkingLaneLookup.TryGetComponent(
                        lane,
                        out Game.Net.ParkingLane parkingLane) ||
                    (parkingLane.m_Flags & Game.Net.ParkingLaneFlags.ParkingDisabled) == 0)
                {
                    // The ban was removed, the road was rebuilt, or the car already moved elsewhere.
                    m_TotalSkipped++;
                    continue;
                }

                // This is the same vanilla repair component used when game edits invalidate
                // a parked car location. resetLocation=vehicle keeps ownership/home state intact.
                commandBuffer.AddComponent(
                    vehicle,
                    new Game.Vehicles.FixParkingLocation(
                        Unity.Entities.Entity.Null,
                        vehicle));

                sent++;
            }

            if (sent > 0)
            {
                // Playback now so vanilla FixParkingLocationSystem sees these cars later this phase.
                commandBuffer.Playback(EntityManager);
                m_TotalSent += sent;
            }

            return sent;
        }

        private void FinishLogIfNeeded()
        {
            if (!m_HadQueuedWork)
            {
                return;
            }

            LogUtils.Info(
                $"{Mod.ModTag} Auto relocation finished: {m_TotalSent} car(s) sent to " +
                $"vanilla relocation; {m_TotalSkipped} already moved or no longer eligible.");

            m_HadQueuedWork = false;
            m_TotalSent = 0;
            m_TotalSkipped = 0;
        }

        [BurstCompile]
        private struct CollectCarsJob : IJobChunk
        {
            [ReadOnly]
            public EntityTypeHandle m_EntityType;

            [ReadOnly]
            public BufferTypeHandle<Game.Net.LaneObject> m_LaneObjectType;

            [ReadOnly]
            public ComponentLookup<Game.Vehicles.ParkedCar> m_ParkedCarLookup;

            [ReadOnly]
            public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneLookup;

            [ReadOnly]
            public ComponentLookup<StreetParkingState> m_StateLookup;

            [ReadOnly]
            public ComponentLookup<Game.Vehicles.FixParkingLocation> m_FixParkingLookup;

            public NativeQueue<Entity>.ParallelWriter m_Queue;

            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                NativeArray<Entity> entities = chunk.GetNativeArray(m_EntityType);
                BufferAccessor<Game.Net.LaneObject> laneObjects =
                    chunk.GetBufferAccessor(ref m_LaneObjectType);

                for (int i = 0; i < entities.Length; i++)
                {
                    Entity lane = entities[i];

                    if (!m_StateLookup.HasComponent(lane))
                    {
                        continue;
                    }

                    Game.Net.ParkingLane parkingLane = m_ParkingLaneLookup[lane];
                    if ((parkingLane.m_Flags & Game.Net.ParkingLaneFlags.ParkingDisabled) == 0)
                    {
                        continue;
                    }

                    DynamicBuffer<Game.Net.LaneObject> objects = laneObjects[i];
                    foreach (Game.Net.LaneObject laneObject in objects)
                    {
                        Entity vehicle = laneObject.m_LaneObject;

                        if (m_FixParkingLookup.HasComponent(vehicle) ||
                            !m_ParkedCarLookup.TryGetComponent(
                                vehicle,
                                out Game.Vehicles.ParkedCar parkedCar) ||
                            parkedCar.m_Lane != lane)
                        {
                            continue;
                        }

                        m_Queue.Enqueue(vehicle);
                    }
                }
            }
        }
    }
}

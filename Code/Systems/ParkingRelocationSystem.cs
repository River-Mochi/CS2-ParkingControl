// <copyright file="ParkingRelocationSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: One-time request that lets vanilla relocate cars from currently banned curb lanes.

namespace ParkingControl
{
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Net;
    using Game.Tools;
    using Game.Vehicles;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// Marks occupied banned curb lanes Updated so vanilla FixParkingLocationSystem moves their cars.
    /// </summary>
    public sealed partial class ParkingRelocationSystem : GameSystemBase
    {
        private static bool s_RequestPending;

        private EntityQuery m_ParkingLanesQuery;
        private bool m_IsGame;
        private bool m_RelocateNextPass;

        /// <summary>
        /// Queues one relocation pass after Parking Control enforcement has refreshed.
        /// </summary>
        internal static void RequestRelocation()
        {
            s_RequestPending = true;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_ParkingLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<ParkingLane, Owner, Game.Prefabs.PrefabRef>()
                .WithNone<Deleted, Temp>()
                .Build();
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

            s_RequestPending = false;
            m_RelocateNextPass = false;
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!m_IsGame)
            {
                return;
            }

            if (s_RequestPending)
            {
                s_RequestPending = false;
                m_RelocateNextPass = true;

                // First let PC refresh ParkingDisabled and the path graph.
                NoStreetParkingSystem.RequestReconcile();
                return;
            }

            if (!m_RelocateNextPass)
            {
                return;
            }

            m_RelocateNextPass = false;
            Dependency.Complete();

            PCSettings.ParkingScope scope =
                Mod.Settings?.Scope ?? PCSettings.ParkingScope.Off;

            Entity policyEntity = ParkingPolicySystem.PolicyEntity;

            ComponentLookup<ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<ParkingLane>(true);

            ComponentLookup<Owner> ownerLookup =
                SystemAPI.GetComponentLookup<Owner>(true);

            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup =
                SystemAPI.GetComponentLookup<Game.Prefabs.PrefabRef>(true);

            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup =
                SystemAPI.GetComponentLookup<Game.Prefabs.ParkingLaneData>(true);

            ComponentLookup<Road> roadLookup =
                SystemAPI.GetComponentLookup<Road>(true);

            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup =
                SystemAPI.GetComponentLookup<Game.Areas.BorderDistrict>(true);

            ComponentLookup<ManualRoadParkingBan> manualBanLookup =
                SystemAPI.GetComponentLookup<ManualRoadParkingBan>(true);

            BufferLookup<Game.Policies.Policy> policyLookup =
                SystemAPI.GetBufferLookup<Game.Policies.Policy>(true);

            BufferLookup<LaneObject> laneObjectLookup =
                SystemAPI.GetBufferLookup<LaneObject>(true);

            ComponentLookup<ParkedCar> parkedCarLookup =
                SystemAPI.GetComponentLookup<ParkedCar>(true);

            ComponentLookup<Updated> updatedLookup =
                SystemAPI.GetComponentLookup<Updated>(true);

            int occupiedTargetLanes = 0;
            int parkedCars = 0;

            using NativeList<Entity> lanesToUpdate =
                new(Allocator.Temp);

            using NativeArray<Entity> parkingLanes =
                m_ParkingLanesQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity lane in parkingLanes)
            {
                ParkingLane parkingLane = parkingLaneLookup[lane];

                if (!NoStreetParkingSystem.IsStreetCarParkingLane(
                        lane,
                        parkingLane,
                        ownerLookup,
                        prefabRefLookup,
                        parkingLaneDataLookup,
                        roadLookup))
                {
                    continue;
                }

                if (!NoStreetParkingSystem.IsRestrictionTarget(
                        lane,
                        parkingLane,
                        scope,
                        policyEntity,
                        ownerLookup,
                        borderDistrictLookup,
                        manualBanLookup,
                        policyLookup))
                {
                    continue;
                }

                if (!laneObjectLookup.TryGetBuffer(
                        lane,
                        out DynamicBuffer<LaneObject> laneObjects))
                {
                    continue;
                }

                int carsOnLane = 0;

                foreach (LaneObject laneObject in laneObjects)
                {
                    if (parkedCarLookup.HasComponent(laneObject.m_LaneObject))
                    {
                        carsOnLane++;
                    }
                }

                if (carsOnLane == 0)
                {
                    continue;
                }

                occupiedTargetLanes++;
                parkedCars += carsOnLane;

                if (!updatedLookup.HasComponent(lane))
                {
                    lanesToUpdate.Add(lane);
                }
            }

            if (lanesToUpdate.Length > 0)
            {
                // Vanilla FixParkingLocationSystem runs later this phase and
                // relocates parked cars from Updated parking lanes.
                EntityManager.AddComponent<Updated>(lanesToUpdate.AsArray());

                // Parking-lane recalculation can clear our flag later; reapply it
                // before LanesModified rebuilds the path data.
                NoStreetParkingSystem.RequestReconcile();
            }

            ParkingStatusCache.MarkDirty();

            LogUtils.Info(
                $"{Mod.ModTag} Relocate parked cars: " +
                $"{parkedCars} car(s) on {occupiedTargetLanes} banned curb lane(s); " +
                $"{lanesToUpdate.Length} lane(s) queued for vanilla relocation.");
        }
    }
}

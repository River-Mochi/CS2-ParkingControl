// <copyright file="NoStreetParkingSystem.Reconcile.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Applies Parking Control restrictions to parking lanes and path data.

namespace ParkingControl
{
    using Game.Common;
    using Game.Net;
    using Unity.Collections;
    using Unity.Entities;

    public sealed partial class NoStreetParkingSystem
    {
        private ReconcileResult ReconcileStreetParking(
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            bool fullReconcile)
        {
            ComponentLookup<ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<ParkingLane>();

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

            ComponentLookup<StreetParkingState> stateLookup =
                SystemAPI.GetComponentLookup<StreetParkingState>(true);

            ComponentLookup<Created> createdLookup =
                SystemAPI.GetComponentLookup<Created>(true);

            ComponentLookup<Updated> updatedLookup =
                SystemAPI.GetComponentLookup<Updated>(true);

            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup =
                SystemAPI.GetComponentLookup<PathfindUpdated>(true);

            BufferLookup<Game.Policies.Policy> policyLookup =
                SystemAPI.GetBufferLookup<Game.Policies.Policy>(true);

            NativeList<Entity> addStateEntities = new(Allocator.Temp);
            NativeList<Entity> removeStateEntities = new(Allocator.Temp);
            NativeList<Entity> pathfindUpdateEntities = new(Allocator.Temp);

            ReconcileResult result = default;

            EntityQuery sourceQuery =
                fullReconcile
                    ? m_AllParkingLanesQuery
                    : m_ChangedParkingLanesQuery;

            using (NativeArray<Entity> parkingLaneEntities =
                sourceQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity entity in parkingLaneEntities)
                {
                    ReconcileLane(
                        entity,
                        scope,
                        policyEntity,
                        ref parkingLaneLookup,
                        ownerLookup,
                        prefabRefLookup,
                        parkingLaneDataLookup,
                        roadLookup,
                        borderDistrictLookup,
                        manualBanLookup,
                        stateLookup,
                        createdLookup,
                        updatedLookup,
                        pathfindUpdatedLookup,
                        policyLookup,
                        ref addStateEntities,
                        ref removeStateEntities,
                        ref pathfindUpdateEntities,
                        ref result);
                }
            }

            ApplyPendingChanges(
                ref addStateEntities,
                ref removeStateEntities,
                ref pathfindUpdateEntities);

            return result;
        }

        private ReconcileResult ReconcileRoad(
            Entity road,
            PCSettings.ParkingScope scope,
            Entity policyEntity)
        {
            ReconcileResult result = default;

            if (road == Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasComponent<Road>(road) ||
                !EntityManager.HasBuffer<SubLane>(road))
            {
                return result;
            }

            ComponentLookup<ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<ParkingLane>();

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

            ComponentLookup<StreetParkingState> stateLookup =
                SystemAPI.GetComponentLookup<StreetParkingState>(true);

            ComponentLookup<Created> createdLookup =
                SystemAPI.GetComponentLookup<Created>(true);

            ComponentLookup<Updated> updatedLookup =
                SystemAPI.GetComponentLookup<Updated>(true);

            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup =
                SystemAPI.GetComponentLookup<PathfindUpdated>(true);

            BufferLookup<Game.Policies.Policy> policyLookup =
                SystemAPI.GetBufferLookup<Game.Policies.Policy>(true);

            NativeList<Entity> addStateEntities = new(Allocator.Temp);
            NativeList<Entity> removeStateEntities = new(Allocator.Temp);
            NativeList<Entity> pathfindUpdateEntities = new(Allocator.Temp);

            DynamicBuffer<SubLane> subLanes =
                EntityManager.GetBuffer<SubLane>(
                    road,
                    isReadOnly: true);

            foreach (SubLane subLane in subLanes)
            {
                Entity lane = subLane.m_SubLane;

                if (!parkingLaneLookup.HasComponent(lane))
                {
                    continue;
                }

                ReconcileLane(
                    lane,
                    scope,
                    policyEntity,
                    ref parkingLaneLookup,
                    ownerLookup,
                    prefabRefLookup,
                    parkingLaneDataLookup,
                    roadLookup,
                    borderDistrictLookup,
                    manualBanLookup,
                    stateLookup,
                    createdLookup,
                    updatedLookup,
                    pathfindUpdatedLookup,
                    policyLookup,
                    ref addStateEntities,
                    ref removeStateEntities,
                    ref pathfindUpdateEntities,
                    ref result);
            }

            ApplyPendingChanges(
                ref addStateEntities,
                ref removeStateEntities,
                ref pathfindUpdateEntities);

            return result;
        }

        private static void ReconcileLane(
            Entity entity,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            ref ComponentLookup<ParkingLane> parkingLaneLookup,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup,
            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup,
            ComponentLookup<Road> roadLookup,
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup,
            ComponentLookup<ManualRoadParkingBan> manualBanLookup,
            ComponentLookup<StreetParkingState> stateLookup,
            ComponentLookup<Created> createdLookup,
            ComponentLookup<Updated> updatedLookup,
            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup,
            BufferLookup<Game.Policies.Policy> policyLookup,
            ref NativeList<Entity> addStateEntities,
            ref NativeList<Entity> removeStateEntities,
            ref NativeList<Entity> pathfindUpdateEntities,
            ref ReconcileResult result)
        {
            if (!parkingLaneLookup.HasComponent(entity) ||
                !ownerLookup.HasComponent(entity) ||
                !prefabRefLookup.HasComponent(entity))
            {
                return;
            }

            ParkingLane parkingLane = parkingLaneLookup[entity];
            bool hasState = stateLookup.HasComponent(entity);

            bool isStreetParking =
                IsStreetCarParkingLane(
                    entity,
                    parkingLane,
                    ownerLookup,
                    prefabRefLookup,
                    parkingLaneDataLookup,
                    roadLookup);

            bool shouldRestrict =
                isStreetParking &&
                IsRestrictionTarget(
                    entity,
                    parkingLane,
                    scope,
                    policyEntity,
                    ownerLookup,
                    borderDistrictLookup,
                    manualBanLookup,
                    policyLookup);

            bool parkingDisabled =
                (parkingLane.m_Flags &
                    ParkingLaneFlags.ParkingDisabled) != 0;

            if (!shouldRestrict)
            {
                if (!hasState)
                {
                    return;
                }

                // We own this flag, so remove it when PC no longer targets the lane.
                if (parkingDisabled)
                {
                    parkingLane.m_Flags &=
                        ~ParkingLaneFlags.ParkingDisabled;

                    parkingLaneLookup[entity] = parkingLane;
                    result.m_Changed++;

                    QueuePathfindUpdate(
                        entity,
                        createdLookup,
                        updatedLookup,
                        pathfindUpdatedLookup,
                        ref pathfindUpdateEntities);
                }

                removeStateEntities.Add(entity);
                return;
            }

            if (parkingDisabled)
            {
                // Keep our ownership marker. Updated/PathfindUpdated can be ours too,
                // so they are not proof that vanilla owns this flag.
                return;
            }

            parkingLane.m_Flags |=
                ParkingLaneFlags.ParkingDisabled;

            parkingLaneLookup[entity] = parkingLane;
            result.m_Changed++;

            if (!hasState)
            {
                addStateEntities.Add(entity);
            }

            QueuePathfindUpdate(
                entity,
                createdLookup,
                updatedLookup,
                pathfindUpdatedLookup,
                ref pathfindUpdateEntities);
        }


        private void ApplyPendingChanges(
            ref NativeList<Entity> addStateEntities,
            ref NativeList<Entity> removeStateEntities,
            ref NativeList<Entity> pathfindUpdateEntities)
        {
            if (addStateEntities.Length > 0)
            {
                EntityManager.AddComponent<StreetParkingState>(
                    addStateEntities.AsArray());
            }

            if (removeStateEntities.Length > 0)
            {
                EntityManager.RemoveComponent<StreetParkingState>(
                    removeStateEntities.AsArray());
            }

            if (pathfindUpdateEntities.Length > 0)
            {
                EntityManager.AddComponent<PathfindUpdated>(
                    pathfindUpdateEntities.AsArray());
            }

            addStateEntities.Dispose();
            removeStateEntities.Dispose();
            pathfindUpdateEntities.Dispose();
        }

        private static void QueuePathfindUpdate(
            Entity entity,
            ComponentLookup<Created> createdLookup,
            ComponentLookup<Updated> updatedLookup,
            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup,
            ref NativeList<Entity> pathfindUpdateEntities)
        {
            if (!createdLookup.HasComponent(entity) &&
                !updatedLookup.HasComponent(entity) &&
                !pathfindUpdatedLookup.HasComponent(entity))
            {
                pathfindUpdateEntities.Add(entity);
            }
        }

        private struct ReconcileResult
        {
            public int m_Changed;
        }
    }
}

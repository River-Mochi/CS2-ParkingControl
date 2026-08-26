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
    using Unity.Entities;

    public sealed partial class NoStreetParkingSystem
    {
        private ReconcileResult ReconcileStreetParking(
            PCSettings.ParkingScope scope,
            Unity.Entities.Entity policyEntity,
            bool fullReconcile)
        {
            Unity.Entities.ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<Game.Net.ParkingLane>();

            Unity.Entities.ComponentLookup<Game.Common.Owner> ownerLookup =
                SystemAPI.GetComponentLookup<Game.Common.Owner>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup =
                SystemAPI.GetComponentLookup<Game.Prefabs.PrefabRef>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup =
                SystemAPI.GetComponentLookup<Game.Prefabs.ParkingLaneData>(true);

            Unity.Entities.ComponentLookup<Game.Net.Road> roadLookup =
                SystemAPI.GetComponentLookup<Game.Net.Road>(true);

            Unity.Entities.ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup =
                SystemAPI.GetComponentLookup<Game.Areas.BorderDistrict>(true);

            Unity.Entities.ComponentLookup<ManualRoadParkingBan> manualBanLookup =
                SystemAPI.GetComponentLookup<ManualRoadParkingBan>(true);

            Unity.Entities.ComponentLookup<StreetParkingState> stateLookup =
                SystemAPI.GetComponentLookup<StreetParkingState>(true);

            Unity.Entities.ComponentLookup<ParkingRelocationRequest> relocationRequestLookup =
                SystemAPI.GetComponentLookup<ParkingRelocationRequest>(true);

            Unity.Entities.ComponentLookup<Game.Common.Created> createdLookup =
                SystemAPI.GetComponentLookup<Game.Common.Created>(true);

            Unity.Entities.ComponentLookup<Game.Common.Updated> updatedLookup =
                SystemAPI.GetComponentLookup<Game.Common.Updated>(true);

            Unity.Entities.ComponentLookup<Game.Common.PathfindUpdated> pathfindUpdatedLookup =
                SystemAPI.GetComponentLookup<Game.Common.PathfindUpdated>(true);

            Unity.Entities.BufferLookup<Game.Policies.Policy> policyLookup =
                SystemAPI.GetBufferLookup<Game.Policies.Policy>(true);

            Unity.Collections.NativeList<Unity.Entities.Entity> addStateEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> relocationRequestEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> pathfindUpdateEntities =
                new(Unity.Collections.Allocator.Temp);

            ReconcileResult result = default;

            Unity.Entities.EntityQuery sourceQuery =
                fullReconcile
                    ? m_AllParkingLanesQuery
                    : m_ChangedParkingLanesQuery;

            using (Unity.Collections.NativeArray<Unity.Entities.Entity> parkingLaneEntities =
                sourceQuery.ToEntityArray(Unity.Collections.Allocator.Temp))
            {
                foreach (Unity.Entities.Entity entity in parkingLaneEntities)
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
                        relocationRequestLookup,
                        createdLookup,
                        updatedLookup,
                        pathfindUpdatedLookup,
                        policyLookup,
                        ref addStateEntities,
                        ref relocationRequestEntities,
                        ref removeStateEntities,
                        ref pathfindUpdateEntities,
                        ref result);
                }
            }

            ApplyPendingChanges(
                ref addStateEntities,
                ref relocationRequestEntities,
                ref removeStateEntities,
                ref pathfindUpdateEntities);

            return result;
        }

        private ReconcileResult ReconcileRoad(
            Unity.Entities.Entity road,
            PCSettings.ParkingScope scope,
            Unity.Entities.Entity policyEntity)
        {
            ReconcileResult result = default;

            if (road == Unity.Entities.Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasComponent<Game.Net.Road>(road) ||
                !EntityManager.HasBuffer<Game.Net.SubLane>(road))
            {
                return result;
            }

            Unity.Entities.ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<Game.Net.ParkingLane>();

            Unity.Entities.ComponentLookup<Game.Common.Owner> ownerLookup =
                SystemAPI.GetComponentLookup<Game.Common.Owner>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup =
                SystemAPI.GetComponentLookup<Game.Prefabs.PrefabRef>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup =
                SystemAPI.GetComponentLookup<Game.Prefabs.ParkingLaneData>(true);

            Unity.Entities.ComponentLookup<Game.Net.Road> roadLookup =
                SystemAPI.GetComponentLookup<Game.Net.Road>(true);

            Unity.Entities.ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup =
                SystemAPI.GetComponentLookup<Game.Areas.BorderDistrict>(true);

            Unity.Entities.ComponentLookup<ManualRoadParkingBan> manualBanLookup =
                SystemAPI.GetComponentLookup<ManualRoadParkingBan>(true);

            Unity.Entities.ComponentLookup<StreetParkingState> stateLookup =
                SystemAPI.GetComponentLookup<StreetParkingState>(true);

            Unity.Entities.ComponentLookup<ParkingRelocationRequest> relocationRequestLookup =
                SystemAPI.GetComponentLookup<ParkingRelocationRequest>(true);

            Unity.Entities.ComponentLookup<Game.Common.Created> createdLookup =
                SystemAPI.GetComponentLookup<Game.Common.Created>(true);

            Unity.Entities.ComponentLookup<Game.Common.Updated> updatedLookup =
                SystemAPI.GetComponentLookup<Game.Common.Updated>(true);

            Unity.Entities.ComponentLookup<Game.Common.PathfindUpdated> pathfindUpdatedLookup =
                SystemAPI.GetComponentLookup<Game.Common.PathfindUpdated>(true);

            Unity.Entities.BufferLookup<Game.Policies.Policy> policyLookup =
                SystemAPI.GetBufferLookup<Game.Policies.Policy>(true);

            Unity.Collections.NativeList<Unity.Entities.Entity> addStateEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> relocationRequestEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> pathfindUpdateEntities =
                new(Unity.Collections.Allocator.Temp);

            Unity.Entities.DynamicBuffer<Game.Net.SubLane> subLanes =
                EntityManager.GetBuffer<Game.Net.SubLane>(
                    road,
                    isReadOnly: true);

            foreach (Game.Net.SubLane subLane in subLanes)
            {
                Unity.Entities.Entity lane = subLane.m_SubLane;

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
                    relocationRequestLookup,
                    createdLookup,
                    updatedLookup,
                    pathfindUpdatedLookup,
                    policyLookup,
                    ref addStateEntities,
                    ref relocationRequestEntities,
                    ref removeStateEntities,
                    ref pathfindUpdateEntities,
                    ref result);
            }

            ApplyPendingChanges(
                ref addStateEntities,
                ref relocationRequestEntities,
                ref removeStateEntities,
                ref pathfindUpdateEntities);

            return result;
        }

        private static void ReconcileLane(
            Unity.Entities.Entity entity,
            PCSettings.ParkingScope scope,
            Unity.Entities.Entity policyEntity,
            ref Unity.Entities.ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup,
            Unity.Entities.ComponentLookup<Game.Common.Owner> ownerLookup,
            Unity.Entities.ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup,
            Unity.Entities.ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup,
            Unity.Entities.ComponentLookup<Game.Net.Road> roadLookup,
            Unity.Entities.ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup,
            Unity.Entities.ComponentLookup<ManualRoadParkingBan> manualBanLookup,
            Unity.Entities.ComponentLookup<StreetParkingState> stateLookup,
            Unity.Entities.ComponentLookup<ParkingRelocationRequest> relocationRequestLookup,
            Unity.Entities.ComponentLookup<Game.Common.Created> createdLookup,
            Unity.Entities.ComponentLookup<Game.Common.Updated> updatedLookup,
            Unity.Entities.ComponentLookup<Game.Common.PathfindUpdated> pathfindUpdatedLookup,
            Unity.Entities.BufferLookup<Game.Policies.Policy> policyLookup,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> addStateEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> relocationRequestEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> pathfindUpdateEntities,
            ref ReconcileResult result)
        {
            if (!parkingLaneLookup.HasComponent(entity) ||
                !ownerLookup.HasComponent(entity) ||
                !prefabRefLookup.HasComponent(entity))
            {
                return;
            }

            Game.Net.ParkingLane parkingLane = parkingLaneLookup[entity];
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
                    Game.Net.ParkingLaneFlags.ParkingDisabled) != 0;

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
                        ~Game.Net.ParkingLaneFlags.ParkingDisabled;

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
                Game.Net.ParkingLaneFlags.ParkingDisabled;

            parkingLaneLookup[entity] = parkingLane;
            result.m_Changed++;

            if (!hasState)
            {
                addStateEntities.Add(entity);

                // Queue only when PC first takes ownership of this restriction.
                // Compatibility re-applies must never relocate the same lane again.
                if (!relocationRequestLookup.HasComponent(entity))
                {
                    relocationRequestEntities.Add(entity);
                }
            }

            QueuePathfindUpdate(
                entity,
                createdLookup,
                updatedLookup,
                pathfindUpdatedLookup,
                ref pathfindUpdateEntities);
        }

        private void ApplyPendingChanges(
            ref Unity.Collections.NativeList<Unity.Entities.Entity> addStateEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> relocationRequestEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> pathfindUpdateEntities)
        {
            if (addStateEntities.Length > 0)
            {
                EntityManager.AddComponent<StreetParkingState>(
                    addStateEntities.AsArray());
            }

            if (relocationRequestEntities.Length > 0)
            {
                EntityManager.AddComponent<ParkingRelocationRequest>(
                    relocationRequestEntities.AsArray());
            }

            if (removeStateEntities.Length > 0)
            {
                EntityManager.RemoveComponent<StreetParkingState>(
                    removeStateEntities.AsArray());
            }

            if (pathfindUpdateEntities.Length > 0)
            {
                EntityManager.AddComponent<Game.Common.PathfindUpdated>(
                    pathfindUpdateEntities.AsArray());
            }

            addStateEntities.Dispose();
            relocationRequestEntities.Dispose();
            removeStateEntities.Dispose();
            pathfindUpdateEntities.Dispose();
        }

        private static void QueuePathfindUpdate(
            Unity.Entities.Entity entity,
            Unity.Entities.ComponentLookup<Game.Common.Created> createdLookup,
            Unity.Entities.ComponentLookup<Game.Common.Updated> updatedLookup,
            Unity.Entities.ComponentLookup<Game.Common.PathfindUpdated> pathfindUpdatedLookup,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> pathfindUpdateEntities)
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

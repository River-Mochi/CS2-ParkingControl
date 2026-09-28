// <copyright file="NoStreetParkingSystem.Reconcile.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Applies Parking Control restrictions to parking lanes and path data.

using Unity.Entities;

namespace ParkingControl
{

    public sealed partial class NoStreetParkingSystem
    {
        /// <summary>
        /// Builds the citywide road-size rule for one pass.
        /// </summary>
        /// <remarks>
        /// Kept in its own method so the SystemAPI source generator never has to
        /// relocate a method that carries nullable annotations, which would emit
        /// CS8669 from generated code that has no #nullable directive.
        /// </remarks>
        /// <returns>A rule that must be disposed when the pass ends.</returns>
        private RoadSizeRule CreateRoadSizeRule()
        {
            Unity.Entities.BufferLookup<Game.Prefabs.NetGeometrySection> geometrySections =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetGeometrySection>(true);

            Unity.Entities.BufferLookup<Game.Prefabs.NetSubSection> subSections =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetSubSection>(true);

            Unity.Entities.BufferLookup<Game.Prefabs.NetSectionPiece> sectionPieces =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetSectionPiece>(true);

            Unity.Entities.BufferLookup<Game.Prefabs.NetPieceLane> pieceLanes =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetPieceLane>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.NetLaneData> laneData =
                SystemAPI.GetComponentLookup<Game.Prefabs.NetLaneData>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.NetPieceData> pieceData =
                SystemAPI.GetComponentLookup<Game.Prefabs.NetPieceData>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.NetVertexMatchData> vertexMatchData =
                SystemAPI.GetComponentLookup<Game.Prefabs.NetVertexMatchData>(true);

            RoadSizeRule.DefaultCompositionLookups defaultComposition = new()
            {
                GeometrySections = geometrySections,
                SubSections = subSections,
                SectionPieces = sectionPieces,
                PieceLanes = pieceLanes,
                LaneData = laneData,
                PieceData = pieceData,
                VertexMatchData = vertexMatchData,
            };

            return RoadSizeRule.Create(
                Mod.Settings,
                SystemAPI.GetComponentLookup<Game.Net.Composition>(true),
                SystemAPI.GetBufferLookup<Game.Prefabs.NetCompositionLane>(true),
                SystemAPI.GetComponentLookup<Game.Prefabs.PrefabRef>(true),
                SystemAPI.GetComponentLookup<Game.Prefabs.ParkingLaneData>(true),
                defaultComposition,
                Unity.Collections.Allocator.Temp);
        }

        /// <summary>
        /// Reconciles a slice of parking lanes and applies the resulting changes.
        /// </summary>
        /// <param name="lanes">Lanes to consider.</param>
        /// <param name="startIndex">First lane in this slice.</param>
        /// <param name="count">How many lanes to take from <paramref name="startIndex"/>.</param>
        /// <param name="scope">Active scope.</param>
        /// <param name="policyEntity">District policy prefab entity.</param>
        /// <param name="roadSizeRule">Road-size rule for this pass.</param>
        /// <returns>How many lane flags changed.</returns>
        private ReconcileResult ReconcileLaneSlice(
            Unity.Collections.NativeArray<Unity.Entities.Entity> lanes,
            int startIndex,
            int count,
            PCSettings.ParkingScope scope,
            Unity.Entities.Entity policyEntity,
            RoadSizeRule roadSizeRule)
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

            Unity.Entities.ComponentLookup<ParkingRelocationCleanupRequest> cleanupRequestLookup =
                SystemAPI.GetComponentLookup<ParkingRelocationCleanupRequest>(true);

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
            Unity.Collections.NativeList<Unity.Entities.Entity> cleanupRequestEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> removeCleanupRequestEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> pathfindUpdateEntities =
                new(Unity.Collections.Allocator.Temp);

            ReconcileResult result = default;

            int endIndex = Unity.Mathematics.math.min(
                startIndex + count,
                lanes.Length);

            {
                for (int index = startIndex; index < endIndex; index++)
                {
                    ReconcileLane(
                        lanes[index],
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
                        cleanupRequestLookup,
                        createdLookup,
                        updatedLookup,
                        pathfindUpdatedLookup,
                        policyLookup,
                        roadSizeRule,
                        ref addStateEntities,
                        ref relocationRequestEntities,
                        ref cleanupRequestEntities,
                        ref removeStateEntities,
                        ref removeCleanupRequestEntities,
                        ref pathfindUpdateEntities,
                        ref result);
                }
            }

            ApplyPendingChanges(
                ref addStateEntities,
                ref relocationRequestEntities,
                ref cleanupRequestEntities,
                ref removeStateEntities,
                ref removeCleanupRequestEntities,
                ref pathfindUpdateEntities);

            return result;
        }

        /// <summary>
        /// Reconciles only the lanes the game marked as changed this pass.
        /// </summary>
        /// <remarks>
        /// Bounded by what the player just built or upgraded, so it is not batched.
        /// </remarks>
        /// <param name="scope">Active scope.</param>
        /// <param name="policyEntity">District policy prefab entity.</param>
        /// <param name="roadSizeRule">Road-size rule for this pass.</param>
        /// <returns>How many lane flags changed.</returns>
        private ReconcileResult ReconcileChangedLanes(
            PCSettings.ParkingScope scope,
            Unity.Entities.Entity policyEntity,
            RoadSizeRule roadSizeRule)
        {
            using Unity.Collections.NativeArray<Unity.Entities.Entity> lanes =
                m_ChangedParkingLanesQuery.ToEntityArray(
                    Unity.Collections.Allocator.Temp);

            return ReconcileLaneSlice(
                lanes,
                0,
                lanes.Length,
                scope,
                policyEntity,
                roadSizeRule);
        }

        /// <summary>
        /// Starts a citywide pass by snapshotting every parking lane.
        /// </summary>
        private void BeginFullReconcile()
        {
            m_FullReconcileLanes.Clear();
            m_FullReconcileIndex = 0;
            m_FullReconcileChanged = 0;

            using Unity.Collections.NativeArray<Unity.Entities.Entity> lanes =
                m_AllParkingLanesQuery.ToEntityArray(
                    Unity.Collections.Allocator.Temp);

            m_FullReconcileLanes.AddRange(lanes);
        }

        /// <summary>
        /// Works through the citywide snapshot a slice at a time.
        /// </summary>
        /// <remarks>
        /// Each lane that starts or stops being restricted gains or loses components,
        /// and every one of those is an archetype move. Doing thousands in one frame
        /// is what makes a citywide toggle stutter, so the pass is spread out. Lanes
        /// deleted part way through are skipped by the guards in ReconcileLane.
        /// </remarks>
        /// <param name="scope">Active scope.</param>
        /// <param name="policyEntity">District policy prefab entity.</param>
        /// <param name="roadSizeRule">Road-size rule for this pass.</param>
        /// <returns>True once the whole snapshot has been processed.</returns>
        private bool RunFullReconcileBatch(
            PCSettings.ParkingScope scope,
            Unity.Entities.Entity policyEntity,
            RoadSizeRule roadSizeRule)
        {
            ReconcileResult batch =
                ReconcileLaneSlice(
                    m_FullReconcileLanes.AsArray(),
                    m_FullReconcileIndex,
                    kFullReconcileBatchSize,
                    scope,
                    policyEntity,
                    roadSizeRule);

            m_FullReconcileChanged += batch.m_Changed;
            m_FullReconcileIndex += kFullReconcileBatchSize;

            if (m_FullReconcileIndex < m_FullReconcileLanes.Length)
            {
                return false;
            }

            m_FullReconcileLanes.Clear();
            m_FullReconcileIndex = 0;

            return true;
        }

        private ReconcileResult ReconcileRoad(
            Unity.Entities.Entity road,
            PCSettings.ParkingScope scope,
            Unity.Entities.Entity policyEntity,
            RoadSizeRule roadSizeRule)
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

            Unity.Entities.ComponentLookup<ParkingRelocationCleanupRequest> cleanupRequestLookup =
                SystemAPI.GetComponentLookup<ParkingRelocationCleanupRequest>(true);

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
            Unity.Collections.NativeList<Unity.Entities.Entity> cleanupRequestEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities =
                new(Unity.Collections.Allocator.Temp);
            Unity.Collections.NativeList<Unity.Entities.Entity> removeCleanupRequestEntities =
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
                    cleanupRequestLookup,
                    createdLookup,
                    updatedLookup,
                    pathfindUpdatedLookup,
                    policyLookup,
                    roadSizeRule,
                    ref addStateEntities,
                    ref relocationRequestEntities,
                    ref cleanupRequestEntities,
                    ref removeStateEntities,
                    ref removeCleanupRequestEntities,
                    ref pathfindUpdateEntities,
                    ref result);
            }

            ApplyPendingChanges(
                ref addStateEntities,
                ref relocationRequestEntities,
                ref cleanupRequestEntities,
                ref removeStateEntities,
                ref removeCleanupRequestEntities,
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
            Unity.Entities.ComponentLookup<ParkingRelocationCleanupRequest> cleanupRequestLookup,
            Unity.Entities.ComponentLookup<Game.Common.Created> createdLookup,
            Unity.Entities.ComponentLookup<Game.Common.Updated> updatedLookup,
            Unity.Entities.ComponentLookup<Game.Common.PathfindUpdated> pathfindUpdatedLookup,
            Unity.Entities.BufferLookup<Game.Policies.Policy> policyLookup,
            RoadSizeRule roadSizeRule,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> addStateEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> relocationRequestEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> cleanupRequestEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> removeCleanupRequestEntities,
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
                    policyLookup,
                    roadSizeRule);

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

                if (cleanupRequestLookup.HasComponent(entity))
                {
                    removeCleanupRequestEntities.Add(entity);
                }

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

                if (!cleanupRequestLookup.HasComponent(entity))
                {
                    cleanupRequestEntities.Add(entity);
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
            ref Unity.Collections.NativeList<Unity.Entities.Entity> cleanupRequestEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> removeStateEntities,
            ref Unity.Collections.NativeList<Unity.Entities.Entity> removeCleanupRequestEntities,
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

            if (cleanupRequestEntities.Length > 0)
            {
                EntityManager.AddComponent<ParkingRelocationCleanupRequest>(
                    cleanupRequestEntities.AsArray());
            }

            if (removeStateEntities.Length > 0)
            {
                EntityManager.RemoveComponent<StreetParkingState>(
                    removeStateEntities.AsArray());
            }

            if (removeCleanupRequestEntities.Length > 0)
            {
                EntityManager.RemoveComponent<ParkingRelocationCleanupRequest>(
                    removeCleanupRequestEntities.AsArray());
            }

            if (pathfindUpdateEntities.Length > 0)
            {
                EntityManager.AddComponent<Game.Common.PathfindUpdated>(
                    pathfindUpdateEntities.AsArray());
            }

            addStateEntities.Dispose();
            relocationRequestEntities.Dispose();
            cleanupRequestEntities.Dispose();
            removeStateEntities.Dispose();
            removeCleanupRequestEntities.Dispose();
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

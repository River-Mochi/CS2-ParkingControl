// <copyright file="NoStreetParkingSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Disables normal street-parking lanes citywide, by district,
// or on player-selected individual road sides.

namespace ParkingControl
{
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Unity.Collections;
    using Unity.Entities;
    using ParkingLane = Game.Net.ParkingLane;
    using ParkingLaneFlags = Game.Net.ParkingLaneFlags;
    using Road = Game.Net.Road;
    using RoadTypes = Game.Net.RoadTypes;
    using Temp = Game.Tools.Temp;

    /// <summary>
    /// Keeps ordinary street-parking lanes synchronized with all Parking Control restrictions.
    /// </summary>
    public sealed partial class NoStreetParkingSystem : GameSystemBase
    {
        private static bool s_ReconcileRequested = true;
        private static bool s_SaveRecoveryRequested;
        private static Entity s_RoadReconcileRequested;

        private EntityQuery m_AllParkingLanesQuery;
        private EntityQuery m_ChangedParkingLanesQuery;
        private EntityQuery m_ModifiedParkingLanesQuery;
        private EntityQuery m_ManualRoadBanQuery;
        private EntityQuery m_ChangedManualRoadBanQuery;
        private EntityQuery m_PolicyModifyQuery;

        private bool m_IsGame;
        private bool m_Initialized;
        private PCSettings.ParkingScope m_LastScope;

        /// <summary>
        /// Requests a full reconciliation during the next modification pass.
        /// </summary>
        public static void RequestReconcile()
        {
            s_ReconcileRequested = true;
        }

        /// <summary>
        /// Requests immediate reconciliation of one road after a manual tool change.
        /// </summary>
        internal static void RequestRoadReconcile(Entity road)
        {
            if (road != Entity.Null)
            {
                s_RoadReconcileRequested = road;
            }
        }

        /// <summary>
        /// Requests a fallback reconciliation in case post-save restoration cannot run.
        /// </summary>
        internal static void RequestSaveRecovery()
        {
            s_SaveRecoveryRequested = true;
        }

        /// <summary>
        /// Confirms that the post-save system restored the live runtime flags.
        /// </summary>
        internal static void CompleteSaveRestore()
        {
            s_SaveRecoveryRequested = false;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_AllParkingLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<ParkingLane, Owner, PrefabRef>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_ChangedParkingLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<ParkingLane, Owner, PrefabRef>()
                .WithAny<Created, Updated, PathfindUpdated>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_ModifiedParkingLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<ParkingLane, StreetParkingState>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_ManualRoadBanQuery = SystemAPI.QueryBuilder()
                .WithAll<ManualRoadParkingBan, Road>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_ChangedManualRoadBanQuery = SystemAPI.QueryBuilder()
                .WithAll<ManualRoadParkingBan, Road, Updated>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_PolicyModifyQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Common.Event, Game.Policies.Modify>()
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

            s_RoadReconcileRequested = Entity.Null;

            if (m_IsGame)
            {
                m_Initialized = false;
                RequestReconcile();
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!m_IsGame)
            {
                return;
            }

            PCSettings.ParkingScope scope =
                Mod.Settings?.Scope ?? PCSettings.ParkingScope.Off;

            Entity policyEntity = ParkingPolicySystem.PolicyEntity;

            bool policyChanged =
                scope == PCSettings.ParkingScope.ByDistrict &&
                HasPolicyChange(policyEntity);

            bool fullReconcile =
                s_ReconcileRequested ||
                s_SaveRecoveryRequested ||
                !m_Initialized ||
                scope != m_LastScope ||
                policyChanged;

            bool changedParkingLanes =
                !m_ChangedParkingLanesQuery.IsEmptyIgnoreFilter;

            bool changedManualRoads =
                !m_ChangedManualRoadBanQuery.IsEmptyIgnoreFilter;

            bool hasManualRoadBans =
                !m_ManualRoadBanQuery.IsEmptyIgnoreFilter;

            Entity requestedRoad = s_RoadReconcileRequested;

            if (!fullReconcile &&
                requestedRoad == Entity.Null &&
                !changedManualRoads &&
                (!changedParkingLanes ||
                    (scope == PCSettings.ParkingScope.Off &&
                        !hasManualRoadBans)))
            {
                return;
            }

            Dependency.Complete();

            ReconcileResult result = default;
            int prunedManualSides = 0;

            if (fullReconcile)
            {
                prunedManualSides =
                    PruneInvalidManualBans(m_ManualRoadBanQuery);

                ReconcileResult fullResult =
                    ReconcileStreetParking(
                        scope,
                        policyEntity,
                        fullReconcile: true);

                result.m_Changed += fullResult.m_Changed;
            }
            else
            {
                if (requestedRoad != Entity.Null)
                {
                    prunedManualSides +=
                        PruneInvalidManualBan(requestedRoad);

                    ReconcileResult roadResult =
                        ReconcileRoad(
                            requestedRoad,
                            scope,
                            policyEntity);

                    result.m_Changed += roadResult.m_Changed;
                }

                if (changedManualRoads)
                {
                    using NativeArray<Entity> changedRoads =
                        m_ChangedManualRoadBanQuery
                            .ToEntityArray(Allocator.Temp);

                    foreach (Entity road in changedRoads)
                    {
                        if (road == requestedRoad)
                        {
                            continue;
                        }

                        prunedManualSides +=
                            PruneInvalidManualBan(road);

                        ReconcileResult roadResult =
                            ReconcileRoad(
                                road,
                                scope,
                                policyEntity);

                        result.m_Changed += roadResult.m_Changed;
                    }
                }

                // Newly rebuilt lanes still need the broader scope/manual
                // rule evaluated, but this scans only lanes flagged changed.
                if (changedParkingLanes &&
                    (scope != PCSettings.ParkingScope.Off ||
                        !m_ManualRoadBanQuery.IsEmptyIgnoreFilter))
                {
                    ReconcileResult changedResult =
                        ReconcileStreetParking(
                            scope,
                            policyEntity,
                            fullReconcile: false);

                    result.m_Changed += changedResult.m_Changed;
                }
            }

            m_Initialized = true;
            m_LastScope = scope;

            s_ReconcileRequested = false;
            s_SaveRecoveryRequested = false;
            s_RoadReconcileRequested = Entity.Null;

            if (prunedManualSides > 0)
            {
                ParkingStatusCache.MarkDirty();

#if DEBUG
                LogUtils.Info(
                    $"{Mod.ModTag} [RoadTool] Cleared " +
                    $"{prunedManualSides} stale manual road-side ban(s).");
#endif
            }

            if (fullReconcile)
            {
                int ownedLanes =
                    m_ModifiedParkingLanesQuery.CalculateEntityCount();

                LogUtils.Info(
                    $"{Mod.ModTag} Street parking reconciled ({scope}): " +
                    $"{result.m_Changed} lane flags changed, " +
                    $"{ownedLanes} lanes owned by Parking Control.");
            }
        }

        private ReconcileResult ReconcileStreetParking(
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            bool fullReconcile)
        {
            ComponentLookup<ParkingLane> parkingLaneLookup =
                SystemAPI.GetComponentLookup<ParkingLane>();

            ComponentLookup<Owner> ownerLookup =
                SystemAPI.GetComponentLookup<Owner>(true);

            ComponentLookup<PrefabRef> prefabRefLookup =
                SystemAPI.GetComponentLookup<PrefabRef>(true);

            ComponentLookup<ParkingLaneData> parkingLaneDataLookup =
                SystemAPI.GetComponentLookup<ParkingLaneData>(true);

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

            ComponentLookup<PrefabRef> prefabRefLookup =
                SystemAPI.GetComponentLookup<PrefabRef>(true);

            ComponentLookup<ParkingLaneData> parkingLaneDataLookup =
                SystemAPI.GetComponentLookup<ParkingLaneData>(true);

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
            ComponentLookup<PrefabRef> prefabRefLookup,
            ComponentLookup<ParkingLaneData> parkingLaneDataLookup,
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

            bool recalculatedByVanilla =
                createdLookup.HasComponent(entity) ||
                updatedLookup.HasComponent(entity) ||
                pathfindUpdatedLookup.HasComponent(entity);

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

            if (!shouldRestrict)
            {
                if (hasState)
                {
                    if (!recalculatedByVanilla &&
                        (parkingLane.m_Flags &
                            ParkingLaneFlags.ParkingDisabled) != 0)
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
                }

                return;
            }

            // If vanilla recalculated this lane and independently disabled it,
            // stop claiming ownership of that restriction.
            if ((parkingLane.m_Flags &
                    ParkingLaneFlags.ParkingDisabled) != 0)
            {
                if (hasState && recalculatedByVanilla)
                {
                    removeStateEntities.Add(entity);
                }

                return;
            }

            parkingLane.m_Flags |= ParkingLaneFlags.ParkingDisabled;
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

        /// <summary>
        /// Returns whether this road side is covered by any Parking Control rule.
        /// </summary>
        internal static bool IsRestrictionTarget(
            Entity lane,
            ParkingLane parkingLane,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup,
            ComponentLookup<ManualRoadParkingBan> manualBanLookup,
            BufferLookup<Game.Policies.Policy> policyLookup)
        {
            Entity road = ownerLookup[lane].m_Owner;

            if (manualBanLookup.TryGetComponent(
                    road,
                    out ManualRoadParkingBan manualBan))
            {
                bool rightSide =
                    (parkingLane.m_Flags &
                        ParkingLaneFlags.RightSide) != 0;

                if (manualBan.IsBanned(rightSide))
                {
                    return true;
                }
            }

            if (scope == PCSettings.ParkingScope.WholeCity)
            {
                return true;
            }

            if (scope != PCSettings.ParkingScope.ByDistrict ||
                policyEntity == Entity.Null)
            {
                return false;
            }

            Entity district =
                GetLaneDistrict(
                    lane,
                    parkingLane,
                    ownerLookup,
                    borderDistrictLookup);

            return IsDistrictPolicyActive(
                district,
                policyEntity,
                policyLookup);
        }

        /// <summary>
        /// Gets the district governing this side of a road parking lane.
        /// </summary>
        internal static Entity GetLaneDistrict(
            Entity lane,
            ParkingLane parkingLane,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup)
        {
            Entity road = ownerLookup[lane].m_Owner;

            if (!borderDistrictLookup.TryGetComponent(
                    road,
                    out Game.Areas.BorderDistrict borderDistrict))
            {
                return Entity.Null;
            }

            return (parkingLane.m_Flags &
                    ParkingLaneFlags.RightSide) != 0
                ? borderDistrict.m_Right
                : borderDistrict.m_Left;
        }

        /// <summary>
        /// Returns whether a district has Parking Control's policy enabled.
        /// </summary>
        internal static bool IsDistrictPolicyActive(
            Entity district,
            Entity policyEntity,
            BufferLookup<Game.Policies.Policy> policyLookup)
        {
            if (district == Entity.Null ||
                policyEntity == Entity.Null ||
                !policyLookup.TryGetBuffer(
                    district,
                    out DynamicBuffer<Game.Policies.Policy> policies))
            {
                return false;
            }

            foreach (Game.Policies.Policy policy in policies)
            {
                if (policy.m_Policy == policyEntity &&
                    (policy.m_Flags &
                        Game.Policies.PolicyFlags.Active) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasPolicyChange(Entity policyEntity)
        {
            if (policyEntity == Entity.Null ||
                m_PolicyModifyQuery.IsEmptyIgnoreFilter)
            {
                return false;
            }

            using NativeArray<Game.Policies.Modify> modifications =
                m_PolicyModifyQuery
                    .ToComponentDataArray<Game.Policies.Modify>(
                        Allocator.Temp);

            foreach (Game.Policies.Modify modification in modifications)
            {
                if (modification.m_Policy == policyEntity)
                {
                    return true;
                }
            }

            return false;
        }

        private int PruneInvalidManualBans(EntityQuery query)
        {
            int removedSides = 0;

            using NativeArray<Entity> roads =
                query.ToEntityArray(Allocator.Temp);

            foreach (Entity road in roads)
            {
                removedSides += PruneInvalidManualBan(road);
            }

            return removedSides;
        }

        private int PruneInvalidManualBan(Entity road)
        {
            if (road == Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasComponent<ManualRoadParkingBan>(road))
            {
                return 0;
            }

            ManualRoadParkingBan ban =
                EntityManager.GetComponentData<ManualRoadParkingBan>(road);

            bool hadLeft = ban.IsBanned(rightSide: false);
            bool hadRight = ban.IsBanned(rightSide: true);

            bool hasLeftParking = false;
            bool hasRightParking = false;

            if (EntityManager.HasBuffer<SubLane>(road))
            {
                DynamicBuffer<SubLane> subLanes =
                    EntityManager.GetBuffer<SubLane>(
                        road,
                        isReadOnly: true);

                foreach (SubLane subLane in subLanes)
                {
                    Entity lane = subLane.m_SubLane;

                    if (!NoParkingRoadToolSystem.TryGetEligibleParkingLane(
                            EntityManager,
                            road,
                            lane,
                            out ParkingLane parkingLane,
                            out Curve _))
                    {
                        continue;
                    }

                    if ((parkingLane.m_Flags &
                            ParkingLaneFlags.RightSide) != 0)
                    {
                        hasRightParking = true;
                    }
                    else
                    {
                        hasLeftParking = true;
                    }
                }
            }

            if (hadLeft && !hasLeftParking)
            {
                ban.SetBanned(rightSide: false, banned: false);
            }

            if (hadRight && !hasRightParking)
            {
                ban.SetBanned(rightSide: true, banned: false);
            }

            int removedSides =
                (hadLeft && !hasLeftParking ? 1 : 0) +
                (hadRight && !hasRightParking ? 1 : 0);

            if (removedSides == 0)
            {
                return 0;
            }

            if (ban.IsEmpty)
            {
                EntityManager.RemoveComponent<ManualRoadParkingBan>(road);
            }
            else
            {
                EntityManager.SetComponentData(road, ban);
            }

            return removedSides;
        }

        internal static bool IsStreetCarParkingLane(
            Entity entity,
            ParkingLane parkingLane,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<PrefabRef> prefabRefLookup,
            ComponentLookup<ParkingLaneData> parkingLaneDataLookup,
            ComponentLookup<Road> roadLookup)
        {
            if ((parkingLane.m_Flags &
                    (ParkingLaneFlags.VirtualLane |
                        ParkingLaneFlags.SpecialVehicles)) != 0)
            {
                return false;
            }

            Owner owner = ownerLookup[entity];

            if (!roadLookup.HasComponent(owner.m_Owner))
            {
                return false;
            }

            PrefabRef prefabRef = prefabRefLookup[entity];

            return parkingLaneDataLookup.TryGetComponent(
                    prefabRef.m_Prefab,
                    out ParkingLaneData parkingLaneData) &&
                (parkingLaneData.m_RoadTypes & RoadTypes.Car) != 0;
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

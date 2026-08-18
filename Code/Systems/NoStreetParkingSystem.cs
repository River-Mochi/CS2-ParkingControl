// <copyright file="NoStreetParkingSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Disables ordinary street-parking lanes citywide or in opted-in districts.

namespace ParkingControl
{
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Prefabs;
    using Unity.Collections;
    using Unity.Entities;
    using ParkingLane = Game.Net.ParkingLane;
    using ParkingLaneFlags = Game.Net.ParkingLaneFlags;
    using Road = Game.Net.Road;
    using RoadTypes = Game.Net.RoadTypes;
    using Temp = Game.Tools.Temp;

    /// <summary>
    /// Keeps ordinary street-parking lanes synchronized with the selected coverage.
    /// </summary>
    public sealed partial class NoStreetParkingSystem : GameSystemBase
    {
        private static bool s_ReconcileRequested = true;
        private static bool s_SaveRecoveryRequested;

        private EntityQuery m_AllParkingLanesQuery;
        private EntityQuery m_ChangedParkingLanesQuery;
        private EntityQuery m_ModifiedParkingLanesQuery;
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

            if (!fullReconcile &&
                (scope == PCSettings.ParkingScope.Off ||
                    m_ChangedParkingLanesQuery.IsEmptyIgnoreFilter))
            {
                return;
            }

            Dependency.Complete();

            ReconcileResult result = scope != PCSettings.ParkingScope.Off
                ? ReconcileStreetParking(scope, policyEntity, fullReconcile)
                : RestoreStreetParking();

            m_Initialized = true;
            m_LastScope = scope;
            s_ReconcileRequested = false;
            s_SaveRecoveryRequested = false;

            if (fullReconcile)
            {
                string action = scope == PCSettings.ParkingScope.Off ? "restored" : "blocked";
                int ownedLanes = m_ModifiedParkingLanesQuery.CalculateEntityCount();
                LogUtils.Info(
                    $"{Mod.ModTag} Street parking {action} ({scope}): " +
                    $"{result.m_Changed} lane flags changed, " +
                    $"{ownedLanes} lanes owned by Parking Control.");
            }
        }

        private ReconcileResult ReconcileStreetParking(
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            bool fullReconcile)
        {
            ComponentLookup<ParkingLane> parkingLaneLookup = SystemAPI.GetComponentLookup<ParkingLane>();
            ComponentLookup<Owner> ownerLookup = SystemAPI.GetComponentLookup<Owner>(true);
            ComponentLookup<PrefabRef> prefabRefLookup = SystemAPI.GetComponentLookup<PrefabRef>(true);
            ComponentLookup<ParkingLaneData> parkingLaneDataLookup = SystemAPI.GetComponentLookup<ParkingLaneData>(true);
            ComponentLookup<Road> roadLookup = SystemAPI.GetComponentLookup<Road>(true);
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup =
                SystemAPI.GetComponentLookup<Game.Areas.BorderDistrict>(true);
            ComponentLookup<StreetParkingState> stateLookup = SystemAPI.GetComponentLookup<StreetParkingState>(true);
            ComponentLookup<Created> createdLookup = SystemAPI.GetComponentLookup<Created>(true);
            ComponentLookup<Updated> updatedLookup = SystemAPI.GetComponentLookup<Updated>(true);
            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup = SystemAPI.GetComponentLookup<PathfindUpdated>(true);
            BufferLookup<Game.Policies.Policy> policyLookup =
                SystemAPI.GetBufferLookup<Game.Policies.Policy>(true);

            NativeList<Entity> addStateEntities = new(Allocator.Temp);
            NativeList<Entity> removeStateEntities = new(Allocator.Temp);
            NativeList<Entity> pathfindUpdateEntities = new(Allocator.Temp);
            ReconcileResult result = default;

            EntityQuery sourceQuery = fullReconcile ? m_AllParkingLanesQuery : m_ChangedParkingLanesQuery;
            using (NativeArray<Entity> parkingLaneEntities = sourceQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity entity in parkingLaneEntities)
                {
                    ParkingLane parkingLane = parkingLaneLookup[entity];
                    bool hasState = stateLookup.HasComponent(entity);
                    bool recalculatedByVanilla =
                        createdLookup.HasComponent(entity) ||
                        updatedLookup.HasComponent(entity) ||
                        pathfindUpdatedLookup.HasComponent(entity);
                    bool isStreetParking = IsStreetCarParkingLane(
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
                            policyLookup);

                    if (!shouldRestrict)
                    {
                        if (hasState)
                        {
                            // A changed lane has already been recalculated by vanilla this pass.
                            // Otherwise clear the temp flag we previously owned.
                            if (!recalculatedByVanilla &&
                                (parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0)
                            {
                                parkingLane.m_Flags &= ~ParkingLaneFlags.ParkingDisabled;
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

                        continue;
                    }

                    // Vanilla recalculates changed lanes before this system. If it now disables
                    // a lane we tracked, vanilla owns that restriction from this point forward.
                    if ((parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0)
                    {
                        if (hasState && recalculatedByVanilla)
                        {
                            removeStateEntities.Add(entity);
                        }

                        continue;
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
            }

            if (addStateEntities.Length > 0)
            {
                EntityManager.AddComponent<StreetParkingState>(addStateEntities.AsArray());
            }

            if (removeStateEntities.Length > 0)
            {
                EntityManager.RemoveComponent<StreetParkingState>(removeStateEntities.AsArray());
            }

            if (pathfindUpdateEntities.Length > 0)
            {
                EntityManager.AddComponent<PathfindUpdated>(pathfindUpdateEntities.AsArray());
            }

            addStateEntities.Dispose();
            removeStateEntities.Dispose();
            pathfindUpdateEntities.Dispose();
            return result;
        }

        /// <summary>
        /// Returns whether this road side is covered by the selected scope.
        /// </summary>
        internal static bool IsRestrictionTarget(
            Entity lane,
            ParkingLane parkingLane,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup,
            BufferLookup<Game.Policies.Policy> policyLookup)
        {
            if (scope == PCSettings.ParkingScope.WholeCity)
            {
                return true;
            }

            if (scope != PCSettings.ParkingScope.ByDistrict || policyEntity == Entity.Null)
            {
                return false;
            }

            Entity district = GetLaneDistrict(
                lane,
                parkingLane,
                ownerLookup,
                borderDistrictLookup);
            return IsDistrictPolicyActive(district, policyEntity, policyLookup);
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

            // Match vanilla roadside parking fees: RightSide uses the road's right
            // district; every other ordinary parking lane uses its left district.
            return (parkingLane.m_Flags & ParkingLaneFlags.RightSide) != 0
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
                    (policy.m_Flags & Game.Policies.PolicyFlags.Active) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasPolicyChange(Entity policyEntity)
        {
            if (policyEntity == Entity.Null || m_PolicyModifyQuery.IsEmptyIgnoreFilter)
            {
                return false;
            }

            using NativeArray<Game.Policies.Modify> modifications =
                m_PolicyModifyQuery.ToComponentDataArray<Game.Policies.Modify>(Allocator.Temp);
            foreach (Game.Policies.Modify modification in modifications)
            {
                if (modification.m_Policy == policyEntity)
                {
                    return true;
                }
            }

            return false;
        }

        private ReconcileResult RestoreStreetParking()
        {
            ComponentLookup<ParkingLane> parkingLaneLookup = SystemAPI.GetComponentLookup<ParkingLane>();
            ComponentLookup<Created> createdLookup = SystemAPI.GetComponentLookup<Created>(true);
            ComponentLookup<Updated> updatedLookup = SystemAPI.GetComponentLookup<Updated>(true);
            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup = SystemAPI.GetComponentLookup<PathfindUpdated>(true);
            NativeList<Entity> pathfindUpdateEntities = new(Allocator.Temp);
            ReconcileResult result = default;

            using (NativeArray<Entity> modifiedEntities = m_ModifiedParkingLanesQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity entity in modifiedEntities)
                {
                    ParkingLane parkingLane = parkingLaneLookup[entity];
                    if ((parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0)
                    {
                        parkingLane.m_Flags &= ~ParkingLaneFlags.ParkingDisabled;
                        parkingLaneLookup[entity] = parkingLane;
                        result.m_Changed++;
                        QueuePathfindUpdate(
                            entity,
                            createdLookup,
                            updatedLookup,
                            pathfindUpdatedLookup,
                            ref pathfindUpdateEntities);
                    }
                }
            }

            if (!m_ModifiedParkingLanesQuery.IsEmptyIgnoreFilter)
            {
                EntityManager.RemoveComponent<StreetParkingState>(m_ModifiedParkingLanesQuery);
            }

            if (pathfindUpdateEntities.Length > 0)
            {
                EntityManager.AddComponent<PathfindUpdated>(pathfindUpdateEntities.AsArray());
            }

            pathfindUpdateEntities.Dispose();
            return result;
        }

        internal static bool IsStreetCarParkingLane(
            Entity entity,
            ParkingLane parkingLane,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<PrefabRef> prefabRefLookup,
            ComponentLookup<ParkingLaneData> parkingLaneDataLookup,
            ComponentLookup<Road> roadLookup)
        {
            if ((parkingLane.m_Flags & (ParkingLaneFlags.VirtualLane | ParkingLaneFlags.SpecialVehicles)) != 0)
            {
                return false;
            }

            Owner owner = ownerLookup[entity];
            if (!roadLookup.HasComponent(owner.m_Owner))
            {
                return false;
            }

            PrefabRef prefabRef = prefabRefLookup[entity];
            return parkingLaneDataLookup.TryGetComponent(prefabRef.m_Prefab, out ParkingLaneData parkingLaneData) &&
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

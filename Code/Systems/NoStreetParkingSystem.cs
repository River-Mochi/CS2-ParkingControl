// <copyright file="NoStreetParkingSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Disables ordinary car curb-parking lanes while the citywide option is enabled.

namespace ParkingControl
{
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Pathfind;
    using Game.Prefabs;
    using Unity.Collections;
    using Unity.Entities;
    using ParkingLane = Game.Net.ParkingLane;
    using ParkingLaneFlags = Game.Net.ParkingLaneFlags;
    using Road = Game.Net.Road;
    using RoadTypes = Game.Net.RoadTypes;
    using Temp = Game.Tools.Temp;

    /// <summary>
    /// Keeps ordinary car curb-parking lanes synchronized with the whole-city option.
    /// </summary>
    public sealed partial class NoStreetParkingSystem : GameSystemBase
    {
        private static bool s_ReconcileRequested = true;
        private static bool s_SaveRecoveryRequested;

        private EntityQuery m_AllParkingLanesQuery;
        private EntityQuery m_ChangedParkingLanesQuery;
        private EntityQuery m_ModifiedParkingLanesQuery;
        private bool m_Initialized;
        private bool m_LastEnabled;

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
        }

        /// <inheritdoc/>
        protected override void OnGameLoadingComplete(
            Colossal.Serialization.Entities.Purpose purpose,
            GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (mode == GameMode.Game &&
                (purpose == Colossal.Serialization.Entities.Purpose.NewGame ||
                    purpose == Colossal.Serialization.Entities.Purpose.LoadGame))
            {
                m_Initialized = false;
                StreetParkingBaselineSystem.RequestScan();
                RequestReconcile();
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            bool enabled = Mod.Settings?.NoStreetParking ?? false;
            bool fullReconcile = s_ReconcileRequested || s_SaveRecoveryRequested || !m_Initialized || enabled != m_LastEnabled;

            if (!fullReconcile && (!enabled || m_ChangedParkingLanesQuery.IsEmptyIgnoreFilter))
            {
                return;
            }

            Dependency.Complete();

            ReconcileResult result = enabled
                ? DisableStreetParking(fullReconcile)
                : RestoreStreetParking();

            m_Initialized = true;
            m_LastEnabled = enabled;
            s_ReconcileRequested = false;
            s_SaveRecoveryRequested = false;

            if (fullReconcile)
            {
                string action = enabled ? "blocked" : "restored";
                int ownedLanes = m_ModifiedParkingLanesQuery.CalculateEntityCount();
                LogUtils.Info($"{Mod.ModTag} Street parking {action}: {result.m_Changed} lane flags changed, {ownedLanes} lanes owned by Parking Control.");
            }
        }

        private ReconcileResult DisableStreetParking(bool fullReconcile)
        {
            ComponentLookup<ParkingLane> parkingLaneLookup = SystemAPI.GetComponentLookup<ParkingLane>();
            ComponentLookup<Owner> ownerLookup = SystemAPI.GetComponentLookup<Owner>(true);
            ComponentLookup<PrefabRef> prefabRefLookup = SystemAPI.GetComponentLookup<PrefabRef>(true);
            ComponentLookup<ParkingLaneData> parkingLaneDataLookup = SystemAPI.GetComponentLookup<ParkingLaneData>(true);
            ComponentLookup<Road> roadLookup = SystemAPI.GetComponentLookup<Road>(true);
            ComponentLookup<StreetParkingState> stateLookup = SystemAPI.GetComponentLookup<StreetParkingState>(true);
            ComponentLookup<Created> createdLookup = SystemAPI.GetComponentLookup<Created>(true);
            ComponentLookup<Updated> updatedLookup = SystemAPI.GetComponentLookup<Updated>(true);
            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup = SystemAPI.GetComponentLookup<PathfindUpdated>(true);

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

                    if (!isStreetParking)
                    {
                        if (hasState)
                        {
                            // A changed lane has already been recalculated by vanilla this pass.
                            // Otherwise clear the temporary flag we previously owned.
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

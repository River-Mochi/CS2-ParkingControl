// <copyright file="NoStreetParkingSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Applies the whole-city policy by disabling only ordinary car curb-parking lanes.

namespace ParkingControl
{
    using Colossal.Serialization.Entities;
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
/// Keeps ordinary car curb-parking lanes in sync with the whole-city no-street-parking option.
/// </summary>
    public sealed partial class NoStreetParkingSystem : GameSystemBase
    {
        private static bool s_ReconcileRequested = true;

        private EntityQuery m_AllParkingLanesQuery;
        private EntityQuery m_ChangedParkingLanesQuery;
        private EntityQuery m_StreetParkingStateQuery;
        private bool m_Initialized;
        private bool m_LastEnabled;

    /// <summary>
    /// Requests a full reconciliation during the next modification pass.
    /// </summary>
        public static void RequestReconcile()
        {
            s_ReconcileRequested = true;
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
        m_StreetParkingStateQuery = SystemAPI.QueryBuilder()
            .WithAll<ParkingLane, Owner, PrefabRef, StreetParkingState>()
            .WithNone<Deleted, Temp>()
            .Build();
        }

    /// <inheritdoc/>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
        base.OnGameLoadingComplete(purpose, mode);

        if (mode == GameMode.Game && (purpose == Purpose.NewGame || purpose == Purpose.LoadGame))
        {
            m_Initialized = false;
            RequestReconcile();
        }
        }

    /// <inheritdoc/>
        protected override void OnUpdate()
        {
        bool enabled = Mod.Settings?.NoStreetParking ?? false;
        bool fullReconcile = s_ReconcileRequested || !m_Initialized || enabled != m_LastEnabled;

        if (!fullReconcile)
        {
            if (enabled && m_ChangedParkingLanesQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            if (!enabled && m_StreetParkingStateQuery.IsEmptyIgnoreFilter)
            {
                return;
            }
        }

        Dependency.Complete();

        ComponentLookup<ParkingLane> parkingLaneLookup = SystemAPI.GetComponentLookup<ParkingLane>();
        ComponentLookup<Owner> ownerLookup = SystemAPI.GetComponentLookup<Owner>(true);
        ComponentLookup<PrefabRef> prefabRefLookup = SystemAPI.GetComponentLookup<PrefabRef>(true);
        ComponentLookup<ParkingLaneData> parkingLaneDataLookup = SystemAPI.GetComponentLookup<ParkingLaneData>(true);
        ComponentLookup<Road> roadLookup = SystemAPI.GetComponentLookup<Road>(true);
        ComponentLookup<StreetParkingState> stateLookup = SystemAPI.GetComponentLookup<StreetParkingState>(true);
        ComponentLookup<Created> createdLookup = SystemAPI.GetComponentLookup<Created>(true);
        ComponentLookup<Updated> updatedLookup = SystemAPI.GetComponentLookup<Updated>(true);
        ComponentLookup<PathfindUpdated> pathfindUpdatedLookup = SystemAPI.GetComponentLookup<PathfindUpdated>(true);

        ReconcileResult result = Reconcile(
            enabled,
            fullReconcile,
            parkingLaneLookup,
            ownerLookup,
            prefabRefLookup,
            parkingLaneDataLookup,
            roadLookup,
            stateLookup,
            createdLookup,
            updatedLookup,
            pathfindUpdatedLookup);

        m_Initialized = true;
        m_LastEnabled = enabled;
        s_ReconcileRequested = false;

        if (fullReconcile)
        {
            string action = enabled ? "blocked" : "restored";
            LogUtils.Info($"{Mod.ModTag} Street parking {action}: {result.m_Changed} lane flags changed, {result.m_StateAdded} lanes tracked, {result.m_StateRemoved} lanes untracked.");
        }
        }

        private ReconcileResult Reconcile(
        bool enabled,
        bool fullReconcile,
        ComponentLookup<ParkingLane> parkingLaneLookup,
        ComponentLookup<Owner> ownerLookup,
        ComponentLookup<PrefabRef> prefabRefLookup,
        ComponentLookup<ParkingLaneData> parkingLaneDataLookup,
        ComponentLookup<Road> roadLookup,
        ComponentLookup<StreetParkingState> stateLookup,
        ComponentLookup<Created> createdLookup,
        ComponentLookup<Updated> updatedLookup,
        ComponentLookup<PathfindUpdated> pathfindUpdatedLookup)
        {
        NativeList<Entity> addStateEntities = new(Allocator.Temp);
        NativeList<StreetParkingState> addStateValues = new(Allocator.Temp);
        NativeList<Entity> removeStateEntities = new(Allocator.Temp);
        NativeList<Entity> pathfindUpdateEntities = new(Allocator.Temp);
        ReconcileResult result = default;

        if (enabled && fullReconcile)
        {
            NativeArray<Entity> trackedEntities = m_StreetParkingStateQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in trackedEntities)
            {
                ParkingLane parkingLane = parkingLaneLookup[entity];
                if (IsStreetCarParkingLane(entity, parkingLane, ownerLookup, prefabRefLookup, parkingLaneDataLookup, roadLookup))
                {
                    continue;
                }

                if (RestoreParkingDisabled(entity, parkingLane, stateLookup[entity], ref parkingLaneLookup))
                {
                    QueuePathfindUpdate(entity, createdLookup, updatedLookup, pathfindUpdatedLookup, ref pathfindUpdateEntities);
                    result.m_Changed++;
                }

                removeStateEntities.Add(entity);
                result.m_StateRemoved++;
            }

            trackedEntities.Dispose();
        }

        if (enabled)
        {
            EntityQuery sourceQuery = fullReconcile ? m_AllParkingLanesQuery : m_ChangedParkingLanesQuery;
            NativeArray<Entity> parkingLaneEntities = sourceQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in parkingLaneEntities)
            {
                ParkingLane parkingLane = parkingLaneLookup[entity];
                if (!IsStreetCarParkingLane(entity, parkingLane, ownerLookup, prefabRefLookup, parkingLaneDataLookup, roadLookup))
                {
                    continue;
                }

                if (!stateLookup.HasComponent(entity))
                {
                    bool wasParkingDisabled = (parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0;
                    addStateEntities.Add(entity);
                    addStateValues.Add(new StreetParkingState(wasParkingDisabled));
                    result.m_StateAdded++;
                }

                if ((parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) == 0)
                {
                    parkingLane.m_Flags |= ParkingLaneFlags.ParkingDisabled;
                    parkingLaneLookup[entity] = parkingLane;
                    QueuePathfindUpdate(entity, createdLookup, updatedLookup, pathfindUpdatedLookup, ref pathfindUpdateEntities);
                    result.m_Changed++;
                }
            }

            parkingLaneEntities.Dispose();
        }
        else
        {
            NativeArray<Entity> trackedEntities = m_StreetParkingStateQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in trackedEntities)
            {
                ParkingLane parkingLane = parkingLaneLookup[entity];
                if (RestoreParkingDisabled(entity, parkingLane, stateLookup[entity], ref parkingLaneLookup))
                {
                    QueuePathfindUpdate(entity, createdLookup, updatedLookup, pathfindUpdatedLookup, ref pathfindUpdateEntities);
                    result.m_Changed++;
                }

                removeStateEntities.Add(entity);
                result.m_StateRemoved++;
            }

            trackedEntities.Dispose();
        }

        if (addStateEntities.Length > 0)
        {
            EntityManager.AddComponent<StreetParkingState>(addStateEntities.AsArray());
            for (int index = 0; index < addStateEntities.Length; index++)
            {
                EntityManager.SetComponentData(addStateEntities[index], addStateValues[index]);
            }
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
        addStateValues.Dispose();
        removeStateEntities.Dispose();
        pathfindUpdateEntities.Dispose();
        return result;
        }

        private static bool IsStreetCarParkingLane(
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
        if (!parkingLaneDataLookup.TryGetComponent(prefabRef.m_Prefab, out ParkingLaneData parkingLaneData))
        {
            return false;
        }

        return (parkingLaneData.m_RoadTypes & RoadTypes.Car) != 0;
        }

        private static bool RestoreParkingDisabled(
        Entity entity,
        ParkingLane parkingLane,
        StreetParkingState state,
        ref ComponentLookup<ParkingLane> parkingLaneLookup)
        {
        bool parkingDisabled = (parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) != 0;
        if (parkingDisabled == state.m_WasParkingDisabled)
        {
            return false;
        }

        if (state.m_WasParkingDisabled)
        {
            parkingLane.m_Flags |= ParkingLaneFlags.ParkingDisabled;
        }
        else
        {
            parkingLane.m_Flags &= ~ParkingLaneFlags.ParkingDisabled;
        }

        parkingLaneLookup[entity] = parkingLane;
        return true;
        }

        private static void QueuePathfindUpdate(
        Entity entity,
        ComponentLookup<Created> createdLookup,
        ComponentLookup<Updated> updatedLookup,
        ComponentLookup<PathfindUpdated> pathfindUpdatedLookup,
        ref NativeList<Entity> pathfindUpdateEntities)
        {
        if (!createdLookup.HasComponent(entity) && !updatedLookup.HasComponent(entity) && !pathfindUpdatedLookup.HasComponent(entity))
        {
            pathfindUpdateEntities.Add(entity);
        }
        }

        private struct ReconcileResult
        {
        public int m_Changed;
        public int m_StateAdded;
        public int m_StateRemoved;
        }
    }
}

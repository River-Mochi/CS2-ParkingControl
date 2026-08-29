// <copyright file="NoStreetParkingSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Disables normal street-parking lanes citywide, by district,
// or on player-selected individual road sides.

    using System.Collections.Generic;
    using Game;
    using Game.Common;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;

namespace ParkingControl
{
    /// <summary>
    /// Keeps ordinary street-parking lanes synchronized with all Parking Control restrictions.
    /// </summary>
    public sealed partial class NoStreetParkingSystem : GameSystemBase
    {
        private static bool s_ReconcileRequested = true;

        private static bool s_SaveRecoveryRequested;

        // dragging can change several roads before this system gets its turn.
        private static readonly HashSet<Entity> s_RoadReconcileRequests = new();

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
                s_RoadReconcileRequests.Add(road);
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
                .WithAll<Game.Net.ParkingLane, Game.Common.Owner, Game.Prefabs.PrefabRef>()
                .WithNone<Game.Common.Deleted, Temp>()
                .Build();

            m_ChangedParkingLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Net.ParkingLane, Game.Common.Owner, Game.Prefabs.PrefabRef>()
                .WithAny<Game.Common.Created, Game.Common.Updated, Game.Common.PathfindUpdated>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_ModifiedParkingLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Net.ParkingLane, StreetParkingState>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_ManualRoadBanQuery = SystemAPI.QueryBuilder()
                .WithAll<ManualRoadParkingBan, Game.Net.Road>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_ChangedManualRoadBanQuery = SystemAPI.QueryBuilder()
                .WithAll<ManualRoadParkingBan, Game.Net.Road, Game.Common.Updated>()
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

            s_RoadReconcileRequests.Clear();

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

            bool hasRequestedRoads =
                s_RoadReconcileRequests.Count > 0;

            if (!fullReconcile &&
                !hasRequestedRoads &&
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
                // Every distinct road touched by the drag gets reconciled.
                foreach (Entity requestedRoad in s_RoadReconcileRequests)
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
                        if (s_RoadReconcileRequests.Contains(road))
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

                // Road rebuilds can make vanilla recalculate ParkingDisabled.
                // Recheck only the lanes that CS2 already marked as changed.
                if (changedParkingLanes &&
                    (scope != PCSettings.ParkingScope.Off ||
                        hasManualRoadBans))
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
            s_RoadReconcileRequests.Clear();

            if (prunedManualSides > 0)
            {
                ParkingStatusCache.MarkDirty();

#if DEBUG
                CS2Shared.RiverMochi.LogUtils.Info(
                    $"{Mod.ModTag} [RoadTool] Cleared " +
                    $"{prunedManualSides} stale manual road-side ban(s).");
#endif
            }

#if DEBUG
            if (fullReconcile)
            {
                int ownedLanes =
                    m_ModifiedParkingLanesQuery.CalculateEntityCount();

                CS2Shared.RiverMochi.LogUtils.Info(
                    $"{Mod.ModTag} Street parking reconciled ({scope}): " +
                    $"{result.m_Changed} lane flags changed, " +
                    $"{ownedLanes} lanes owned by Parking Control.");
            }
#endif
        }
    }
}

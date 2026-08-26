// <copyright file="ParkingStatusSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Schedules on-demand parking probes for Options status and detailed log reports.

namespace ParkingControl
{
    using System;
    using System.Collections.Generic;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Simulation;
    using Unity.Entities;

    /// <summary>
    /// Owns parking queries and runs only when status or a manual report is requested.
    /// </summary>
    public sealed partial class ParkingStatusSystem : GameSystemBase
    {
        private const int kVehicleSampleLimit = 20;
        private const int kUnresolvedLaneSampleLimit = 100;

        private EntityQuery m_CurbLaneQuery;
        private EntityQuery m_DistrictPolicyPrefabQuery;
        private EntityQuery m_DistrictQuery;
        private EntityQuery m_GarageLaneQuery;
        private EntityQuery m_ParkingFacilityQuery;
        private EntityQuery m_PersonalVehicleQuery;
        private Game.UI.NameSystem m_NameSystem = null!;
        private SimulationSystem m_SimulationSystem = null!;
        private bool m_HasPreviousReport;
        private bool m_ReportRequested;
        private bool m_StatusRequested;
        private readonly Dictionary<Entity, int> m_PreviousDistrictStreetCars = new();
        private readonly List<Entity> m_PreviousOutsideSamples = new(kVehicleSampleLimit);
        private readonly List<Entity> m_PreviousStreetSamples = new(kVehicleSampleLimit);
        private readonly List<Entity> m_PreviousUnknownSamples = new(kVehicleSampleLimit);
        private ParkingSnapshot m_PreviousReport;

        /// <summary>
        /// Schedules a detailed report for the next modification pass.
        /// </summary>
        public void ScheduleReport()
        {
            m_ReportRequested = true;
            Enabled = true;
        }

        /// <summary>
        /// Schedules an Options UI status refresh for the next modification pass.
        /// </summary>
        internal void ScheduleStatus()
        {
            m_StatusRequested = true;
            Enabled = true;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_NameSystem = World.GetOrCreateSystemManaged<Game.UI.NameSystem>();
            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();

            // Fully qualified query types prevent similarly named Game components from
            // becoming ambiguous when another namespace is added to a partial file.
            m_CurbLaneQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Net.ParkingLane, Game.Common.Owner, Game.Prefabs.PrefabRef>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .Build();
            m_DistrictQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Areas.District, Game.Policies.Policy>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .Build();

            // Report-only: find district policies that can modify ParkingFee even if
            // another mod damages or removes their PolicySliderData component.
            m_DistrictPolicyPrefabQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Prefabs.PolicyData, Game.Prefabs.DistrictModifierData>()
                .Build();

            m_GarageLaneQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Net.GarageLane>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .Build();
            m_ParkingFacilityQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Buildings.CarParkingFacility>(),
                    },
                    Any = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Net.SubLane>(),
                        ComponentType.ReadOnly<Game.Net.SubNet>(),
                        ComponentType.ReadOnly<Game.Objects.SubObject>(),
                    },
                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Tools.Temp>(),
                        ComponentType.ReadOnly<Game.Common.Deleted>(),
                    },
                },
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Routes.CarParking>(),
                    },
                    Any = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Net.SubLane>(),
                        ComponentType.ReadOnly<Game.Objects.SubObject>(),
                    },
                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Tools.Temp>(),
                        ComponentType.ReadOnly<Game.Common.Deleted>(),
                    },
                });
            m_PersonalVehicleQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Prefabs.PrefabRef, Game.Vehicles.PersonalCar>()
                .WithNone<
                    Game.Vehicles.CarTrailer,
                    Game.Common.Deleted,
                    Game.Tools.Temp,
                    Game.Common.Destroyed>()
                .Build();
            Enabled = false;
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
                // Entity Index:Version values are valid only within this loaded-city session.
                ResetReportHistory();
                ParkingStatusCache.InvalidateCache();
            }
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            // GameSystemBase.OnDestroy unregisters its game/world event handlers.
            // Managed report samples are reclaimed with this ECS system.
            ParkingStatusCache.InvalidateCache();
            base.OnDestroy();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            bool reportRequested = m_ReportRequested;
            bool statusRequested = m_StatusRequested;
            if (!reportRequested && !statusRequested)
            {
                Enabled = false;
                return;
            }

            m_ReportRequested = false;
            m_StatusRequested = false;
            try
            {
                Dependency.Complete();
                ParkingReportDetails? details = reportRequested
                    ? new ParkingReportDetails(kVehicleSampleLimit)
                    : null;
                ParkingSnapshot snapshot = BuildSnapshot(details);

                // These counters are UI/report-only and keep scope bans separate from manual bans.
                AddScopeManualCounters(ref snapshot);

                ParkingStatusCache.Publish(snapshot);

                if (reportRequested && details != null)
                {
                    WriteScopeManualReportSummary(snapshot);
                    WriteParkingFeeDebug();
                    WriteReport(snapshot, details);
                }
            }
            catch (Exception ex)
            {
                ParkingStatusCache.PublishFailure();
                LogUtils.Warn($"{Mod.ModTag} Parking snapshot failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
            finally
            {
                // Status is entirely on demand; no steady periodic probe.
                Enabled = false;
            }
        }

        private void ResetReportHistory()
        {
            m_HasPreviousReport = false;
            m_PreviousReport = default;
            m_PreviousDistrictStreetCars.Clear();
            m_PreviousOutsideSamples.Clear();
            m_PreviousStreetSamples.Clear();
            m_PreviousUnknownSamples.Clear();
        }
    }
}

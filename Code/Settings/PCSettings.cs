// <copyright file="PCSettings.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Defines immediately persisted Parking Control Options UI settings.

namespace ParkingControl
{
    using System;
    using Colossal.IO.AssetDatabase;
    using Colossal.Json;
    using CS2Shared.RiverMochi;
    using Game.Modding;
    using Game.Settings;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// Stores Parking Control options.
    /// </summary>
    [FileLocation("ModsSettings/" + Mod.ModId + "/" + Mod.ModId)]
    [SettingsUITabOrder(kActionsTab, kAboutTab)]
    [SettingsUIGroupOrder(kStreetParkingGroup, kStatusGroup, kAboutInfoGroup, kAboutLinksGroup, kAboutDiagnosticsGroup)]
    [SettingsUIShowGroupName(kStreetParkingGroup, kStatusGroup, kAboutLinksGroup, kAboutDiagnosticsGroup)]
    public class PCSettings : ModSetting
    {
        internal const string kActionsTab = "Actions";
        internal const string kAboutTab = "About";
        internal const string kStreetParkingGroup = "StreetParking";
        internal const string kStatusGroup = "Status";
        internal const string kAboutInfoGroup = "AboutInfo";
        internal const string kAboutLinksGroup = "AboutLinks";
        internal const string kAboutDiagnosticsGroup = "AboutDiagnostics";

        private const string kAboutLinksRow = nameof(kAboutLinksRow);
        private const string kAboutDiagnosticsRow = nameof(kAboutDiagnosticsRow);
        private const string kUrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        private ParkingScope m_AppliedScope;
        private ParkingScope m_ParkingScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="PCSettings"/> class.
        /// </summary>
        /// <param name="mod">The owning mod.</param>
        public PCSettings(IMod mod)
            : base(mod)
        {
            SetDefaults();
        }

        /// <summary>
        /// Gets or sets where ordinary street parking is disabled.
        /// </summary>
        [SettingsUISection(kActionsTab, kStreetParkingGroup)]
        public ParkingScope Scope
        {
            get => m_ParkingScope;
            set => m_ParkingScope = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether district-mode instructions are shown.
        /// </summary>
        [SettingsUISection(kActionsTab, kStreetParkingGroup)]
        public bool ShowInstructions { get; set; }

        /// <summary>
        /// Gets the localized district-mode instructions shown by the multiline widget.
        /// </summary>
        [SettingsUIMultilineText]
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideInstructions))]
        [SettingsUISection(kActionsTab, kStreetParkingGroup)]
        public string DistrictInstructions => string.Empty;

        /// <summary>
        /// Gets the cached lane-enforcement status.
        /// </summary>
        [Exclude]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string EnforcementStatus => ParkingStatusCache.EnforcementRow;

        /// <summary>
        /// Gets the cached personal-vehicle location status.
        /// </summary>
        [Exclude]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string VehicleStatus => ParkingStatusCache.VehicleRow;

        /// <summary>
        /// Gets the cached parking-supply status.
        /// </summary>
        [Exclude]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string SupplyStatus => ParkingStatusCache.SupplyRow;

        /// <summary>
        /// Gets the cached street share and update time.
        /// </summary>
        [Exclude]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string ShareStatus => ParkingStatusCache.ShareRow;

        /// <summary>
        /// Gets the player-facing mod name.
        /// </summary>
        [SettingsUISection(kAboutTab, kAboutInfoGroup)]
        public string NameText => Mod.ModName;

        /// <summary>
        /// Gets the running mod version.
        /// </summary>
        [SettingsUISection(kAboutTab, kAboutInfoGroup)]
        public string VersionText =>
#if DEBUG
            Mod.ModVersion + " (DEBUG)";
#else
            Mod.ModVersion;
#endif

        /// <summary>
        /// Opens River-Mochi's Cities: Skylines II Paradox Mods page.
        /// </summary>
        [SettingsUIButtonGroup(kAboutLinksRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutLinksGroup)]
        public bool OpenParadox
        {
            set
            {
                if (value)
                {
                    TryOpenUrl(kUrlParadox);
                }
            }
        }

        /// <summary>
        /// Writes an on-demand parking report to the mod log.
        /// </summary>
        [SettingsUIButtonGroup(kAboutDiagnosticsRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutDiagnosticsGroup)]
        public bool ReportToLog
        {
            set
            {
                if (value)
                {
                    ReportToLogAction();
                }
            }
        }

        /// <summary>
        /// Opens the mod log, or its containing folder if the log does not exist yet.
        /// </summary>
        [SettingsUIButtonGroup(kAboutDiagnosticsRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutDiagnosticsGroup)]
        public bool OpenLog
        {
            set
            {
                if (value)
                {
                    ShellOpen.OpenModLogOrLogsFolder();
                }
            }
        }

        /// <inheritdoc/>
        public override void Apply()
        {
            base.Apply();
            if (m_AppliedScope == m_ParkingScope)
            {
                return;
            }

            // Instruction visibility is also a setting change, but it must not rescan lanes.
            m_AppliedScope = m_ParkingScope;
            if (m_ParkingScope != ParkingScope.Off)
            {
                StreetParkingBaselineSystem.RequestScan();
            }

            NoStreetParkingSystem.RequestReconcile();
            ParkingStatusCache.MarkDirty();
        }

        /// <inheritdoc/>
        public override void SetDefaults()
        {
            m_ParkingScope = ParkingScope.WholeCity;
            ShowInstructions = false;
        }

        private bool HideInstructions()
        {
            return !ShowInstructions;
        }

        private static void ReportToLogAction()
        {
            try
            {
                World? world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                {
                    LogUtils.Warn($"{Mod.ModTag} Cannot write parking report: game world is not available.");
                    return;
                }

                ParkingStatusSystem? statusSystem = world.GetExistingSystemManaged<ParkingStatusSystem>();
                if (statusSystem == null)
                {
                    LogUtils.Warn($"{Mod.ModTag} Cannot write parking report: status system was not found.");
                    return;
                }

                statusSystem.ScheduleReport();
            }
            catch (Exception ex)
            {
                LogUtils.Warn($"{Mod.ModTag} Failed to request parking report: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static void TryOpenUrl(string url)
        {
            try
            {
                Application.OpenURL(url);
            }
            catch (Exception ex)
            {
                LogUtils.Warn($"{Mod.ModTag} Failed to open URL: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Selects the area covered by the no-street-parking rule.
        /// </summary>
        public enum ParkingScope
        {
            WholeCity,
            ByDistrict,
            Off,
        }
    }
}

// <copyright file="PCSettings.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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
    [SettingsUIGroupOrder(kStreetParkingGroup, kStatusGroup, kAboutInfoGroup, kAboutLinksGroup, kAboutDebugGroup)]
    [SettingsUIShowGroupName(kStreetParkingGroup, kStatusGroup, kAboutLinksGroup, kAboutDebugGroup)]
    public class PCSettings : ModSetting
    {
        internal const string kActionsTab = "Actions";
        internal const string kAboutTab = "About";
        internal const string kStreetParkingGroup = "StreetParking";
        internal const string kStatusGroup = "Status";
        internal const string kAboutInfoGroup = "AboutInfo";
        internal const string kAboutLinksGroup = "AboutLinks";
        internal const string kAboutDebugGroup = "AboutDebug";

        private const string kAboutLinksRow = nameof(kAboutLinksRow);
        private const string kAboutDebugRow = nameof(kAboutDebugRow);
        private const string kUrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        // Null until the first Options Apply because LoadSettings does not call Apply.
        private ParkingScope? m_AppliedScope;
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
        /// Legacy hidden button kept so existing locale IDs stay valid.
        /// </summary>
        [SettingsUIButton]
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideRelocateParkedCars))]
        [SettingsUISection(kActionsTab, kStreetParkingGroup)]
        public bool RelocateParkedCars
        {
            set
            {
                // Relocation is automatic and done by the game.
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether live parking status rows are shown.
        /// </summary>
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public bool ShowStatus { get; set; }

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
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideStatus))]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string EnforcementStatus => ParkingStatusCache.EnforcementRow;

        /// <summary>
        /// Gets the cached manual No Parking status.
        /// </summary>
        [Exclude]
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideStatus))]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string ManualStatus => ParkingStatusCache.ManualRow;

        /// <summary>
        /// Gets the cached citywide parking-use summary.
        /// </summary>
        [Exclude]
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideStatus))]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string ShareStatus => ParkingStatusCache.ShareRow;

        /// <summary>
        /// Gets the cached parking-capacity rating.
        /// </summary>
        [Exclude]
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideStatus))]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string SupplyStatus => ParkingStatusCache.SupplyRow;

        /// <summary>
        /// Gets the cached personal-vehicle location status.
        /// </summary>
        [Exclude]
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideStatus))]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string VehicleStatus => ParkingStatusCache.VehicleRow;

        /// <summary>
        /// Gets the time when the cached status snapshot was collected.
        /// </summary>
        [Exclude]
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideStatus))]
        [SettingsUIValueVersion(typeof(ParkingStatusCache), nameof(ParkingStatusCache.GetUiVersion))]
        [SettingsUISection(kActionsTab, kStatusGroup)]
        public string UpdatedStatus => ParkingStatusCache.UpdatedRow;

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
        /// Opens River-Mochi's Paradox Mods page.
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
        [SettingsUIButtonGroup(kAboutDebugRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutDebugGroup)]
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
        [SettingsUIButtonGroup(kAboutDebugRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutDebugGroup)]
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

        /// <summary>
        /// Gets or sets a value indicating whether extra automatic DEBUG details are logged.
        /// </summary>
        [SettingsUIHideByCondition(typeof(PCSettings), nameof(HideVerboseLog))]
        [SettingsUISection(kAboutTab, kAboutDebugGroup)]
        public bool VerboseLog { get; set; }

        /// <inheritdoc/>
        public override void Apply()
        {
            base.Apply();
            if (m_AppliedScope == m_ParkingScope)
            {
                return;
            }

            // Visibility-only setting changes must not rescan lanes.
            m_AppliedScope = m_ParkingScope;

            ParkingPolicySystem.RefreshVisibility();
            NoStreetParkingSystem.RequestReconcile();
            ParkingStatusCache.MarkDirty();
        }

        /// <inheritdoc/>
        public override void SetDefaults()
        {
            m_ParkingScope = ParkingScope.ByDistrict;
            ShowInstructions = false;
            ShowStatus = false;
#if DEBUG
            VerboseLog = true;
#else
            VerboseLog = false;
#endif
        }

        private bool HideInstructions()
        {
            return !ShowInstructions;
        }

        private bool HideStatus()
        {
            return !ShowStatus;
        }

        private static bool HideRelocateParkedCars()
        {
            return true;
        }

        private static bool HideVerboseLog()
        {
#if DEBUG
            return false;
#else
            return true;
#endif
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
            // order here controls the Options dropdown order.
            // Explicit values preserve existing settings files.
            ByDistrict = 1,
            WholeCity = 0,
            Off = 2,
        }
    }
}

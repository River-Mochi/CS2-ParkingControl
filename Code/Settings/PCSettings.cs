// <copyright file="PCSettings.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Defines the immediately persisted Options UI settings for Parking Control.

namespace ParkingControl
{
    using Colossal.IO.AssetDatabase;
    using Game.Modding;
    using Game.Settings;

/// <summary>
/// Stores the user-configurable Parking Control options.
/// </summary>
[FileLocation("ModsSettings/" + Mod.ModId + "/" + Mod.ModId)]
[SettingsUITabOrder(kActionsTab, kAboutTab)]
[SettingsUIGroupOrder(kStreetParkingGroup, kAboutGroup)]
[SettingsUIShowGroupName(kActionsTab, kStreetParkingGroup)]
    public class PCSettings : ModSetting
    {
    /// <summary>
    /// The Options UI tab containing active policy settings.
    /// </summary>
        public const string kActionsTab = "Actions";

    /// <summary>
    /// The Options UI tab containing mod information.
    /// </summary>
        public const string kAboutTab = "About";

    /// <summary>
    /// The group containing whole-city curb-parking controls.
    /// </summary>
        public const string kStreetParkingGroup = "StreetParking";

    /// <summary>
    /// The group containing mod information.
    /// </summary>
        public const string kAboutGroup = "About";

        private bool m_NoStreetParking;

    /// <summary>
    /// Initializes a new instance of the <see cref="PCSettings"/> class.
    /// </summary>
    /// <param name="mod">The owning mod instance.</param>
        public PCSettings(IMod mod)
            : base(mod)
        {
            SetDefaults();
        }

    /// <summary>
    /// Gets or sets a value indicating whether ordinary car curb parking is disabled citywide.
    /// </summary>
    [SettingsUISection(kActionsTab, kStreetParkingGroup)]
        public bool NoStreetParking
        {
            get => m_NoStreetParking;
            set
            {
                m_NoStreetParking = value;
            }
        }

        /// <inheritdoc/>
        public override void Apply()
        {
            base.Apply();
            NoStreetParkingSystem.RequestReconcile();
        }

    /// <summary>
    /// Gets the running mod version for troubleshooting.
    /// </summary>
    [SettingsUISection(kAboutTab, kAboutGroup)]
        public string Version => Mod.ModVersion;

    /// <inheritdoc/>
        public override void SetDefaults()
        {
            m_NoStreetParking = false;
        }
    }
}

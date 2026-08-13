// <copyright file="LocaleEN.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the English text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// English localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleEN"/> class.
        /// </summary>
        /// <param name="settings">The Options settings whose localization IDs are used.</param>
        public LocaleEN(PCSettings settings)
        {
            m_Settings = settings;
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Actions" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "About" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Street parking" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Status" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Mod information" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Links" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostics" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "No street parking (whole city)" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)), "Prevents new personal motor vehicles, including motorcycles, from choosing ordinary curb parking. Parking lots, garages, building parking, bicycle parking, taxi and boarding spaces, and special-vehicle spaces stay available. Existing parked vehicles are not removed. Make sure the city has adequate off-street parking." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Enforcement" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)), "Eligible public-road curb lanes, disabled lanes, and curb lane entities that still contain parked personal vehicles." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Personal-vehicle locations" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)), "Current non-bicycle personal motor vehicles by parking location. Hidden building and outside-connection vehicles are not visibly rendered." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parking supply" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)), "The first total matches the vanilla Roads infoview scope. The garage total also includes non-border building garage lanes; continuous unslotted parking has no exact capacity." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Street-parking share" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)), "Share of parked personal motor vehicles currently on street curbs. Status refreshes once after the simulation advances; it does not repeatedly scan while Options is paused." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod name" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "River-Mochi on Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Open the author's Cities: Skylines II page on Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Write parking report" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)), "Write curb, parking-supply, ownership, and personal-vehicle location details to ParkingControl.log. A second report in the same loaded city tracks the same street-car entity IDs." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Open log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Open ParkingControl.log, or the Logs folder if the file does not exist yet." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

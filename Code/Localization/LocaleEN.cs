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
    using Colossal.Localization;

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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "General" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "About" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Street parking" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutGroup), "About" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "No street parking (whole city)" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)), "Prevents new personal cars from choosing ordinary curb parking. Parking lots, garages, building parking, bicycle parking, taxi and boarding spaces, and special-vehicle spaces stay available. Existing parked cars are not removed." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Version)), "Version" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

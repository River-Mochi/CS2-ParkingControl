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
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Personal vehicle status" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Mod information" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Links" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostics" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "No street parking (whole city)" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)),
                    "- Prevents future personal vehicles + motorcycles, from using street parking.\n" +
                    "- Parking lots, building parking stay available.\n" +
                    "- Existing parked vehicles are not removed. They naturally leave the next time a citizen uses the car. \n" +
                    "- Make sure the city has adequate off-street parking or cars may drive a lot looking for a space." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Street Parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Parked> = cars still parked on streets.\n" +
                    "<Lanes> = roadside parking sections holding those cars. One lane can hold several cars.\n" +
                    "<Disabled> = street-parking lanes closed to new parking.\n" +
                    "<OK> = No Street Parking is on and working.\n" +
                    "<OFF> = No Street Parking is off; cars may park freely on ordinary streets.\n" +
                    "<CHECK> = roads may still be updating. Wait a moment; write a log report if it remains.\n" +
                    "**Some cars can remain after enabling the no-street rule or changing roads. They leave naturally if the citizen uses the car.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Car Locations" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Street> = parked on public roads.\n" +
                    "<Visible> = cars you can see and click in open-air lots or outdoor parking included with buildings.\n" +
                    "<Hidden> = inside buildings or garages.\n" +
                    "<OC> = outside connection storage at the city border; some incoming household cars start there.\n" +
                    "**Unassigned vanilla staging is log-only.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<Percent> and <used / total> show parking occupancy.\n" +
                    "<Public> = facilities counted by the vanilla Parking InfoView.\n" +
                    "<Building> = parking included with homes and workplaces.\n" +
                    "**Building parking includes visible outdoor spaces and internal parking.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Street usage" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Street parked> = share of cars using known street, public, or building parking.\n" +
                    "<Moving> = personal vehicles driving or waiting in traffic.\n" +
                    "<Updated> = last refresh of Status.\n" +
                    "**Outside connection (OC) and unassigned staging are excluded.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod name" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods link" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Open the author's page on Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Write parking report" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Write street-parking, supply, ownership, and vehicle-location\n" +
                    "details to <ParkingControl.log>\n" +
                    "A second report in the same loaded city tracks the same street-car entity IDs." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Open log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Open <ParkingControl.log>, or the Logs folder if the file does not exist yet." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

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
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "No street parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Choose if street parking is blocked for the Whole City or per District.\n" +
                    "Lane is flagged for no parking to prevent new street parking.\n" +
                    "Existing parked cars leave naturally over time when it is next used.\n" +
                    "Of course, fee-based parking lots and normal building parking spaces are still useable."
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Whole City" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "By District" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "Off" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Show instructions" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)), "Show how to use district mode." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Show status" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "Show current parking totals below. Data is collected only while shown." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<District mode>\n" +
                    "1. Choose <By district> above.\n" +
                    "2. Create or select a district in the city.\n" +
                    "3. Open <Policies> and enable <No Street Parking>.\n" +
                    "Roads outside selected districts keep normal street parking." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "No Street Parking" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "In By District mode, new street parking is blocked here. Existing parked cars leave naturally." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Street Parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Parked> = cars still parked on streets covered by the selected mode.\n" +
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
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "No city loaded yet." },
                { ParkingStatusLocale.kCollecting, "Parking status is being collected..." },
                { ParkingStatusLocale.kUnavailable, "Parking status is unavailable." },
                { ParkingStatusLocale.kCollectionFailed, "Parking status could not be collected; see ParkingControl.log." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} parked ({1} lanes) | {2}/{3} disabled | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} street | {1} visible | {2} hidden | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} public | {3}  {4} / {5} building" },
                { ParkingStatusLocale.kShareFormat, "{0} street parked | {1} moving | updated {2}" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "OFF" },
                { ParkingStatusLocale.kStatusCheck, "CHECK" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

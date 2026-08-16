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
    using Game.Vehicles;
    using Unity.Entities;

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
                    "Select a dropdown choice to block street parking or turn this Off.\n" +
                    "**Whole City** or\n" +
                    "**by District**.\n" +
                    "- Eligible Lanes are flagged to prevent new street parking.\n" +
                    "- Existing parked cars leave naturally over time when it is next used.\n" +
                    "- Of course, fee-based parking lots and normal building parking are still useable and not affected.\n" +
                    "**Highways, and asymetric 3-lane roads are examples of vanilla roads that are not eligible for street parking already.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Whole City" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "by District" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "OFF" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Show instructions" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Shows how to use <District> mode.\n" +
                    "Off = mod does not run.\n" +
                    "Whole city = all city gets no street parking flags."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Show status" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Show current parking totals below.>\n" +
                    "Data is collected for status only while in the Options menu, no city performance impact." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<District mode>\n" +
                    "1. Choose <by District> above.\n" +
                    "2. Create/select a district in the city.\n" +
                    "3. <Policies>: open + enable **Roadside Parking Ban [✓]**.\n" +
                    "Roads outside selected districts keep normal street parking." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Roadside Parking Ban" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "Select By District in Parking Control Options to block new street parking here. " +
                    "Existing parked cars leave when their citizens next use them." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Street Parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Parked> = cars still parked on streets covered by the selected mode.\n" +
                    "<Lanes> = roadside parking sections holding those cars. One lane can hold many.\n" +
                    "<Disabled> = street-parking lanes closed to new parking.\n" +
                    "<By District> shows affected / total city lanes and enabled / total districts.\n" +
                    "<CHECK> = roads may still be updating. Wait a moment; write a log report if it remains.\n" +
                    "**Some cars can remain after enabling the no-street rule or changing roads. They leave naturally if the citizen uses the car.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Car Locations" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Street> = parked on public roads.\n" +
                    "<Visible> = cars you can see and click in open-air lots or outdoor parking included with buildings.\n" +
                    "<Hidden> = inside buildings or garages.\n" +
                    "<OC> = outside connection storage at the city border; some incoming household cars start there.\n" +
                    "**Some hidden cars have no assigned parking lane; their diagnostic count is shown only in the log.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parking spaces" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Shows parking occupancy.\n" +
                    "<Public> spaced used = facilities counted by the vanilla Parking InfoView.\n" +
                    "<Building> spaces used = parking included with homes, workplaces, commercial shops.\n" +
                    "**High percentages indicates a need for more parking facilities.**"
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
                    "Write street-parking and related details to \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "If curious, write a 2nd report later in the same loaded city.\n" +
                    "- Compares up to 20 sampled street, OC, and unassigned Entity IDs.\n" +
                    "- Shows if each sample stayed, started driving, parked elsewhere, or disappeared."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Open log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Open <Logs/ParkingControl.log>, or the Logs folder if the file does not exist yet." },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "No city loaded yet." },
                { ParkingStatusLocale.kCollecting, "Parking status is being collected..." },
                { ParkingStatusLocale.kUnavailable, "Parking status is unavailable." },
                { ParkingStatusLocale.kCollectionFailed, "Parking status could not be collected; see ParkingControl.log." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} parked ({1} lanes) | {2}/{3} disabled | {4}" },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} parked ({1} lanes) | {2}/{3} disabled{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} parked ({1}/{2} lanes) | {3}/{4} disabled | {5}/{6} districts{7}" },
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

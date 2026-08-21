// <copyright file="LocaleEN.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: English text for Parking Control's Options UI.

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
        /// <param name="settings">Options settings whose localization IDs are used.</param>
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
                // Options tabs and groups.
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Actions" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "About" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Street parking" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Personal vehicle status" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Mod information" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Links" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostics" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "No street parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Choose:\n" +
                    "**by District**\n" +
                    "**Whole City**\n" +
                    " or **OFF**.\n" +
                    "- Eligible lanes are flagged to prevent new street parking.\n" +
                    "- Existing parked cars leave naturally when they are next used.\n" +
                    "- Fee-based parking lots and normal building parking remain usable.\n" +
                    "**Some roads already exclude street parking, like Highways and asymmetric 3-lane roads.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Whole City" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "by District" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "OFF" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Show instructions" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Shows how to use <by District> mode.\n" +
                    "1.a. OFF = citywide and district restrictions are disabled; mostly back to game defaults.\n" +
                    "1.b. Single-road <No Parking> button in the Road Services panel still applies just like applying a crosswalk.\n" +
                    "2. Whole City = blocks all city eligible street public parking."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Show status" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Show current parking totals below.>\n" +
                    "Status is collected only while the Options menu is open; " +
                    "no background status scan runs during city play." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<District mode>\n" +
                    "1. Choose <by District> above.\n" +
                    "2. Create/select a district in the city.\n" +
                    "3. Open the <Policies> panel and enable **Roadside Parking Ban [✓]**.\n" +
                    "4. It's okay to have both the Ban and the Parking fee enabled. Fee is charged to any cars still remaining or sneaking in.\n" +
                    "Roads outside banned parking districts keep normal street parking." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },


                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "No Parking" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]",
                    "Toggle roadside parking on one side of a road." },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Roadside Parking Ban" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Prevents cars and motorcycles from parking on roadsides in this district. " +
                    "Existing parked vehicles leave when their owners next use them." },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Street Parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Parked> = cars still parked on streets banned by the selected mode.\n" +
                    "<Lanes> = roadside parking sections holding those cars. One lane can hold many.\n" +
                    "<Disabled> = street-parking lanes closed to new parking.\n" +
                    "<by District> shows:\n" +
                    "- Occupied lanes in banned districts / occupied lanes citywide.\n" +
                    "- Disabled lanes / eligible city lanes.\n" +
                    "- Enabled districts / total districts.\n" +
                    "<New or rebuilt roads> may briefly accept a few cars while their lanes update. " +
                    "Cars already parked leave naturally when citizens use them.\n" +
                    "<CHECK> = some selected roads are not blocked yet. Run the city briefly and check again. " +
                    "If <CHECK> remains, include a parking log report when asking for help." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Street use" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "This row includes the <whole city>, not just districts.\n" +
                    "<Street parked> = percentage parked on streets instead of in public or building parking.\n" +
                    "<Active> = personal vehicles driving or waiting in traffic.\n" +
                    "<Formula> = street ÷ (street + occupied public + occupied building).\n" +
                    "**Outside connection (OC) storage and cars without an assigned parking lane are excluded.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parking spaces" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Shows citywide parking occupancy.\n" +
                    "<Public> spaces used = facilities counted by the vanilla Parking InfoView.\n" +
                    "<Building> spaces used = parking included with homes, workplaces, and shops.\n" +
                    "**Higher % usage = more parking may be needed.**" },


                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Car Locations" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "This row shows citywide data, not only districts with the ban.\n" +
                    "<Street> = parked on public roads.\n" +
                    "<Visible> = cars you can see and click in open-air lots or outdoor parking included with buildings.\n" +
                    "<Inside> = in buildings or garages.\n" +
                    "<OC> = outside connection storage at the city border; some incoming household cars start there (staging area).\n" +
                    "Cars without an assigned parking lane are omitted here and shown only in the log report (About tab)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Updated" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "Time when these citywide status values were last refreshed." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod name" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods link" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Open the author's page on Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Write parking report" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Write street-parking and related details to \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "If curious, write a 2nd report later in the same loaded city.\n" +
                    "- Compares up to 20 sample Entity IDs from different categories.\n" +
                    "- Shows if each sample stayed, started driving, parked elsewhere, or disappeared.\n" +
                    "- Scene Explorer mod is needed to track the Entity ID numbers in the city."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Open log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Open <Logs/ParkingControl.log>, or the Logs folder if the file does not exist yet." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "No city loaded yet." },
                { ParkingStatusLocale.kCollecting, "Parking status is being collected..." },
                { ParkingStatusLocale.kUnavailable, "Parking status is unavailable." },
                { ParkingStatusLocale.kCollectionFailed, "Parking status could not be collected; see ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} parked ({1} lanes) | {2}/{3} disabled{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} parked ({1}/{2} lanes) | {3}/{4} disabled | {5}/{6} districts{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} street | {1} visible | {2} inside | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} public {1}/{2} | {3} building {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} street parked {1} | {2} active" },
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

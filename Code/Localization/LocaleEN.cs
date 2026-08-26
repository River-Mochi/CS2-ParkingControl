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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Debug" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "No street parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Choose:\n" +
                    "<1. by District>\n" +
                    "<2. Whole City>\n" +
                    "<3. OFF>\n" +
                    "- Eligible lanes are flagged to prevent new street parking.\n" +
                    "- Existing parked cars leave naturally when they are next used; use <Relocate parked cars> to ask CS2 to move them sooner.\n" +
                    "- Fee-based parking lots and normal building parking remain usable.\n" +
                    "**Some roads already exclude street parking, like Highways and small 2-way alley roads.**"
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
                    "Roads outside banned parking districts keep normal street parking."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.RelocateParkedCars)),
                    "Relocate parked cars" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.RelocateParkedCars)),
                    "One-time action: asks CS2's own parking relocation system to move cars still parked on curb lanes currently banned by Parking Control.\n" +
                    "Works with Whole City, District, and manual No Parking bans.\n" +
                    "Cars are not deleted. Close Options and run the city after clicking." },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "No Parking" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]",
                    "Toggle roadside parking on one side of a road. For multiple sides, drag over them before releasing the Left Mouse button." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Roadside Parking Ban" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Prevents cars and motorcycles from parking on roadsides in this district.\n" +
                    "- Existing parked cars move gradually; large banned areas take longer."
                },

                // Native mouse action hints for the No Parking road tool.
                {
                    $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]",
                    "Upgrade"
                },
                {
                    $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]",
                    "Downgrade"
                },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Street Parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Shows only the selected <Whole City> or <by District> Parking Ban scope. Manual No Parking roads are listed separately.\n" +
                    "<Parked> = cars still parked on streets covered by the selected scope.\n" +
                    "<Disabled> = disabled curb-lane sections / target curb-lane sections.\n" +
                    "<Districts> = districts with the Parking Ban / total districts.\n" +
                    "<---------------------->\n" +
                    "**After changing or rebuilding roads, the disabled count may need a little time while CS2 rebuilds parking lanes. " +
                    "Run the city briefly and reopen Options > Status. If CHECK remains, use About > Debug > Write Report and send Logs/ParkingControl.log and your mod list.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Manual No Parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Shows only road sides set with Parking Control's manual <No Parking> road tool.\n" +
                    "<Parked> = cars still parked on those manually banned road sides.\n" +
                    "<Disabled> = disabled curb-lane sections / manually targeted curb-lane sections.\n" +
                    "Manual bans can overlap Whole City or District bans, so do not add this row to <Street Parking> totals.\n" +
                    "**If CHECK remains after running the city briefly, use About > Debug > Write Report.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Parking use" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Public> = occupied parking spaces in parking facilities.\n" +
                    "Roughly matches CS2's Roads parking InfoView panel.\n" +
                    "<Bldg> = vehicles parked at buildings or garages.\n" +
                    "<Street> = cars parked on streets.\n" +
                    "<Total> = total known in-city parked cars (street + public + building).\n" +
                    "**Outside connections and unknown staging are excluded from the total.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parking rating" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Rates how much exact-capacity parking is still <free>.\n" +
                    "<POOR> = less than 15% free.\n" +
                    "<OK> = 15% to less than 30% free.\n" +
                    "<GOOD> = 30% or more free.\n" +
                    "<Public> uses parking facilities counted by CS2's Roads parking InfoView.\n" +
                    "<Bldg> uses exact-capacity parking at buildings and garages."
                },

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
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Write Report" },
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
                    "{0} parked | {1}/{2} disabled{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat,
                    "{0} parked | {1}/{2} disabled lanes{3}" },

                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} parked | {1}/{2} disabled | {3}/{4} districts{5}" },
                { ParkingStatusLocale.kVehicleFormat,
                    "{0} street | {1} visible | {2} inside | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat,
                    "{0} = {1} public free | {2} = {3} bldg free" },
                { ParkingStatusLocale.kShareFormat,
                    "{0} public | {1} bldg | {2} street | {3} total" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "OFF" },
                { ParkingStatusLocale.kStatusCheck, "CHECK" },
                { ParkingStatusLocale.kRatingPoor, "POOR" },
                { ParkingStatusLocale.kRatingGood, "GOOD" },
                { ParkingStatusLocale.kRatingNA, "N/A" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

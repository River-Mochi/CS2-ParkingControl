// <copyright file="ParkingStatusSystem.Report.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Writes parking report summaries, enforcement details, and district totals.

namespace ParkingControl
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CS2Shared.RiverMochi;
    using Unity.Entities;

    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Writes the current snapshot, identity transitions, and entity samples to the mod log.
        /// </summary>
        private void WriteReport(ParkingSnapshot snapshot, ParkingReportDetails details)
        {
            int otherDisabledCurbLanes = Math.Max(
                0,
                snapshot.DisabledCurbLanes - snapshot.TrackedCurbLanes);

            bool districtScope =
                snapshot.Scope == PCSettings.ParkingScope.ByDistrict;

            bool offScope =
                snapshot.Scope == PCSettings.ParkingScope.Off;

            bool manualOnlyScope =
                offScope && snapshot.TargetCurbLanes > 0;

            bool useTargetSubset =
                districtScope || manualOnlyScope;

            int enforcementCurbLanes =
                useTargetSubset
                    ? snapshot.TargetCurbLanes
                    : offScope
                        ? 0
                        : snapshot.CurbLanes;

            int enforcementDisabledCurbLanes =
                useTargetSubset
                    ? snapshot.DisabledTargetCurbLanes
                    : offScope
                        ? 0
                        : snapshot.DisabledCurbLanes;

            string enforcementStatus = GetOwnershipStatus(
                snapshot.RestrictionEnabled,
                enforcementCurbLanes,
                enforcementDisabledCurbLanes,
                snapshot.TrackedCurbLanes);

            string enforcementDetails = GetOwnershipDetails(
                snapshot.RestrictionEnabled,
                enforcementCurbLanes,
                enforcementDisabledCurbLanes,
                snapshot.TrackedCurbLanes);

            StringBuilder text = new(8192);
            text.AppendLine();
            text.AppendLine($"==================== {Mod.ModTag} PARKING REPORT ====================");
            text.AppendLine("-------------------- SUMMARY --------------------");
            text.AppendLine($"Mod={Mod.ModName} v{Mod.ModVersion}");
            text.AppendLine(
                $"SimulationFrame={snapshot.SimulationFrame} (simulation tick when data was collected)");
            text.AppendLine($"ParkingScope={snapshot.Scope}");
            text.AppendLine(
                $"DistrictPolicy=Active in {snapshot.DistrictsWithPolicy}/{snapshot.Districts} districts " +
                $"(PolicyEntity={FormatEntity(ParkingPolicySystem.PolicyEntity)})");
            text.AppendLine();
            text.AppendLine("-------------------- STREET PARKING CONTROL --------------------");
            text.AppendLine($"EligibleStreetParkingLanes={snapshot.CurbLanes}");
            text.AppendLine(
                $"TargetStreetParkingLanes={snapshot.TargetCurbLanes} " +
                $"(Disabled={snapshot.DisabledTargetCurbLanes}, " +
                $"Occupied={snapshot.OccupiedTargetCurbLanes}, " +
                $"ParkedCars={snapshot.TargetStreetParked})");
            AppendDistrictStreetParking(text, snapshot, details);
            text.AppendLine(
                $"DisabledStreetParkingLanes={snapshot.DisabledCurbLanes} " +
                $"(ParkingControlTracked={snapshot.TrackedCurbLanes}, VanillaOrOther={otherDisabledCurbLanes})");
            text.AppendLine($"EnforcementStatus={enforcementStatus}");
            text.AppendLine($"EnforcementDetails={enforcementDetails}");
            text.AppendLine(
                $"OccupiedStreetParkingLanes={snapshot.OccupiedCurbLanes}/{snapshot.CurbLanes} " +
                $"({FormatPercent(snapshot.OccupiedCurbLanes, snapshot.CurbLanes)} of lane entities; not a parking-space percentage)");
            text.AppendLine(
                $"FixedSlotStreetParking={snapshot.FixedSlotCurbParked}/{snapshot.FixedSlotCurbCapacity} " +
                $"across {snapshot.FixedSlotCurbLanes} fixed-slot lane entities");
            text.AppendLine(
                $"ContinuousStreetParking={snapshot.ContinuousCurbParked} vehicles across " +
                $"{snapshot.ContinuousCurbLanes} continuous lane entities (no exact slot capacity)");
            text.AppendLine();
            text.AppendLine("-------------------- PERSONAL VEHICLES --------------------");
            text.AppendLine(
                $"PersonalMotorVehicles={snapshot.TotalVehicles} " +
                $"(Parked={snapshot.ParkedVehicles}, Active={snapshot.ActiveVehicles}, Neither={snapshot.UnlocatedVehicles})");
            text.AppendLine(
                $"PersonalVehicleOwnership=Household={snapshot.HouseholdOwnerVehicles} " +
                $"(Live={snapshot.LiveHouseholdOwnerVehicles}, Deleted={snapshot.DeletedHouseholdOwnerVehicles}, " +
                $"OwnedVehicleBufferMatched={snapshot.OwnedVehicleMatches}, Missing={snapshot.OwnedVehicleMissing}), " +
                $"MissingOwner={snapshot.MissingOwnerVehicles}, DummyTraffic={snapshot.DummyTrafficVehicles}, " +
                $"OtherOrUnowned={snapshot.OtherOrUnownedVehicles}");
            text.AppendLine(
                $"PersonalVehicleKinds=ResidentHousehold={snapshot.ResidentHouseholdVehicles} " +
                $"(NotMovedIn={snapshot.ResidentNotMovedInVehicles}), " +
                $"TouristHousehold={snapshot.TouristHouseholdVehicles}, " +
                $"CommuterHousehold={snapshot.CommuterHouseholdVehicles}, " +
                $"DummyTraffic={snapshot.DummyTrafficVehicles}, OtherOrUnowned={snapshot.OtherOrUnownedVehicles}");
            text.AppendLine();
            text.AppendLine("-------------------- PARKING LOCATIONS --------------------");
            text.AppendLine($"ParkedOnStreets={snapshot.StreetParked}");
            text.AppendLine(
                $"ParkedElsewhere={snapshot.ParkedElsewhere} " +
                $"(VisibleOffStreet={snapshot.VisibleOffStreet}, HiddenInBuildings={snapshot.HiddenInBuildings}, " +
                $"OutsideConnection={snapshot.OutsideConnection}, UnassignedOrUnknown={snapshot.UnassignedOrUnknownParked})");
            text.AppendLine(
                $"UnknownParkedDetails={snapshot.UnassignedOrUnknownParked} total " +
                $"(NullLane={snapshot.UnknownNullLane}, MissingLaneEntity={snapshot.UnknownMissingLane}, " +
                $"Unspawned={snapshot.UnknownUnspawned})");
            text.AppendLine(
                $"UnknownOwnership=ValidLiveHousehold={snapshot.UnknownValidHouseholdOwned}, " +
                $"HouseholdInvalid={snapshot.UnknownHouseholdOwnershipInvalid}, " +
                $"DummyTraffic={snapshot.UnknownDummyTraffic}, " +
                $"OtherOrUnowned={snapshot.UnknownOtherOrUnowned}");
            text.AppendLine(
                $"UnknownVehicleKinds=ResidentHousehold={snapshot.UnknownResidentHousehold} " +
                $"(NotMovedIn={snapshot.UnknownResidentNotMovedIn}), " +
                $"TouristHousehold={snapshot.UnknownTouristHousehold}, " +
                $"CommuterHousehold={snapshot.UnknownCommuterHousehold}");
            text.AppendLine(
                $"UnknownLikelyIncomingStaging={snapshot.UnknownResidentNotMovedIn} " +
                "(inference: valid resident household without MovedIn)");
            text.AppendLine(
                $"UnknownTripSources=Present={snapshot.UnknownWithTripSource}, " +
                $"Absent={snapshot.UnknownWithoutTripSource} " +
                $"(OutsideConnection={snapshot.UnknownTripSourceOutside}, " +
                $"Building={snapshot.UnknownTripSourceBuilding}, " +
                $"Other={snapshot.UnknownTripSourceOther}, MissingEntity={snapshot.UnknownTripSourceMissing})");
            text.AppendLine(
                $"VisibleParkingKinds=Facility={snapshot.VisibleFacilityParking}, " +
                $"BuildingLot={snapshot.VisibleBuildingParking}, Other={snapshot.VisibleOtherParking}");
            text.AppendLine($"OutsideConnectionHiddenUnspawned={snapshot.OutsideConnectionHidden}");
            text.AppendLine(
                $"OutsideConnectionOwnership=ValidLiveHousehold={snapshot.OutsideValidHouseholdOwned}, " +
                $"HouseholdInvalid={snapshot.OutsideHouseholdOwnershipInvalid}, " +
                $"DirectOutsideConnectionOwner={snapshot.OutsideConnectionOwner}, " +
                $"OtherOrUnowned={snapshot.OutsideOtherOrUnowned}");
            text.AppendLine(
                $"OutsideConnectionVehicleKinds=ResidentHousehold={snapshot.OutsideResidentHousehold} " +
                $"(NotMovedIn={snapshot.OutsideResidentNotMovedIn}), " +
                $"TouristHousehold={snapshot.OutsideTouristHousehold}, " +
                $"CommuterHousehold={snapshot.OutsideCommuterHousehold}, " +
                $"DummyTraffic={snapshot.OutsideDummyTraffic}");
            text.AppendLine();
            text.AppendLine("-------------------- PARKING SUPPLY --------------------");
            text.AppendLine(
                $"VanillaRoadsInfoviewParking={snapshot.OfficialParkingOccupied}/{snapshot.OfficialParkingCapacity} " +
                $"across {snapshot.OfficialParkingFacilities} facility entities");
            text.AppendLine(
                $"BuildingParking={snapshot.BuildingParkingOccupied}/{snapshot.BuildingParkingCapacity} " +
                $"occupied/capacity across {snapshot.BuildingParkingLanes} exact-capacity lane entities " +
                $"(VisibleFixedSlot={snapshot.BuildingFixedSlotLanes}, HiddenGarage={snapshot.BuildingGarageLanes})");
            text.AppendLine(
                $"BuildingContinuousParking={snapshot.BuildingContinuousOccupied} parked cars across " +
                $"{snapshot.BuildingContinuousLanes} unslotted lane entities excluded from capacity percentage");
            text.AppendLine(
                $"KnownInCityParking={snapshot.KnownInCityParking} " +
                $"(Street={snapshot.StreetParked}, Public={snapshot.OfficialParkingOccupied}, " +
                $"Building={snapshot.BuildingParkingOccupied}); " +
                $"StreetUsage={FormatPercent(snapshot.StreetParked, snapshot.KnownInCityParking)}");
            text.AppendLine(
                $"NonBorderGarageLanes={snapshot.GarageOccupied}/{snapshot.GarageCapacity} occupied/capacity " +
                $"across {snapshot.GarageLanes} garage lane entities");
            text.AppendLine();
            text.AppendLine("-------------------- CHANGES SINCE PREVIOUS REPORT --------------------");
            text.AppendLine("ChangeSincePrevious:");
            if (m_HasPreviousReport)
            {
                text.AppendLine(
                    $"    PersonalMotorVehicles={FormatDelta(snapshot.TotalVehicles - m_PreviousReport.TotalVehicles)}");
                text.AppendLine(
                    $"    StreetParking={FormatDelta(snapshot.StreetParked - m_PreviousReport.StreetParked)}");
                text.AppendLine(
                    $"    ParkedElsewhere={FormatDelta(snapshot.ParkedElsewhere - m_PreviousReport.ParkedElsewhere)}");
                text.AppendLine(
                    $"    OutsideConnection={FormatDelta(snapshot.OutsideConnection - m_PreviousReport.OutsideConnection)}");
                text.AppendLine(
                    $"    OutsideHidden={FormatDelta(snapshot.OutsideConnectionHidden - m_PreviousReport.OutsideConnectionHidden)}");
            }
            else
            {
                text.AppendLine("    <first report for this loaded city>");
            }

            text.AppendLine();
            text.AppendLine("-------------------- SAMPLE TRANSITIONS SINCE PREVIOUS REPORT --------------------");
            AppendSampleTransitions(text, details);
            text.AppendLine();
            text.AppendLine("-------------------- SAMPLE ENTITY IDS --------------------");
            AppendVehicleEntitySamples(text, snapshot, details);
            text.AppendLine();
            text.AppendLine("-------------------- NOTES --------------------");
            text.AppendLine(
                "Note: existing curb-parked cars leave when a keeper next uses them.");
            text.AppendLine(
                "Note: PersonalCar.m_Keeper is the current reserver/user, not the persistent vehicle owner.");
            text.AppendLine(
                "Note: OutsideConnection describes the parking lane/root location, not the vehicle Owner. " +
                "Valid household-owned cars at that location are legit and should probably not all be deleted.");
            text.AppendLine(
                "Note: Unknown parked cars have no usable concrete lane. Vanilla can leave an unspawned car " +
                "at its trip source when initial parking assignment fails; TripSource may later be removed.");
            text.AppendLine(
                "Note: UnknownLikelyIncomingStaging is inferred from household state because the original " +
                "TripSource is no longer retained; it is not proof of the car's exact location.");
            text.AppendLine(
                "Note: HiddenInBuildings means a non-border GarageLane, or an unspawned vehicle on a lane owned by a building.");
            text.AppendLine(
                "Note: VisibleOffStreet means a rendered parked car that is not on an eligible public street, " +
                "inside hidden storage, or at an outside connection; VisibleParkingKinds provides the narrower split.");
            text.AppendLine(
                "Note: VanillaRoadsInfoviewParking excludes ordinary street curbs and most implicit residential storage; " +
                "continuous unslotted lanes have no exact capacity.");
            text.AppendLine(
                "Note: BuildingParking excludes vanilla public parking and includes car spaces owned by residential, " +
                "mixed, commercial, office, industrial, and specialized-industry buildings.");
            text.AppendLine(
                "Note: StreetUsage compares street cars only with known street, public, and building parking; " +
                "outside-connection and unknown staging are excluded.");
            text.AppendLine(
                "Note: Entity IDs use Index:Version for Scene Explorer mod use.");
            text.AppendLine(
                $"Note: SampleTransitions trace up to {kVehicleSampleLimit} IDs from each previous " +
                "Street, OutsideConnection, and Unknown group; they do not represent every vehicle.");
            text.AppendLine(
                "Note: NoLongerTracked means the entity still exists but no longer matches this " +
                "report's current personal-vehicle query.");
            text.Append($"==================== {Mod.ModTag} END OF PARKING REPORT ====================");
            LogUtils.Info(text.ToString());

            m_PreviousReport = snapshot;
            m_PreviousDistrictStreetCars.Clear();
            foreach (DistrictParkingStats district in details.DistrictParking.Values)
            {
                m_PreviousDistrictStreetCars[district.District] = district.StreetCars;
            }

            ReplaceSamples(m_PreviousStreetSamples, details.StreetSamples);
            ReplaceSamples(m_PreviousOutsideSamples, details.OutsideSamples);
            ReplaceSamples(m_PreviousUnknownSamples, details.UnknownSamples);
            m_HasPreviousReport = true;
        }

        private static void ReplaceSamples(List<Entity> destination, List<Entity> source)
        {
            destination.Clear();
            destination.AddRange(source);
        }

        /// <summary>
        /// Lists each district by player-facing name so repeated reports show local trends.
        /// </summary>
        private void AppendDistrictStreetParking(
            StringBuilder text,
            ParkingSnapshot snapshot,
            ParkingReportDetails details)
        {
            List<DistrictParkingStats> districts =
                new(details.DistrictParking.Values);
            districts.Sort((left, right) => string.Compare(
                GetDistrictName(left.District),
                GetDistrictName(right.District),
                StringComparison.CurrentCultureIgnoreCase));

            text.AppendLine(
                "District details (lane counts are roadside parking sections, not individual spaces):");
            foreach (DistrictParkingStats district in districts)
            {
                bool effective = snapshot.Scope == PCSettings.ParkingScope.WholeCity ||
                    (snapshot.Scope == PCSettings.ParkingScope.ByDistrict && district.PolicyActive);

                string status = effective
                    ? GetOwnershipStatus(
                        restrictionEnabled: true,
                        district.EligibleLanes,
                        district.DisabledLanes,
                        district.TrackedLanes)
                    : "OFF";

                string change = "<first>";
                if (m_HasPreviousReport)
                {
                    change = m_PreviousDistrictStreetCars.TryGetValue(
                        district.District,
                        out int previousCars)
                        ? FormatDelta(district.StreetCars - previousCars)
                        : "<new>";
                }

                text.AppendLine(
                    $"  {GetDistrictName(district.District)} [{FormatEntity(district.District)}] | " +
                    $"Policy={(district.PolicyActive ? "ON" : "OFF")} | " +
                    $"{district.StreetCars} parked ({district.OccupiedLanes} lanes) | " +
                    $"{district.DisabledLanes}/{district.EligibleLanes} disabled | " +
                    $"{status} | Change={change}");
            }
        }

        private string GetDistrictName(Entity district)
        {
            if (district == Entity.Null)
            {
                return "<No district>";
            }

            if (!EntityManager.Exists(district))
            {
                return "<Missing district>";
            }

            string name = m_NameSystem.GetRenderedLabelName(district);
            return string.IsNullOrWhiteSpace(name)
                ? $"District {FormatEntity(district)}"
                : name.Replace('\r', ' ').Replace('\n', ' ').Replace('|', '/');
        }


        /// <summary>
        /// Describes whether targeted street-parking lanes match the current restriction setting.
        /// </summary>
        internal static string GetOwnershipStatus(
            bool restrictionEnabled,
            int curbLanes,
            int disabledCurbLanes,
            int trackedCurbLanes)
        {
            if (!restrictionEnabled)
            {
                return trackedCurbLanes == 0
                    ? "OFF"
                    : "CHECK";
            }

            if (curbLanes > 0 && trackedCurbLanes == 0)
            {
                return "CHECK";
            }

            if (disabledCurbLanes < curbLanes)
            {
                return "CHECK";
            }

            if (disabledCurbLanes < trackedCurbLanes)
            {
                return "CHECK";
            }

            return "OK";
        }

        /// <summary>
        /// Keeps technical details in the manual log rather than the Options UI.
        /// </summary>
        private static string GetOwnershipDetails(
            bool restrictionEnabled,
            int curbLanes,
            int disabledCurbLanes,
            int trackedCurbLanes)
        {
            if (!restrictionEnabled)
            {
                return trackedCurbLanes == 0
                    ? "Restriction off; no Parking Control lanes remain"
                    : "Restriction off but Parking Control lanes remain";
            }

            if (curbLanes > 0 && trackedCurbLanes == 0)
            {
                return "Restriction on but Parking Control owns no targeted lanes";
            }

            if (disabledCurbLanes < curbLanes)
            {
                return "Some targeted street-parking lanes are not disabled at snapshot time; this can be temporary after changing or rebuilding roads";
            }

            if (disabledCurbLanes < trackedCurbLanes)
            {
                return "Some Parking Control lanes are not disabled";
            }

            return "Restriction on and targeted street-parking lanes are disabled";
        }

        private static string FormatDelta(int value)
        {
            return value.ToString("+#;-#;0");
        }

        private static string FormatPercent(int numerator, int denominator)
        {
            if (denominator <= 0)
            {
                return "0.00%";
            }

            return (numerator * 100d / denominator).ToString("0.00") + "%";
        }

        private static string FormatEntity(Entity entity)
        {
            return entity == Entity.Null
                ? "Null"
                : $"{entity.Index}:{entity.Version}";
        }
    }
}

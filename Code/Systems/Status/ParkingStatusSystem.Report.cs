// <copyright file="ParkingStatusSystem.Report.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Builds identity-based parking reports and Scene Explorer entity samples.

namespace ParkingControl
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CS2Shared.RiverMochi;
    using Unity.Entities;

    public sealed partial class ParkingStatusSystem
    {
        private const int kVehicleSampleCount = 10;

        /// <summary>
        /// Records one vehicle in the manual report's identity and transition collections.
        /// </summary>
        private void AddReportVehicle(
            ParkingReportDetails details,
            Entity vehicle,
            VehicleLocation location,
            bool isParked)
        {
            // Index:Version is stable only for this loaded city session, so history is
            // cleared on every new/load callback in the main partial.
            bool wasStreetPrevious = m_HasPreviousReport && m_PreviousStreetVehicles.Contains(vehicle);
            if (wasStreetPrevious)
            {
                details.SeenPreviousStreet++;
                switch (location)
                {
                    case VehicleLocation.Active:
                        details.NowActive++;
                        break;
                    case VehicleLocation.StreetCurb:
                        details.RemainedOnStreet++;
                        break;
                    case VehicleLocation.VisibleOffStreet:
                        details.NowOffStreet++;
                        break;
                    case VehicleLocation.HiddenInBuilding:
                        details.NowHiddenInBuilding++;
                        break;
                    case VehicleLocation.OutsideConnection:
                        details.NowAtOutsideConnection++;
                        break;
                    default:
                        details.NowUnassignedOrUnknown++;
                        break;
                }
            }

            if (location == VehicleLocation.StreetCurb)
            {
                details.CurrentStreetVehicles.Add(vehicle);
                if (!wasStreetPrevious)
                {
                    details.NewlyParkedOnStreet++;
                }
            }

            // Store IDs by location only for an on-demand report. The writer prints
            // at most ten from each group so Scene Explorer samples stay practical.
            switch (location)
            {
                case VehicleLocation.VisibleOffStreet:
                    details.VisibleSamples.Add(vehicle);
                    break;
                case VehicleLocation.HiddenInBuilding:
                    details.HiddenSamples.Add(vehicle);
                    break;
                case VehicleLocation.OutsideConnection:
                    details.OutsideSamples.Add(vehicle);
                    break;
                default:
                    if (isParked && location != VehicleLocation.StreetCurb)
                    {
                        details.UnknownSamples.Add(vehicle);
                    }

                    break;
            }
        }

        /// <summary>
        /// Writes the current snapshot, identity transitions, and entity samples to the mod log.
        /// </summary>
        private void WriteReport(ParkingSnapshot snapshot, ParkingReportDetails details)
        {
            int otherDisabledCurbLanes = Math.Max(
                0,
                snapshot.DisabledCurbLanes - snapshot.TrackedCurbLanes);
            string enforcementStatus = GetOwnershipStatus(
                snapshot.RestrictionEnabled,
                snapshot.Scope == PCSettings.ParkingScope.ByDistrict
                    ? snapshot.TargetCurbLanes
                    : snapshot.CurbLanes,
                snapshot.Scope == PCSettings.ParkingScope.ByDistrict
                    ? snapshot.DisabledTargetCurbLanes
                    : snapshot.DisabledCurbLanes,
                snapshot.TrackedCurbLanes);
            string enforcementDetails = GetOwnershipDetails(
                snapshot.RestrictionEnabled,
                snapshot.Scope == PCSettings.ParkingScope.ByDistrict
                    ? snapshot.TargetCurbLanes
                    : snapshot.CurbLanes,
                snapshot.Scope == PCSettings.ParkingScope.ByDistrict
                    ? snapshot.DisabledTargetCurbLanes
                    : snapshot.DisabledCurbLanes,
                snapshot.TrackedCurbLanes);
            string deltaLine = m_HasPreviousReport
                ? "ChangeSincePrevious: " +
                    $"PersonalMotorVehicles={FormatDelta(snapshot.TotalVehicles - m_PreviousReport.TotalVehicles)}, " +
                    $"StreetParking={FormatDelta(snapshot.StreetParked - m_PreviousReport.StreetParked)}, " +
                    $"ParkedElsewhere={FormatDelta(snapshot.ParkedElsewhere - m_PreviousReport.ParkedElsewhere)}, " +
                    $"OutsideConnection={FormatDelta(snapshot.OutsideConnection - m_PreviousReport.OutsideConnection)}, " +
                    $"OutsideHidden={FormatDelta(snapshot.OutsideConnectionHidden - m_PreviousReport.OutsideConnectionHidden)}"
                : "ChangeSincePrevious=<first report for this loaded city>";

            StringBuilder text = new StringBuilder(8192);
            text.AppendLine();
            text.AppendLine($"==================== {Mod.ModTag} PARKING REPORT ====================");
            text.AppendLine("-------------------- SUMMARY --------------------");
            text.AppendLine($"Mod={Mod.ModName} v{Mod.ModVersion}");
            text.AppendLine($"SimulationFrame={snapshot.SimulationFrame}");
            text.AppendLine($"ParkingScope={snapshot.Scope}");
            text.AppendLine(
                $"DistrictPolicy=ActiveIn{snapshot.DistrictsWithPolicy}/{snapshot.Districts}Districts " +
                $"(PolicyEntity={FormatEntity(ParkingPolicySystem.PolicyEntity)})");
            text.AppendLine();
            text.AppendLine("-------------------- STREET PARKING ENFORCEMENT --------------------");
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
            text.AppendLine(deltaLine);
            AppendStreetTransitions(text, snapshot, details);
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
            text.Append($"==================== {Mod.ModTag} END OF PARKING REPORT ====================");
            LogUtils.Info(text.ToString());

            m_PreviousReport = snapshot;
            m_PreviousDistrictStreetCars = new Dictionary<Entity, int>();
            foreach (DistrictParkingStats district in details.DistrictParking.Values)
            {
                m_PreviousDistrictStreetCars[district.District] = district.StreetCars;
            }

            m_PreviousStreetVehicles = details.CurrentStreetVehicles;
            m_HasPreviousReport = true;
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
                new List<DistrictParkingStats>(details.DistrictParking.Values);
            districts.Sort((left, right) => string.Compare(
                GetDistrictName(left.District),
                GetDistrictName(right.District),
                StringComparison.CurrentCultureIgnoreCase));

            text.AppendLine(
                "DistrictStreetParking=<lane counts are road-side parking sections, not spaces>");
            foreach (DistrictParkingStats district in districts)
            {
                bool effective = snapshot.Scope == PCSettings.ParkingScope.WholeCity ||
                    (snapshot.Scope == PCSettings.ParkingScope.ByDistrict && district.PolicyActive);
                string status = GetOwnershipStatus(
                    effective,
                    district.EligibleLanes,
                    district.DisabledLanes,
                    district.TrackedLanes);
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
                    $"{district.StreetCars} parked ({district.OccupiedLanes.Count} lanes) | " +
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
        /// Lists a small sample from each parked location for Scene Explorer inspection.
        /// </summary>
        private static void AppendVehicleEntitySamples(
            StringBuilder text,
            ParkingSnapshot snapshot,
            ParkingReportDetails details)
        {
            List<Entity> vehicles = new List<Entity>(details.CurrentStreetVehicles);
            text.AppendLine(
                "Samples=<up to 10 per parked location; enter Index:Version in Scene Explorer>");
            AppendEntitySampleLine(text, "Street", snapshot.StreetParked, vehicles);
            AppendEntitySampleLine(text, "Visible", snapshot.VisibleOffStreet, details.VisibleSamples);
            AppendEntitySampleLine(text, "Hidden", snapshot.HiddenInBuildings, details.HiddenSamples);
            AppendEntitySampleLine(text, "Outside", snapshot.OutsideConnection, details.OutsideSamples);
            AppendEntitySampleLine(
                text,
                "Unknown",
                snapshot.UnassignedOrUnknownParked,
                details.UnknownSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-OCSource",
                snapshot.UnknownTripSourceOutside,
                details.UnknownOutsideSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-BuildingSource",
                snapshot.UnknownTripSourceBuilding,
                details.UnknownBuildingSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-OtherSource",
                snapshot.UnknownTripSourceOther,
                details.UnknownOtherSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-MissingSource",
                snapshot.UnknownTripSourceMissing,
                details.UnknownMissingSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-NoTripSource",
                snapshot.UnknownWithoutTripSource,
                details.UnknownNoSourceSamples);
        }

        private static void AppendEntitySampleLine(
            StringBuilder text,
            string label,
            int total,
            List<Entity> vehicles)
        {
            vehicles.Sort(CompareEntities);
            int count = Math.Min(kVehicleSampleCount, vehicles.Count);
            text.Append($"  {label}={total} total; samples=");
            if (count == 0)
            {
                text.AppendLine("<none>");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (i != 0)
                {
                    text.Append(", ");
                }

                text.Append(FormatEntity(vehicles[i]));
            }

            text.AppendLine();
        }

        private static int CompareEntities(Entity left, Entity right)
        {
            int indexOrder = left.Index.CompareTo(right.Index);
            return indexOrder != 0
                ? indexOrder
                : left.Version.CompareTo(right.Version);
        }

        private void AppendStreetTransitions(
            StringBuilder text,
            ParkingSnapshot snapshot,
            ParkingReportDetails details)
        {
            if (!m_HasPreviousReport)
            {
                text.AppendLine(
                    $"StreetCarsSincePrevious=<first report; stored {snapshot.StreetParked} current street-car IDs as baseline>");
                return;
            }

            int deletedOrNoLongerExists = Math.Max(
                0,
                m_PreviousStreetVehicles.Count - details.SeenPreviousStreet);
            int leftStreetTotal = Math.Max(
                0,
                m_PreviousStreetVehicles.Count - details.RemainedOnStreet);
            int classifiedLeft =
                details.NowActive +
                details.NowOffStreet +
                details.NowHiddenInBuilding +
                details.NowAtOutsideConnection +
                details.NowUnassignedOrUnknown +
                deletedOrNoLongerExists;

            text.AppendLine("StreetCarsSincePrevious:");
            text.AppendLine($"  RemainedOnStreet={details.RemainedOnStreet}");
            text.AppendLine($"  NewlyParkedOnStreet={details.NewlyParkedOnStreet}");
            text.AppendLine($"  LeftStreetTotal={leftStreetTotal}");
            text.AppendLine($"  NowActive={details.NowActive}");
            text.AppendLine($"  NowOffStreet={details.NowOffStreet}");
            text.AppendLine($"  NowHiddenInBuilding={details.NowHiddenInBuilding}");
            text.AppendLine($"  NowAtOutsideConnection={details.NowAtOutsideConnection}");
            text.AppendLine($"  NowUnassignedOrUnknown={details.NowUnassignedOrUnknown}");
            text.AppendLine($"  DeletedOrNoLongerExists={deletedOrNoLongerExists}");
            text.AppendLine(
                $"  TransitionClassification={(classifiedLeft == leftStreetTotal ? "OK" : "WARNING")} " +
                $"(Classified={classifiedLeft})");
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
        /// Keeps technical enforcement details in the manual log rather than the Options value.
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
                return "Some targeted street-parking lanes are not disabled";
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

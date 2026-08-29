// <copyright file="ParkingStatusSystem.Report.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Writes parking report summaries, enforcement details, and district totals.

using System;
using System.Collections.Generic;
using System.Text;
using CS2Shared.RiverMochi;
using Unity.Entities;

namespace ParkingControl
{

    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Writes a support report in Release and additional research diagnostics in Debug.
        /// </summary>
        private void WriteReport(ParkingSnapshot snapshot, ParkingReportDetails details)
        {
            int otherDisabledCurbLanes = Math.Max(
                0,
                snapshot.DisabledCurbLanes - snapshot.TrackedCurbLanes);

#if DEBUG
            int buildingParkingAvailable = Math.Max(
                0,
                snapshot.BuildingParkingCapacity - snapshot.BuildingParkingUsedSlots);

            int classifiedBuildingGarageVehicles =
                snapshot.BuildingGarageCarOccupied +
                snapshot.BuildingGarageBicycleOccupied +
                snapshot.BuildingGarageUnknownVehicleOccupied;

            int buildingGarageRawMinusClassified =
                snapshot.BuildingGarageRawOccupied -
                classifiedBuildingGarageVehicles;
#endif

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
#if DEBUG
            text.AppendLine("Build=DEBUG");
#else
            text.AppendLine("Build=RELEASE");
#endif
            text.AppendLine(
                $"SimulationFrame={snapshot.SimulationFrame} (simulation tick when data was collected)");
            text.AppendLine($"ParkingScope={snapshot.Scope}");
            text.AppendLine(
                $"DistrictPolicy=Active in {snapshot.DistrictsWithPolicy}/{snapshot.Districts} districts " +
                $"(PolicyEntity={FormatEntity(ParkingPolicySystem.PolicyEntity)})");
            ParkingRelocationSystem? relocationSystem =
                World.GetExistingSystemManaged<ParkingRelocationSystem>();
            AppendAutomaticRelocationReport(text, relocationSystem?.GetReport());
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

            AppendUnresolvedTargetLanes(text, snapshot, details);
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
#if DEBUG
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
#endif
            text.AppendLine();
            text.AppendLine("-------------------- PARKING LOCATIONS --------------------");
            text.AppendLine($"ParkedOnStreets={snapshot.StreetParked}");
            text.AppendLine(
                $"ParkedElsewhere={snapshot.ParkedElsewhere} " +
                $"(VisibleOffStreet={snapshot.VisibleOffStreet}, HiddenInBuildings={snapshot.HiddenInBuildings}, " +
                $"OutsideConnection={snapshot.OutsideConnection}, UnassignedOrUnknown={snapshot.UnassignedOrUnknownParked})");

#if DEBUG
            text.AppendLine(
                $"UnknownParkedDetails={snapshot.UnassignedOrUnknownParked} total " +
                $"(NullLane={snapshot.UnknownNullLane}, MissingLaneEntity={snapshot.UnknownMissingLane}, " +
                $"Unspawned={snapshot.UnknownUnspawned}, " +
                $"NullLaneUnspawned={snapshot.UnknownNullLaneUnspawned})");

            text.AppendLine(
                $"LocationFallbacks=OutsideConnection={snapshot.OutsideConnection}, " +
                $"NullLaneUnspawned={snapshot.UnknownNullLaneUnspawned}");

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
#endif
            text.AppendLine();
            text.AppendLine("-------------------- PARKING SUPPLY --------------------");
            text.AppendLine(
                $"VanillaRoadsInfoviewParking={snapshot.OfficialParkingOccupied}/{snapshot.OfficialParkingCapacity} " +
                $"across {snapshot.OfficialParkingFacilities} facility entities");
           
            text.AppendLine(
                $"BuildingParkingCars={snapshot.BuildingParkingOccupied} " +
                $"(VisibleFixedSlotLanes={snapshot.BuildingFixedSlotLanes}, " +
                $"HiddenGarageLanes={snapshot.BuildingGarageLanes})");

#if DEBUG
            text.AppendLine(
                $"BuildingParkingSlots={snapshot.BuildingParkingUsedSlots}/" +
                $"{snapshot.BuildingParkingCapacity} used/capacity " +
                $"(Available={buildingParkingAvailable}, " +
                $"{FormatPercent(buildingParkingAvailable, snapshot.BuildingParkingCapacity)} free)");

            text.AppendLine(
                $"BuildingHiddenGarage=Capacity={snapshot.BuildingGarageCapacity}, " +
                $"RawUsed={snapshot.BuildingGarageRawOccupied}, " +
                $"Cars={snapshot.BuildingGarageCarOccupied}, " +
                $"Bicycles={snapshot.BuildingGarageBicycleOccupied}, " +
                $"UnknownPrefab={snapshot.BuildingGarageUnknownVehicleOccupied}, " +
                $"RawMinusClassified={buildingGarageRawMinusClassified}");

            text.AppendLine(
                $"GarageLaneDiagnostics=" +
                $"AllNonBorder={snapshot.GarageOccupied}/{snapshot.GarageCapacity} " +
                $"across {snapshot.GarageLanes} lanes; " +
                $"Primary={snapshot.GaragePrimaryOccupied}/{snapshot.GaragePrimaryCapacity} " +
                $"across {snapshot.GaragePrimaryLanes}; " +
                $"Slave={snapshot.GarageSlaveOccupied}/{snapshot.GarageSlaveCapacity} " +
                $"across {snapshot.GarageSlaveLanes}; " +
                $"NoConnection={snapshot.GarageWithoutConnectionOccupied}/" +
                $"{snapshot.GarageWithoutConnectionCapacity} " +
                $"across {snapshot.GarageWithoutConnectionLanes}; " +
                $"PrimaryNonCar={snapshot.GarageNonCarPrimaryLanes}; " +
                $"CarNonBuilding={snapshot.GarageCarNonBuildingLanes}");

            text.AppendLine(
                $"BuildingContinuousParking={snapshot.BuildingContinuousOccupied} parked cars across " +
                $"{snapshot.BuildingContinuousLanes} unslotted lane entities excluded from capacity percentage");
#endif
            text.AppendLine(
                $"KnownInCityParking={snapshot.KnownInCityParking} " +
                $"(Street={snapshot.StreetParked}, Public={snapshot.OfficialParkingOccupied}, " +
                $"Building={snapshot.BuildingParkingOccupied}); " +
                $"StreetUsage={FormatPercent(snapshot.StreetParked, snapshot.KnownInCityParking)}");

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
                text.AppendLine(
                    $"    NullLaneUnspawned={FormatDelta(snapshot.UnknownNullLaneUnspawned - m_PreviousReport.UnknownNullLaneUnspawned)}");
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
                "Note: roadside lane counts are lane sections, not individual parking spaces.");
            text.AppendLine(
                "Note: HiddenInBuildings is hidden garage/building storage; VisibleOffStreet is rendered off-street parking.");
            text.AppendLine(
                "Note: OutsideConnection is vanilla border storage/staging and is not automatically an error.");
            text.AppendLine(
                "Note: Unknown parked vehicles have no usable parking lane identified at snapshot time.");
            text.AppendLine(
                "Note: Entity IDs use Index:Version for Scene Explorer; sample transitions are samples, not every vehicle.");

#if DEBUG
            text.AppendLine();
            text.AppendLine("-------------------- DEBUG NOTES --------------------");
            text.AppendLine(
                "Note: PersonalCar.m_Keeper is the current reserver/user, not the persistent vehicle owner.");
            text.AppendLine(
                "Note: UnknownLikelyIncomingStaging is inferred from household state because the original " +
                "TripSource may no longer be retained; it is not proof of the car's exact location.");
            text.AppendLine(
                "Note: VanillaRoadsInfoviewParking excludes ordinary street curbs and most implicit residential storage; " +
                "continuous unslotted lanes have no exact slot capacity.");
            text.AppendLine(
                "Note: BuildingParkingCars is the motor-vehicle count used by the Options Status row and excludes bicycles.");
            text.AppendLine(
                "Note: BuildingParkingCapacity, BuildingParkingSlots, BuildingHiddenGarage, and GarageLaneDiagnostics " +
                "are research diagnostics only. GarageLane.m_VehicleCapacity does not currently match observed vanilla " +
                "garage occupancy reliably enough to expose as player-facing building capacity.");
            text.AppendLine(
                "Note: BuildingHiddenGarage RawUsed is GarageLane.m_VehicleCount; Cars and Bicycles are independently " +
                "classified from parked personal vehicles and may not match RawUsed exactly.");
            text.AppendLine(
                $"Note: SampleTransitions trace up to {kVehicleSampleLimit} IDs from each previous Street, " +
                "OutsideConnection, and Unknown group; NoLongerTracked means the entity still exists but no longer " +
                "matches the report's personal-vehicle query.");
#endif

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

        private static void AppendAutomaticRelocationReport(
            StringBuilder text,
            AutomaticRelocationReport? report)
        {
            text.AppendLine();
            text.AppendLine("-------------------- AUTOMATIC RELOCATION --------------------");

            if (!report.HasValue)
            {
                text.AppendLine("State=Unavailable (ParkingRelocationSystem was not loaded)");
                return;
            }

            AutomaticRelocationReport automatic = report.Value;
            text.AppendLine(
                $"Tuning={automatic.FrameInterval} frames, " +
                $"{automatic.LaneRequestsPerPass} lanes/pass, " +
                $"{automatic.CarsPerPass} cars/pass");
            text.AppendLine($"State={(automatic.IsActive ? "Active" : "Idle")}");
            text.AppendLine($"DelayedCleanup={automatic.CleanupState}");
            text.AppendLine(
                $"CleanupLaneRequestsPending=" +
                $"{automatic.CleanupLaneRequestsPending}");
            text.AppendLine(
                $"ParkingControlFixParkingPending=" +
                $"{automatic.ParkingControlFixParkingPending} " +
                "(Parking Control handoffs only)");

            if (automatic.CleanupDueFrame != 0u)
            {
                text.AppendLine($"CleanupDueFrame={automatic.CleanupDueFrame}");
            }

            text.AppendLine($"CyclesStarted={automatic.CyclesStarted}");

            if (!automatic.HasRun)
            {
                text.AppendLine("No automatic relocation has run since this city was loaded.");
                text.AppendLine($"LaneRequestsPending={automatic.LaneRequestsPending}");
                text.AppendLine($"CarsPending={automatic.CarsPending}");
                text.AppendLine(
                    $"VanillaFixParkingPendingAllSources=" +
                    $"{automatic.VanillaFixParkingPendingAllSources} " +
                    "(global count; includes vanilla and other mods)");
                return;
            }

            text.AppendLine($"Passes={automatic.Passes}");
            text.AppendLine($"LaneRequestsProcessed={automatic.LaneRequestsProcessed}");
            text.AppendLine($"LaneRequestsPending={automatic.LaneRequestsPending}");
            text.AppendLine($"CarsQueued={automatic.CarsQueued}");
            text.AppendLine($"CarsSentToVanilla={automatic.CarsSentToVanilla}");
            text.AppendLine($"CarsSkipped={automatic.CarsSkipped}");
            text.AppendLine($"CarsPending={automatic.CarsPending}");
            text.AppendLine($"StartFrame={automatic.StartFrame}");
            text.AppendLine(
                $"EndFrame={automatic.EndFrame}" +
                (automatic.IsActive ? " (current)" : string.Empty));
            text.AppendLine(
                $"ElapsedSimulationFrames={automatic.ElapsedSimulationFrames}");
            text.AppendLine(
                $"ElapsedWallSeconds=" +
                automatic.ElapsedWallSeconds.ToString(
                    "0.000",
                    System.Globalization.CultureInfo.InvariantCulture));
            text.AppendLine(
                $"MaxPCPassMilliseconds=" +
                automatic.MaxPCPassMilliseconds.ToString(
                    "0.000",
                    System.Globalization.CultureInfo.InvariantCulture) +
                " (PC collection, validation, and ECB playback only)");
            text.AppendLine(
                $"VanillaFixParkingPendingAllSources=" +
                $"{automatic.VanillaFixParkingPendingAllSources} " +
                "(global count; includes vanilla and other mods)");
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

        private void AppendUnresolvedTargetLanes(
            StringBuilder text,
            ParkingSnapshot snapshot,
            ParkingReportDetails details)
        {
            text.AppendLine();
            text.AppendLine(
                "-------------------- UNRESOLVED TARGET LANES --------------------");

            text.AppendLine(
                $"UnresolvedTargetLanes={details.UnresolvedTargetLaneCount} " +
                $"(showing {details.UnresolvedTargetLanes.Count})");

            if (details.UnresolvedTargetLanes.Count == 0)
            {
                text.AppendLine("  <none>");
                return;
            }

            foreach (UnresolvedTargetLane item in
                details.UnresolvedTargetLanes)
            {
                string source;

                if (item.ManualTarget && item.ScopeTarget)
                {
                    source = "Both";
                }
                else if (item.ManualTarget)
                {
                    source = "Manual";
                }
                else
                {
                    source =
                        snapshot.Scope == PCSettings.ParkingScope.ByDistrict
                            ? "District"
                            : "WholeCity";
                }

                text.AppendLine(
                    $"  Lane={FormatEntity(item.Lane)} | " +
                    $"Road={FormatEntity(item.Road)} | " +
                    $"Side={(item.RightSide ? "Right" : "Left")} | " +
                    $"District={GetDistrictName(item.District)} " +
                    $"[{FormatEntity(item.District)}] | " +
                    $"Source={source} | " +
                    $"ParkingDisabled={item.ParkingDisabled} | " +
                    $"StreetParkingState={item.StreetParkingState}");
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

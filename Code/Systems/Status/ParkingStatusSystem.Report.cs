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
    using Game.Common;
    using Game.Vehicles;
    using Unity.Entities;

    public sealed partial class ParkingStatusSystem
    {
        private const int kOutsideSampleCount = 10;
        private const int kStreetVehicleLimit = 100;

        /// <summary>
        /// Records one vehicle in the manual report's identity and transition collections.
        /// </summary>
        private void AddReportVehicle(
            ParkingReportDetails details,
            Entity vehicle,
            VehicleLocation location,
            Entity parkedLane,
            Entity vehicleOwner,
            PersonalCar personalCar,
            bool ownerExists,
            bool householdOwner,
            bool ownerDeleted,
            bool ownerMovingAway,
            bool ownerHasProperty,
            bool ownedVehicleMatch,
            bool keeperExists,
            bool unspawned,
            ComponentLookup<Owner> ownerLookup)
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

            if (location != VehicleLocation.OutsideConnection)
            {
                return;
            }

            details.OutsideSamples.Add(new OutsideVehicleSample
            {
                Vehicle = vehicle,
                Lane = parkedLane,
                LaneRoot = GetTopOwner(parkedLane, ownerLookup),
                Owner = vehicleOwner,
                Keeper = personalCar.m_Keeper,
                Flags = personalCar.m_State,
                OwnerExists = ownerExists,
                HouseholdOwner = householdOwner,
                OwnerDeleted = ownerDeleted,
                OwnerMovingAway = ownerMovingAway,
                OwnerHasProperty = ownerHasProperty,
                OwnedVehicleMatch = ownedVehicleMatch,
                KeeperExists = keeperExists,
                Unspawned = unspawned,
                WasStreetPrevious = wasStreetPrevious,
            });
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
                snapshot.CurbLanes,
                snapshot.DisabledCurbLanes,
                snapshot.TrackedCurbLanes);
            string deltaLine = m_HasPreviousReport
                ? "ChangeSincePrevious: " +
                    $"PersonalMotorVehicles={FormatDelta(snapshot.TotalVehicles - m_PreviousReport.TotalVehicles)}, " +
                    $"StreetParking={FormatDelta(snapshot.StreetParked - m_PreviousReport.StreetParked)}, " +
                    $"ParkedElsewhere={FormatDelta(snapshot.ParkedElsewhere - m_PreviousReport.ParkedElsewhere)}, " +
                    $"OutsideConnection={FormatDelta(snapshot.OutsideConnection - m_PreviousReport.OutsideConnection)}, " +
                    $"OutsideHidden={FormatDelta(snapshot.OutsideConnectionHidden - m_PreviousReport.OutsideConnectionHidden)}"
                : "ChangeSincePrevious=<first report for this loaded city>";

            StringBuilder text = new StringBuilder(6144);
            text.AppendLine();
            text.AppendLine($"==================== {Mod.ModTag} PARKING REPORT ====================");
            text.AppendLine($"Mod={Mod.ModName} v{Mod.ModVersion}");
            text.AppendLine($"SimulationFrame={snapshot.SimulationFrame}");
            text.AppendLine($"WholeCityNoStreetParking={snapshot.RestrictionEnabled}");
            text.AppendLine($"EligibleStreetParkingLanes={snapshot.CurbLanes}");
            text.AppendLine(
                $"DisabledStreetParkingLanes={snapshot.DisabledCurbLanes} " +
                $"(ParkingControlTracked={snapshot.TrackedCurbLanes}, VanillaOrOther={otherDisabledCurbLanes})");
            text.AppendLine($"EnforcementStatus={enforcementStatus}");
            text.AppendLine(
                $"OccupiedStreetParkingLanes={snapshot.OccupiedCurbLanes}/{snapshot.CurbLanes} " +
                $"({FormatPercent(snapshot.OccupiedCurbLanes, snapshot.CurbLanes)} of lane entities; not a parking-space percentage)");
            text.AppendLine(
                $"FixedSlotStreetParking={snapshot.FixedSlotCurbParked}/{snapshot.FixedSlotCurbCapacity} " +
                $"across {snapshot.FixedSlotCurbLanes} fixed-slot lane entities");
            text.AppendLine(
                $"ContinuousStreetParking={snapshot.ContinuousCurbParked} vehicles across " +
                $"{snapshot.ContinuousCurbLanes} continuous lane entities (no exact slot capacity)");
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
            text.AppendLine($"ParkedOnStreets={snapshot.StreetParked}");
            AppendStreetVehicleEntities(text, snapshot, details);
            text.AppendLine(
                $"ParkedElsewhere={snapshot.ParkedElsewhere} " +
                $"(VisibleOffStreet={snapshot.VisibleOffStreet}, HiddenInBuildings={snapshot.HiddenInBuildings}, " +
                $"OutsideConnection={snapshot.OutsideConnection}, UnassignedOrUnknown={snapshot.UnassignedOrUnknownParked})");
            text.AppendLine(
                $"VisibleParkingKinds=Facility={snapshot.VisibleFacilityParking}, " +
                $"BuildingLot={snapshot.VisibleBuildingParking}, Other={snapshot.VisibleOtherParking}");
            text.AppendLine($"OutsideConnectionHiddenUnspawned={snapshot.OutsideConnectionHidden}");
            text.AppendLine(
                $"OutsideConnectionOwnership=ValidLiveHousehold={snapshot.OutsideValidHouseholdOwned}, " +
                $"HouseholdInvalid={snapshot.OutsideHouseholdOwnershipInvalid}, " +
                $"DummyTraffic={snapshot.OutsideDummyTraffic}, OtherOrUnowned={snapshot.OutsideOtherOrUnowned}");
            text.AppendLine(
                $"OutsideConnectionKinds=ResidentHousehold={snapshot.OutsideResidentHousehold} " +
                $"(NotMovedIn={snapshot.OutsideResidentNotMovedIn}), " +
                $"TouristHousehold={snapshot.OutsideTouristHousehold}, " +
                $"CommuterHousehold={snapshot.OutsideCommuterHousehold}, " +
                $"DummyTraffic={snapshot.OutsideDummyTraffic}, OtherOrUnowned={snapshot.OutsideOtherOrUnowned}");
            text.AppendLine(
                $"VanillaRoadsInfoviewParking={snapshot.OfficialParkingOccupied}/{snapshot.OfficialParkingCapacity} " +
                $"across {snapshot.OfficialParkingFacilities} facility entities");
            text.AppendLine(
                $"NonBorderGarageLanes={snapshot.GarageOccupied}/{snapshot.GarageCapacity} occupied/capacity " +
                $"across {snapshot.GarageLanes} garage lane entities");
            text.AppendLine(deltaLine);
            AppendStreetTransitions(text, snapshot, details);
            AppendOutsideSamples(text, details);
            text.AppendLine(
                "Note: existing curb-parked cars leave when a keeper next uses them; this is not limited to a workday event.");
            text.AppendLine(
                "Note: PersonalCar.m_Keeper is the current reserver/user, not the persistent vehicle owner.");
            text.AppendLine(
                "Note: OutsideConnection describes the parking lane/root location, not the vehicle Owner. " +
                "Valid household-owned cars at that location are legitimate and must not be deleted.");
            text.AppendLine(
                "Note: HiddenInBuildings means a non-border GarageLane, or an unspawned vehicle on a lane owned by a building.");
            text.AppendLine(
                "Note: VisibleOffStreet means a rendered parked car that is not on an eligible public street, " +
                "inside hidden storage, or at an outside connection; VisibleParkingKinds provides the narrower split.");
            text.AppendLine(
                "Note: VanillaRoadsInfoviewParking excludes ordinary street curbs and most implicit residential storage; " +
                "continuous unslotted lanes have no exact capacity.");
            text.AppendLine(
                "Note: Entity IDs use Index:Version for Scene Explorer and are valid only within this loaded city session.");
            text.Append("===================================================================");
            LogUtils.Info(text.ToString());

            m_PreviousReport = snapshot;
            m_PreviousStreetVehicles = details.CurrentStreetVehicles;
            m_HasPreviousReport = true;
        }

        /// <summary>
        /// Lists current street-parked cars so they can be opened directly in Scene Explorer.
        /// </summary>
        private static void AppendStreetVehicleEntities(
            StringBuilder text,
            ParkingSnapshot snapshot,
            ParkingReportDetails details)
        {
            if (!snapshot.RestrictionEnabled)
            {
                text.AppendLine(
                    $"StreetParkedVehicleEntities=<restriction off; {snapshot.StreetParked} IDs omitted>");
                return;
            }

            List<Entity> vehicles = new List<Entity>(details.CurrentStreetVehicles);
            vehicles.Sort((left, right) =>
            {
                int indexOrder = left.Index.CompareTo(right.Index);
                return indexOrder != 0
                    ? indexOrder
                    : left.Version.CompareTo(right.Version);
            });

            int count = Math.Min(kStreetVehicleLimit, vehicles.Count);
            text.AppendLine(
                $"StreetParkedVehicleEntities={vehicles.Count} " +
                $"(showing {count}; enter Vehicle Index:Version in Scene Explorer)");
            for (int i = 0; i < count; i++)
            {
                text.AppendLine($"  Vehicle={FormatEntity(vehicles[i])}");
            }
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

        private static void AppendOutsideSamples(StringBuilder text, ParkingReportDetails details)
        {
            // Previous curb occupants are most useful when tracing a transition in Scene Explorer.
            details.OutsideSamples.Sort((left, right) =>
            {
                int previousOrder = right.WasStreetPrevious.CompareTo(left.WasStreetPrevious);
                if (previousOrder != 0)
                {
                    return previousOrder;
                }

                int indexOrder = left.Vehicle.Index.CompareTo(right.Vehicle.Index);
                return indexOrder != 0
                    ? indexOrder
                    : left.Vehicle.Version.CompareTo(right.Vehicle.Version);
            });

            int count = Math.Min(kOutsideSampleCount, details.OutsideSamples.Count);
            text.AppendLine(
                $"OutsideConnectionEntitySamples={count} " +
                "(previous-street matches first; IDs are Index:Version)");
            for (int i = 0; i < count; i++)
            {
                OutsideVehicleSample sample = details.OutsideSamples[i];
                text.AppendLine($"  Vehicle={FormatEntity(sample.Vehicle)}");
            }
        }

        /// <summary>
        /// Describes whether eligible curb lanes match the current restriction setting.
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
                    ? "OK (restriction off and no mod-owned lanes)"
                    : "WARNING (restriction off but mod-owned lanes remain)";
            }

            if (curbLanes > 0 && trackedCurbLanes == 0)
            {
                return "WARNING (restriction on but no lanes are owned by Parking Control)";
            }

            if (disabledCurbLanes < curbLanes)
            {
                return "WARNING (some eligible curb lanes are not disabled)";
            }

            if (disabledCurbLanes < trackedCurbLanes)
            {
                return "WARNING (some mod-owned lanes are not disabled)";
            }

            return "OK";
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

// <copyright file="ParkingStatusSystem.ProbeVehicles.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Collects personal-vehicle ownership and parking-location data for status snapshots.

using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{
    public sealed partial class ParkingStatusSystem
    {
        private void CollectPersonalVehicles(
            ref ParkingSnapshot snapshot,
            ParkingReportDetails? details,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            RoadSizeRule roadSizeRule,
            NativeHashSet<Entity> occupiedCurbLanes,
            NativeHashSet<Entity> buildingCarGarageLanes,
            ref ParkingProbeLookups lookups)
        {
            using NativeArray<Entity> vehicles =
                m_PersonalVehicleQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity vehicle in vehicles)
            {
                Entity prefab = lookups.PrefabRef[vehicle].m_Prefab;
                bool isBicycle =
                    prefab != Entity.Null &&
                    lookups.BicycleData.HasComponent(prefab);

                // Classify occupants of valid hidden building garages before bicycles are
                // excluded from the normal personal-motor-vehicle statistics.
                if (lookups.ParkedCar.TryGetComponent(
                        vehicle,
                        out Game.Vehicles.ParkedCar parkedVehicle) &&
                    buildingCarGarageLanes.Contains(parkedVehicle.m_Lane))
                {
                    if (prefab == Entity.Null)
                    {
                        snapshot.BuildingGarageUnknownVehicleOccupied++;
                    }
                    else if (isBicycle)
                    {
                        snapshot.BuildingGarageBicycleOccupied++;
                    }
                    else
                    {
                        snapshot.BuildingGarageCarOccupied++;
                    }
                }

                // Everything below this point remains motor vehicles only.
                if (prefab == Entity.Null || isBicycle)
                {
                    continue;
                }

                snapshot.TotalVehicles++;

                Game.Vehicles.PersonalCar personalCar = lookups.PersonalCar[vehicle];
                Entity vehicleOwner = Entity.Null;
                if (lookups.Owner.TryGetComponent(vehicle, out Game.Common.Owner owner))
                {
                    vehicleOwner = owner.m_Owner;
                }

                bool ownerExists =
                    vehicleOwner != Entity.Null && EntityManager.Exists(vehicleOwner);
                bool householdOwner =
                    ownerExists && lookups.Household.HasComponent(vehicleOwner);
                bool touristHousehold =
                    householdOwner &&
                    (lookups.Household[vehicleOwner].m_Flags &
                        Game.Citizens.HouseholdFlags.Tourist) != 0;
                bool commuterHousehold =
                    householdOwner &&
                    (lookups.Household[vehicleOwner].m_Flags &
                        Game.Citizens.HouseholdFlags.Commuter) != 0;
                bool residentMovedIn =
                    householdOwner &&
                    (lookups.Household[vehicleOwner].m_Flags &
                        Game.Citizens.HouseholdFlags.MovedIn) != 0;
                bool ownerDeleted =
                    ownerExists && lookups.Deleted.HasComponent(vehicleOwner);
                bool ownedVehicleMatch =
                    householdOwner &&
                    HasOwnedVehicle(vehicleOwner, vehicle, lookups.OwnedVehicle);
                bool outsideConnectionOwner =
                    ownerExists &&
                    (lookups.ObjectOutsideConnection.HasComponent(vehicleOwner) ||
                        lookups.NetOutsideConnection.HasComponent(vehicleOwner));
                bool dummyTraffic =
                    (personalCar.m_State & Game.Vehicles.PersonalCarFlags.DummyTraffic) != 0;

                AddVehicleOwnershipCounters(
                    ref snapshot,
                    ownerExists,
                    householdOwner,
                    touristHousehold,
                    commuterHousehold,
                    residentMovedIn,
                    ownerDeleted,
                    ownedVehicleMatch,
                    dummyTraffic);

                VehicleLocation location = GetVehicleLocation(
                    vehicle,
                    lookups.ParkedCar,
                    lookups.CurrentLane,
                    lookups.ParkingLane,
                    lookups.Owner,
                    lookups.PrefabRef,
                    lookups.ParkingLaneData,
                    lookups.Road,
                    lookups.NetOutsideConnection,
                    lookups.GarageLane,
                    lookups.Unspawned,
                    lookups.Building,
                    out Entity parkedLane);

                switch (location)
                {
                    case VehicleLocation.Active:
                        snapshot.ActiveVehicles++;
                        break;

                    case VehicleLocation.StreetCurb:
                        CollectStreetParkedVehicle(
                            ref snapshot,
                            details,
                            parkedLane,
                            scope,
                            policyEntity,
                            roadSizeRule,
                            occupiedCurbLanes,
                            ref lookups);
                        break;

                    case VehicleLocation.VisibleOffStreet:
                        snapshot.ParkedVehicles++;
                        snapshot.VisibleOffStreet++;
                        switch (GetParkingKind(
                            parkedLane,
                            lookups.CarParkingFacility,
                            lookups.CarParking,
                            lookups.Building,
                            lookups.Owner))
                        {
                            case VisibleParkingKind.Facility:
                                snapshot.VisibleFacilityParking++;
                                break;
                            case VisibleParkingKind.Building:
                                snapshot.VisibleBuildingParking++;
                                break;
                            default:
                                snapshot.VisibleOtherParking++;
                                break;
                        }

                        break;

                    case VehicleLocation.HiddenInBuilding:
                        snapshot.ParkedVehicles++;
                        snapshot.HiddenInBuildings++;
                        break;

                    case VehicleLocation.OutsideConnection:
                        CollectOutsideConnectionVehicle(
                            ref snapshot,
                            vehicle,
                            householdOwner,
                            touristHousehold,
                            commuterHousehold,
                            residentMovedIn,
                            ownerDeleted,
                            ownedVehicleMatch,
                            outsideConnectionOwner,
                            dummyTraffic,
                            ref lookups);
                        break;

                    default:
                        CollectUnknownVehicle(
                            ref snapshot,
                            details,
                            vehicle,
                            householdOwner,
                            touristHousehold,
                            commuterHousehold,
                            residentMovedIn,
                            ownerDeleted,
                            ownedVehicleMatch,
                            dummyTraffic,
                            ref lookups);
                        break;
                }

                if (details != null)
                {
                    AddReportVehicle(
                        details,
                        vehicle,
                        location,
                        lookups.ParkedCar.HasComponent(vehicle));
                }
            }
        }

        private static void AddVehicleOwnershipCounters(
            ref ParkingSnapshot snapshot,
            bool ownerExists,
            bool householdOwner,
            bool touristHousehold,
            bool commuterHousehold,
            bool residentMovedIn,
            bool ownerDeleted,
            bool ownedVehicleMatch,
            bool dummyTraffic)
        {
            if (!ownerExists)
            {
                // Diagnostic only; this overlaps the mutually exclusive buckets below.
                snapshot.MissingOwnerVehicles++;
            }

            if (dummyTraffic)
            {
                snapshot.DummyTrafficVehicles++;
                return;
            }

            if (!householdOwner)
            {
                snapshot.OtherOrUnownedVehicles++;
                return;
            }

            snapshot.HouseholdOwnerVehicles++;
            if (touristHousehold)
            {
                snapshot.TouristHouseholdVehicles++;
            }
            else if (commuterHousehold)
            {
                snapshot.CommuterHouseholdVehicles++;
            }
            else
            {
                snapshot.ResidentHouseholdVehicles++;
                if (!residentMovedIn)
                {
                    snapshot.ResidentNotMovedInVehicles++;
                }
            }

            if (ownerDeleted)
            {
                snapshot.DeletedHouseholdOwnerVehicles++;
            }
            else
            {
                snapshot.LiveHouseholdOwnerVehicles++;
            }

            if (ownedVehicleMatch)
            {
                snapshot.OwnedVehicleMatches++;
            }
            else
            {
                snapshot.OwnedVehicleMissing++;
            }
        }

        private static void CollectStreetParkedVehicle(
            ref ParkingSnapshot snapshot,
            ParkingReportDetails? details,
            Entity parkedLane,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            RoadSizeRule roadSizeRule,
            NativeHashSet<Entity> occupiedCurbLanes,
            ref ParkingProbeLookups lookups)
        {
            snapshot.ParkedVehicles++;
            snapshot.StreetParked++;

            Game.Net.ParkingLane streetLane = lookups.ParkingLane[parkedLane];
            bool restrictionTarget = NoStreetParkingSystem.IsRestrictionTarget(
                parkedLane,
                streetLane,
                scope,
                policyEntity,
                lookups.Owner,
                lookups.BorderDistrict,
                lookups.ManualRoadBan,
                lookups.Policy,
                roadSizeRule);

            bool firstParkedCarOnLane = occupiedCurbLanes.Add(parkedLane);
            if (firstParkedCarOnLane)
            {
                snapshot.OccupiedCurbLanes++;
                if (restrictionTarget)
                {
                    snapshot.OccupiedTargetCurbLanes++;
                }
            }

            if (details != null)
            {
                Entity district = NoStreetParkingSystem.GetLaneDistrict(
                    parkedLane,
                    streetLane,
                    lookups.Owner,
                    lookups.BorderDistrict);

                DistrictParkingStats districtStats = GetDistrictParkingStats(
                    details,
                    district,
                    policyEntity,
                    lookups.Policy);

                districtStats.StreetCars++;
                if (firstParkedCarOnLane)
                {
                    districtStats.OccupiedLanes++;
                }
            }

            if (restrictionTarget)
            {
                snapshot.TargetStreetParked++;
            }

            if (IsFixedSlotLane(
                    parkedLane,
                    lookups.PrefabRef,
                    lookups.ParkingLaneData))
            {
                snapshot.FixedSlotCurbParked++;
            }
            else
            {
                snapshot.ContinuousCurbParked++;
            }
        }

        private static void CollectOutsideConnectionVehicle(
            ref ParkingSnapshot snapshot,
            Entity vehicle,
            bool householdOwner,
            bool touristHousehold,
            bool commuterHousehold,
            bool residentMovedIn,
            bool ownerDeleted,
            bool ownedVehicleMatch,
            bool outsideConnectionOwner,
            bool dummyTraffic,
            ref ParkingProbeLookups lookups)
        {
            snapshot.ParkedVehicles++;
            snapshot.OutsideConnection++;

            if (householdOwner)
            {
                if (touristHousehold)
                {
                    snapshot.OutsideTouristHousehold++;
                }
                else if (commuterHousehold)
                {
                    snapshot.OutsideCommuterHousehold++;
                }
                else
                {
                    snapshot.OutsideResidentHousehold++;
                    if (!residentMovedIn)
                    {
                        snapshot.OutsideResidentNotMovedIn++;
                    }
                }
            }

            if (dummyTraffic)
            {
                snapshot.OutsideDummyTraffic++;
            }

            if (householdOwner)
            {
                // A moving-away household can still validly own its car. Deletion or a
                // missing OwnedVehicle link is the ownership-health failure here.
                if (!ownerDeleted && ownedVehicleMatch)
                {
                    snapshot.OutsideValidHouseholdOwned++;
                }
                else
                {
                    snapshot.OutsideHouseholdOwnershipInvalid++;
                }
            }
            else if (outsideConnectionOwner)
            {
                snapshot.OutsideConnectionOwner++;
            }
            else
            {
                snapshot.OutsideOtherOrUnowned++;
            }

            if (lookups.Unspawned.HasComponent(vehicle))
            {
                snapshot.OutsideConnectionHidden++;
            }
        }

    }
}

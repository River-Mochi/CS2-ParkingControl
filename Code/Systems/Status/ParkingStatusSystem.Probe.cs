// <copyright file="ParkingStatusSystem.Probe.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Collects on-demand curb, parking-supply, ownership, and personal-vehicle status snapshots.

    using System;
    using Unity.Collections;
    using Unity.Entities;

namespace ParkingControl
{
    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Builds one read-only snapshot; callers keep the system disabled between requests.
        /// </summary>
        private ParkingSnapshot BuildSnapshot(ParkingReportDetails? details)
        {
            ComponentLookup<ManualRoadParkingBan> manualRoadBanLookup =
                GetComponentLookup<ManualRoadParkingBan>(true);

            ComponentLookup<Game.Prefabs.BicycleData> bicycleDataLookup =
                GetComponentLookup<Game.Prefabs.BicycleData>(true);
            ComponentLookup<Game.Buildings.Building> buildingLookup =
                GetComponentLookup<Game.Buildings.Building>(true);
            ComponentLookup<Game.Buildings.CarParkingFacility> carParkingFacilityLookup =
                GetComponentLookup<Game.Buildings.CarParkingFacility>(true);
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup =
                GetComponentLookup<Game.Areas.BorderDistrict>(true);
            ComponentLookup<Game.Vehicles.CarCurrentLane> currentLaneLookup =
                GetComponentLookup<Game.Vehicles.CarCurrentLane>(true);
            ComponentLookup<Game.Net.ConnectionLane> connectionLaneLookup =
                GetComponentLookup<Game.Net.ConnectionLane>(true);
            ComponentLookup<Game.Net.Curve> curveLookup = GetComponentLookup<Game.Net.Curve>(true);
            ComponentLookup<Game.Common.Deleted> deletedLookup =
                GetComponentLookup<Game.Common.Deleted>(true);
            ComponentLookup<Game.Net.GarageLane> garageLaneLookup =
                GetComponentLookup<Game.Net.GarageLane>(true);
            ComponentLookup<Game.Citizens.Household> householdLookup =
                GetComponentLookup<Game.Citizens.Household>(true);
            ComponentLookup<Game.Net.OutsideConnection> outsideConnectionLookup =
                GetComponentLookup<Game.Net.OutsideConnection>(true);
            ComponentLookup<Game.Objects.OutsideConnection> objectOutsideConnectionLookup =
                GetComponentLookup<Game.Objects.OutsideConnection>(true);
            ComponentLookup<Game.Common.Owner> ownerLookup =
                GetComponentLookup<Game.Common.Owner>(true);
            ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup =
                GetComponentLookup<Game.Vehicles.ParkedCar>(true);
            ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup =
                GetComponentLookup<Game.Net.ParkingLane>(true);
            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup =
                GetComponentLookup<Game.Prefabs.ParkingLaneData>(true);
            ComponentLookup<Game.Vehicles.PersonalCar> personalCarLookup =
                GetComponentLookup<Game.Vehicles.PersonalCar>(true);
            ComponentLookup<Game.Routes.CarParking> carParkingLookup =
                GetComponentLookup<Game.Routes.CarParking>(true);
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup =
                GetComponentLookup<Game.Prefabs.PrefabRef>(true);
            ComponentLookup<Game.Net.Road> roadLookup = GetComponentLookup<Game.Net.Road>(true);
            ComponentLookup<StreetParkingState> stateLookup =
                GetComponentLookup<StreetParkingState>(true);
            ComponentLookup<Game.Objects.TripSource> tripSourceLookup =
                GetComponentLookup<Game.Objects.TripSource>(true);
            ComponentLookup<Game.Objects.Unspawned> unspawnedLookup =
                GetComponentLookup<Game.Objects.Unspawned>(true);
            BufferLookup<Game.Net.LaneObject> laneObjectLookup =
                GetBufferLookup<Game.Net.LaneObject>(true);
            BufferLookup<Game.Vehicles.OwnedVehicle> ownedVehicleLookup =
                GetBufferLookup<Game.Vehicles.OwnedVehicle>(true);
            BufferLookup<Game.Policies.Policy> policyLookup =
                GetBufferLookup<Game.Policies.Policy>(true);
            BufferLookup<Game.Net.SubLane> subLaneLookup = GetBufferLookup<Game.Net.SubLane>(true);
            BufferLookup<Game.Net.SubNet> subNetLookup = GetBufferLookup<Game.Net.SubNet>(true);
            BufferLookup<Game.Objects.SubObject> subObjectLookup =
                GetBufferLookup<Game.Objects.SubObject>(true);

            PCSettings.ParkingScope scope =
                Mod.Settings?.Scope ?? PCSettings.ParkingScope.Off;
            Entity policyEntity = ParkingPolicySystem.PolicyEntity;
            ParkingSnapshot snapshot = new()
            {
                CapturedAtLocal = DateTime.Now,
                SimulationFrame = m_SimulationSystem.frameIndex,
                Scope = scope,
            };
            // This temporary set scales with curb lanes, not vehicle count, and is released
            // as soon as the on-demand snapshot is complete.
            int occupiedLaneCapacity = Math.Max(1, m_CurbLaneQuery.CalculateEntityCount());
            using NativeHashSet<Entity> occupiedCurbLanes =
                new(occupiedLaneCapacity, Allocator.Temp);

            using (NativeArray<Entity> districts = m_DistrictQuery.ToEntityArray(Allocator.Temp))
            {
                snapshot.Districts = districts.Length;
                foreach (Entity district in districts)
                {
                    // no manual lookup if PolicyActive
                    bool policyActive = NoStreetParkingSystem.IsDistrictPolicyActive(
                        district,
                        policyEntity,
                        policyLookup);
                    if (policyActive)
                    {
                        snapshot.DistrictsWithPolicy++;
                    }

                    if (details != null)
                    {
                        details.DistrictParking[district] =
                            new DistrictParkingStats(district, policyActive);
                    }
                }
            }

            using (NativeArray<Entity> lanes = m_CurbLaneQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity lane in lanes)
                {
                    Game.Net.ParkingLane parkingLane = parkingLaneLookup[lane];
                    bool isStreetParking = NoStreetParkingSystem.IsStreetCarParkingLane(
                        lane,
                        parkingLane,
                        ownerLookup,
                        prefabRefLookup,
                        parkingLaneDataLookup,
                        roadLookup);
                    if (!isStreetParking)
                    {
                        VisibleParkingKind parkingKind = GetParkingKind(
                            lane,
                            carParkingFacilityLookup,
                            carParkingLookup,
                            buildingLookup,
                            ownerLookup);
                        Entity buildingLanePrefab = prefabRefLookup[lane].m_Prefab;
                        if (parkingKind == VisibleParkingKind.Building &&
                            (parkingLane.m_Flags & Game.Net.ParkingLaneFlags.VirtualLane) == 0 &&
                            parkingLaneDataLookup.TryGetComponent(
                                buildingLanePrefab,
                                out Game.Prefabs.ParkingLaneData buildingLaneData) &&
                            (buildingLaneData.m_RoadTypes & Game.Net.RoadTypes.Car) != 0)
                        {
                            int occupied = CountParkedCarsOnLane(
                                lane,
                                laneObjectLookup,
                                parkedCarLookup);
                            if (buildingLaneData.m_SlotInterval != 0f &&
                                curveLookup.TryGetComponent(lane, out Game.Net.Curve buildingCurve))
                            {
                                // Visible lots use fixed geometry, don't infer extra spaces.
                                snapshot.BuildingParkingLanes++;
                                snapshot.BuildingFixedSlotLanes++;
                                snapshot.BuildingParkingCapacity += Math.Max(
                                    0,
                                    Game.Net.NetUtils.GetParkingSlotCount(
                                        buildingCurve,
                                        parkingLane,
                                        buildingLaneData));
                                snapshot.BuildingParkingOccupied += occupied;
                            }
                            else
                            {
                                // Continuous lanes have no honest slot capacity and stay log-only.
                                snapshot.BuildingContinuousLanes++;
                                snapshot.BuildingContinuousOccupied += occupied;
                            }
                        }

                        continue;
                    }

                    snapshot.CurbLanes++;
                    DistrictParkingStats? districtStats = null;
                    if (details != null)
                    {
                        Entity district = NoStreetParkingSystem.GetLaneDistrict(
                            lane,
                            parkingLane,
                            ownerLookup,
                            borderDistrictLookup);
                        districtStats = GetDistrictParkingStats(
                            details,
                            district,
                            policyEntity,
                            policyLookup);
                        districtStats.EligibleLanes++;
                    }

                    // Yes, manual lookup
                    bool manualTarget =
                        NoStreetParkingSystem.IsManualRestrictionTarget(
                            lane,
                            parkingLane,
                            ownerLookup,
                            manualRoadBanLookup);

                    bool scopeTarget =
                        NoStreetParkingSystem.IsScopeRestrictionTarget(
                            lane,
                            parkingLane,
                            scope,
                            policyEntity,
                            ownerLookup,
                            borderDistrictLookup,
                            policyLookup);

                    bool isTarget = manualTarget || scopeTarget;

                    if (isTarget)
                    {
                        snapshot.TargetCurbLanes++;
                    }

                    bool parkingDisabled =
                        (parkingLane.m_Flags &
                            Game.Net.ParkingLaneFlags.ParkingDisabled) != 0;

                    if (parkingDisabled)
                    {
                        snapshot.DisabledCurbLanes++;

                        if (districtStats != null)
                        {
                            districtStats.DisabledLanes++;
                        }

                        if (isTarget)
                        {
                            snapshot.DisabledTargetCurbLanes++;
                        }
                    }

                    bool streetParkingState =
                        stateLookup.HasComponent(lane);

                    if (streetParkingState)
                    {
                        snapshot.TrackedCurbLanes++;

                        if (districtStats != null)
                        {
                            districtStats.TrackedLanes++;
                        }
                    }

                    if (details != null &&
                        isTarget &&
                        !parkingDisabled)
                    {
                        details.UnresolvedTargetLaneCount++;

                        if (details.UnresolvedTargetLanes.Count <
                            kUnresolvedLaneSampleLimit)
                        {
                            Entity road = ownerLookup[lane].m_Owner;

                            Entity district =
                                NoStreetParkingSystem.GetLaneDistrict(
                                    lane,
                                    parkingLane,
                                    ownerLookup,
                                    borderDistrictLookup);

                            bool rightSide =
                                (parkingLane.m_Flags &
                                    Game.Net.ParkingLaneFlags.RightSide) != 0;

                            details.UnresolvedTargetLanes.Add(
                                new UnresolvedTargetLane(
                                    lane,
                                    road,
                                    district,
                                    rightSide,
                                    manualTarget,
                                    scopeTarget,
                                    parkingDisabled,
                                    streetParkingState));
                        }
                    }





                    Entity prefab = prefabRefLookup[lane].m_Prefab;
                    if (parkingLaneDataLookup.TryGetComponent(
                            prefab,
                            out Game.Prefabs.ParkingLaneData laneData) &&
                        laneData.m_SlotInterval != 0f &&
                        curveLookup.TryGetComponent(lane, out Game.Net.Curve curve))
                    {
                        snapshot.FixedSlotCurbLanes++;
                        snapshot.FixedSlotCurbCapacity += Math.Max(
                            0,
                            Game.Net.NetUtils.GetParkingSlotCount(curve, parkingLane, laneData));
                    }
                    else
                    {
                        // Continuous curb lanes do not expose an honest fixed-space capacity.
                        snapshot.ContinuousCurbLanes++;
                    }
                }
            }

            using (NativeArray<Entity> garageLanes = m_GarageLaneQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity lane in garageLanes)
                {
                    if (IsOutsideConnectionLane(lane, outsideConnectionLookup, ownerLookup))
                    {
                        continue;
                    }

                    Game.Net.GarageLane garageLane = garageLaneLookup[lane];
                    snapshot.GarageLanes++;
                    snapshot.GarageCapacity += garageLane.m_VehicleCapacity;
                    snapshot.GarageOccupied += garageLane.m_VehicleCount;

                    // GarageLane is shared by cars and bicycles; this status is motor vehicles only.
                    if (connectionLaneLookup.TryGetComponent(
                            lane,
                            out Game.Net.ConnectionLane connectionLane) &&
                        (connectionLane.m_RoadTypes & Game.Net.RoadTypes.Car) != 0 &&
                        GetParkingKind(
                            lane,
                            carParkingFacilityLookup,
                            carParkingLookup,
                            buildingLookup,
                            ownerLookup) == VisibleParkingKind.Building)
                    {
                        snapshot.BuildingParkingLanes++;
                        snapshot.BuildingGarageLanes++;
                        snapshot.BuildingParkingCapacity += garageLane.m_VehicleCapacity;
                        snapshot.BuildingParkingOccupied += garageLane.m_VehicleCount;
                    }
                }
            }

            AddOfficialParkingTotals(
                ref snapshot,
                ref parkingLaneLookup,
                ref prefabRefLookup,
                ref curveLookup,
                ref parkingLaneDataLookup,
                ref parkedCarLookup,
                ref garageLaneLookup,
                ref laneObjectLookup,
                ref subLaneLookup,
                ref subNetLookup,
                ref subObjectLookup);

            using (NativeArray<Entity> vehicles = m_PersonalVehicleQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity vehicle in vehicles)
                {
                    Entity prefab = prefabRefLookup[vehicle].m_Prefab;
                    if (prefab == Entity.Null || bicycleDataLookup.HasComponent(prefab))
                    {
                        continue;
                    }

                    snapshot.TotalVehicles++;
                    Game.Vehicles.PersonalCar personalCar = personalCarLookup[vehicle];
                    Entity vehicleOwner = Entity.Null;
                    if (ownerLookup.TryGetComponent(vehicle, out Game.Common.Owner owner))
                    {
                        vehicleOwner = owner.m_Owner;
                    }

                    bool ownerExists = vehicleOwner != Entity.Null && EntityManager.Exists(vehicleOwner);
                    bool householdOwner =
                        ownerExists && householdLookup.HasComponent(vehicleOwner);
                    bool touristHousehold = householdOwner &&
                        (householdLookup[vehicleOwner].m_Flags &
                            Game.Citizens.HouseholdFlags.Tourist) != 0;
                    bool commuterHousehold = householdOwner &&
                        (householdLookup[vehicleOwner].m_Flags &
                            Game.Citizens.HouseholdFlags.Commuter) != 0;
                    bool residentMovedIn = householdOwner &&
                        (householdLookup[vehicleOwner].m_Flags &
                            Game.Citizens.HouseholdFlags.MovedIn) != 0;
                    bool ownerDeleted =
                        ownerExists && deletedLookup.HasComponent(vehicleOwner);
                    bool ownedVehicleMatch = householdOwner &&
                        HasOwnedVehicle(vehicleOwner, vehicle, ownedVehicleLookup);
                    bool outsideConnectionOwner = ownerExists &&
                        (objectOutsideConnectionLookup.HasComponent(vehicleOwner) ||
                            outsideConnectionLookup.HasComponent(vehicleOwner));
                    bool dummyTraffic =
                        (personalCar.m_State & Game.Vehicles.PersonalCarFlags.DummyTraffic) != 0;

                    if (!ownerExists)
                    {
                        // MissingOwnerVehicles is diagnostic and overlaps the categories below.
                        snapshot.MissingOwnerVehicles++;
                    }

                    if (dummyTraffic)
                    {
                        snapshot.DummyTrafficVehicles++;
                    }
                    else if (householdOwner)
                    {
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
                    else
                    {
                        snapshot.OtherOrUnownedVehicles++;
                    }

                    VehicleLocation location = GetVehicleLocation(
                        vehicle,
                        parkedCarLookup,
                        currentLaneLookup,
                        parkingLaneLookup,
                        ownerLookup,
                        prefabRefLookup,
                        parkingLaneDataLookup,
                        roadLookup,
                        outsideConnectionLookup,
                        garageLaneLookup,
                        unspawnedLookup,
                        buildingLookup,
                        out Entity parkedLane);

                    switch (location)
                    {
                        case VehicleLocation.Active:
                            snapshot.ActiveVehicles++;
                            break;
                        case VehicleLocation.StreetCurb:
                            snapshot.ParkedVehicles++;
                            snapshot.StreetParked++;
                            Game.Net.ParkingLane streetLane = parkingLaneLookup[parkedLane];

                        bool restrictionTarget = NoStreetParkingSystem.IsRestrictionTarget(
                            parkedLane,
                            streetLane,
                            scope,
                            policyEntity,
                            ownerLookup,
                            borderDistrictLookup,
                            manualRoadBanLookup,
                            policyLookup);

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
                                    ownerLookup,
                                    borderDistrictLookup);
                                DistrictParkingStats districtStats = GetDistrictParkingStats(
                                    details,
                                    district,
                                    policyEntity,
                                    policyLookup);
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
                                prefabRefLookup,
                                parkingLaneDataLookup))
                            {
                                snapshot.FixedSlotCurbParked++;
                            }
                            else
                            {
                                snapshot.ContinuousCurbParked++;
                            }

                            break;
                        case VehicleLocation.VisibleOffStreet:
                            snapshot.ParkedVehicles++;
                            snapshot.VisibleOffStreet++;
                            switch (GetParkingKind(
                                parkedLane,
                                carParkingFacilityLookup,
                                carParkingLookup,
                                buildingLookup,
                                ownerLookup))
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
                                // A moving-away household can still validly own its car; deletion
                                // or a missing OwnedVehicle link is the ownership-health failure.
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

                            if (unspawnedLookup.HasComponent(vehicle))
                            {
                                snapshot.OutsideConnectionHidden++;
                            }

                            break;
                        default:
                            if (parkedCarLookup.TryGetComponent(
                                    vehicle,
                                    out Game.Vehicles.ParkedCar unknownParkedCar))
                            {
                                snapshot.ParkedVehicles++;
                                snapshot.UnassignedOrUnknownParked++;
                                if (details != null)
                                {
                                    // Vanilla can stage a valid household car at its trip
                                    // source without finding a concrete parking lane.
                                    AddUnknownDiagnostics(
                                        ref snapshot,
                                        details,
                                        vehicle,
                                        unknownParkedCar,
                                        householdOwner,
                                        touristHousehold,
                                        commuterHousehold,
                                        residentMovedIn,
                                        ownerDeleted,
                                        ownedVehicleMatch,
                                        dummyTraffic,
                                        tripSourceLookup,
                                        unspawnedLookup,
                                        outsideConnectionLookup,
                                        objectOutsideConnectionLookup,
                                        buildingLookup,
                                        ownerLookup);
                                }
                            }
                            else
                            {
                                snapshot.UnlocatedVehicles++;
                            }

                            break;
                    }

                    if (details != null)
                    {
                        AddReportVehicle(
                            details,
                            vehicle,
                            location,
                            parkedCarLookup.HasComponent(vehicle));
                    }
                }
            }

            // found a real mismatch, so request one full reconcile when sim resumes (low cost hardening).
            if (snapshot.TargetCurbLanes > snapshot.DisabledTargetCurbLanes ||
                snapshot.TrackedCurbLanes > snapshot.DisabledCurbLanes)
            {
                NoStreetParkingSystem.RequestReconcile();
            }
            return snapshot;
        }

        private static DistrictParkingStats GetDistrictParkingStats(
            ParkingReportDetails details,
            Entity district,
            Entity policyEntity,
            BufferLookup<Game.Policies.Policy> policyLookup)
        {
            if (!details.DistrictParking.TryGetValue(
                    district,
                    out DistrictParkingStats districtStats))
            {
                districtStats = new DistrictParkingStats(
                    district,
                    NoStreetParkingSystem.IsDistrictPolicyActive(
                        district,
                        policyEntity,
                        policyLookup));
                details.DistrictParking.Add(district, districtStats);
            }

            return districtStats;
        }

        /// <summary>
        /// Breaks down parked cars without a usable lane for the manual log report.
        /// </summary>
        private void AddUnknownDiagnostics(
            ref ParkingSnapshot snapshot,
            ParkingReportDetails details,
            Entity vehicle,
            Game.Vehicles.ParkedCar parkedCar,
            bool householdOwner,
            bool touristHousehold,
            bool commuterHousehold,
            bool residentMovedIn,
            bool ownerDeleted,
            bool ownedVehicleMatch,
            bool dummyTraffic,
            ComponentLookup<Game.Objects.TripSource> tripSourceLookup,
            ComponentLookup<Game.Objects.Unspawned> unspawnedLookup,
            ComponentLookup<Game.Net.OutsideConnection> outsideConnectionLookup,
            ComponentLookup<Game.Objects.OutsideConnection> objectOutsideConnectionLookup,
            ComponentLookup<Game.Buildings.Building> buildingLookup,
            ComponentLookup<Game.Common.Owner> ownerLookup)
        {

            bool nullLane = parkedCar.m_Lane == Entity.Null;
            if (nullLane)
            {
                snapshot.UnknownNullLane++;
            }
            else if (!EntityManager.Exists(parkedCar.m_Lane))
            {
                snapshot.UnknownMissingLane++;
            }

            if (unspawnedLookup.HasComponent(vehicle))
            {
                snapshot.UnknownUnspawned++;

                if (nullLane)
                {
                    snapshot.UnknownNullLaneUnspawned++;
                }
            }


            if (dummyTraffic)
            {
                snapshot.UnknownDummyTraffic++;
            }
            else if (householdOwner)
            {
                if (!ownerDeleted && ownedVehicleMatch)
                {
                    snapshot.UnknownValidHouseholdOwned++;
                }
                else
                {
                    snapshot.UnknownHouseholdOwnershipInvalid++;
                }

                if (touristHousehold)
                {
                    snapshot.UnknownTouristHousehold++;
                }
                else if (commuterHousehold)
                {
                    snapshot.UnknownCommuterHousehold++;
                }
                else
                {
                    snapshot.UnknownResidentHousehold++;
                    if (!residentMovedIn)
                    {
                        snapshot.UnknownResidentNotMovedIn++;
                    }
                }
            }
            else
            {
                snapshot.UnknownOtherOrUnowned++;
            }

            if (!tripSourceLookup.TryGetComponent(
                    vehicle,
                    out Game.Objects.TripSource tripSource))
            {
                snapshot.UnknownWithoutTripSource++;
                AddBoundedSample(details.UnknownNoSourceSamples, vehicle);
                return;
            }

            snapshot.UnknownWithTripSource++;
            Entity source = tripSource.m_Source;
            if (source == Entity.Null || !EntityManager.Exists(source))
            {
                snapshot.UnknownTripSourceMissing++;
                AddBoundedSample(details.UnknownMissingSourceSamples, vehicle);
            }
            else if (IsOutsideConnectionEntity(
                         source,
                         outsideConnectionLookup,
                         objectOutsideConnectionLookup,
                         ownerLookup))
            {
                snapshot.UnknownTripSourceOutside++;
                AddBoundedSample(details.UnknownOutsideSourceSamples, vehicle);
            }
            else if (IsOwnedByBuilding(source, buildingLookup, ownerLookup))
            {
                snapshot.UnknownTripSourceBuilding++;
                AddBoundedSample(details.UnknownBuildingSourceSamples, vehicle);
            }
            else
            {
                snapshot.UnknownTripSourceOther++;
                AddBoundedSample(details.UnknownOtherSourceSamples, vehicle);
            }
        }

        /// <summary>
        /// Adds parking totals using the same public helper and facility scope as the Roads infoview.
        /// </summary>
        private void AddOfficialParkingTotals(
            ref ParkingSnapshot snapshot,
            ref ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup,
            ref ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup,
            ref ComponentLookup<Game.Net.Curve> curveLookup,
            ref ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup,
            ref ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup,
            ref ComponentLookup<Game.Net.GarageLane> garageLaneLookup,
            ref BufferLookup<Game.Net.LaneObject> laneObjectLookup,
            ref BufferLookup<Game.Net.SubLane> subLaneLookup,
            ref BufferLookup<Game.Net.SubNet> subNetLookup,
            ref BufferLookup<Game.Objects.SubObject> subObjectLookup)
        {
            using NativeArray<Entity> facilities = m_ParkingFacilityQuery.ToEntityArray(Allocator.Temp);
            snapshot.OfficialParkingFacilities = facilities.Length;
            foreach (Entity facility in facilities)
            {
                int laneCount = 0;
                int capacity = 0;
                int occupied = 0;
                int parkingFee = 0;
                Game.Vehicles.VehicleUtils.GetParkingData(
                    facility,
                    ref laneCount,
                    ref capacity,
                    ref occupied,
                    ref parkingFee,
                    ref parkingLaneLookup,
                    ref prefabRefLookup,
                    ref curveLookup,
                    ref parkingLaneDataLookup,
                    ref parkedCarLookup,
                    ref garageLaneLookup,
                    ref laneObjectLookup,
                    ref subLaneLookup,
                    ref subNetLookup,
                    ref subObjectLookup);
                snapshot.OfficialParkingOccupied += occupied;
                if (capacity > 0)
                {
                    // Continuous unslotted lanes have no exact capacity and are omitted.
                    snapshot.OfficialParkingCapacity += capacity;
                }
            }
        }

        /// <summary>
        /// Counts only parked cars on a fixed-slot lane; other lane objects are ignored.
        /// </summary>
        private static int CountParkedCarsOnLane(
            Entity lane,
            BufferLookup<Game.Net.LaneObject> laneObjectLookup,
            ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup)
        {
            if (!laneObjectLookup.TryGetBuffer(
                    lane,
                    out DynamicBuffer<Game.Net.LaneObject> laneObjects))
            {
                return 0;
            }

            int count = 0;
            foreach (Game.Net.LaneObject laneObject in laneObjects)
            {
                if (parkedCarLookup.HasComponent(laneObject.m_LaneObject))
                {
                    count++;
                }
            }

            return count;
        }
    }
}

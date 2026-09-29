// <copyright file="ParkingStatusSystem.ProbeParking.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Collects district, curb, garage, and public parking data for status snapshots.

using System;
using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{
    public sealed partial class ParkingStatusSystem
    {
        private void CollectDistricts(
            ref ParkingSnapshot snapshot,
            ParkingReportDetails? details,
            Entity policyEntity,
            ref ParkingProbeLookups lookups)
        {
            using NativeArray<Entity> districts =
                m_DistrictQuery.ToEntityArray(Allocator.Temp);

            snapshot.Districts = districts.Length;
            foreach (Entity district in districts)
            {
                bool policyActive = NoStreetParkingSystem.IsDistrictPolicyActive(
                    district,
                    policyEntity,
                    lookups.Policy);

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

        private void CollectParkingLanes(
            ref ParkingSnapshot snapshot,
            ParkingReportDetails? details,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            RoadSizeRule roadSizeRule,
            ref ParkingProbeLookups lookups)
        {
            using NativeArray<Entity> lanes =
                m_CurbLaneQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity lane in lanes)
            {
                Game.Net.ParkingLane parkingLane = lookups.ParkingLane[lane];
                bool isStreetParking = NoStreetParkingSystem.IsStreetCarParkingLane(
                    lane,
                    parkingLane,
                    lookups.Owner,
                    lookups.PrefabRef,
                    lookups.ParkingLaneData,
                    lookups.Road);

                if (!isStreetParking)
                {
                    CollectVisibleBuildingParkingLane(
                        ref snapshot,
                        lane,
                        parkingLane,
                        ref lookups);
                    continue;
                }

                snapshot.CurbLanes++;

                DistrictParkingStats? districtStats = null;
                if (details != null)
                {
                    Entity district = NoStreetParkingSystem.GetLaneDistrict(
                        lane,
                        parkingLane,
                        lookups.Owner,
                        lookups.BorderDistrict);

                    districtStats = GetDistrictParkingStats(
                        details,
                        district,
                        policyEntity,
                        lookups.Policy);
                    districtStats.EligibleLanes++;
                }

                bool manualTarget =
                    NoStreetParkingSystem.IsManualRestrictionTarget(
                        lane,
                        parkingLane,
                        lookups.Owner,
                        lookups.ManualRoadBan);

                bool scopeTarget =
                    NoStreetParkingSystem.IsScopeRestrictionTarget(
                        lane,
                        parkingLane,
                        scope,
                        policyEntity,
                        lookups.Owner,
                        lookups.BorderDistrict,
                        lookups.Policy,
                        roadSizeRule) ||
                    NoStreetParkingSystem.IsRoadSizeRestrictionTarget(
                        lane,
                        lookups.Owner,
                        roadSizeRule);

                bool isTarget = manualTarget || scopeTarget;
                if (isTarget)
                {
                    snapshot.TargetCurbLanes++;
                    if (districtStats != null)
                    {
                        districtStats.TargetLanes++;
                    }
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

                        // Whole City can intentionally exclude marked angled/perpendicular bays.
                        // Keep district target counts aligned with that actual target subset.
                        if (districtStats != null)
                        {
                            districtStats.DisabledTargetLanes++;
                        }
                    }
                }

                bool streetParkingState =
                    lookups.StreetParkingState.HasComponent(lane);

                if (streetParkingState)
                {
                    snapshot.TrackedCurbLanes++;

                    if (districtStats != null)
                    {
                        districtStats.TrackedLanes++;

                        if (isTarget)
                        {
                            districtStats.TrackedTargetLanes++;
                        }
                    }
                }

                if (details != null && isTarget && !parkingDisabled)
                {
                    details.UnresolvedTargetLaneCount++;

                    if (details.UnresolvedTargetLanes.Count < kUnresolvedLaneSampleLimit)
                    {
                        Entity road = lookups.Owner[lane].m_Owner;
                        Entity district = NoStreetParkingSystem.GetLaneDistrict(
                            lane,
                            parkingLane,
                            lookups.Owner,
                            lookups.BorderDistrict);

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

                Entity prefab = lookups.PrefabRef[lane].m_Prefab;
                if (lookups.ParkingLaneData.TryGetComponent(
                        prefab,
                        out Game.Prefabs.ParkingLaneData laneData) &&
                    laneData.m_SlotInterval != 0f &&
                    lookups.Curve.TryGetComponent(lane, out Game.Net.Curve curve))
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

        private static void CollectVisibleBuildingParkingLane(
            ref ParkingSnapshot snapshot,
            Entity lane,
            Game.Net.ParkingLane parkingLane,
            ref ParkingProbeLookups lookups)
        {
            VisibleParkingKind parkingKind = GetParkingKind(
                lane,
                lookups.CarParkingFacility,
                lookups.CarParking,
                lookups.Building,
                lookups.Owner);

            Entity buildingLanePrefab = lookups.PrefabRef[lane].m_Prefab;
            if (parkingKind != VisibleParkingKind.Building ||
                (parkingLane.m_Flags & Game.Net.ParkingLaneFlags.VirtualLane) != 0 ||
                !lookups.ParkingLaneData.TryGetComponent(
                    buildingLanePrefab,
                    out Game.Prefabs.ParkingLaneData buildingLaneData) ||
                (buildingLaneData.m_RoadTypes & Game.Net.RoadTypes.Car) == 0)
            {
                return;
            }

            int occupied = CountParkedCarsOnLane(
                lane,
                lookups.LaneObject,
                lookups.ParkedCar);

            if (buildingLaneData.m_SlotInterval != 0f &&
                lookups.Curve.TryGetComponent(lane, out Game.Net.Curve buildingCurve))
            {
                // Visible lots use fixed geometry; do not infer extra spaces.
                snapshot.BuildingParkingLanes++;
                snapshot.BuildingFixedSlotLanes++;
                snapshot.BuildingParkingCapacity += Math.Max(
                    0,
                    Game.Net.NetUtils.GetParkingSlotCount(
                        buildingCurve,
                        parkingLane,
                        buildingLaneData));
                snapshot.BuildingParkingOccupied += occupied;
                snapshot.BuildingParkingUsedSlots += occupied;
                return;
            }

            // Continuous lanes have no honest slot capacity and stay log-only.
            snapshot.BuildingContinuousLanes++;
            snapshot.BuildingContinuousOccupied += occupied;
        }

        private void CollectGarageLanes(
            ref ParkingSnapshot snapshot,
            NativeHashSet<Entity> buildingCarGarageLanes,
            ref ParkingProbeLookups lookups)
        {
            using NativeArray<Entity> garageLanes =
                m_GarageLaneQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity lane in garageLanes)
            {
                if (IsOutsideConnectionLane(
                        lane,
                        lookups.NetOutsideConnection,
                        lookups.Owner))
                {
                    continue;
                }

                Game.Net.GarageLane garageLane = lookups.GarageLane[lane];

                // Broad totals are retained for diagnostics explaining raw GarageLane capacity.
                snapshot.GarageLanes++;
                snapshot.GarageCapacity += garageLane.m_VehicleCapacity;
                snapshot.GarageOccupied += garageLane.m_VehicleCount;

                // Slave lanes are not independent physical garage capacity.
                if (lookups.SlaveLane.HasComponent(lane))
                {
                    snapshot.GarageSlaveLanes++;
                    snapshot.GarageSlaveCapacity += garageLane.m_VehicleCapacity;
                    snapshot.GarageSlaveOccupied += garageLane.m_VehicleCount;
                    continue;
                }

                // RealisticParking mirrors vanilla garage processing with
                // ConnectionLane + GarageLane and excludes SlaveLane.
                if (!lookups.ConnectionLane.TryGetComponent(
                        lane,
                        out Game.Net.ConnectionLane connectionLane))
                {
                    snapshot.GarageWithoutConnectionLanes++;
                    snapshot.GarageWithoutConnectionCapacity += garageLane.m_VehicleCapacity;
                    snapshot.GarageWithoutConnectionOccupied += garageLane.m_VehicleCount;
                    continue;
                }

                snapshot.GaragePrimaryLanes++;
                snapshot.GaragePrimaryCapacity += garageLane.m_VehicleCapacity;
                snapshot.GaragePrimaryOccupied += garageLane.m_VehicleCount;

                // Bicycle-only and other non-car garage connections are not car parking.
                if ((connectionLane.m_RoadTypes & Game.Net.RoadTypes.Car) == 0)
                {
                    snapshot.GarageNonCarPrimaryLanes++;
                    continue;
                }

                // Exclude public parking facilities and other garage-like entities.
                if (GetParkingKind(
                        lane,
                        lookups.CarParkingFacility,
                        lookups.CarParking,
                        lookups.Building,
                        lookups.Owner) != VisibleParkingKind.Building)
                {
                    snapshot.GarageCarNonBuildingLanes++;
                    continue;
                }

                snapshot.BuildingParkingLanes++;
                snapshot.BuildingGarageLanes++;
                snapshot.BuildingGarageCapacity += garageLane.m_VehicleCapacity;
                snapshot.BuildingGarageRawOccupied += garageLane.m_VehicleCount;

                // Capacity is the live game value. Another mod changing it is seen automatically.
                snapshot.BuildingParkingCapacity += garageLane.m_VehicleCapacity;

                // Raw occupancy is actual consumed capacity and may include bicycles.
                snapshot.BuildingParkingUsedSlots += garageLane.m_VehicleCount;

                buildingCarGarageLanes.Add(lane);
            }
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
        /// Adds parking totals using the same public helper and facility scope as the Roads InfoView.
        /// </summary>
        private void AddOfficialParkingTotals(
            ref ParkingSnapshot snapshot,
            ref ParkingProbeLookups lookups)
        {
            using NativeArray<Entity> facilities =
                m_ParkingFacilityQuery.ToEntityArray(Allocator.Temp);

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
                    ref lookups.ParkingLane,
                    ref lookups.PrefabRef,
                    ref lookups.Curve,
                    ref lookups.ParkingLaneData,
                    ref lookups.ParkedCar,
                    ref lookups.GarageLane,
                    ref lookups.LaneObject,
                    ref lookups.SubLane,
                    ref lookups.SubNet,
                    ref lookups.SubObject);

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

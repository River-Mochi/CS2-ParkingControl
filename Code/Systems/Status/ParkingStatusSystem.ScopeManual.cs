// <copyright file="ParkingStatusSystem.ScopeManual.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Builds separate scope and manual No Parking counters for the Options status rows.

using Game.Common;
using Game.Net;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{

    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Separates District/Whole City targets from manual road-tool targets.
        /// </summary>
        private void AddScopeManualCounters(ref ParkingSnapshot snapshot)
        {
            ComponentLookup<ParkingLane> parkingLaneLookup =
                GetComponentLookup<ParkingLane>(true);
            ComponentLookup<Owner> ownerLookup =
                GetComponentLookup<Owner>(true);
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup =
                GetComponentLookup<Game.Prefabs.PrefabRef>(true);
            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup =
                GetComponentLookup<Game.Prefabs.ParkingLaneData>(true);
            ComponentLookup<Road> roadLookup =
                GetComponentLookup<Road>(true);
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup =
                GetComponentLookup<Game.Areas.BorderDistrict>(true);
            ComponentLookup<ManualRoadParkingBan> manualBanLookup =
                GetComponentLookup<ManualRoadParkingBan>(true);
            ComponentLookup<StreetParkingState> stateLookup =
                GetComponentLookup<StreetParkingState>(true);
            ComponentLookup<ParkedCar> parkedCarLookup =
                GetComponentLookup<ParkedCar>(true);
            ComponentLookup<PersonalCar> personalCarLookup =
                GetComponentLookup<PersonalCar>(true);
            ComponentLookup<CarTrailer> carTrailerLookup =
                GetComponentLookup<CarTrailer>(true);
            ComponentLookup<Deleted> deletedLookup =
                GetComponentLookup<Deleted>(true);
            ComponentLookup<Game.Tools.Temp> tempLookup =
                GetComponentLookup<Game.Tools.Temp>(true);
            ComponentLookup<Destroyed> destroyedLookup =
                GetComponentLookup<Destroyed>(true);
            ComponentLookup<Game.Prefabs.BicycleData> bicycleDataLookup =
                GetComponentLookup<Game.Prefabs.BicycleData>(true);
            BufferLookup<LaneObject> laneObjectLookup =
                GetBufferLookup<LaneObject>(true);
            BufferLookup<Game.Policies.Policy> policyLookup =
                GetBufferLookup<Game.Policies.Policy>(true);

            Entity policyEntity = ParkingPolicySystem.PolicyEntity;
            PCSettings.ParkingScope scope = snapshot.Scope;

            using NativeArray<Entity> lanes =
                m_CurbLaneQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity lane in lanes)
            {
                ParkingLane parkingLane = parkingLaneLookup[lane];

                if (!NoStreetParkingSystem.IsStreetCarParkingLane(
                        lane,
                        parkingLane,
                        ownerLookup,
                        prefabRefLookup,
                        parkingLaneDataLookup,
                        roadLookup))
                {
                    continue;
                }

                bool manualTarget =
                    NoStreetParkingSystem.IsManualRestrictionTarget(
                        lane,
                        parkingLane,
                        ownerLookup,
                        manualBanLookup);

                bool scopeTarget =
                    NoStreetParkingSystem.IsScopeRestrictionTarget(
                        lane,
                        parkingLane,
                        scope,
                        policyEntity,
                        ownerLookup,
                        borderDistrictLookup,
                        policyLookup);

                if (!manualTarget && !scopeTarget)
                {
                    continue;
                }

                bool parkingDisabled =
                    (parkingLane.m_Flags &
                        ParkingLaneFlags.ParkingDisabled) != 0;

                bool tracked =
                    stateLookup.HasComponent(lane);

                int parkedCars =
                    CountPersonalMotorCarsOnLane(
                        lane,
                        laneObjectLookup,
                        parkedCarLookup,
                        personalCarLookup,
                        carTrailerLookup,
                        deletedLookup,
                        tempLookup,
                        destroyedLookup,
                        prefabRefLookup,
                        bicycleDataLookup);

                if (scopeTarget)
                {
                    snapshot.ScopeTargetCurbLanes++;

                    if (parkingDisabled)
                    {
                        snapshot.DisabledScopeTargetCurbLanes++;
                    }

                    if (tracked)
                    {
                        snapshot.TrackedScopeTargetCurbLanes++;
                    }

                    if (parkedCars > 0)
                    {
                        snapshot.OccupiedScopeTargetCurbLanes++;
                    }

                    snapshot.ScopeTargetStreetParked += parkedCars;
                }

                if (manualTarget)
                {
                    snapshot.ManualTargetCurbLanes++;

                    if (parkingDisabled)
                    {
                        snapshot.DisabledManualTargetCurbLanes++;
                    }

                    if (tracked)
                    {
                        snapshot.TrackedManualTargetCurbLanes++;
                    }

                    if (parkedCars > 0)
                    {
                        snapshot.OccupiedManualTargetCurbLanes++;
                    }

                    snapshot.ManualTargetStreetParked += parkedCars;
                }
            }
        }

        private static int CountPersonalMotorCarsOnLane(
            Entity lane,
            BufferLookup<LaneObject> laneObjectLookup,
            ComponentLookup<ParkedCar> parkedCarLookup,
            ComponentLookup<PersonalCar> personalCarLookup,
            ComponentLookup<CarTrailer> carTrailerLookup,
            ComponentLookup<Deleted> deletedLookup,
            ComponentLookup<Game.Tools.Temp> tempLookup,
            ComponentLookup<Destroyed> destroyedLookup,
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup,
            ComponentLookup<Game.Prefabs.BicycleData> bicycleDataLookup)
        {
            if (!laneObjectLookup.TryGetBuffer(
                    lane,
                    out DynamicBuffer<LaneObject> laneObjects))
            {
                return 0;
            }

            int count = 0;

            foreach (LaneObject laneObject in laneObjects)
            {
                Entity vehicle = laneObject.m_LaneObject;

                if (!parkedCarLookup.HasComponent(vehicle) ||
                    !personalCarLookup.HasComponent(vehicle) ||
                    carTrailerLookup.HasComponent(vehicle) ||
                    deletedLookup.HasComponent(vehicle) ||
                    tempLookup.HasComponent(vehicle) ||
                    destroyedLookup.HasComponent(vehicle) ||
                    !prefabRefLookup.TryGetComponent(
                        vehicle,
                        out Game.Prefabs.PrefabRef prefabRef) ||
                    prefabRef.m_Prefab == Entity.Null ||
                    bicycleDataLookup.HasComponent(prefabRef.m_Prefab))
                {
                    continue;
                }

                count++;
            }

            return count;
        }
    }
}

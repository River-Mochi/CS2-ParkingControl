// <copyright file="ParkingStatusSystem.Probe.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Coordinates on-demand parking status snapshots and owns their ECS lookups.

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
            ParkingProbeLookups lookups = CreateParkingProbeLookups();

            PCSettings.ParkingScope scope =
                Mod.Settings?.Scope ?? PCSettings.ParkingScope.Off;

            using RoadSizeRule roadSizeRule = CreateRoadSizeRule();

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

            // Used only during this on-demand snapshot. It lets the existing personal-vehicle
            // pass classify exact-capacity building garage occupants without another scan.
            int buildingGarageSetCapacity =
                Math.Max(1, m_GarageLaneQuery.CalculateEntityCount());
            using NativeHashSet<Entity> buildingCarGarageLanes =
                new(buildingGarageSetCapacity, Allocator.Temp);

            CollectDistricts(
                ref snapshot,
                details,
                policyEntity,
                ref lookups);

            CollectParkingLanes(
                ref snapshot,
                details,
                scope,
                policyEntity,
                roadSizeRule,
                ref lookups);

            CollectGarageLanes(
                ref snapshot,
                buildingCarGarageLanes,
                ref lookups);

            AddOfficialParkingTotals(
                ref snapshot,
                ref lookups);

            CollectPersonalVehicles(
                ref snapshot,
                details,
                scope,
                policyEntity,
                roadSizeRule,
                occupiedCurbLanes,
                buildingCarGarageLanes,
                ref lookups);

            // Visible fixed-slot building parking was counted directly from lane objects.
            // Hidden garage cars were classified in the personal-vehicle pass.
            snapshot.BuildingParkingOccupied += snapshot.BuildingGarageCarOccupied;

            // A real mismatch gets one full reconcile when simulation resumes.
            if (snapshot.TargetCurbLanes > snapshot.DisabledTargetCurbLanes ||
                snapshot.TrackedCurbLanes > snapshot.DisabledCurbLanes)
            {
                NoStreetParkingSystem.RequestReconcile();
            }

            return snapshot;
        }

        /// <summary>
        /// Captures the read-only ECS lookups shared by one on-demand snapshot.
        /// </summary>
        private ParkingProbeLookups CreateParkingProbeLookups()
        {
            return new ParkingProbeLookups
            {
                ManualRoadBan = GetComponentLookup<ManualRoadParkingBan>(true),
                BicycleData = GetComponentLookup<Game.Prefabs.BicycleData>(true),
                Building = GetComponentLookup<Game.Buildings.Building>(true),
                CarParkingFacility = GetComponentLookup<Game.Buildings.CarParkingFacility>(true),
                BorderDistrict = GetComponentLookup<Game.Areas.BorderDistrict>(true),
                CurrentLane = GetComponentLookup<Game.Vehicles.CarCurrentLane>(true),
                ConnectionLane = GetComponentLookup<Game.Net.ConnectionLane>(true),
                Curve = GetComponentLookup<Game.Net.Curve>(true),
                Deleted = GetComponentLookup<Game.Common.Deleted>(true),
                GarageLane = GetComponentLookup<Game.Net.GarageLane>(true),
                Household = GetComponentLookup<Game.Citizens.Household>(true),
                NetOutsideConnection = GetComponentLookup<Game.Net.OutsideConnection>(true),
                ObjectOutsideConnection = GetComponentLookup<Game.Objects.OutsideConnection>(true),
                Owner = GetComponentLookup<Game.Common.Owner>(true),
                ParkedCar = GetComponentLookup<Game.Vehicles.ParkedCar>(true),
                ParkingLane = GetComponentLookup<Game.Net.ParkingLane>(true),
                ParkingLaneData = GetComponentLookup<Game.Prefabs.ParkingLaneData>(true),
                PersonalCar = GetComponentLookup<Game.Vehicles.PersonalCar>(true),
                CarParking = GetComponentLookup<Game.Routes.CarParking>(true),
                PrefabRef = GetComponentLookup<Game.Prefabs.PrefabRef>(true),
                Road = GetComponentLookup<Game.Net.Road>(true),
                SlaveLane = GetComponentLookup<Game.Net.SlaveLane>(true),
                StreetParkingState = GetComponentLookup<StreetParkingState>(true),
                TripSource = GetComponentLookup<Game.Objects.TripSource>(true),
                Unspawned = GetComponentLookup<Game.Objects.Unspawned>(true),
                LaneObject = GetBufferLookup<Game.Net.LaneObject>(true),
                OwnedVehicle = GetBufferLookup<Game.Vehicles.OwnedVehicle>(true),
                Policy = GetBufferLookup<Game.Policies.Policy>(true),
                SubLane = GetBufferLookup<Game.Net.SubLane>(true),
                SubNet = GetBufferLookup<Game.Net.SubNet>(true),
                SubObject = GetBufferLookup<Game.Objects.SubObject>(true),
            };
        }

        /// <summary>
        /// Read-only ECS data reused across the curb, garage, supply, and vehicle passes.
        /// </summary>
        private struct ParkingProbeLookups
        {
            internal ComponentLookup<ManualRoadParkingBan> ManualRoadBan;
            internal ComponentLookup<Game.Prefabs.BicycleData> BicycleData;
            internal ComponentLookup<Game.Buildings.Building> Building;
            internal ComponentLookup<Game.Buildings.CarParkingFacility> CarParkingFacility;
            internal ComponentLookup<Game.Areas.BorderDistrict> BorderDistrict;
            internal ComponentLookup<Game.Vehicles.CarCurrentLane> CurrentLane;
            internal ComponentLookup<Game.Net.ConnectionLane> ConnectionLane;
            internal ComponentLookup<Game.Net.Curve> Curve;
            internal ComponentLookup<Game.Common.Deleted> Deleted;
            internal ComponentLookup<Game.Net.GarageLane> GarageLane;
            internal ComponentLookup<Game.Citizens.Household> Household;
            internal ComponentLookup<Game.Net.OutsideConnection> NetOutsideConnection;
            internal ComponentLookup<Game.Objects.OutsideConnection> ObjectOutsideConnection;
            internal ComponentLookup<Game.Common.Owner> Owner;
            internal ComponentLookup<Game.Vehicles.ParkedCar> ParkedCar;
            internal ComponentLookup<Game.Net.ParkingLane> ParkingLane;
            internal ComponentLookup<Game.Prefabs.ParkingLaneData> ParkingLaneData;
            internal ComponentLookup<Game.Vehicles.PersonalCar> PersonalCar;
            internal ComponentLookup<Game.Routes.CarParking> CarParking;
            internal ComponentLookup<Game.Prefabs.PrefabRef> PrefabRef;
            internal ComponentLookup<Game.Net.Road> Road;
            internal ComponentLookup<Game.Net.SlaveLane> SlaveLane;
            internal ComponentLookup<StreetParkingState> StreetParkingState;
            internal ComponentLookup<Game.Objects.TripSource> TripSource;
            internal ComponentLookup<Game.Objects.Unspawned> Unspawned;
            internal BufferLookup<Game.Net.LaneObject> LaneObject;
            internal BufferLookup<Game.Vehicles.OwnedVehicle> OwnedVehicle;
            internal BufferLookup<Game.Policies.Policy> Policy;
            internal BufferLookup<Game.Net.SubLane> SubLane;
            internal BufferLookup<Game.Net.SubNet> SubNet;
            internal BufferLookup<Game.Objects.SubObject> SubObject;
        }
    }
}

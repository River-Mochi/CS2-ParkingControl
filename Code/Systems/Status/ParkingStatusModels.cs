// <copyright file="ParkingStatusModels.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Holds the short-lived models shared by parking probes, status text, and log reports.

namespace ParkingControl
{
    using System;
    using System.Collections.Generic;
    using Unity.Entities;

    /// <summary>
    /// A read-only snapshot of curb enforcement, parking supply, and personal vehicles.
    /// </summary>
    internal struct ParkingSnapshot
    {
        public DateTime CapturedAtLocal;
        public uint SimulationFrame;
        public PCSettings.ParkingScope Scope;
        public int CurbLanes;
        public int TargetCurbLanes;
        public int DisabledCurbLanes;
        public int DisabledTargetCurbLanes;
        public int TrackedCurbLanes;
        public int OccupiedCurbLanes;
        public int OccupiedTargetCurbLanes;
        public int FixedSlotCurbLanes;
        public int FixedSlotCurbCapacity;
        public int FixedSlotCurbParked;
        public int ContinuousCurbLanes;
        public int ContinuousCurbParked;
        public int TotalVehicles;
        public int ParkedVehicles;
        public int ActiveVehicles;
        public int UnlocatedVehicles;
        public int StreetParked;
        public int TargetStreetParked;
        public int VisibleOffStreet;
        public int VisibleFacilityParking;
        public int VisibleBuildingParking;
        public int VisibleOtherParking;
        public int HiddenInBuildings;
        public int OutsideConnection;
        public int OutsideConnectionHidden;
        public int OutsideValidHouseholdOwned;
        public int OutsideHouseholdOwnershipInvalid;
        public int OutsideConnectionOwner;
        public int OutsideDummyTraffic;
        public int OutsideOtherOrUnowned;
        public int OutsideResidentHousehold;
        public int OutsideTouristHousehold;
        public int OutsideCommuterHousehold;
        public int OutsideResidentNotMovedIn;
        public int UnassignedOrUnknownParked;
        public int UnknownNullLane;
        public int UnknownMissingLane;
        public int UnknownUnspawned;
        public int UnknownWithTripSource;
        public int UnknownWithoutTripSource;
        public int UnknownTripSourceOutside;
        public int UnknownTripSourceBuilding;
        public int UnknownTripSourceOther;
        public int UnknownTripSourceMissing;
        public int UnknownValidHouseholdOwned;
        public int UnknownHouseholdOwnershipInvalid;
        public int UnknownDummyTraffic;
        public int UnknownOtherOrUnowned;
        public int UnknownResidentHousehold;
        public int UnknownTouristHousehold;
        public int UnknownCommuterHousehold;
        public int UnknownResidentNotMovedIn;
        public int HouseholdOwnerVehicles;
        public int ResidentHouseholdVehicles;
        public int TouristHouseholdVehicles;
        public int CommuterHouseholdVehicles;
        public int ResidentNotMovedInVehicles;
        public int LiveHouseholdOwnerVehicles;
        public int DeletedHouseholdOwnerVehicles;
        public int OwnedVehicleMatches;
        public int OwnedVehicleMissing;
        public int MissingOwnerVehicles;
        public int DummyTrafficVehicles;
        public int OtherOrUnownedVehicles;
        public int OfficialParkingFacilities;
        public int OfficialParkingCapacity;
        public int OfficialParkingOccupied;
        public int BuildingParkingLanes;
        public int BuildingParkingCapacity;
        public int BuildingParkingOccupied;
        public int BuildingFixedSlotLanes;
        public int BuildingGarageLanes;
        public int BuildingContinuousLanes;
        public int BuildingContinuousOccupied;
        public int GarageLanes;
        public int GarageCapacity;
        public int GarageOccupied;
        public int Districts;
        public int DistrictsWithPolicy;

        public readonly bool RestrictionEnabled =>
            Scope != PCSettings.ParkingScope.Off ||
            TargetCurbLanes > 0;

        public readonly int ParkedElsewhere =>
            VisibleOffStreet + HiddenInBuildings + OutsideConnection + UnassignedOrUnknownParked;

        // This excludes outside-connection and unassigned staging because neither is
        // a known parking place inside the city.
        public readonly int KnownInCityParking =>
            StreetParked + OfficialParkingOccupied + BuildingParkingOccupied;
    }

    /// <summary>
    /// Extra entity collections and transition counters needed only by manual log reports.
    /// </summary>
    internal sealed class ParkingReportDetails
    {
        public ParkingReportDetails(int sampleLimit)
        {
            StreetSamples = new List<Entity>(sampleLimit);
            VisibleSamples = new List<Entity>(sampleLimit);
            HiddenSamples = new List<Entity>(sampleLimit);
            OutsideSamples = new List<Entity>(sampleLimit);
            UnknownSamples = new List<Entity>(sampleLimit);
            UnknownOutsideSourceSamples = new List<Entity>(sampleLimit);
            UnknownBuildingSourceSamples = new List<Entity>(sampleLimit);
            UnknownOtherSourceSamples = new List<Entity>(sampleLimit);
            UnknownMissingSourceSamples = new List<Entity>(sampleLimit);
            UnknownNoSourceSamples = new List<Entity>(sampleLimit);
            SampleTransitions = new List<VehicleSampleTransition>(sampleLimit * 3);
        }

        public Dictionary<Entity, DistrictParkingStats> DistrictParking { get; } =
            new Dictionary<Entity, DistrictParkingStats>();

        public List<Entity> StreetSamples { get; }

        public List<Entity> VisibleSamples { get; }

        public List<Entity> HiddenSamples { get; }

        public List<Entity> OutsideSamples { get; }

        public List<Entity> UnknownSamples { get; }

        public List<Entity> UnknownOutsideSourceSamples { get; }

        public List<Entity> UnknownBuildingSourceSamples { get; }

        public List<Entity> UnknownOtherSourceSamples { get; }

        public List<Entity> UnknownMissingSourceSamples { get; }

        public List<Entity> UnknownNoSourceSamples { get; }

        public List<VehicleSampleTransition> SampleTransitions { get; }
    }

    /// <summary>
    /// Log-only street-parking counters for one district side, or Entity.Null outside districts.
    /// </summary>
    internal sealed class DistrictParkingStats
    {
        public DistrictParkingStats(Entity district, bool policyActive)
        {
            District = district;
            PolicyActive = policyActive;
        }

        public Entity District { get; }

        public bool PolicyActive { get; set; }

        public int EligibleLanes { get; set; }

        public int DisabledLanes { get; set; }

        public int TrackedLanes { get; set; }

        public int StreetCars { get; set; }

        public int OccupiedLanes { get; set; }
    }

    /// <summary>
    /// Mutually exclusive location buckets for non-bicycle personal motor vehicles.
    /// </summary>
    internal enum VehicleLocation
    {
        UnassignedOrUnknown,
        Active,
        StreetCurb,
        VisibleOffStreet,
        HiddenInBuilding,
        OutsideConnection,
    }

    /// <summary>
    /// Identifies which previous-report sample group a vehicle came from.
    /// </summary>
    internal enum VehicleSampleSource
    {
        Street,
        OutsideConnection,
        Unknown,
    }

    /// <summary>
    /// Records where one bounded previous-report sample was found now.
    /// </summary>
    internal readonly struct VehicleSampleTransition
    {
        public VehicleSampleTransition(
            Entity vehicle,
            VehicleSampleSource source,
            VehicleLocation currentLocation)
        {
            Vehicle = vehicle;
            Source = source;
            CurrentLocation = currentLocation;
        }

        public Entity Vehicle { get; }

        public VehicleSampleSource Source { get; }

        public VehicleLocation CurrentLocation { get; }
    }

    /// <summary>
    /// Splits rendered non-street parking into useful log-only categories.
    /// </summary>
    internal enum VisibleParkingKind
    {
        Other,
        Facility,
        Building,
    }
}

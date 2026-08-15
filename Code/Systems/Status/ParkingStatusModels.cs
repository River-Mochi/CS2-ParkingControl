// <copyright file="ParkingStatusModels.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
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

        public readonly bool RestrictionEnabled => Scope != PCSettings.ParkingScope.Off;

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
        public HashSet<Entity> CurrentStreetVehicles { get; } = new HashSet<Entity>();

        public List<Entity> VisibleSamples { get; } = new List<Entity>();

        public List<Entity> HiddenSamples { get; } = new List<Entity>();

        public List<Entity> OutsideSamples { get; } = new List<Entity>();

        public List<Entity> UnknownSamples { get; } = new List<Entity>();

        public List<Entity> UnknownOutsideSourceSamples { get; } = new List<Entity>();

        public List<Entity> UnknownBuildingSourceSamples { get; } = new List<Entity>();

        public List<Entity> UnknownOtherSourceSamples { get; } = new List<Entity>();

        public List<Entity> UnknownMissingSourceSamples { get; } = new List<Entity>();

        public List<Entity> UnknownNoSourceSamples { get; } = new List<Entity>();

        public int SeenPreviousStreet { get; set; }

        public int RemainedOnStreet { get; set; }

        public int NewlyParkedOnStreet { get; set; }

        public int NowActive { get; set; }

        public int NowOffStreet { get; set; }

        public int NowHiddenInBuilding { get; set; }

        public int NowAtOutsideConnection { get; set; }

        public int NowUnassignedOrUnknown { get; set; }
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
    /// Splits rendered non-street parking into useful log-only categories.
    /// </summary>
    internal enum VisibleParkingKind
    {
        Other,
        Facility,
        Building,
    }
}

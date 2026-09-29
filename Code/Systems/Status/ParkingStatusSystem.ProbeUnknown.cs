// <copyright file="ParkingStatusSystem.ProbeUnknown.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Collects diagnostics for personal vehicles without a usable parking location.

using Unity.Entities;

namespace ParkingControl
{
    public sealed partial class ParkingStatusSystem
    {
        private void CollectUnknownVehicle(
            ref ParkingSnapshot snapshot,
            ParkingReportDetails? details,
            Entity vehicle,
            bool householdOwner,
            bool touristHousehold,
            bool commuterHousehold,
            bool residentMovedIn,
            bool ownerDeleted,
            bool ownedVehicleMatch,
            bool dummyTraffic,
            ref ParkingProbeLookups lookups)
        {
            if (!lookups.ParkedCar.TryGetComponent(
                    vehicle,
                    out Game.Vehicles.ParkedCar unknownParkedCar))
            {
                snapshot.UnlocatedVehicles++;
                return;
            }

            snapshot.ParkedVehicles++;
            snapshot.UnassignedOrUnknownParked++;

            if (details == null)
            {
                return;
            }

            // Vanilla can stage a valid household car at its trip source without
            // finding a concrete parking lane.
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
                lookups.TripSource,
                lookups.Unspawned,
                lookups.NetOutsideConnection,
                lookups.ObjectOutsideConnection,
                lookups.Building,
                lookups.Owner);
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
    }
}

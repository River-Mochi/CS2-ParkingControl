// <copyright file="ParkingStatusSystem.Vehicle.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Classifies personal-vehicle locations and validates their ownership relationships.

namespace ParkingControl
{
    using Unity.Entities;

    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Classifies one personal car without changing its current parking or travel state.
        /// </summary>
        private VehicleLocation GetVehicleLocation(
            Entity vehicle,
            ComponentLookup<Game.Vehicles.ParkedCar> parkedCarLookup,
            ComponentLookup<Game.Vehicles.CarCurrentLane> currentLaneLookup,
            ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup,
            ComponentLookup<Game.Common.Owner> ownerLookup,
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup,
            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup,
            ComponentLookup<Game.Net.Road> roadLookup,
            ComponentLookup<Game.Net.OutsideConnection> outsideConnectionLookup,
            ComponentLookup<Game.Net.GarageLane> garageLaneLookup,
            ComponentLookup<Game.Objects.Unspawned> unspawnedLookup,
            ComponentLookup<Game.Buildings.Building> buildingLookup,
            out Entity parkedLane)
        {
            parkedLane = Entity.Null;
            if (!parkedCarLookup.TryGetComponent(vehicle, out Game.Vehicles.ParkedCar parkedCar))
            {
                return currentLaneLookup.HasComponent(vehicle)
                    ? VehicleLocation.Active
                    : VehicleLocation.UnassignedOrUnknown;
            }

            parkedLane = parkedCar.m_Lane;
            if (parkedLane == Entity.Null || !EntityManager.Exists(parkedLane))
            {
                return VehicleLocation.UnassignedOrUnknown;
            }

            if (parkingLaneLookup.TryGetComponent(
                    parkedLane,
                    out Game.Net.ParkingLane parkingLane) &&
                ownerLookup.HasComponent(parkedLane) &&
                prefabRefLookup.HasComponent(parkedLane) &&
                NoStreetParkingSystem.IsStreetCarParkingLane(
                    parkedLane,
                    parkingLane,
                    ownerLookup,
                    prefabRefLookup,
                    parkingLaneDataLookup,
                    roadLookup))
            {
                return VehicleLocation.StreetCurb;
            }

            if (IsOutsideConnectionLane(parkedLane, outsideConnectionLookup, ownerLookup))
            {
                return VehicleLocation.OutsideConnection;
            }

            if (garageLaneLookup.HasComponent(parkedLane) ||
                (unspawnedLookup.HasComponent(vehicle) &&
                    IsOwnedByBuilding(parkedLane, buildingLookup, ownerLookup)))
            {
                return VehicleLocation.HiddenInBuilding;
            }

            return VehicleLocation.VisibleOffStreet;
        }

        /// <summary>
        /// Verifies that the household's ownership buffer points back to this vehicle.
        /// </summary>
        private static bool HasOwnedVehicle(
            Entity owner,
            Entity vehicle,
            BufferLookup<Game.Vehicles.OwnedVehicle> ownedVehicleLookup)
        {
            if (!ownedVehicleLookup.TryGetBuffer(
                    owner,
                    out DynamicBuffer<Game.Vehicles.OwnedVehicle> vehicles))
            {
                return false;
            }

            foreach (Game.Vehicles.OwnedVehicle ownedVehicle in vehicles)
            {
                if (ownedVehicle.m_Vehicle == vehicle)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks that the owner still rents a non-deleted property entity.
        /// </summary>
        private bool HasLiveProperty(
            Entity owner,
            bool ownerExists,
            ComponentLookup<Game.Buildings.PropertyRenter> propertyRenterLookup,
            ComponentLookup<Game.Common.Deleted> deletedLookup)
        {
            if (!ownerExists ||
                !propertyRenterLookup.TryGetComponent(
                    owner,
                    out Game.Buildings.PropertyRenter propertyRenter))
            {
                return false;
            }

            Entity property = propertyRenter.m_Property;
            return property != Entity.Null &&
                EntityManager.Exists(property) &&
                !deletedLookup.HasComponent(property);
        }

        /// <summary>
        /// Identifies lanes whose prefab exposes an exact repeatable slot interval.
        /// </summary>
        private static bool IsFixedSlotLane(
            Entity lane,
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup,
            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup)
        {
            if (!prefabRefLookup.TryGetComponent(lane, out Game.Prefabs.PrefabRef prefabRef))
            {
                return false;
            }

            return parkingLaneDataLookup.TryGetComponent(
                    prefabRef.m_Prefab,
                    out Game.Prefabs.ParkingLaneData laneData) &&
                laneData.m_SlotInterval != 0f;
        }

        /// <summary>
        /// Follows lane ownership because outside-connection lanes are usually nested entities.
        /// </summary>
        private static bool IsOutsideConnectionLane(
            Entity lane,
            ComponentLookup<Game.Net.OutsideConnection> outsideConnectionLookup,
            ComponentLookup<Game.Common.Owner> ownerLookup)
        {
            Entity current = lane;
            for (int i = 0; i < 16 && current != Entity.Null; i++)
            {
                if (outsideConnectionLookup.HasComponent(current))
                {
                    return true;
                }

                if (!ownerLookup.TryGetComponent(current, out Game.Common.Owner owner))
                {
                    return false;
                }

                current = owner.m_Owner;
            }

            return false;
        }

        /// <summary>
        /// Follows nested lane owners to distinguish hidden building storage.
        /// </summary>
        private static bool IsOwnedByBuilding(
            Entity entity,
            ComponentLookup<Game.Buildings.Building> buildingLookup,
            ComponentLookup<Game.Common.Owner> ownerLookup)
        {
            Entity current = entity;
            for (int i = 0; i < 16 && current != Entity.Null; i++)
            {
                if (buildingLookup.HasComponent(current))
                {
                    return true;
                }

                if (!ownerLookup.TryGetComponent(current, out Game.Common.Owner owner))
                {
                    return false;
                }

                current = owner.m_Owner;
            }

            return false;
        }

        /// <summary>
        /// Returns the final reachable owner for Scene Explorer's LaneRoot entity ID.
        /// </summary>
        private static Entity GetTopOwner(
            Entity entity,
            ComponentLookup<Game.Common.Owner> ownerLookup)
        {
            Entity current = entity;
            Entity topOwner = entity;
            for (int i = 0; i < 16 && current != Entity.Null; i++)
            {
                topOwner = current;
                if (!ownerLookup.TryGetComponent(current, out Game.Common.Owner owner) ||
                    owner.m_Owner == Entity.Null)
                {
                    break;
                }

                current = owner.m_Owner;
            }

            return topOwner;
        }
    }
}

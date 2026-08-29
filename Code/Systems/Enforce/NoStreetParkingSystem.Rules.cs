// <copyright file="NoStreetParkingSystem.Rules.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Decides which curb lanes are targeted and prunes stale manual bans.

    using Game.Common;
    using Game.Net;
    using Unity.Collections;
    using Unity.Entities;

namespace ParkingControl
{


    public sealed partial class NoStreetParkingSystem
    {
        /// <summary>
        /// Returns whether this road side is covered by any Parking Control rule.
        /// </summary>

        internal static bool IsRestrictionTarget(
            Entity lane,
            ParkingLane parkingLane,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup,
            ComponentLookup<ManualRoadParkingBan> manualBanLookup,
            BufferLookup<Game.Policies.Policy> policyLookup)
        {
            return IsManualRestrictionTarget(
                    lane,
                    parkingLane,
                    ownerLookup,
                    manualBanLookup) ||
                IsScopeRestrictionTarget(
                    lane,
                    parkingLane,
                    scope,
                    policyEntity,
                    ownerLookup,
                    borderDistrictLookup,
                    policyLookup);
        }

        internal static bool IsManualRestrictionTarget(
            Entity lane,
            ParkingLane parkingLane,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<ManualRoadParkingBan> manualBanLookup)
        {
            Entity road = ownerLookup[lane].m_Owner;

            if (!manualBanLookup.TryGetComponent(
                    road,
                    out ManualRoadParkingBan manualBan))
            {
                return false;
            }

            bool rightSide =
                (parkingLane.m_Flags &
                    ParkingLaneFlags.RightSide) != 0;

            return manualBan.IsBanned(rightSide);
        }

        internal static bool IsScopeRestrictionTarget(
            Entity lane,
            ParkingLane parkingLane,
            PCSettings.ParkingScope scope,
            Entity policyEntity,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup,
            BufferLookup<Game.Policies.Policy> policyLookup)
        {
            if (scope == PCSettings.ParkingScope.WholeCity)
            {
                return true;
            }

            if (scope != PCSettings.ParkingScope.ByDistrict ||
                policyEntity == Entity.Null)
            {
                return false;
            }

            Entity district =
                GetLaneDistrict(
                    lane,
                    parkingLane,
                    ownerLookup,
                    borderDistrictLookup);

            return IsDistrictPolicyActive(
                district,
                policyEntity,
                policyLookup);
        }

        /// <summary>
        /// Gets the district governing this side of a road parking lane.
        /// </summary>
        internal static Entity GetLaneDistrict(
            Entity lane,
            ParkingLane parkingLane,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Areas.BorderDistrict> borderDistrictLookup)
        {
            Entity road = ownerLookup[lane].m_Owner;

            if (!borderDistrictLookup.TryGetComponent(
                    road,
                    out Game.Areas.BorderDistrict borderDistrict))
            {
                return Entity.Null;
            }

            return (parkingLane.m_Flags &
                    ParkingLaneFlags.RightSide) != 0
                ? borderDistrict.m_Right
                : borderDistrict.m_Left;
        }

        /// <summary>
        /// Returns whether a district has Parking Control's policy enabled.
        /// </summary>
        internal static bool IsDistrictPolicyActive(
            Entity district,
            Entity policyEntity,
            BufferLookup<Game.Policies.Policy> policyLookup)
        {
            if (district == Entity.Null ||
                policyEntity == Entity.Null ||
                !policyLookup.TryGetBuffer(
                    district,
                    out DynamicBuffer<Game.Policies.Policy> policies))
            {
                return false;
            }

            foreach (Game.Policies.Policy policy in policies)
            {
                if (policy.m_Policy == policyEntity &&
                    (policy.m_Flags &
                        Game.Policies.PolicyFlags.Active) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasPolicyChange(Entity policyEntity)
        {
            if (policyEntity == Entity.Null ||
                m_PolicyModifyQuery.IsEmptyIgnoreFilter)
            {
                return false;
            }

            using NativeArray<Game.Policies.Modify> modifications =
                m_PolicyModifyQuery
                    .ToComponentDataArray<Game.Policies.Modify>(
                        Allocator.Temp);

            foreach (Game.Policies.Modify modification in modifications)
            {
                if (modification.m_Policy == policyEntity)
                {
                    return true;
                }
            }

            return false;
        }

        private int PruneInvalidManualBans(EntityQuery query)
        {
            int removedSides = 0;

            using NativeArray<Entity> roads =
                query.ToEntityArray(Allocator.Temp);

            foreach (Entity road in roads)
            {
                removedSides += PruneInvalidManualBan(road);
            }

            return removedSides;
        }

        private int PruneInvalidManualBan(Entity road)
        {
            if (road == Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasComponent<ManualRoadParkingBan>(road))
            {
                return 0;
            }

            ManualRoadParkingBan ban =
                EntityManager.GetComponentData<ManualRoadParkingBan>(road);

            bool hadLeft = ban.IsBanned(rightSide: false);
            bool hadRight = ban.IsBanned(rightSide: true);

            bool hasLeftParking = false;
            bool hasRightParking = false;

            if (EntityManager.HasBuffer<SubLane>(road))
            {
                DynamicBuffer<SubLane> subLanes =
                    EntityManager.GetBuffer<SubLane>(
                        road,
                        isReadOnly: true);

                foreach (SubLane subLane in subLanes)
                {
                    Entity lane = subLane.m_SubLane;

                    if (!ManualNoParkingToolSystem.TryGetEligibleParkingLane(
                            EntityManager,
                            road,
                            lane,
                            out ParkingLane parkingLane,
                            out Curve _))
                    {
                        continue;
                    }

                    if ((parkingLane.m_Flags &
                            ParkingLaneFlags.RightSide) != 0)
                    {
                        hasRightParking = true;
                    }
                    else
                    {
                        hasLeftParking = true;
                    }
                }
            }

            if (hadLeft && !hasLeftParking)
            {
                ban.SetBanned(rightSide: false, banned: false);
            }

            if (hadRight && !hasRightParking)
            {
                ban.SetBanned(rightSide: true, banned: false);
            }

            int removedSides =
                (hadLeft && !hasLeftParking ? 1 : 0) +
                (hadRight && !hasRightParking ? 1 : 0);

            if (removedSides == 0)
            {
                return 0;
            }

            if (ban.IsEmpty)
            {
                EntityManager.RemoveComponent<ManualRoadParkingBan>(road);
            }
            else
            {
                EntityManager.SetComponentData(road, ban);
            }

            return removedSides;
        }

        internal static bool IsStreetCarParkingLane(
            Entity entity,
            ParkingLane parkingLane,
            ComponentLookup<Owner> ownerLookup,
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup,
            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup,
            ComponentLookup<Road> roadLookup)
        {
            if ((parkingLane.m_Flags &
                    (ParkingLaneFlags.VirtualLane |
                        ParkingLaneFlags.SpecialVehicles)) != 0)
            {
                return false;
            }

            Owner owner = ownerLookup[entity];

            if (!roadLookup.HasComponent(owner.m_Owner))
            {
                return false;
            }

            Game.Prefabs.PrefabRef prefabRef = prefabRefLookup[entity];

            return parkingLaneDataLookup.TryGetComponent(
                    prefabRef.m_Prefab,
                    out Game.Prefabs.ParkingLaneData parkingLaneData) &&
                (parkingLaneData.m_RoadTypes & Game.Net.RoadTypes.Car) != 0;
        }
    }
}

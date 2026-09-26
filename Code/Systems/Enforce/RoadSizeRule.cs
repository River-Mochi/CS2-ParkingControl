// <copyright file="RoadSizeRule.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Counts driving lanes per road so citywide road-size bans can target them.

using System;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{

    /// <summary>
    /// Matches roads by driving-lane count for the citywide road-size parking bans.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Lane counting reads the edge composition rather than the prefab name, because
    /// prefab names do not match the player-facing road names, are not localized, and
    /// do not exist at all for roads built by mods such as Road Builder.
    /// </para>
    /// <para>
    /// Safe to pass by value once created: the cache is a native container whose
    /// entries are shared by every copy, so cached counts survive the copy.
    /// </para>
    /// </remarks>
    internal struct RoadSizeRule : IDisposable
    {
        internal const int kFourLaneRoad = 4;
        internal const int kSixLaneRoad = 6;

        /// <summary>Returned when a road has no readable edge composition.</summary>
        internal const int kUnknownLaneCount = -1;

        // Vanilla counts a driving lane as a Road lane that is not bicycles-only.
        // Game.Prefabs.NetCompositionHelpers, where HasForwardRoadLanes and
        // HasBackwardRoadLanes are set from exactly this test.
        private const LaneFlags kDrivingLaneMask =
            LaneFlags.Road | LaneFlags.BicyclesOnly;

        // One extra Master lane is appended per multi-lane group as a group aggregate.
        // Counting it would report a Six-Lane Road as eight lanes.
        private const LaneFlags kGroupAggregate = LaneFlags.Master;

        private const int kInitialCacheCapacity = 64;

        // Keyed by composition entity, which every edge of the same road type and
        // upgrade combination shares, so a whole city resolves to a few hundred entries.
        private NativeHashMap<Entity, int> m_DrivingLaneCounts;

        private ComponentLookup<Game.Net.Composition> m_Compositions;
        private BufferLookup<NetCompositionLane> m_CompositionLanes;
        private ComponentLookup<PrefabRef> m_PrefabRefs;
        private ComponentLookup<ParkingLaneData> m_ParkingLaneData;

        private bool m_BanFourLaneRoads;
        private bool m_BanSixLaneRoads;

        /// <summary>
        /// Gets a value indicating whether either road-size ban is switched on.
        /// </summary>
        internal readonly bool IsActive =>
            m_BanFourLaneRoads || m_BanSixLaneRoads;

        /// <summary>
        /// Creates a rule and its per-pass lane-count cache from the current options.
        /// </summary>
        /// <param name="settings">Mod options, or null before options load.</param>
        /// <param name="compositions">Edge composition lookup.</param>
        /// <param name="compositionLanes">Composition lane buffer lookup.</param>
        /// <param name="prefabRefs">Prefab reference lookup.</param>
        /// <param name="parkingLaneData">Parking lane prefab data lookup.</param>
        /// <param name="allocator">Allocator for the lane-count cache.</param>
        /// <returns>A rule that must be disposed when the pass ends.</returns>
        internal static RoadSizeRule Create(
            PCSettings? settings,
            ComponentLookup<Game.Net.Composition> compositions,
            BufferLookup<NetCompositionLane> compositionLanes,
            ComponentLookup<PrefabRef> prefabRefs,
            ComponentLookup<ParkingLaneData> parkingLaneData,
            Allocator allocator)
        {
            RoadSizeRule rule = default;

            rule.m_BanFourLaneRoads = settings?.BanFourLaneRoads ?? false;
            rule.m_BanSixLaneRoads = settings?.BanSixLaneRoads ?? false;
            rule.m_Compositions = compositions;
            rule.m_CompositionLanes = compositionLanes;
            rule.m_PrefabRefs = prefabRefs;
            rule.m_ParkingLaneData = parkingLaneData;

            // Always cached: the log report measures every road even while both
            // toggles are off, and an empty map costs almost nothing.
            rule.m_DrivingLaneCounts =
                new NativeHashMap<Entity, int>(
                    kInitialCacheCapacity,
                    allocator);

            return rule;
        }

        /// <summary>
        /// Returns whether a parking lane's road matches an enabled road-size ban.
        /// </summary>
        /// <param name="lane">Parking lane being tested.</param>
        /// <param name="road">Road edge owning the parking lane.</param>
        /// <returns>True when this road's lane count is banned citywide.</returns>
        internal bool IsRoadSizeTarget(Entity lane, Entity road)
        {
            if (!IsActive || HasBuiltInParkingSpaces(lane))
            {
                return false;
            }

            int lanes = GetDrivingLaneCount(road);

            return (m_BanFourLaneRoads && lanes == kFourLaneRoad) ||
                (m_BanSixLaneRoads && lanes == kSixLaneRoad);
        }

        /// <summary>
        /// Returns whether this lane is a marked parking bay rather than plain curb parking.
        /// </summary>
        /// <remarks>
        /// Roads such as the Four-Lane Angled Parking Road exist to provide parking, so a
        /// road-size ban must leave them alone. Their lanes carry a fixed slot interval,
        /// which is the same test the status probe uses to count fixed-slot curb lanes;
        /// ordinary parallel curb parking is continuous and has an interval of zero.
        /// </remarks>
        /// <param name="lane">Parking lane being tested.</param>
        /// <returns>True when the lane has built-in marked parking spaces.</returns>
        internal bool HasBuiltInParkingSpaces(Entity lane)
        {
            return m_PrefabRefs.TryGetComponent(
                    lane,
                    out PrefabRef prefabRef) &&
                m_ParkingLaneData.TryGetComponent(
                    prefabRef.m_Prefab,
                    out ParkingLaneData parkingLaneData) &&
                parkingLaneData.m_SlotInterval != 0f;
        }

        /// <summary>
        /// Gets the driving-lane count for a road, caching it per composition.
        /// </summary>
        /// <param name="road">Road edge to measure.</param>
        /// <returns>Driving lanes, or <see cref="kUnknownLaneCount"/> when unreadable.</returns>
        internal int GetDrivingLaneCount(Entity road)
        {
            if (road == Entity.Null ||
                !m_Compositions.TryGetComponent(
                    road,
                    out Game.Net.Composition composition) ||
                composition.m_Edge == Entity.Null)
            {
                // Nodes and building owners have no edge composition. Street parking
                // lanes only ever belong to edges, so this is not a miss in practice.
                return kUnknownLaneCount;
            }

            if (m_DrivingLaneCounts.TryGetValue(
                    composition.m_Edge,
                    out int cached))
            {
                return cached;
            }

            int counted = CountDrivingLanes(composition.m_Edge);
            m_DrivingLaneCounts.TryAdd(composition.m_Edge, counted);

            return counted;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (m_DrivingLaneCounts.IsCreated)
            {
                m_DrivingLaneCounts.Dispose();
            }
        }

        private int CountDrivingLanes(Entity edgeComposition)
        {
            if (!m_CompositionLanes.TryGetBuffer(
                    edgeComposition,
                    out DynamicBuffer<NetCompositionLane> lanes))
            {
                return kUnknownLaneCount;
            }

            int count = 0;

            foreach (NetCompositionLane lane in lanes)
            {
                if ((lane.m_Flags & kDrivingLaneMask) != LaneFlags.Road)
                {
                    // Sidewalks, tracks, parking lanes and bicycle lanes.
                    continue;
                }

                if ((lane.m_Flags & kGroupAggregate) != 0)
                {
                    continue;
                }

                count++;
            }

            return count;
        }
    }
}

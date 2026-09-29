// <copyright file="RoadSizeRule.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Counts driving lanes per road type so citywide road-size bans can target them.

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
    /// Lanes are counted from composition data rather than the prefab name, because
    /// prefab names do not match the player-facing road names, are not localized, and
    /// do not exist at all for roads built by mods such as Road Builder.
    /// </para>
    /// <para>
    /// The count comes from the road type's own base composition, not from the segment
    /// as built. A player who lays a Six-Lane Road and later adds tram tracks still
    /// calls it a six-lane road, but the tram replaces one driving lane per direction
    /// and the built segment would measure four. Measuring the road type keeps the ban
    /// matching the name the player sees and gives one stable answer per road type.
    /// </para>
    /// <para>
    /// Safe to pass by value once created: the caches are native containers whose
    /// entries are shared by every copy, so cached counts survive the copy.
    /// </para>
    /// </remarks>
    internal struct RoadSizeRule : IDisposable
    {
        /// <summary>
        /// Read-only prefab data needed to rebuild the default composition of a road.
        /// </summary>
        internal struct DefaultCompositionLookups
        {
            internal BufferLookup<NetGeometrySection> GeometrySections;
            internal BufferLookup<NetSubSection> SubSections;
            internal BufferLookup<NetSectionPiece> SectionPieces;
            internal BufferLookup<NetPieceLane> PieceLanes;
            internal ComponentLookup<NetLaneData> LaneData;
            internal ComponentLookup<NetPieceData> PieceData;
            internal ComponentLookup<NetVertexMatchData> VertexMatchData;
        }

        internal const int kFourLaneRoad = 4;
        internal const int kSixLaneRoad = 6;

        /// <summary>Returned when a road has no readable composition.</summary>
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

        // Keyed by road prefab. This is what the bans use, and a city has only a few
        // dozen road types, so the map stays tiny however large the city grows.
        //
        // Owned by the caller, not by the rule. A citywide pass spans many frames and
        // builds a fresh rule each one, so a rule-owned map would be thrown away every
        // frame and every default composition would be rebuilt from scratch again.
        // A road type's default lanes never change while a city is loaded.
        private NativeHashMap<Entity, int> m_RoadTypeLaneCounts;

        // Keyed by composition entity, shared by every edge with the same road type and
        // upgrade combination. Feeds the diagnostics report, not the ban decision.
        private NativeHashMap<Entity, int> m_EdgeLaneCounts;

        private ComponentLookup<Game.Net.Composition> m_Compositions;
        private BufferLookup<NetCompositionLane> m_CompositionLanes;
        private ComponentLookup<PrefabRef> m_PrefabRefs;
        private ComponentLookup<ParkingLaneData> m_ParkingLaneData;
        private DefaultCompositionLookups m_DefaultComposition;

        private bool m_BanFourLaneRoads;
        private bool m_BanSixLaneRoads;

        /// <summary>
        /// Gets a value indicating whether either road-size ban is switched on.
        /// </summary>
        internal readonly bool IsActive =>
            m_BanFourLaneRoads || m_BanSixLaneRoads;

        /// <summary>
        /// Creates a rule and its per-pass caches from the current options.
        /// </summary>
        /// <param name="settings">Mod options, or null before options load.</param>
        /// <param name="compositions">Edge composition lookup.</param>
        /// <param name="compositionLanes">Composition lane buffer lookup.</param>
        /// <param name="prefabRefs">Prefab reference lookup.</param>
        /// <param name="parkingLaneData">Parking lane prefab data lookup.</param>
        /// <param name="defaultComposition">Lookups for rebuilding default lanes.</param>
        /// <param name="roadTypeLaneCounts">Caller-owned cache that outlives this rule.</param>
        /// <param name="allocator">Allocator for the rule's own short-lived cache.</param>
        /// <returns>A rule that must be disposed when the frame's work ends.</returns>
        internal static RoadSizeRule Create(
            PCSettings? settings,
            ComponentLookup<Game.Net.Composition> compositions,
            BufferLookup<NetCompositionLane> compositionLanes,
            ComponentLookup<PrefabRef> prefabRefs,
            ComponentLookup<ParkingLaneData> parkingLaneData,
            DefaultCompositionLookups defaultComposition,
            NativeHashMap<Entity, int> roadTypeLaneCounts,
            Allocator allocator)
        {
            RoadSizeRule rule = default;

            rule.m_BanFourLaneRoads = settings?.BanFourLaneRoads ?? false;
            rule.m_BanSixLaneRoads = settings?.BanSixLaneRoads ?? false;
            rule.m_Compositions = compositions;
            rule.m_CompositionLanes = compositionLanes;
            rule.m_PrefabRefs = prefabRefs;
            rule.m_ParkingLaneData = parkingLaneData;
            rule.m_DefaultComposition = defaultComposition;

            rule.m_RoadTypeLaneCounts = roadTypeLaneCounts;

            // Only the as-built cache is short-lived. It feeds the diagnostics report,
            // and edge compositions can change while a city is being edited.
            rule.m_EdgeLaneCounts =
                new NativeHashMap<Entity, int>(kInitialCacheCapacity, allocator);

            return rule;
        }

        /// <summary>
        /// Returns whether a parking lane's road matches an enabled road-size ban.
        /// </summary>
        /// <param name="lane">Parking lane being tested.</param>
        /// <param name="road">Road edge owning the parking lane.</param>
        /// <returns>True when this road type's lane count is banned citywide.</returns>
        internal bool IsRoadSizeTarget(Entity lane, Entity road)
        {
            if (!IsActive || HasBuiltInParkingSpaces(lane))
            {
                return false;
            }

            int lanes = GetRoadTypeDrivingLaneCount(road);

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
        /// Gets the driving-lane count for a road's type, ignoring segment upgrades.
        /// </summary>
        /// <param name="road">Road edge whose type to measure.</param>
        /// <returns>Driving lanes, or <see cref="kUnknownLaneCount"/> when unreadable.</returns>
        internal int GetRoadTypeDrivingLaneCount(Entity road)
        {
            if (road == Entity.Null ||
                !m_PrefabRefs.TryGetComponent(road, out PrefabRef prefabRef))
            {
                return kUnknownLaneCount;
            }

            if (m_RoadTypeLaneCounts.TryGetValue(
                    prefabRef.m_Prefab,
                    out int cached))
            {
                return cached;
            }

            // Deliberately no fallback to the as-built count. If the default cannot be
            // rebuilt the answer stays unknown and the road is left alone, rather than
            // being banned on a number that means something different.
            int counted = CountDefaultDrivingLanes(prefabRef.m_Prefab);

            m_RoadTypeLaneCounts.TryAdd(prefabRef.m_Prefab, counted);

            return counted;
        }

        /// <summary>
        /// Gets the driving-lane count of a road segment exactly as it is built.
        /// </summary>
        /// <remarks>
        /// Diagnostics only. Upgrades such as tram tracks change this, which is why the
        /// bans use <see cref="GetRoadTypeDrivingLaneCount"/> instead.
        /// </remarks>
        /// <param name="road">Road edge to measure.</param>
        /// <returns>Driving lanes, or <see cref="kUnknownLaneCount"/> when unreadable.</returns>
        internal int GetEdgeDrivingLaneCount(Entity road)
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

            if (m_EdgeLaneCounts.TryGetValue(composition.m_Edge, out int cached))
            {
                return cached;
            }

            int counted = CountDrivingLanes(composition.m_Edge);
            m_EdgeLaneCounts.TryAdd(composition.m_Edge, counted);

            return counted;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // m_RoadTypeLaneCounts belongs to the caller and is deliberately left alone.
            if (m_EdgeLaneCounts.IsCreated)
            {
                m_EdgeLaneCounts.Dispose();
            }
        }

        /// <summary>
        /// Counts driving lanes on the default composition of a road type.
        /// </summary>
        /// <remarks>
        /// Rebuilt with the same three calls vanilla uses for net defaults in
        /// Game.Prefabs.NetInitializeSystem.InitializeNetDefaultsJob: build the pieces
        /// for an empty CompositionFlags, calculate the composition data, then turn the
        /// pieces into lanes. The middle call is not optional, because it writes the
        /// piece offsets back into the piece list and those offsets position the lanes.
        /// Reading a
        /// ready-made composition off the prefab instead would be cheaper but unsafe,
        /// because compositions are created on demand per built segment, so a road type
        /// whose every segment carries trams would expose no unupgraded one to read.
        /// </remarks>
        /// <param name="prefab">Road prefab entity.</param>
        /// <returns>Driving lanes, or <see cref="kUnknownLaneCount"/> when unreadable.</returns>
        private int CountDefaultDrivingLanes(Entity prefab)
        {
            if (!m_DefaultComposition.GeometrySections.TryGetBuffer(
                    prefab,
                    out DynamicBuffer<NetGeometrySection> sections))
            {
                return kUnknownLaneCount;
            }

            NativeList<NetCompositionPiece> pieces = new(32, Allocator.Temp);
            NativeList<NetCompositionLane> lanes = new(32, Allocator.Temp);

            try
            {
                NetCompositionHelpers.GetCompositionPieces(
                    pieces,
                    sections.AsNativeArray(),
                    default,
                    m_DefaultComposition.SubSections,
                    m_DefaultComposition.SectionPieces);

                NetCompositionData compositionData = default;

                NetCompositionHelpers.CalculateCompositionData(
                    ref compositionData,
                    pieces.AsArray(),
                    m_DefaultComposition.PieceData,
                    m_DefaultComposition.VertexMatchData);

                NetCompositionHelpers.AddCompositionLanes(
                    Entity.Null,
                    ref compositionData,
                    pieces,
                    lanes,
                    default,
                    m_DefaultComposition.LaneData,
                    m_DefaultComposition.PieceLanes);

                int count = 0;

                foreach (NetCompositionLane lane in lanes)
                {
                    if (IsDrivingLane(lane.m_Flags))
                    {
                        count++;
                    }
                }

                return count;
            }
            finally
            {
                pieces.Dispose();
                lanes.Dispose();
            }
        }

        private static bool IsDrivingLane(LaneFlags flags)
        {
            // Sidewalks, tram tracks, parking lanes and bicycle lanes are not driving
            // lanes, and Master entries are per-group aggregates rather than lanes.
            return (flags & kDrivingLaneMask) == LaneFlags.Road &&
                (flags & kGroupAggregate) == 0;
        }

        private int CountDrivingLanes(Entity composition)
        {
            if (!m_CompositionLanes.TryGetBuffer(
                    composition,
                    out DynamicBuffer<NetCompositionLane> lanes))
            {
                return kUnknownLaneCount;
            }

            int count = 0;

            foreach (NetCompositionLane lane in lanes)
            {
                if (IsDrivingLane(lane.m_Flags))
                {
                    count++;
                }
            }

            return count;
        }
    }
}

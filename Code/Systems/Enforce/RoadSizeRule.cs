// <copyright file="RoadSizeRule.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Counts driving lanes per road type so citywide road-size bans can target them.

using System;
using CS2Shared.RiverMochi;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{

    /// <summary>
    /// Matches roads by driving-lane count for the citywide road-size parking bans.
    /// </summary>
    internal struct RoadSizeRule : IDisposable
    {
        /// <summary>
        /// Lookups needed to rebuild a road prefab's default composition.
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

        /// <summary>Represents an unreadable road composition.</summary>
        internal const int kUnknownLaneCount = -1;

        private const LaneFlags kDrivingLaneMask =
            LaneFlags.Road | LaneFlags.BicyclesOnly;

        private const LaneFlags kGroupAggregate = LaneFlags.Master;

        private const int kInitialCacheCapacity = 64;

        // Caller-owned and keyed by prefab; survives across reconciliation passes.
        private NativeHashMap<Entity, int> m_RoadTypeLaneCounts;

        // Per-rule diagnostics cache keyed by the as-built composition.
        private NativeHashMap<Entity, int> m_EdgeLaneCounts;

        private ComponentLookup<Game.Net.Composition> m_Compositions;
        private BufferLookup<NetCompositionLane> m_CompositionLanes;
        private ComponentLookup<PrefabRef> m_PrefabRefs;
        private ComponentLookup<ParkingLaneData> m_ParkingLaneData;
        private DefaultCompositionLookups m_DefaultComposition;

        private bool m_BanFourLaneRoads;
        private bool m_BanSixLaneRoads;

        /// <summary>Gets whether either road-size ban is enabled.</summary>
        internal readonly bool IsActive =>
            m_BanFourLaneRoads || m_BanSixLaneRoads;

        /// <summary>Creates a rule for one reconciliation pass.</summary>
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

            // Native-container storage remains shared when this struct is passed by value.
            rule.m_RoadTypeLaneCounts = roadTypeLaneCounts;

            // Edge compositions can change while the city is edited; do not persist this.
            rule.m_EdgeLaneCounts =
                new NativeHashMap<Entity, int>(kInitialCacheCapacity, allocator);

            return rule;
        }

        /// <summary>Checks whether a parking lane matches an enabled road-size ban.</summary>
        internal bool IsRoadSizeTarget(Entity lane, Entity road)
        {
            // Size bans preserve roads designed around marked parking bays.
            if (!IsActive || HasBuiltInParkingSpaces(lane))
            {
                return false;
            }

            int lanes = GetRoadTypeDrivingLaneCount(road);

            return (m_BanFourLaneRoads && lanes == kFourLaneRoad) ||
                (m_BanSixLaneRoads && lanes == kSixLaneRoad);
        }

        /// <summary>Checks whether a lane contains built-in marked parking spaces.</summary>
        internal bool HasBuiltInParkingSpaces(Entity lane)
        {
            // Marked bays have fixed spacing; ordinary curb parking is continuous.
            return m_PrefabRefs.TryGetComponent(
                    lane,
                    out PrefabRef prefabRef) &&
                m_ParkingLaneData.TryGetComponent(
                    prefabRef.m_Prefab,
                    out ParkingLaneData parkingLaneData) &&
                parkingLaneData.m_SlotInterval != 0f;
        }

        /// <summary>Gets a road type's default driving-lane count.</summary>
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

            // Never substitute the edge count: upgrades can change it.
        #if DEBUG
            long countStartTimestamp =
                System.Diagnostics.Stopwatch.GetTimestamp();
        #endif

            int counted = CountDefaultDrivingLanes(prefabRef.m_Prefab);

        #if DEBUG
            double countMilliseconds =
                (System.Diagnostics.Stopwatch.GetTimestamp() - countStartTimestamp) *
                1000.0 /
                System.Diagnostics.Stopwatch.Frequency;

            if (countMilliseconds >= 2.0)
            {
                LogUtils.Info(
                    $"{Mod.ModTag} Slow road-type lane count: " +
                    $"prefab={prefabRef.m_Prefab.Index}:{prefabRef.m_Prefab.Version}, " +
                    $"lanes={counted}, time={countMilliseconds:0.###} ms.");
            }
        #endif

            m_RoadTypeLaneCounts.TryAdd(prefabRef.m_Prefab, counted);

            return counted;
        }

        /// <summary>Gets the upgraded, as-built lane count used by diagnostics.</summary>
        internal int GetEdgeDrivingLaneCount(Entity road)
        {
            if (road == Entity.Null ||
                !m_Compositions.TryGetComponent(
                    road,
                    out Game.Net.Composition composition) ||
                composition.m_Edge == Entity.Null)
            {
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
            // The caller owns m_RoadTypeLaneCounts.
            if (m_EdgeLaneCounts.IsCreated)
            {
                m_EdgeLaneCounts.Dispose();
            }
        }

        /// <summary>Counts driving lanes in a road prefab's default composition.</summary>
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
                // Keep this sequence aligned with vanilla net-default initialization.
                NetCompositionHelpers.GetCompositionPieces(
                    pieces,
                    sections.AsNativeArray(),
                    default,
                    m_DefaultComposition.SubSections,
                    m_DefaultComposition.SectionPieces);

                NetCompositionData compositionData = default;

                // Required: this writes the piece offsets used to group lanes.
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
            // Mirrors vanilla's Road test; Master entries are group totals, not lanes.
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

// <copyright file="ParkingStatusSystem.RoadSizeReport.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Lists every road prefab that carries street parking, with its driving-lane count.

using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{

    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Writes one line per road prefab that owns street parking, with lane counts.
        /// </summary>
        /// <remarks>
        /// Prefab names never reach players, so this is the only way to confirm which
        /// real roads the four- and six-lane bans cover. It runs only from the
        /// About &gt; Debug &gt; Write Report button, never during normal play.
        /// </remarks>
        /// <param name="text">Report builder.</param>
        private void AppendRoadSizeInventory(StringBuilder text)
        {
            text.AppendLine();
            text.AppendLine(
                "-------------------- ROAD SIZE INVENTORY --------------------");
            text.AppendLine(
                "Lanes = the road type's own driving-lane count, which is what the bans " +
                "use and what the player-facing road names show. AsBuilt = the live count " +
                "for this segment, which upgrades such as tram tracks and bus stops change.");
            text.AppendLine(
                "Roads with built-in marked parking bays are listed but never banned " +
                "by road size, because they exist to provide parking.");

            ComponentLookup<Game.Net.ParkingLane> parkingLaneLookup =
                GetComponentLookup<Game.Net.ParkingLane>(true);
            ComponentLookup<Game.Common.Owner> ownerLookup =
                GetComponentLookup<Game.Common.Owner>(true);
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup =
                GetComponentLookup<Game.Prefabs.PrefabRef>(true);
            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup =
                GetComponentLookup<Game.Prefabs.ParkingLaneData>(true);
            ComponentLookup<Game.Net.Road> roadLookup =
                GetComponentLookup<Game.Net.Road>(true);

            using RoadSizeRule roadSizeRule = CreateRoadSizeRule();

            Dictionary<Entity, RoadPrefabParkingStats> byPrefab = new(64);

            // Native container for the Entity set: managed collections keyed by Entity
            // allocate on the GC heap and cannot be used from jobs at all.
            using NativeHashSet<Entity> countedRoads =
                new(Math.Max(1, m_CurbLaneQuery.CalculateEntityCount()), Allocator.Temp);

            using (NativeArray<Entity> lanes =
                m_CurbLaneQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity lane in lanes)
                {
                    Game.Net.ParkingLane parkingLane = parkingLaneLookup[lane];

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

                    Entity road = ownerLookup[lane].m_Owner;

                    if (!prefabRefLookup.TryGetComponent(
                            road,
                            out Game.Prefabs.PrefabRef roadPrefabRef))
                    {
                        continue;
                    }

                    if (!byPrefab.TryGetValue(
                            roadPrefabRef.m_Prefab,
                            out RoadPrefabParkingStats stats))
                    {
                        stats = new RoadPrefabParkingStats();
                        byPrefab[roadPrefabRef.m_Prefab] = stats;
                    }

                    // Cached, so this stays cheap across many roads.
                    stats.RoadTypeLanes =
                        roadSizeRule.GetRoadTypeDrivingLaneCount(road);

                    stats.AddEdgeLaneCount(
                        roadSizeRule.GetEdgeDrivingLaneCount(road),
                        road);

                    stats.HasBuiltInParkingSpaces |=
                        roadSizeRule.HasBuiltInParkingSpaces(lane);

                    if (countedRoads.Add(road))
                    {
                        stats.Roads++;
                    }

                    stats.CurbLanes++;

                    if ((parkingLane.m_Flags &
                            Game.Net.ParkingLaneFlags.ParkingDisabled) != 0)
                    {
                        stats.DisabledCurbLanes++;
                    }
                }
            }

            if (byPrefab.Count == 0)
            {
                text.AppendLine("  <no roads with street parking>");
                return;
            }

            List<KeyValuePair<Entity, RoadPrefabParkingStats>> ordered =
                new(byPrefab);

            ordered.Sort(static (left, right) =>
            {
                int byLanes = right.Value.RoadTypeLanes.CompareTo(
                    left.Value.RoadTypeLanes);

                return byLanes != 0
                    ? byLanes
                    : right.Value.CurbLanes.CompareTo(left.Value.CurbLanes);
            });

            foreach (KeyValuePair<Entity, RoadPrefabParkingStats> item in ordered)
            {
                RoadPrefabParkingStats stats = item.Value;

                // AsBuilt differing from Lanes means upgrades such as tram tracks
                // changed that segment. The ban follows Lanes, the road type.
                string asBuiltText = FormatLaneCounts(stats.EdgeLanes);

                string banText;

                if (stats.HasBuiltInParkingSpaces)
                {
                    banText = " (built-in parking bays, never banned by road size)";
                }
                else if (stats.RoadTypeLanes == RoadSizeRule.kFourLaneRoad)
                {
                    banText = " <- four-lane ban";
                }
                else if (stats.RoadTypeLanes == RoadSizeRule.kSixLaneRoad)
                {
                    banText = " <- six-lane ban";
                }
                else
                {
                    banText = string.Empty;
                }

                text.AppendLine(
                    $"  Lanes={FormatLaneCount(stats.RoadTypeLanes),2} | " +
                    $"AsBuilt={asBuiltText,-8} | " +
                    $"Roads={stats.Roads,5} | " +
                    $"CurbLanes={stats.CurbLanes,5} | " +
                    $"Disabled={stats.DisabledCurbLanes,5} | " +
                    $"Prefab={GetRoadPrefabName(item.Key)}{banText}");
            }

#if DEBUG
            AppendCompositionLaneDump(text, ordered);
#endif
        }

#if DEBUG
        /// <summary>
        /// Dumps raw composition lane flags so lane-count mismatches can be diagnosed.
        /// </summary>
        /// <remarks>
        /// One sample per distinct as-built lane count, so an upgraded variant such as
        /// a tram-equipped segment is shown next to the plain one. Research only.
        /// </remarks>
        /// <param name="text">Report builder.</param>
        /// <param name="ordered">Road prefabs found in the city.</param>
        private void AppendCompositionLaneDump(
            StringBuilder text,
            List<KeyValuePair<Entity, RoadPrefabParkingStats>> ordered)
        {
            ComponentLookup<Game.Net.Composition> compositions =
                GetComponentLookup<Game.Net.Composition>(true);

            ComponentLookup<Game.Prefabs.NetCompositionData> compositionData =
                GetComponentLookup<Game.Prefabs.NetCompositionData>(true);

            BufferLookup<Game.Prefabs.NetCompositionLane> compositionLanes =
                GetBufferLookup<Game.Prefabs.NetCompositionLane>(true);

            ComponentLookup<Game.Prefabs.UtilityLaneData> utilityLaneData =
                GetComponentLookup<Game.Prefabs.UtilityLaneData>(true);

            text.AppendLine();
            text.AppendLine(
                "-------------------- COMPOSITION LANE DUMP (DEBUG) --------------------");

            foreach (KeyValuePair<Entity, RoadPrefabParkingStats> item in ordered)
            {
                foreach (KeyValuePair<int, Entity> sample in
                    item.Value.SamplesByEdgeLanes)
                {
                    Entity road = sample.Value;

                    if (!compositions.TryGetComponent(
                            road,
                            out Game.Net.Composition composition) ||
                        !compositionLanes.TryGetBuffer(
                            composition.m_Edge,
                            out DynamicBuffer<Game.Prefabs.NetCompositionLane> lanes))
                    {
                        continue;
                    }

                    string mask = compositionData.TryGetComponent(
                            composition.m_Edge,
                            out Game.Prefabs.NetCompositionData data)
                        ? $"general={data.m_Flags.m_General} | " +
                            $"left={data.m_Flags.m_Left} | " +
                            $"right={data.m_Flags.m_Right}"
                        : "<no composition data>";

                    text.AppendLine(
                        $"  {GetRoadPrefabName(item.Key)} " +
                        $"[roadType={FormatLaneCount(item.Value.RoadTypeLanes)} " +
                        $"asBuilt={FormatLaneCount(sample.Key)}] " +
                        $"sample road {FormatEntity(road)}, " +
                        $"edge composition {FormatEntity(composition.m_Edge)}, " +
                        $"{lanes.Length} lane entries");

                    text.AppendLine($"      mask: {mask}");

                    foreach (Game.Prefabs.NetCompositionLane lane in lanes)
                    {
                        // UtilityLaneData is how the game itself identifies water,
                        // sewage and power lanes, independent of any prefab name.
                        string utility =
                            utilityLaneData.TryGetComponent(
                                lane.m_Lane,
                                out Game.Prefabs.UtilityLaneData laneUtility)
                                ? $" utility={laneUtility.m_UtilityTypes}"
                                : string.Empty;

                        text.AppendLine(
                            $"      idx={lane.m_Index,3} " +
                            $"cw={lane.m_Carriageway,3} " +
                            $"grp={lane.m_Group,3} " +
                            $"x={lane.m_Position.x,7:0.00} " +
                            $"flags={lane.m_Flags}{utility}");
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Builds the citywide road-size rule for one pass.
        /// </summary>
        /// <remarks>
        /// Kept in its own method so the SystemAPI source generator never has to
        /// relocate a method that carries nullable annotations, which would emit
        /// CS8669 from generated code that has no #nullable directive.
        /// </remarks>
        /// <returns>A rule that must be disposed when the pass ends.</returns>
        private RoadSizeRule CreateRoadSizeRule()
        {
            Unity.Entities.BufferLookup<Game.Prefabs.NetGeometrySection> geometrySections =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetGeometrySection>(true);

            Unity.Entities.BufferLookup<Game.Prefabs.NetSubSection> subSections =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetSubSection>(true);

            Unity.Entities.BufferLookup<Game.Prefabs.NetSectionPiece> sectionPieces =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetSectionPiece>(true);

            Unity.Entities.BufferLookup<Game.Prefabs.NetPieceLane> pieceLanes =
                SystemAPI.GetBufferLookup<Game.Prefabs.NetPieceLane>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.NetLaneData> laneData =
                SystemAPI.GetComponentLookup<Game.Prefabs.NetLaneData>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.NetPieceData> pieceData =
                SystemAPI.GetComponentLookup<Game.Prefabs.NetPieceData>(true);

            Unity.Entities.ComponentLookup<Game.Prefabs.NetVertexMatchData> vertexMatchData =
                SystemAPI.GetComponentLookup<Game.Prefabs.NetVertexMatchData>(true);

            // The report runs once per button press and finishes inside one frame, so a
            // throwaway Temp cache is right here. Allocator.Temp frees itself at the end
            // of the frame, which is why this one is not disposed by hand.
            Unity.Collections.NativeHashMap<Entity, int> roadTypeLaneCounts =
                new(64, Unity.Collections.Allocator.Temp);

            RoadSizeRule.DefaultCompositionLookups defaultComposition = new()
            {
                GeometrySections = geometrySections,
                SubSections = subSections,
                SectionPieces = sectionPieces,
                PieceLanes = pieceLanes,
                LaneData = laneData,
                PieceData = pieceData,
                VertexMatchData = vertexMatchData,
            };

            return RoadSizeRule.Create(
                Mod.Settings,
                SystemAPI.GetComponentLookup<Game.Net.Composition>(true),
                SystemAPI.GetBufferLookup<Game.Prefabs.NetCompositionLane>(true),
                SystemAPI.GetComponentLookup<Game.Prefabs.PrefabRef>(true),
                SystemAPI.GetComponentLookup<Game.Prefabs.ParkingLaneData>(true),
                defaultComposition,
                roadTypeLaneCounts,
                Unity.Collections.Allocator.Temp);
        }

        /// <summary>
        /// Lists every road prefab in the city and whether its lanes support parking.
        /// </summary>
        /// <remarks>
        /// The inventory above is built from parking lanes, so roads that allow no
        /// street parking never appear there. This walks road edges instead, which
        /// is the only way to see that a road type has no parking to begin with.
        /// </remarks>
        /// <param name="text">Report builder.</param>
        private void AppendRoadParkingSupport(StringBuilder text)
        {
            ComponentLookup<Game.Prefabs.PrefabRef> prefabRefLookup =
                GetComponentLookup<Game.Prefabs.PrefabRef>(true);

            ComponentLookup<Game.Net.Composition> compositions =
                GetComponentLookup<Game.Net.Composition>(true);

            ComponentLookup<Game.Prefabs.ParkingLaneData> parkingLaneDataLookup =
                GetComponentLookup<Game.Prefabs.ParkingLaneData>(true);

            ComponentLookup<Game.Prefabs.UtilityLaneData> utilityLaneDataLookup =
                GetComponentLookup<Game.Prefabs.UtilityLaneData>(true);

            BufferLookup<Game.Prefabs.NetCompositionLane> compositionLanes =
                GetBufferLookup<Game.Prefabs.NetCompositionLane>(true);

            using RoadSizeRule roadSizeRule = CreateRoadSizeRule();

            text.AppendLine();
            text.AppendLine(
                "-------------------- ROAD PARKING SUPPORT --------------------");
            text.AppendLine(
                "Every road type in the city, including ones that allow no street " +
                "parking. ParkingLanes counts Parking entries in the edge composition, " +
                "and Utils lists the utility types present in the as-built composition.");

            Dictionary<Entity, RoadParkingSupport> byPrefab = new(64);

            using (NativeArray<Entity> roads =
                m_RoadEdgeQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity road in roads)
                {
                    if (!prefabRefLookup.TryGetComponent(
                            road,
                            out Game.Prefabs.PrefabRef prefabRef) ||
                        !compositions.TryGetComponent(
                            road,
                            out Game.Net.Composition composition) ||
                        !compositionLanes.TryGetBuffer(
                            composition.m_Edge,
                            out DynamicBuffer<Game.Prefabs.NetCompositionLane> lanes))
                    {
                        continue;
                    }

                    if (!byPrefab.TryGetValue(
                            prefabRef.m_Prefab,
                            out RoadParkingSupport support))
                    {
                        support = new RoadParkingSupport();
                        byPrefab[prefabRef.m_Prefab] = support;
                    }

                    support.Edges++;
                    support.RoadTypeLanes =
                        roadSizeRule.GetRoadTypeDrivingLaneCount(road);

                    support.AddEdgeLaneCount(
                        roadSizeRule.GetEdgeDrivingLaneCount(road));

                    int parkingLanes = 0;
                    bool markedBays = false;

                    foreach (Game.Prefabs.NetCompositionLane lane in lanes)
                    {
                        // UtilityTypes is how the game identifies water, sewage and
                        // power lanes, with no dependence on any prefab name.
                        if (utilityLaneDataLookup.TryGetComponent(
                                lane.m_Lane,
                                out Game.Prefabs.UtilityLaneData utilityLaneData))
                        {
                            support.Utilities |= utilityLaneData.m_UtilityTypes;
                        }

                        if ((lane.m_Flags & Game.Prefabs.LaneFlags.Parking) == 0 ||
                            (lane.m_Flags & Game.Prefabs.LaneFlags.Virtual) != 0)
                        {
                            continue;
                        }

                        parkingLanes++;

                        markedBays |=
                            parkingLaneDataLookup.TryGetComponent(
                                lane.m_Lane,
                                out Game.Prefabs.ParkingLaneData parkingLaneData) &&
                            parkingLaneData.m_SlotInterval != 0f;
                    }

                    support.MaxParkingLanes =
                        Math.Max(support.MaxParkingLanes, parkingLanes);

                    support.HasMarkedBays |= markedBays;
                }
            }

            if (byPrefab.Count == 0)
            {
                text.AppendLine("  <no roads>");
                return;
            }

            List<KeyValuePair<Entity, RoadParkingSupport>> ordered = new(byPrefab);

            ordered.Sort(static (left, right) =>
                right.Value.Edges.CompareTo(left.Value.Edges));

            foreach (KeyValuePair<Entity, RoadParkingSupport> item in ordered)
            {
                RoadParkingSupport support = item.Value;

                text.AppendLine(
                    $"  Lanes={FormatLaneCount(support.RoadTypeLanes),2} | " +
                    $"AsBuilt={FormatLaneCounts(support.EdgeLanes),-8} | " +
                    $"Edges={support.Edges,5} | " +
                    $"ParkingLanes={support.MaxParkingLanes,2} | " +
                    $"MarkedBays={(support.HasMarkedBays ? "YES" : "no "),3} | " +
                    $"Utils={support.Utilities,-40} | " +
                    $"Prefab={GetRoadPrefabName(item.Key)}");
            }
        }

        private sealed class RoadParkingSupport
        {
            internal int RoadTypeLanes { get; set; } = RoadSizeRule.kUnknownLaneCount;

            internal List<int> EdgeLanes { get; } = new(1);

            internal int Edges { get; set; }

            internal int MaxParkingLanes { get; set; }

            internal bool HasMarkedBays { get; set; }

            internal Game.Net.UtilityTypes Utilities { get; set; }

            internal void AddEdgeLaneCount(int lanes)
            {
                int index = EdgeLanes.BinarySearch(lanes);

                if (index < 0)
                {
                    EdgeLanes.Insert(~index, lanes);
                }
            }
        }

        private static string FormatLaneCount(int lanes)
        {
            return lanes == RoadSizeRule.kUnknownLaneCount
                ? "?"
                : lanes.ToString();
        }

        private static string FormatLaneCounts(List<int> lanes)
        {
            return lanes.Count == 0
                ? "?"
                : string.Join("/", lanes.ConvertAll(FormatLaneCount));
        }

        private string GetRoadPrefabName(Entity prefab)
        {
            try
            {
                return m_PrefabSystem.GetPrefabName(prefab);
            }
            catch (Exception ex)
            {
                return $"<unnamed {prefab.Index}:{prefab.Version} " +
                    $"{ex.GetType().Name}>";
            }
        }

        private sealed class RoadPrefabParkingStats
        {
            /// <summary>Gets the road type's own lane count, which drives the bans.</summary>
            internal int RoadTypeLanes { get; set; } = RoadSizeRule.kUnknownLaneCount;

            /// <summary>Gets the distinct as-built counts seen, ascending.</summary>
            internal List<int> EdgeLanes { get; } = new(1);

            internal int Roads { get; set; }

            internal int CurbLanes { get; set; }

            internal int DisabledCurbLanes { get; set; }

            internal bool HasBuiltInParkingSpaces { get; set; }

            /// <summary>Gets one sample road per distinct as-built lane count.</summary>
            internal Dictionary<int, Entity> SamplesByEdgeLanes { get; } = new(1);

            /// <summary>
            /// Records an as-built count once, keeping a sample road for each.
            /// </summary>
            /// <param name="lanes">Driving lanes measured on one road segment.</param>
            /// <param name="road">The segment that produced that count.</param>
            internal void AddEdgeLaneCount(int lanes, Entity road)
            {
                if (SamplesByEdgeLanes.ContainsKey(lanes))
                {
                    return;
                }

                SamplesByEdgeLanes[lanes] = road;

                int index = EdgeLanes.BinarySearch(lanes);

                if (index < 0)
                {
                    EdgeLanes.Insert(~index, lanes);
                }
            }
        }
    }
}

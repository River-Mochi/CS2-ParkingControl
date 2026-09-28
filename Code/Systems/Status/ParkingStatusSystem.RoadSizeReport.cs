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
                "Lanes = driving lanes counted from the road's edge composition, " +
                "matching the numbers in the player-facing road names.");
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

                    // Cached per composition, so this stays cheap across many roads.
                    stats.AddDrivingLaneCount(
                        roadSizeRule.GetDrivingLaneCount(road));

                    stats.HasBuiltInParkingSpaces |=
                        roadSizeRule.HasBuiltInParkingSpaces(lane);

                    if (stats.SampleRoad == Entity.Null)
                    {
                        stats.SampleRoad = road;
                    }

                    stats.Roads.Add(road);
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
                int byLanes = right.Value.MaxDrivingLanes.CompareTo(
                    left.Value.MaxDrivingLanes);

                return byLanes != 0
                    ? byLanes
                    : right.Value.CurbLanes.CompareTo(left.Value.CurbLanes);
            });

            foreach (KeyValuePair<Entity, RoadPrefabParkingStats> item in ordered)
            {
                RoadPrefabParkingStats stats = item.Value;

                // Normally one value. More than one means some segments of this
                // prefab really do carry a different number of driving lanes.
                string lanesText = string.Join(
                    "/",
                    stats.DrivingLanes.ConvertAll(static lanes =>
                        lanes == RoadSizeRule.kUnknownLaneCount
                            ? "?"
                            : lanes.ToString()));

                string banText;

                if (stats.HasBuiltInParkingSpaces)
                {
                    banText = " (built-in parking bays, never banned by road size)";
                }
                else if (stats.DrivingLanes.Contains(RoadSizeRule.kFourLaneRoad))
                {
                    banText = " <- four-lane ban";
                }
                else if (stats.DrivingLanes.Contains(RoadSizeRule.kSixLaneRoad))
                {
                    banText = " <- six-lane ban";
                }
                else
                {
                    banText = string.Empty;
                }

                text.AppendLine(
                    $"  Lanes={lanesText,2} | " +
                    $"Roads={stats.Roads.Count,5} | " +
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
        /// Research only. Remove once the divided-road and turn-lane counts are settled.
        /// </remarks>
        /// <param name="text">Report builder.</param>
        /// <param name="ordered">Road prefabs found in the city.</param>
        private void AppendCompositionLaneDump(
            StringBuilder text,
            List<KeyValuePair<Entity, RoadPrefabParkingStats>> ordered)
        {
            ComponentLookup<Game.Net.Composition> compositions =
                GetComponentLookup<Game.Net.Composition>(true);

            BufferLookup<Game.Prefabs.NetCompositionLane> compositionLanes =
                GetBufferLookup<Game.Prefabs.NetCompositionLane>(true);

            text.AppendLine();
            text.AppendLine(
                "-------------------- COMPOSITION LANE DUMP (DEBUG) --------------------");

            foreach (KeyValuePair<Entity, RoadPrefabParkingStats> item in ordered)
            {
                Entity road = item.Value.SampleRoad;

                if (!compositions.TryGetComponent(
                        road,
                        out Game.Net.Composition composition) ||
                    !compositionLanes.TryGetBuffer(
                        composition.m_Edge,
                        out DynamicBuffer<Game.Prefabs.NetCompositionLane> lanes))
                {
                    continue;
                }

                text.AppendLine(
                    $"  {GetRoadPrefabName(item.Key)} " +
                    $"(sample road {FormatEntity(road)}, " +
                    $"edge composition {FormatEntity(composition.m_Edge)}, " +
                    $"{lanes.Length} lane entries)");

                foreach (Game.Prefabs.NetCompositionLane lane in lanes)
                {
                    text.AppendLine(
                        $"      idx={lane.m_Index,3} " +
                        $"cw={lane.m_Carriageway,3} " +
                        $"grp={lane.m_Group,3} " +
                        $"x={lane.m_Position.x,7:0.00} " +
                        $"flags={lane.m_Flags}");
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
            return RoadSizeRule.Create(
                Mod.Settings,
                SystemAPI.GetComponentLookup<Game.Net.Composition>(true),
                SystemAPI.GetBufferLookup<Game.Prefabs.NetCompositionLane>(true),
                SystemAPI.GetComponentLookup<Game.Prefabs.PrefabRef>(true),
                SystemAPI.GetComponentLookup<Game.Prefabs.ParkingLaneData>(true),
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

            BufferLookup<Game.Prefabs.NetCompositionLane> compositionLanes =
                GetBufferLookup<Game.Prefabs.NetCompositionLane>(true);

            using RoadSizeRule roadSizeRule = CreateRoadSizeRule();

            text.AppendLine();
            text.AppendLine(
                "-------------------- ROAD PARKING SUPPORT --------------------");
            text.AppendLine(
                "Every road type in the city, including ones that allow no street " +
                "parking. ParkingLanes counts Parking entries in the edge composition.");

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
                    support.AddDrivingLaneCount(
                        roadSizeRule.GetDrivingLaneCount(road));

                    int parkingLanes = 0;
                    bool markedBays = false;

                    foreach (Game.Prefabs.NetCompositionLane lane in lanes)
                    {
                        if ((lane.m_Flags & Game.Prefabs.LaneFlags.Parking) == 0)
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

                string lanesText = string.Join(
                    "/",
                    support.DrivingLanes.ConvertAll(static lanes =>
                        lanes == RoadSizeRule.kUnknownLaneCount
                            ? "?"
                            : lanes.ToString()));

                text.AppendLine(
                    $"  Lanes={lanesText,4} | " +
                    $"Edges={support.Edges,5} | " +
                    $"ParkingLanes={support.MaxParkingLanes,2} | " +
                    $"MarkedBays={(support.HasMarkedBays ? "YES" : "no "),3} | " +
                    $"Prefab={GetRoadPrefabName(item.Key)}");
            }
        }

        private sealed class RoadParkingSupport
        {
            internal List<int> DrivingLanes { get; } = new(1);

            internal int Edges { get; set; }

            internal int MaxParkingLanes { get; set; }

            internal bool HasMarkedBays { get; set; }

            internal void AddDrivingLaneCount(int lanes)
            {
                int index = DrivingLanes.BinarySearch(lanes);

                if (index < 0)
                {
                    DrivingLanes.Insert(~index, lanes);
                }
            }
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
            /// <summary>Gets the distinct lane counts seen, in ascending order.</summary>
            internal List<int> DrivingLanes { get; } = new(1);

            internal HashSet<Entity> Roads { get; } = new();

            internal int CurbLanes { get; set; }

            internal int DisabledCurbLanes { get; set; }

            internal bool HasBuiltInParkingSpaces { get; set; }

            internal Entity SampleRoad { get; set; }

            internal int MaxDrivingLanes =>
                DrivingLanes.Count == 0
                    ? RoadSizeRule.kUnknownLaneCount
                    : DrivingLanes[DrivingLanes.Count - 1];

            /// <summary>
            /// Records a lane count once. A prefab normally yields a single value.
            /// </summary>
            /// <param name="lanes">Driving lanes measured on one road segment.</param>
            internal void AddDrivingLaneCount(int lanes)
            {
                int index = DrivingLanes.BinarySearch(lanes);

                if (index < 0)
                {
                    DrivingLanes.Insert(~index, lanes);
                }
            }
        }
    }
}

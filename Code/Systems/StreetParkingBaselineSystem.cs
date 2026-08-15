// <copyright file="StreetParkingBaselineSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Recalculates unowned disabled curb flags left by the faulty 0.1.0 test build.

namespace ParkingControl
{
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Prefabs;
    using Unity.Collections;
    using Unity.Entities;
    using ParkingLane = Game.Net.ParkingLane;
    using ParkingLaneFlags = Game.Net.ParkingLaneFlags;
    using Road = Game.Net.Road;
    using Temp = Game.Tools.Temp;

    /// <summary>
    /// Gives vanilla one pre-enforcement pass over ambiguous disabled curb lanes.
    /// </summary>
    public sealed partial class StreetParkingBaselineSystem : GameSystemBase
    {
        private static bool s_ScanRequested = true;

        private EntityQuery m_ParkingLaneQuery;

        /// <summary>
        /// Requests a one-shot scan before vanilla refreshes parking-lane data.
        /// </summary>
        public static void RequestScan()
        {
            s_ScanRequested = true;
            World? world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return;
            }

            StreetParkingBaselineSystem? system =
                world.GetExistingSystemManaged<StreetParkingBaselineSystem>();
            if (system != null)
            {
                system.Enabled = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_ParkingLaneQuery = SystemAPI.QueryBuilder()
                .WithAll<ParkingLane, Owner, PrefabRef>()
                .WithNone<Deleted, Temp, StreetParkingState>()
                .Build();
            Enabled = true;
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!s_ScanRequested)
            {
                Enabled = false;
                return;
            }

            s_ScanRequested = false;
            if ((Mod.Settings?.Scope ?? PCSettings.ParkingScope.Off) ==
                PCSettings.ParkingScope.Off)
            {
                Enabled = false;
                return;
            }

            Dependency.Complete();
            ComponentLookup<ParkingLane> parkingLaneLookup = SystemAPI.GetComponentLookup<ParkingLane>(true);
            ComponentLookup<Owner> ownerLookup = SystemAPI.GetComponentLookup<Owner>(true);
            ComponentLookup<PrefabRef> prefabRefLookup = SystemAPI.GetComponentLookup<PrefabRef>(true);
            ComponentLookup<ParkingLaneData> parkingLaneDataLookup = SystemAPI.GetComponentLookup<ParkingLaneData>(true);
            ComponentLookup<Road> roadLookup = SystemAPI.GetComponentLookup<Road>(true);
            ComponentLookup<Created> createdLookup = SystemAPI.GetComponentLookup<Created>(true);
            ComponentLookup<Updated> updatedLookup = SystemAPI.GetComponentLookup<Updated>(true);
            ComponentLookup<PathfindUpdated> pathfindUpdatedLookup = SystemAPI.GetComponentLookup<PathfindUpdated>(true);
            NativeList<Entity> refreshEntities = new(Allocator.Temp);
            int pendingRefresh = 0;

            using (NativeArray<Entity> lanes = m_ParkingLaneQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity lane in lanes)
                {
                    ParkingLane parkingLane = parkingLaneLookup[lane];
                    if ((parkingLane.m_Flags & ParkingLaneFlags.ParkingDisabled) == 0 ||
                        !NoStreetParkingSystem.IsStreetCarParkingLane(
                            lane,
                            parkingLane,
                            ownerLookup,
                            prefabRefLookup,
                            parkingLaneDataLookup,
                            roadLookup))
                    {
                        continue;
                    }

                    pendingRefresh++;
                    if (!createdLookup.HasComponent(lane) &&
                        !updatedLookup.HasComponent(lane) &&
                        !pathfindUpdatedLookup.HasComponent(lane))
                    {
                        refreshEntities.Add(lane);
                    }
                }
            }

            if (refreshEntities.Length > 0)
            {
                EntityManager.AddComponent<PathfindUpdated>(refreshEntities.AsArray());
            }

            if (pendingRefresh > 0)
            {
                LogUtils.Info(
                    $"{Mod.ModTag} Requested vanilla baseline refresh for {pendingRefresh} unowned disabled curb lanes.");
            }

            refreshEntities.Dispose();
            Enabled = false;
        }
    }
}

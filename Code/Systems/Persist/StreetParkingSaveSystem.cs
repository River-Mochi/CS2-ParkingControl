// <copyright file="StreetParkingSaveSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Keeps mod-owned vanilla parking flags out of save data without changing the live path graph.

using Game;
using Game.Common;
using Game.Net;
using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{

    /// <summary>
    /// Clears only the mod-owned vanilla lane flag immediately before serialization.
    /// </summary>
    public sealed partial class StreetParkingSaveSystem : GameSystemBase
    {
        private EntityQuery m_ModifiedParkingLanesQuery;

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_ModifiedParkingLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<ParkingLane, StreetParkingState>()
                .WithNone<Deleted>()
                .Build();
            RequireForUpdate(m_ModifiedParkingLanesQuery);
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            Dependency.Complete();
            ComponentLookup<ParkingLane> parkingLaneLookup = SystemAPI.GetComponentLookup<ParkingLane>();

            using (NativeArray<Entity> entities = m_ModifiedParkingLanesQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity entity in entities)
                {
                    ParkingLane parkingLane = parkingLaneLookup[entity];
                    parkingLane.m_Flags &= ~ParkingLaneFlags.ParkingDisabled;
                    parkingLaneLookup[entity] = parkingLane;
                }
            }

            // StreetParkingState is deliberately non-serializable and remains available to the
            // post-serializer system in this live world. Request recovery in case that system fails.
            NoStreetParkingSystem.RequestSaveRecovery();
        }
    }
}

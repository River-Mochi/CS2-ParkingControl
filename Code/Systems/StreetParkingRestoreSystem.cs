// <copyright file="StreetParkingRestoreSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Restores live no-parking flags immediately after save serialization finishes.

namespace ParkingControl
{
    using Game;
    using Game.Common;
    using Unity.Collections;
    using Unity.Entities;
    using ParkingLane = Game.Net.ParkingLane;
    using ParkingLaneFlags = Game.Net.ParkingLaneFlags;

    /// <summary>
    /// Restores the live vanilla lane flags from the runtime-only ownership markers.
    /// </summary>
    public sealed partial class StreetParkingRestoreSystem : GameSystemBase
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
                    parkingLane.m_Flags |= ParkingLaneFlags.ParkingDisabled;
                    parkingLaneLookup[entity] = parkingLane;
                }
            }

            NoStreetParkingSystem.CompleteSaveRestore();
        }
    }
}

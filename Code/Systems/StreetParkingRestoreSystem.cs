// <copyright file="StreetParkingRestoreSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
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

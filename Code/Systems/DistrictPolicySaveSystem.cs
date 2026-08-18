// <copyright file="DistrictPolicySaveSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Saves district selections as custom markers without retaining the runtime policy prefab.

namespace ParkingControl
{
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Unity.Collections;
    using Unity.Entities;
    using Temp = Game.Tools.Temp;

    /// <summary>
    /// Migrates native policy selections to serializable district markers before prefab collection.
    /// </summary>
    public sealed partial class DistrictPolicySaveSystem : GameSystemBase
    {
        private EntityQuery m_DistrictQuery;
        private DistrictPolicyRestoreSystem m_RestoreSystem = null!;

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_DistrictQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Areas.District, Game.Policies.Policy>()
                .WithNone<Deleted, Temp>()
                .Build();
            m_RestoreSystem = World.GetOrCreateSystemManaged<DistrictPolicyRestoreSystem>();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            Entity policyEntity = ParkingPolicySystem.PolicyEntity;
            if (policyEntity == Entity.Null || m_DistrictQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            Dependency.Complete();

            ComponentLookup<NoStreetParkingDistrict> markerLookup =
                SystemAPI.GetComponentLookup<NoStreetParkingDistrict>(true);
            NativeList<Entity> addMarkers = new(Allocator.Temp);
            NativeList<Entity> removeMarkers = new(Allocator.Temp);

            using (NativeArray<Entity> districts =
                m_DistrictQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity district in districts)
                {
                    DynamicBuffer<Game.Policies.Policy> policies =
                        EntityManager.GetBuffer<Game.Policies.Policy>(district);
                    bool active = false;

                    // Remove every runtime reference so prefab collection cannot retain it.
                    for (int i = policies.Length - 1; i >= 0; i--)
                    {
                        Game.Policies.Policy policy = policies[i];
                        if (policy.m_Policy != policyEntity)
                        {
                            continue;
                        }

                        active |=
                            (policy.m_Flags & Game.Policies.PolicyFlags.Active) != 0;
                        policies.RemoveAt(i);
                    }

                    bool hasMarker = markerLookup.HasComponent(district);
                    if (active && !hasMarker)
                    {
                        addMarkers.Add(district);
                    }
                    else if (!active && hasMarker)
                    {
                        removeMarkers.Add(district);
                    }
                }
            }

            if (addMarkers.Length > 0)
            {
                EntityManager.AddComponent<NoStreetParkingDistrict>(addMarkers.AsArray());
                LogUtils.Info(
                    $"{Mod.ModTag} Migrated {addMarkers.Length} district parking " +
                    "selection(s) to uninstall-safe save markers.");
            }

            if (removeMarkers.Length > 0)
            {
                EntityManager.RemoveComponent<NoStreetParkingDistrict>(removeMarkers.AsArray());
            }

            addMarkers.Dispose();
            removeMarkers.Dispose();

            // The live checkbox entries return only after entity serialization is complete.
            m_RestoreSystem.RequestRestore();
        }
    }
}

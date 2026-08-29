// <copyright file="DistrictPolicyRestoreSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Recreates native district-policy checkboxes from uninstall-safe saved markers.

using Game;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace ParkingControl
{

    /// <summary>
    /// Restores runtime policy-buffer entries after loading and after save serialization.
    /// </summary>
    public sealed partial class DistrictPolicyRestoreSystem : GameSystemBase
    {
        private EntityQuery m_MarkedDistrictQuery;
        private bool m_RestoreRequested;

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_MarkedDistrictQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    Game.Areas.District,
                    Game.Policies.Policy,
                    NoStreetParkingDistrict>()
                .WithNone<Deleted, Temp>()
                .Build();
        }

        /// <summary>
        /// Requests restoration immediately after the current save finishes serializing.
        /// </summary>
        internal void RequestRestore()
        {
            m_RestoreRequested = true;
        }

        /// <summary>
        /// Recreates active native policy entries from the saved district markers.
        /// </summary>
        /// <returns>The number of missing or inactive entries restored.</returns>
        internal int RestoreNow()
        {
            Entity policyEntity = ParkingPolicySystem.PolicyEntity;
            if (policyEntity == Entity.Null || m_MarkedDistrictQuery.IsEmptyIgnoreFilter)
            {
                return 0;
            }

            Dependency.Complete();
            int restored = 0;

            using (NativeArray<Entity> districts =
                m_MarkedDistrictQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity district in districts)
                {
                    DynamicBuffer<Game.Policies.Policy> policies =
                        EntityManager.GetBuffer<Game.Policies.Policy>(district);
                    bool found = false;

                    for (int i = 0; i < policies.Length; i++)
                    {
                        Game.Policies.Policy policy = policies[i];
                        if (policy.m_Policy != policyEntity)
                        {
                            continue;
                        }

                        found = true;
                        if ((policy.m_Flags & Game.Policies.PolicyFlags.Active) == 0)
                        {
                            policy.m_Flags |= Game.Policies.PolicyFlags.Active;
                            policies[i] = policy;
                            restored++;
                        }

                        break;
                    }

                    if (!found)
                    {
                        policies.Add(new Game.Policies.Policy(
                            policyEntity,
                            Game.Policies.PolicyFlags.Active,
                            0f));
                        restored++;
                    }
                }
            }

            return restored;
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!m_RestoreRequested || ParkingPolicySystem.PolicyEntity == Entity.Null)
            {
                return;
            }

            RestoreNow();
            m_RestoreRequested = false;
        }
    }
}

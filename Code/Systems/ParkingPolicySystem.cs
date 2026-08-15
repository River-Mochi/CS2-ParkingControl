// <copyright file="ParkingPolicySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Registers the zero-mask district policy used to select no-street-parking districts.

namespace ParkingControl
{
    using System;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Prefabs;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// Adds Parking Control's toggle to the vanilla district policy panel.
    /// </summary>
    public sealed partial class ParkingPolicySystem : GameSystemBase
    {
        internal const string PrefabName = "ParkingControlNoStreetParking";

        private PrefabSystem m_PrefabSystem = null!;
        private bool m_Installed;

        /// <summary>
        /// Gets the prefab entity stored in each district's native Policy buffer.
        /// </summary>
        internal static Entity PolicyEntity { get; private set; }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        /// <inheritdoc/>
        protected override void OnGamePreload(
            Colossal.Serialization.Entities.Purpose purpose,
            GameMode mode)
        {
            base.OnGamePreload(purpose, mode);
            Install();
        }

        /// <inheritdoc/>
        protected override void OnGameLoadingComplete(
            Colossal.Serialization.Entities.Purpose purpose,
            GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            Install();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            PolicyEntity = Entity.Null;
            base.OnDestroy();
        }

        private void Install()
        {
            if (m_Installed)
            {
                return;
            }

            PolicyTogglePrefab prefab = PrefabBase.Create<PolicyTogglePrefab>(PrefabName);
            prefab.m_Category = PolicyCategory.Traffic;
            prefab.m_Visibility = PolicyVisibility.Default;

            // An empty DistrictOptions component creates DistrictOptionData with a zero mask.
            // The checkbox therefore uses the native Policy buffer without claiming a vanilla bit.
            DistrictOptions options = prefab.AddOrGetComponent<DistrictOptions>();
            options.m_Options = Array.Empty<Game.Areas.DistrictOption>();

            UIObject uiObject = prefab.AddOrGetComponent<UIObject>();
            uiObject.m_Icon = "coui://ui-mods/images/NoStreetParking_Blue.svg";
            uiObject.m_Priority = 50;

            // Place this directly after Roadside Parking Fee when vanilla metadata is available.
            EntityQuery parkingFeeQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DistrictOptionData>(),
                ComponentType.ReadOnly<PolicySliderData>(),
                ComponentType.ReadOnly<UIObjectData>());
            using (NativeArray<Entity> policies = parkingFeeQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity policy in policies)
                {
                    DistrictOptionData optionsData =
                        EntityManager.GetComponentData<DistrictOptionData>(policy);
                    if (Game.Areas.AreaUtils.HasOption(
                            optionsData,
                            Game.Areas.DistrictOption.PaidParking))
                    {
                        uiObject.m_Priority =
                            EntityManager.GetComponentData<UIObjectData>(policy).m_Priority + 1;
                        break;
                    }
                }
            }

            parkingFeeQuery.Dispose();

            if (!m_PrefabSystem.AddPrefab(prefab))
            {
                LogUtils.Warn($"{Mod.ModTag} Could not register the district parking policy prefab.");
                return;
            }

            PolicyEntity = m_PrefabSystem.GetEntity(prefab);
            m_Installed = true;
            NoStreetParkingSystem.RequestReconcile();
            LogUtils.Info(
                $"{Mod.ModTag} District policy registered as {PrefabName} " +
                $"(priority {uiObject.m_Priority}).");
        }
    }
}

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
    using Game.UI.InGame;
    using Unity.Entities;

    /// <summary>
    /// Adds Parking Control's toggle to the vanilla district policy panel.
    /// </summary>
    public sealed partial class ParkingPolicySystem : GameSystemBase
    {
        internal const string PrefabName = "ParkingControlNoStreetParking";

        private PrefabSystem m_PrefabSystem = null!;
        private PolicyTogglePrefab? m_Prefab;
        private bool m_Installed;

        /// <summary>
        /// Gets the prefab entity stored in each district's native Policy buffer.
        /// </summary>
        internal static Entity PolicyEntity { get; private set; }

        /// <summary>
        /// Updates the native policy-list visibility after the Options scope changes.
        /// </summary>
        internal static void RefreshVisibility()
        {
            World? world = World.DefaultGameObjectInjectionWorld;
            world?.GetExistingSystemManaged<ParkingPolicySystem>()?.ApplyVisibility();
        }

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
            m_Prefab = null;
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
            prefab.m_Visibility = GetVisibility();

            // An empty DistrictOptions component creates DistrictOptionData with a zero mask.
            // The checkbox therefore uses the native Policy buffer without claiming a vanilla bit.
            DistrictOptions options = prefab.AddOrGetComponent<DistrictOptions>();
            options.m_Options = Array.Empty<Game.Areas.DistrictOption>();

            UIObject uiObject = prefab.AddOrGetComponent<UIObject>();
            uiObject.m_Icon = "Media/Game/Policies/PaidParking.svg";
            // Match vanilla priority so the policy uses the panel's normal alphabetical order.
            uiObject.m_Priority = 0;

            if (!m_PrefabSystem.AddPrefab(prefab))
            {
                LogUtils.Warn($"{Mod.ModTag} Could not register the district parking policy prefab.");
                return;
            }

            PolicyEntity = m_PrefabSystem.GetEntity(prefab);
            m_Prefab = prefab;
            m_Installed = true;
            NoStreetParkingSystem.RequestReconcile();
            LogUtils.Info($"{Mod.ModTag} District policy registered as {PrefabName}.");
        }

        private static PolicyVisibility GetVisibility()
        {
            return Mod.Settings?.Scope == PCSettings.ParkingScope.ByDistrict
                ? PolicyVisibility.Default
                : PolicyVisibility.HideFromPolicyList;
        }

        private void ApplyVisibility()
        {
            if (m_Prefab == null)
            {
                return;
            }

            PolicyVisibility visibility = GetVisibility();
            if (m_Prefab.m_Visibility == visibility)
            {
                return;
            }

            // Hiding the prefab does not remove saved Policy-buffer selections.
            m_Prefab.m_Visibility = visibility;
            World.GetExistingSystemManaged<SelectedInfoUISystem>()?.RequestUpdate();
            LogUtils.Info(
                $"{Mod.ModTag} District policy row " +
                $"{(visibility == PolicyVisibility.Default ? "shown" : "hidden")} " +
                $"for scope {Mod.Settings?.Scope}.");
        }
    }
}

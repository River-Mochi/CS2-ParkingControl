// <copyright file="ParkingPolicySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Registers the runtime-only zero-mask policy shown in the vanilla district panel.

using System;
using CS2Shared.RiverMochi;
using Game;
using Game.Prefabs;
using Game.UI.InGame;
using Unity.Entities;

namespace ParkingControl
{

    /// <summary>
    /// Adds Parking Control's toggle to the vanilla district policy panel.
    /// </summary>
    public sealed partial class ParkingPolicySystem : GameSystemBase
    {
        internal const string kPrefabName = "PCDistrictParkingBan";

        private PrefabSystem m_PrefabSystem = null!;
        private PolicyTogglePrefab? m_Prefab;
        private bool m_Installed;

        /// <summary>
        /// Gets the prefab entity stored in each district's runtime Policy buffer.
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

            bool isGameLoad =
                mode == GameMode.Game &&
                (purpose == Colossal.Serialization.Entities.Purpose.NewGame ||
                    purpose == Colossal.Serialization.Entities.Purpose.LoadGame);
            if (!isGameLoad)
            {
                return;
            }

            DistrictPolicyRestoreSystem restoreSystem =
                World.GetOrCreateSystemManaged<DistrictPolicyRestoreSystem>();
            int restored = restoreSystem.RestoreNow();
            if (restored > 0)
            {
                LogUtils.Info(
                    $"{Mod.ModTag} Restored district Parking Ban in " +
                    $"{restored} {(restored == 1 ? "district" : "districts")}.");
            }
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

            PolicyTogglePrefab prefab = PrefabBase.Create<PolicyTogglePrefab>(kPrefabName);
            prefab.m_Category = PolicyCategory.Traffic;
            prefab.m_Visibility = GetVisibility();

            // An empty DistrictOptions component creates DistrictOptionData with a zero mask.
            // The checkbox therefore uses the native Policy buffer without claiming a vanilla bit.
            DistrictOptions options = prefab.AddOrGetComponent<DistrictOptions>();
            options.m_Options = Array.Empty<Game.Areas.DistrictOption>();

            UIObject uiObject = prefab.AddOrGetComponent<UIObject>();
            uiObject.m_Icon = "coui://ui-mods/images/PC-DistrictParkingBan.svg";
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

            LogUtils.Info($"{Mod.ModTag} Custom District policy ready.");
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

            // Hiding the row does not clear its runtime district selections.
            m_Prefab.m_Visibility = visibility;
            World.GetExistingSystemManaged<SelectedInfoUISystem>()?.RequestUpdate();
#if DEBUG
            LogUtils.Info(
                $"{Mod.ModTag} District policy row " +
                $"{(visibility == PolicyVisibility.Default ? "shown" : "hidden")} " +
                $"for scope {Mod.Settings?.Scope}.");
#endif
        }
    }
}

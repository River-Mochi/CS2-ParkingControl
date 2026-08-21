// <copyright file="ManualNoParkingToolBuilder.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Creates Parking Control's No Parking tile in the vanilla Roads Services palette.

namespace ParkingControl
{
    using System;
    using CS2Shared.RiverMochi;
    using Game.Prefabs;
    using Unity.Entities;
    using UnityEngine;

    internal static class ManualNoParkingToolBuilder
    {
        internal const string kIconPath =
            "coui://ui-mods/images/ForbidParking.svg";

        private const string kRoadServicesGroup = "RoadsServices";

        private static World? s_World;
        private static PrefabSystem? s_PrefabSystem;
        private static bool s_Instantiated;

        /// <summary>
        /// Resets cached world references after a mod load/hot reload.
        /// </summary>
        internal static void Initialize(bool force = false)
        {
            if (!force && s_World != null)
            {
                return;
            }

            s_World = World.DefaultGameObjectInjectionWorld;
            s_PrefabSystem =
                s_World?.GetExistingSystemManaged<PrefabSystem>();

            s_Instantiated = false;
        }

        /// <summary>
        /// Creates or reconnects the No Parking Roads Services tile.
        /// </summary>
        /// <returns>True once the tool prefab is available.</returns>
        internal static bool TryInstantiate()
        {
            if (s_Instantiated)
            {
                return true;
            }

            s_World ??= World.DefaultGameObjectInjectionWorld;
            s_PrefabSystem ??=
                s_World?.GetExistingSystemManaged<PrefabSystem>();

            if (s_World == null || s_PrefabSystem == null)
            {
                return false;
            }

            NoParkingRoadToolSystem toolSystem =
                s_World.GetOrCreateSystemManaged<NoParkingRoadToolSystem>();

            // ToolSystem activates a prefab by asking registered tool systems
            // in list order whether they accept it. Our cloned FencePrefab is
            // also a NetPrefab, so vanilla NetToolSystem would accept it first
            // unless this very specific tool is placed before NetToolSystem.
            EnsureActivationOrder(toolSystem);

            // Hot-reload/re-entry safety: reuse our existing prefab if it
            // already exists instead of creating a duplicate.
            PrefabID toolId = new(
                "FencePrefab",
                NoParkingRoadToolSystem.kToolId);

            if (s_PrefabSystem.TryGetPrefab(
                    toolId,
                    out PrefabBase? existingPrefab) &&
                existingPrefab != null)
            {
                if (toolSystem.TrySetPrefab(existingPrefab))
                {
                    s_Instantiated = true;

#if DEBUG
                    LogUtils.Info(
                        $"{Mod.ModTag} [RoadTool] Reused existing " +
                        $"{toolId.GetName()} prefab.");
#endif

                    return true;
                }
            }

            if (!TryResolvePlacement(
                    s_PrefabSystem,
                    out PrefabBase? donorPrefab,
                    out UIObject? donorUI,
                    out int priority) ||
                donorPrefab == null ||
                donorUI == null)
            {
                return false;
            }

            try
            {
                PrefabBase toolPrefab =
                    s_PrefabSystem.DuplicatePrefab(
                        donorPrefab,
                        NoParkingRoadToolSystem.kToolId);

                // Never alter the donor. Strip donor-specific parts from
                // the duplicate before adding our own tool metadata.
                if (toolPrefab.Has<Unlockable>())
                {
                    toolPrefab.Remove<Unlockable>();
                }

                if (toolPrefab.Has<NetSubObjects>())
                {
                    toolPrefab.Remove<NetSubObjects>();
                }

                if (toolPrefab.Has<UIObject>())
                {
                    toolPrefab.Remove<UIObject>();
                }

                // Forbid Right Turn / Crosswalk contain real vanilla
                // upgrade behavior. Our clone must not inherit that state.
                if (toolPrefab.Has<NetUpgrade>())
                {
                    toolPrefab.Remove<NetUpgrade>();
                }

                UIObject uiObject =
                    ScriptableObject.CreateInstance<UIObject>();

                uiObject.name = NoParkingRoadToolSystem.kToolId;
                uiObject.m_Icon = kIconPath;
                uiObject.m_IsDebugObject = donorUI.m_IsDebugObject;
                uiObject.m_Group = donorUI.m_Group;
                uiObject.m_Priority = priority;
                uiObject.active = donorUI.active;

                toolPrefab.AddComponentFrom(uiObject);

                // Keep the prefab recognizable as a Roads Services
                // tool-selector asset, but with no vanilla upgrade flags.
                NetUpgrade netUpgrade =
                    ScriptableObject.CreateInstance<NetUpgrade>();

                toolPrefab.AddComponentFrom(netUpgrade);

                // Re-index the duplicate after changing its components.
                s_PrefabSystem.UpdatePrefab(toolPrefab);

                if (!toolSystem.TrySetPrefab(toolPrefab))
                {
                    LogUtils.Warn(
                        $"{Mod.ModTag} [RoadTool] Created No Parking " +
                        "prefab but could not attach it to the tool system.");

                    return false;
                }

                s_Instantiated = true;

                LogUtils.Info(
                    $"{Mod.ModTag} [RoadTool] No Parking tile created: " +
                    $"anchor={donorPrefab.name}, " +
                    $"group={donorUI.m_Group?.name ?? "(null)"}, " +
                    $"priority={priority}.");

                return true;
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    $"{Mod.ModTag} [RoadTool] Failed to create No Parking " +
                    $"tile: {ex.GetType().Name}: {ex.Message}",
                    ex);

                return false;
            }
        }

        private static void EnsureActivationOrder(
            NoParkingRoadToolSystem toolSystem)
        {
            if (s_World == null)
            {
                return;
            }

            Game.Tools.ToolSystem toolRegistry =
                s_World.GetOrCreateSystemManaged<Game.Tools.ToolSystem>();

            System.Collections.Generic.List<Game.Tools.ToolBaseSystem> tools =
                toolRegistry.tools;

            int parkingToolIndex = tools.IndexOf(toolSystem);
            int netToolIndex =
                tools.FindIndex(
                    tool => tool is Game.Tools.NetToolSystem);

            if (parkingToolIndex < 0 || netToolIndex < 0)
            {
                LogUtils.Warn(
                    $"{Mod.ModTag} [RoadTool] Could not establish tool " +
                    $"activation order (NoParking={parkingToolIndex}, " +
                    $"NetTool={netToolIndex}).");

                return;
            }

            if (parkingToolIndex > netToolIndex)
            {
                tools.RemoveAt(parkingToolIndex);

                // Removing our later item does not move NetToolSystem, but
                // resolve it again so this remains correct if list ordering
                // changes in a future game version.
                netToolIndex =
                    tools.FindIndex(
                        tool => tool is Game.Tools.NetToolSystem);

                if (netToolIndex < 0)
                {
                    tools.Add(toolSystem);
                    LogUtils.Warn(
                        $"{Mod.ModTag} [RoadTool] NetToolSystem disappeared " +
                        "while reordering tools.");

                    return;
                }

                tools.Insert(netToolIndex, toolSystem);
            }

            int finalParkingIndex = tools.IndexOf(toolSystem);
            int finalNetIndex =
                tools.FindIndex(
                    tool => tool is Game.Tools.NetToolSystem);

            LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] Activation order ready: " +
                $"NoParking={finalParkingIndex}, NetTool={finalNetIndex}.");
        }

        private static bool TryResolvePlacement(
            PrefabSystem prefabSystem,
            out PrefabBase? donorPrefab,
            out UIObject? donorUI,
            out int priority)
        {
            donorPrefab = null;
            donorUI = null;
            priority = 0;

            bool haveForbidRight = TryGetRoadServicesPrefab(
                prefabSystem,
                "FencePrefab",
                "Forbid Right Turn",
                out PrefabBase? forbidRightPrefab,
                out UIObject? forbidRightUI);

            bool haveCrosswalk = TryGetRoadServicesPrefab(
                prefabSystem,
                "FencePrefab",
                "Crosswalk",
                out PrefabBase? crosswalkPrefab,
                out UIObject? crosswalkUI);

            // Preferred layout:
            //
            // Forbid Right Turn    No Parking    ...    Crosswalk
            //
            if (haveForbidRight &&
                forbidRightPrefab != null &&
                forbidRightUI != null)
            {
                donorPrefab = forbidRightPrefab;
                donorUI = forbidRightUI;
                priority = forbidRightUI.m_Priority + 1;

                // Protect the requested ordering if CO ever narrows the
                // priority gap between the two vanilla buttons.
                if (haveCrosswalk &&
                    crosswalkUI != null &&
                    priority >= crosswalkUI.m_Priority)
                {
                    priority = crosswalkUI.m_Priority - 1;
                }

                return true;
            }

            // Stable fallback already proven by Easy Zoning.
            if (haveCrosswalk &&
                crosswalkPrefab != null &&
                crosswalkUI != null)
            {
                donorPrefab = crosswalkPrefab;
                donorUI = crosswalkUI;
                priority = crosswalkUI.m_Priority - 1;

                LogUtils.Warn(
                    $"{Mod.ModTag} [RoadTool] Forbid Right Turn anchor " +
                    "was unavailable; using Crosswalk fallback.");

                return true;
            }

            // Final exact-name fallback. No reflection/random donor scan.
            if (TryGetRoadServicesPrefab(
                    prefabSystem,
                    "FencePrefab",
                    "Wide Sidewalk",
                    out PrefabBase? sidewalkPrefab,
                    out UIObject? sidewalkUI) &&
                sidewalkPrefab != null &&
                sidewalkUI != null)
            {
                donorPrefab = sidewalkPrefab;
                donorUI = sidewalkUI;
                priority = sidewalkUI.m_Priority + 1;

                LogUtils.Warn(
                    $"{Mod.ModTag} [RoadTool] Using Wide Sidewalk " +
                    "fallback anchor.");

                return true;
            }

            return false;
        }

        private static bool TryGetRoadServicesPrefab(
            PrefabSystem prefabSystem,
            string typeName,
            string prefabName,
            out PrefabBase? prefab,
            out UIObject? uiObject)
        {
            prefab = null;
            uiObject = null;

            PrefabID id = new(typeName, prefabName);

            bool found =
                prefabSystem.TryGetPrefab(id, out PrefabBase? candidate) &&
                candidate != null;

#if DEBUG
            LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] Probe {typeName}:{prefabName}: " +
                $"{(found ? "FOUND" : "missing")}.");
#endif

            if (!found || candidate == null)
            {
                return false;
            }

            if (!candidate.TryGet(out UIObject? candidateUI) ||
                candidateUI == null)
            {
#if DEBUG
                LogUtils.Info(
                    $"{Mod.ModTag} [RoadTool] {prefabName} has no UIObject.");
#endif
                return false;
            }

            string groupName =
                candidateUI.m_Group?.name ?? string.Empty;

            if (!string.Equals(
                    groupName,
                    kRoadServicesGroup,
                    StringComparison.OrdinalIgnoreCase))
            {
#if DEBUG
                LogUtils.Info(
                    $"{Mod.ModTag} [RoadTool] {prefabName} belongs to " +
                    $"'{groupName}', not RoadsServices.");
#endif
                return false;
            }

#if DEBUG
            LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] {prefabName}: " +
                $"priority={candidateUI.m_Priority}, group={groupName}.");
#endif

            prefab = candidate;
            uiObject = candidateUI;
            return true;
        }
    }
}

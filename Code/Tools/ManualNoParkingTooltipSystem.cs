// <copyright file="ManualNoParkingTooltipSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Renames the game's native Apply / Secondary Apply hints while
// Manual No Parking is active. The vanilla InputHintsTooltipSystem renders
// the actual mouse/controller icons; do not add duplicate InputHintTooltips here.

namespace ParkingControl
{
    using Game.Input;
    using Game.Tools;
    using Game.UI.Tooltip;

    public sealed partial class ManualNoParkingTooltipSystem : TooltipSystemBase
    {
        private ToolSystem m_ToolSystem = null!;

        private ProxyAction? m_ApplyAction;
        private ProxyAction? m_SecondaryApplyAction;

        private DisplayNameOverride? m_ApplyOverride;
        private DisplayNameOverride? m_SecondaryOverride;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_ToolSystem =
                World.GetOrCreateSystemManaged<ToolSystem>();

            InputManager? inputManager = InputManager.instance;

            if (inputManager == null)
            {
                return;
            }

            m_ApplyAction =
                inputManager.FindAction(
                    InputManager.kToolMap,
                    "Apply");

            m_SecondaryApplyAction =
                inputManager.FindAction(
                    InputManager.kToolMap,
                    "Secondary Apply");
        }

        protected override void OnDestroy()
        {
            m_ApplyOverride?.Dispose();
            m_SecondaryOverride?.Dispose();

            m_ApplyOverride = null;
            m_SecondaryOverride = null;

            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            InputManager? inputManager = InputManager.instance;

            if (m_ToolSystem.activeTool is not ManualNoParkingToolSystem ||
                inputManager == null ||
                inputManager.activeControlScheme !=
                    InputManager.ControlScheme.KeyboardAndMouse)
            {
                SetOverridesActive(false);
                return;
            }

            EnsureOverrides();
            SetOverridesActive(true);

            // Do NOT call AddMouseTooltip() for Apply / Secondary Apply.
            // CS2's InputHintsTooltipSystem already enumerates the active
            // ToolBaseSystem actions and adds these exact tooltip paths.
            // We only override their display names to Upgrade / Downgrade.
        }

        private void EnsureOverrides()
        {
            if (m_ApplyAction != null &&
                m_ApplyOverride == null)
            {
                m_ApplyOverride =
                    new DisplayNameOverride(
                        "ParkingControl.HintTooltip.Upgrade",
                        m_ApplyAction,
                        "Upgrade",
                        1);
            }

            if (m_SecondaryApplyAction != null &&
                m_SecondaryOverride == null)
            {
                m_SecondaryOverride =
                    new DisplayNameOverride(
                        "ParkingControl.HintTooltip.Downgrade",
                        m_SecondaryApplyAction,
                        "Downgrade",
                        1);
            }

            if (m_ApplyOverride != null)
            {
                m_ApplyOverride.displayName = "Upgrade";
            }

            if (m_SecondaryOverride != null)
            {
                m_SecondaryOverride.displayName = "Downgrade";
            }
        }

        private void SetOverridesActive(bool active)
        {
            if (m_ApplyOverride != null)
            {
                m_ApplyOverride.active = active;
            }

            if (m_SecondaryOverride != null)
            {
                m_SecondaryOverride.active = active;
            }
        }
    }
}

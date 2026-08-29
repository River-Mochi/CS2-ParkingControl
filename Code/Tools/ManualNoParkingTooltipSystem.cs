// <copyright file="ManualNoParkingTooltipSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Renames the game's native mouse Apply / Secondary Apply hints while
// Manual No Parking is active. Vanilla draws the mouse/controller icons.

using Game.Input;
using Game.Tools;
using Game.UI.Tooltip;

namespace ParkingControl
{

    public sealed partial class ManualNoParkingTooltipSystem : TooltipSystemBase
    {
        internal const string kUpgradeHintId = "ParkingControl.Upgrade";
        internal const string kDowngradeHintId = "ParkingControl.Downgrade";

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

            // Vanilla InputHintsTooltipSystem draws the actual mouse hints.
            // We only provide the localized action names.
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
                        kUpgradeHintId,
                        1);
            }

            if (m_SecondaryApplyAction != null &&
                m_SecondaryOverride == null)
            {
                m_SecondaryOverride =
                    new DisplayNameOverride(
                        "ParkingControl.HintTooltip.Downgrade",
                        m_SecondaryApplyAction,
                        kDowngradeHintId,
                        1);
            }

            if (m_ApplyOverride != null)
            {
                m_ApplyOverride.displayName = kUpgradeHintId;
            }

            if (m_SecondaryOverride != null)
            {
                m_SecondaryOverride.displayName = kDowngradeHintId;
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

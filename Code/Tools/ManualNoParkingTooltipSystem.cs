// <copyright file="ManualNoParkingTooltipSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Shows native CS2 LMB/RMB action hints while Manual No Parking is active.

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

        private InputHintTooltip? m_ApplyHint;
        private InputHintTooltip? m_SecondaryHint;

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

            ShowHint(
                m_ApplyAction,
                ref m_ApplyHint);

            ShowHint(
                m_SecondaryApplyAction,
                ref m_SecondaryHint);
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

        private void ShowHint(
            ProxyAction? action,
            ref InputHintTooltip? cachedHint)
        {
            if (action == null ||
                !action.isSet)
            {
                return;
            }

            cachedHint ??=
                new InputHintTooltip(action);

            cachedHint.Refresh(
                InputManager.DeviceType.Mouse);

            AddMouseTooltip(cachedHint);
        }
    }
}

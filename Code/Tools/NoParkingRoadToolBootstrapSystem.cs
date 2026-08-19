// <copyright file="NoParkingRoadToolBootstrapSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Waits for the vanilla Roads Services prefabs, then creates the No Parking tile.

namespace ParkingControl
{
    using Colossal.Serialization.Entities;
    using CS2Shared.RiverMochi;
    using Game;

    public sealed partial class NoParkingRoadToolBootstrapSystem : GameSystemBase
    {
        private const int kMaxTries = 1800;
        private const int kDebugLogEvery = 120;

        private bool m_Armed;
        private int m_Tries;

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_Armed = false;
            m_Tries = 0;
            Enabled = false;
        }

        /// <inheritdoc/>
        protected override void OnGameLoadingComplete(
            Purpose purpose,
            GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            bool isPlayableCity =
                mode == GameMode.Game &&
                (purpose == Purpose.NewGame ||
                    purpose == Purpose.LoadGame);

            if (!isPlayableCity)
            {
                m_Armed = false;
                m_Tries = 0;
                Enabled = false;
                return;
            }

            m_Armed = true;
            m_Tries = 0;
            Enabled = true;

#if DEBUG
            LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] Bootstrap armed.");
#endif
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!m_Armed)
            {
                Enabled = false;
                return;
            }

            if (NoParkingRoadToolBuilder.TryInstantiate())
            {
                m_Armed = false;
                Enabled = false;
                return;
            }

            m_Tries++;

#if DEBUG
            if ((m_Tries % kDebugLogEvery) == 0)
            {
                LogUtils.Info(
                    $"{Mod.ModTag} [RoadTool] Waiting for Roads Services " +
                    $"prefabs; try={m_Tries}.");
            }
#endif

            if (m_Tries < kMaxTries)
            {
                return;
            }

            LogUtils.Warn(
                $"{Mod.ModTag} [RoadTool] Roads Services donor prefabs " +
                "were not found before the bootstrap timeout.");

            m_Armed = false;
            Enabled = false;
        }
    }
}

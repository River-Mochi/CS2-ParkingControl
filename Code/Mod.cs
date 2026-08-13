// <copyright file="Mod.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Registers Parking Control settings, localization, logging, and ECS systems.

namespace ParkingControl
{
    using System;
    using System.Reflection;
    using Colossal.IO.AssetDatabase;
    using Colossal.Localization;
    using Colossal.Logging;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Modding;
    using Game.Pathfind;
    using Game.SceneFlow;
    using Game.Serialization;

    /// <summary>
    /// Parking Control mod entry point.
    /// </summary>
    public sealed class Mod : IMod
    {
        public const string ModName = "Parking Control";
        public const string ModId = "ParkingControl";
        public const string ModTag = "[PC]";

        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        public static readonly ILog s_Log =
            LogManager.GetLogger(ModId).SetShowsErrorsInUI(false);

        private static bool s_BannerLogged;

        /// <summary>
        /// Gets the active Options UI settings instance.
        /// </summary>
        public static PCSettings? Settings { get; private set; }

        /// <inheritdoc/>
        public void OnLoad(UpdateSystem updateSystem)
        {
            // ShellOpen also configures LogUtils with this mod's exact log file.
            ShellOpen.Configure(s_Log, ModId, ModTag);
            ParkingStatusCache.InvalidateCache();
            LogBanner();

            if (GameManager.instance == null)
            {
                LogUtils.Warn($"{ModTag} GameManager.instance is null; initialization stopped.");
                return;
            }

            PCSettings settings = new(this);
            Settings = settings;

            RegisterLocalization(settings);
            LoadSettings(settings);
            RegisterOptions(settings);
            RegisterSystems(updateSystem);

            LogUtils.Info($"{ModTag} Loaded. Whole-city street parking restriction: {settings.NoStreetParking}.");
        }

        /// <inheritdoc/>
        public void OnDispose()
        {
            Settings?.UnregisterInOptionsUI();
            Settings = null;
            ParkingStatusCache.InvalidateCache();
            LogUtils.Info($"{ModTag} Disposed.");
        }

        private static void LogBanner()
        {
            if (s_BannerLogged)
            {
                return;
            }

            s_BannerLogged = true;
#if DEBUG
            LogUtils.Info($"{ModName} v{ModVersion} DEBUG loaded");
#else
            LogUtils.Info($"{ModName} v{ModVersion} loaded");
#endif
        }

        private static void RegisterLocalization(PCSettings settings)
        {
            try
            {
                LocalizationManager? localizationManager = GameManager.instance?.localizationManager;
                if (localizationManager == null)
                {
                    LogUtils.Warn($"{ModTag} LocalizationManager is null; locale sources were not registered.");
                    return;
                }

                // Localization must exist before the Options UI reads setting labels.
                localizationManager.AddSource("en-US", new LocaleEN(settings));
            }
            catch (Exception ex)
            {
                LogUtils.Error($"{ModTag} Localization registration failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private void LoadSettings(PCSettings settings)
        {
            try
            {
                AssetDatabase.global.LoadSettings(ModId, settings, new PCSettings(this));
            }
            catch (Exception ex)
            {
                LogUtils.Error($"{ModTag} Settings load failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static void RegisterOptions(PCSettings settings)
        {
            try
            {
                settings.RegisterInOptionsUI();
            }
            catch (Exception ex)
            {
                LogUtils.Error($"{ModTag} Options UI registration failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static void RegisterSystems(UpdateSystem updateSystem)
        {
            try
            {
                updateSystem.UpdateBefore<StreetParkingBaselineSystem, ParkingLaneDataSystem>(SystemUpdatePhase.ModificationEnd);
                updateSystem.UpdateAfter<NoStreetParkingSystem, ParkingLaneDataSystem>(SystemUpdatePhase.ModificationEnd);
                updateSystem.UpdateAfter<ParkingStatusSystem, NoStreetParkingSystem>(SystemUpdatePhase.ModificationEnd);
                updateSystem.UpdateBefore<StreetParkingSaveSystem, SerializerSystem>(SystemUpdatePhase.Serialize);
                updateSystem.UpdateAfter<StreetParkingRestoreSystem, SerializerSystem>(SystemUpdatePhase.Serialize);
                StreetParkingBaselineSystem.RequestScan();
                NoStreetParkingSystem.RequestReconcile();
            }
            catch (Exception ex)
            {
                LogUtils.Error($"{ModTag} System scheduling failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }
    }
}

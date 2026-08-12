// <copyright file="Mod.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Loads Parking Control settings, localization, logging, and the street-parking policy system.

namespace ParkingControl
{
    using System.Reflection;
    using Colossal;
    using Colossal.IO.AssetDatabase;
    using Colossal.Localization;
    using Colossal.Logging;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Modding;
    using Game.Pathfind;
    using Game.SceneFlow;
    using Game.Settings;
    using Game.Simulation;
    using Unity.Entities;
    using static Game.UI.Menu.AssetUploadPanelUISystem;


    public sealed class Mod : IMod
    {
        public const string ModName = "Parking Control";
        public const string ModId = "ParkingControl";
        public const string ModTag = "[PC]";

    /// <summary>
    /// Gets the mod version from the built assembly.
    /// </summary>
        public static readonly string ModVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        private static readonly ILog s_Log = LogManager.GetLogger(ModId).SetShowsErrorsInUI(false);
        private static bool s_BannerLogged;

    /// <summary>
    /// Gets the current options settings instance.
    /// </summary>
        public static PCSettings? Settings { get; private set; }

    /// <inheritdoc/>
        public void OnLoad(UpdateSystem updateSystem)
        {
            LogUtils.Configure(ModId, s_Log);

            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                LogUtils.Info($"{ModTag} {ModName} {ModVersion} loading.");
            }

            if (GameManager.instance?.gameMode is not GameMode.Game)
            {
                LogUtils.Warn($"{ModTag} Not loading outside a game session.");
                return;
            }

            PCSettings settings = new PCSettings(this);
            Settings = settings;
            RegisterLocalization(settings);
            AssetDatabase.global.LoadSettings(ModId, settings, new PCSettings(this));
            settings.RegisterInOptionsUI();

            updateSystem.UpdateAfter<NoStreetParkingSystem, ParkingLaneDataSystem>(SystemUpdatePhase.ModificationEnd);
            updateSystem.UpdateBefore<NoStreetParkingSystem, LanesModifiedSystem>(SystemUpdatePhase.ModificationEnd);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<NoStreetParkingSystem>();
            NoStreetParkingSystem.RequestReconcile();

            LogUtils.Info($"{ModTag} Loaded. Whole-city street parking: {settings.NoStreetParking}.");
        }

    /// <inheritdoc/>
        public void OnDispose()
        {
            if (Settings is not null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }

            LogUtils.Info($"{ModTag} Disposed.");
        }

        private static void RegisterLocalization(PCSettings settings)
        {
            LocalizationManager? localizationManager = GameManager.instance?.localizationManager;
            if (localizationManager == null)
            {
                LogUtils.Warn($"{ModTag} LocalizationManager is null; Options labels were not registered.");
                return;
            }

                // Register localization before Options UI reads setting labels.
                localizationManager.AddSource("en-US", new LocaleEN(settings));
            // Future to be added, uncomment as needed:
               // localizationManager.AddSource("fr-FR", new LocaleFR(setting));
               // localizationManager.AddSource("es-ES", new LocaleES(setting));
               // localizationManager.AddSource("de-DE", new LocaleDE(setting));
               // localizationManager.AddSource("it-IT", new LocaleIT(setting));
              //  localizationManager.AddSource("ja-JP", new LocaleJA(setting));
              //  localizationManager.AddSource("ko-KR", new LocaleKO(setting));
              //  localizationManager.AddSource("pl-PL", new LocalePL(setting));
              //  localizationManager.AddSource("pt-BR", new LocalePT_BR(setting));
              //  localizationManager.AddSource("zh-HANS", new LocaleZH_HANS(setting));
             //   localizationManager.AddSource("zh-HANT", new LocaleZH_HANT(setting));



        }
    }
}

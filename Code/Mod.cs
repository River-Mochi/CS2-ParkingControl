// <copyright file="Mod.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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

        public static PCSettings? Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            ShellOpen.Configure(s_Log, ModId, ModTag);
            ParkingStatusCache.InvalidateCache();

            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
#if DEBUG
                LogUtils.Info($"{ModName} v{ModVersion} DEBUG");
#else
                LogUtils.Info($"{ModName} v{ModVersion} RELEASE");
#endif
            }

            PCSettings settings = new(this);
            Settings = settings;

            try
            {
                LocalizationManager? localizationManager =
                    GameManager.instance?.localizationManager;

                if (localizationManager == null)
                {
                    LogUtils.Warn(
                        $"{ModTag} LocalizationManager is null; locale sources were not registered.");
                }
                else
                {
                    localizationManager.AddSource("en-US", new LocaleEN(settings));
                    localizationManager.AddSource("fr-FR", new LocaleFR(settings));
                    localizationManager.AddSource("es-ES", new LocaleES(settings));
                    localizationManager.AddSource("de-DE", new LocaleDE(settings));
                    localizationManager.AddSource("it-IT", new LocaleIT(settings));
                    localizationManager.AddSource("ja-JP", new LocaleJA(settings));
                    localizationManager.AddSource("ko-KR", new LocaleKO(settings));
                    localizationManager.AddSource("pl-PL", new LocalePL(settings));
                    localizationManager.AddSource("pt-BR", new LocalePT_BR(settings));
                    localizationManager.AddSource("zh-HANS", new LocaleZH_HANS(settings));
                    localizationManager.AddSource("zh-HANT", new LocaleZH_HANT(settings));
                    localizationManager.AddSource("th-TH", new LocaleTH(settings));
                    localizationManager.AddSource("tr-TR", new LocaleTR(settings));
                    localizationManager.AddSource("vi-VN", new LocaleVI(settings));
                    localizationManager.AddSource("uk-UA", new LocaleUK(settings));
                }
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    $"{ModTag} Localization registration failed: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }

            AssetDatabase.global.LoadSettings(ModId, settings, new PCSettings(this));
            settings.RegisterInOptionsUI();

            ManualNoParkingToolBuilder.Initialize(force: true);
            updateSystem.UpdateAt<ParkingPolicySystem>(SystemUpdatePhase.PrefabUpdate);

            // Roads Services tab No Parking tool.
            updateSystem.UpdateAt<ManualNoParkingBootstrapSystem>(
                SystemUpdatePhase.Modification3);
            updateSystem.UpdateAt<ManualNoParkingToolSystem>(
                SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateAt<ManualNoParkingTooltipSystem>(
                SystemUpdatePhase.UITooltip);
            updateSystem.UpdateAt<ManualNoParkingOverlaySystem>(
                SystemUpdatePhase.Rendering);

            // One-shot button: mark occupied banned lanes before vanilla moves their cars.
            updateSystem.UpdateBefore<
                ParkingRelocationSystem,
                Game.Vehicles.FixParkingLocationSystem>(
                SystemUpdatePhase.Modification5);

            // Run after vanilla/replacement parking-lane calculations but before
            // CS2 rebuilds parking path data from those lanes.
            updateSystem.UpdateBefore<NoStreetParkingSystem, LanesModifiedSystem>(
                SystemUpdatePhase.ModificationEnd);

            updateSystem.UpdateAfter<ParkingStatusSystem, NoStreetParkingSystem>(
                SystemUpdatePhase.ModificationEnd);
            updateSystem.UpdateBefore<DistrictPolicySaveSystem, BeginPrefabSerializationSystem>(
                SystemUpdatePhase.Serialize);
            updateSystem.UpdateBefore<StreetParkingSaveSystem, SerializerSystem>(
                SystemUpdatePhase.Serialize);
            updateSystem.UpdateAfter<StreetParkingRestoreSystem, SerializerSystem>(
                SystemUpdatePhase.Serialize);
            updateSystem.UpdateAfter<DistrictPolicyRestoreSystem, EndPrefabSerializationSystem>(
                SystemUpdatePhase.Serialize);

            NoStreetParkingSystem.RequestReconcile();

            string scopeText = settings.Scope switch
            {
                PCSettings.ParkingScope.WholeCity => "Whole City",
                PCSettings.ParkingScope.ByDistrict => "by District",
                _ => "OFF",
            };

            LogUtils.Info(
                $"{ModTag} Parking Ban dropdown selection: {scopeText}.");
        }

        public void OnDispose()
        {
            Settings?.UnregisterInOptionsUI();
            Settings = null;
            ParkingStatusCache.InvalidateCache();
        }
    }
}

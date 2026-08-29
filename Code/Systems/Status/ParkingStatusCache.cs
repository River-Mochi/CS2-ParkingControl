// <copyright file="ParkingStatusCache.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Caches on-demand parking status text for the Options UI.

using System;
using System.Globalization;
using Game;
using Game.SceneFlow;
using Unity.Entities;
using UnityEngine;

namespace ParkingControl
{

    /// <summary>
    /// Supplies cached status strings without reading ECS data from setting property getters.
    /// </summary>
    internal static class ParkingStatusCache
    {
        private const string kLoadCityFallback = "No city loaded yet.";
        private const string kCollectingFallback = "Parking status is being collected...";
        private const string kUnavailableFallback = "Parking status is unavailable.";
        private const string kCollectionFailedFallback =
            "Parking status could not be collected; see ParkingControl.log.";

        private const string kScopeOffFallback =
            "Manual Only = city/district bans disabled | manual roads still work";

        private const string kManualNoneFallback = "None set";
        private const string kEnforcementFormatFallback =
            "{0} parked | {1}/{2} disabled{3}";
        private const string kManualEnforcementFormatFallback =
            "{0} parked | {1}/{2} disabled lanes{3}";
        private const string kDistrictEnforcementFormatFallback =
            "{0} parked | {1}/{2} disabled | {3}/{4} districts{5}";
        private const string kVehicleFormatFallback =
            "{0} street | {1} visible | {2} inside | {3} OC";
        private const string kSupplyFormatFallback =
            "{0} = {1}, public free {2}";
        private const string kShareFormatFallback =
            "{0} public | {1} bldg | {2} street | {3} total";

        private static bool s_ForceRefresh = true;
        private static bool s_HasRequestedSimulationFrame;
        private static bool s_HasSnapshot;
        private static bool s_RequestPending;
        private static bool s_WasInGame;
        private static int s_LastUiFrame = -1;
        private static uint s_LastRequestedSimulationFrame;
        private static int s_Version;
        private static string s_LastLocaleId = string.Empty;
        private static string s_LastMessageId = ParkingStatusLocale.kLoadCity;
        private static string s_LastMessageFallback = kLoadCityFallback;
        private static ParkingSnapshot s_LastSnapshot;

        /// <summary>
        /// Gets the cached enforcement summary.
        /// </summary>
        internal static string EnforcementRow { get; private set; } = kLoadCityFallback;

        /// <summary>
        /// Gets the cached manual No Parking summary.
        /// </summary>
        internal static string ManualRow { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the cached personal-vehicle location summary.
        /// </summary>
        internal static string VehicleRow { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the cached parking-supply summary.
        /// </summary>
        internal static string SupplyRow { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the cached citywide street-parking share.
        /// </summary>
        internal static string ShareRow { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the time when the cached snapshot was collected.
        /// </summary>
        internal static string UpdatedRow { get; private set; } = string.Empty;

        /// <summary>
        /// Requests one snapshot after the simulation advances and returns the published UI version.
        /// </summary>
        /// <returns>The version of the currently published strings.</returns>
        public static int GetUiVersion()
        {
            // Hidden status widgets receive one initial UI update. Gate here as well so
            // keeping Show Status off never schedules an ECS parking probe.
            if (!(Mod.Settings?.ShowStatus ?? false))
            {
                return s_Version;
            }

            // Status widgets ask for the version each UI frame; perform the check only once.
            int uiFrame = Time.frameCount;
            if (uiFrame == s_LastUiFrame)
            {
                return s_Version;
            }

            s_LastUiFrame = uiFrame;

            GameManager? gameManager = GameManager.instance;
            bool isGame = gameManager != null && gameManager.gameMode == GameMode.Game;
            if (isGame != s_WasInGame)
            {
                s_WasInGame = isGame;
                InvalidateCache();
            }

            RefreshLocalizedTextIfNeeded();

            if (!isGame)
            {
                PublishLocalizedMessage(ParkingStatusLocale.kLoadCity, kLoadCityFallback);
                return s_Version;
            }

            try
            {
                World? world = World.DefaultGameObjectInjectionWorld;
                if (world == null || !world.IsCreated)
                {
                    PublishLocalizedMessage(ParkingStatusLocale.kUnavailable, kUnavailableFallback);
                    return s_Version;
                }

                Game.Simulation.SimulationSystem? simulationSystem =
                    world.GetExistingSystemManaged<Game.Simulation.SimulationSystem>();
                ParkingStatusSystem? statusSystem = world.GetExistingSystemManaged<ParkingStatusSystem>();
                if (simulationSystem == null || statusSystem == null)
                {
                    PublishLocalizedMessage(ParkingStatusLocale.kUnavailable, kUnavailableFallback);
                    return s_Version;
                }

                uint simulationFrame = simulationSystem.frameIndex;
                if (s_RequestPending ||
                    (!s_ForceRefresh &&
                     s_HasRequestedSimulationFrame &&
                     simulationFrame == s_LastRequestedSimulationFrame))
                {
                    return s_Version;
                }

                // Options pauses simulation, so this key cannot trigger another scan during the visit.
                s_ForceRefresh = false;
                s_HasRequestedSimulationFrame = true;
                s_LastRequestedSimulationFrame = simulationFrame;
                s_RequestPending = true;
                if (!s_HasSnapshot)
                {
                    PublishLocalizedMessage(ParkingStatusLocale.kCollecting, kCollectingFallback);
                }

                statusSystem.ScheduleStatus();
            }
            catch
            {
                // The attempted simulation frame remains cached to prevent a retry every UI frame.
                s_RequestPending = false;
                PublishLocalizedMessage(ParkingStatusLocale.kUnavailable, kUnavailableFallback);
            }

            return s_Version;
        }

        /// <summary>
        /// Forces one new snapshot after a setting changes, even while Options remains open.
        /// </summary>
        internal static void MarkDirty()
        {
            s_ForceRefresh = true;
        }

        /// <summary>
        /// Clears status cached for a previous city or game world.
        /// </summary>
        internal static void InvalidateCache()
        {
            s_ForceRefresh = true;
            s_HasRequestedSimulationFrame = false;
            s_HasSnapshot = false;
            s_RequestPending = false;
            s_LastRequestedSimulationFrame = 0;
            s_LastUiFrame = -1;
            s_LastSnapshot = default;
            PublishLocalizedMessage(ParkingStatusLocale.kLoadCity, kLoadCityFallback);
        }

        /// <summary>
        /// Publishes a completed parking snapshot to the Options UI cache.
        /// </summary>
        /// <param name="snapshot">The snapshot built in the scheduled ECS system.</param>
        internal static void Publish(ParkingSnapshot snapshot)
        {
            s_LastSnapshot = snapshot;
            PublishSnapshotRows(snapshot);

            s_HasSnapshot = true;
            s_ForceRefresh = false;
            s_RequestPending = false;
            s_HasRequestedSimulationFrame = true;
            s_LastRequestedSimulationFrame = snapshot.SimulationFrame;
        }

        /// <summary>
        /// Publishes a safe fallback after a scheduled status scan fails.
        /// </summary>
        internal static void PublishFailure()
        {
            s_RequestPending = false;
            PublishLocalizedMessage(ParkingStatusLocale.kCollectionFailed, kCollectionFailedFallback);
        }

        private static void RefreshLocalizedTextIfNeeded()
        {
            string localeId = ParkingStatusLocale.ActiveLocaleId;
            if (string.Equals(s_LastLocaleId, localeId, StringComparison.Ordinal))
            {
                return;
            }

            s_LastLocaleId = localeId;
            if (s_HasSnapshot)
            {
                // Reuse the cached snapshot; changing language must not trigger another ECS scan.
                PublishSnapshotRows(s_LastSnapshot);
                return;
            }

            PublishLocalizedMessage(s_LastMessageId, s_LastMessageFallback);
        }

        private static void PublishSnapshotRows(ParkingSnapshot snapshot)
        {
            bool districtScope =
                snapshot.Scope == PCSettings.ParkingScope.ByDistrict;

            bool wholeCityScope =
                snapshot.Scope == PCSettings.ParkingScope.WholeCity;

            bool scopeEnabled =
                districtScope || wholeCityScope;

            string scopeStatus =
                scopeEnabled
                    ? ParkingStatusSystem.GetOwnershipStatus(
                        true,
                        snapshot.ScopeTargetCurbLanes,
                        snapshot.DisabledScopeTargetCurbLanes,
                        snapshot.TrackedScopeTargetCurbLanes)
                    : "OFF";

            string scopeSuffix =
                string.Equals(scopeStatus, "CHECK", StringComparison.Ordinal)
                    ? " | " +
                        ParkingStatusLocale.Get(ParkingStatusLocale.kStatusCheck, "CHECK")
                    : string.Empty;

            string enforcement;
            if (districtScope)
            {
                enforcement = ParkingStatusLocale.Format(
                    ParkingStatusLocale.kDistrictEnforcementFormat,
                    kDistrictEnforcementFormatFallback,
                    Format(snapshot.ScopeTargetStreetParked),
                    Format(snapshot.DisabledScopeTargetCurbLanes),
                    Format(snapshot.ScopeTargetCurbLanes),
                    Format(snapshot.DistrictsWithPolicy),
                    Format(snapshot.Districts),
                    scopeSuffix);
            }
            else if (wholeCityScope)
            {
                enforcement = ParkingStatusLocale.Format(
                    ParkingStatusLocale.kCompactEnforcementFormat,
                    kEnforcementFormatFallback,
                    Format(snapshot.ScopeTargetStreetParked),
                    Format(snapshot.DisabledScopeTargetCurbLanes),
                    Format(snapshot.ScopeTargetCurbLanes),
                    scopeSuffix);
            }
            else
            {
                enforcement = ParkingStatusLocale.Get(ParkingStatusLocale.kStatusOff, kScopeOffFallback);
            }

            bool manualEnabled =
                snapshot.ManualTargetCurbLanes > 0;

            string manualStatus =
                manualEnabled
                    ? ParkingStatusSystem.GetOwnershipStatus(
                        true,
                        snapshot.ManualTargetCurbLanes,
                        snapshot.DisabledManualTargetCurbLanes,
                        snapshot.TrackedManualTargetCurbLanes)
                    : "OFF";

            string manualSuffix =
                string.Equals(manualStatus, "CHECK", StringComparison.Ordinal)
                    ? " | " +
                        ParkingStatusLocale.Get(ParkingStatusLocale.kStatusCheck, "CHECK")
                    : string.Empty;

            string manual = manualEnabled
                ? ParkingStatusLocale.Format(
                    ParkingStatusLocale.kManualEnforcementFormat,
                    kManualEnforcementFormatFallback,
                    Format(snapshot.ManualTargetStreetParked),
                    Format(snapshot.DisabledManualTargetCurbLanes),
                    Format(snapshot.ManualTargetCurbLanes),
                    manualSuffix)
                : ParkingStatusLocale.Get(ParkingStatusLocale.kManualNone, kManualNoneFallback);

            string vehicles = ParkingStatusLocale.Format(
                ParkingStatusLocale.kVehicleFormat,
                kVehicleFormatFallback,
                Format(snapshot.StreetParked),
                Format(snapshot.VisibleOffStreet),
                Format(snapshot.HiddenInBuildings),
                Format(snapshot.OutsideConnection));

            string publicParkingUse =
                Format(snapshot.OfficialParkingOccupied) + "/" + Format(snapshot.OfficialParkingCapacity);

            string parkingUse = ParkingStatusLocale.Format(
                ParkingStatusLocale.kShareFormat,
                kShareFormatFallback,
                publicParkingUse,
                Format(snapshot.BuildingParkingOccupied),
                Format(snapshot.StreetParked),
                Format(snapshot.KnownInCityParking));

            string publicFree = snapshot.OfficialParkingCapacity > 0
                ? Format(Math.Max(0, snapshot.OfficialParkingCapacity - snapshot.OfficialParkingOccupied))
                : "--";

            string parkingRating = ParkingStatusLocale.Format(
                ParkingStatusLocale.kSupplyFormat,
                kSupplyFormatFallback,
                GetParkingRating(
                    snapshot.OfficialParkingOccupied,
                    snapshot.OfficialParkingCapacity),
                FormatFreePercent(
                    snapshot.OfficialParkingOccupied,
                    snapshot.OfficialParkingCapacity),
                publicFree);

            string updated = snapshot.CapturedAtLocal.ToString(
                "HH:mm:ss",
                CultureInfo.CurrentCulture);

            PublishRows(
                enforcement,
                manual,
                parkingUse,
                parkingRating,
                vehicles,
                updated);
        }

        private static string Format(int value)
        {
            // Keep four-digit values exact; abbreviate only when space savings matter.
            long magnitude = Math.Abs((long)value);
            if (magnitude >= 1_000_000)
            {
                return FormatScaled(value, 1_000_000, "m");
            }

            if (magnitude >= 10_000)
            {
                return FormatScaled(value, 1_000, "k");
            }

            return value.ToString("N0", CultureInfo.CurrentCulture);
        }

        private static string FormatScaled(int value, int divisor, string suffix)
        {
            return (value / (double)divisor).ToString("0.#", CultureInfo.CurrentCulture) + suffix;
        }

        private static string GetParkingRating(int occupied, int capacity)
        {
            if (capacity <= 0)
            {
                return ParkingStatusLocale.Get(ParkingStatusLocale.kRatingNA, "N/A");
            }

            double freePercent =
                Math.Max(0, capacity - occupied) * 100d / capacity;

            if (freePercent < 15d)
            {
                return ParkingStatusLocale.Get(ParkingStatusLocale.kRatingPoor, "POOR");
            }

            if (freePercent < 30d)
            {
                return ParkingStatusLocale.Get(ParkingStatusLocale.kStatusOk, "OK");
            }

            return ParkingStatusLocale.Get(ParkingStatusLocale.kRatingGood, "GOOD");
        }

        private static string FormatFreePercent(int occupied, int capacity)
        {
            if (capacity <= 0)
            {
                return "--";
            }

            double freePercent =
                Math.Max(0, capacity - occupied) * 100d / capacity;

            return freePercent.ToString(
                "0",
                CultureInfo.CurrentCulture) + "%";
        }

        private static void PublishLocalizedMessage(string localeId, string fallback)
        {
            s_LastMessageId = localeId;
            s_LastMessageFallback = fallback;
            PublishMessage(ParkingStatusLocale.Get(localeId, fallback));
        }

        private static void PublishRows(
            string enforcement,
            string manual,
            string parkingUse,
            string parkingRating,
            string vehicles,
            string updated)
        {
            if (EnforcementRow == enforcement &&
                ManualRow == manual &&
                ShareRow == parkingUse &&
                SupplyRow == parkingRating &&
                VehicleRow == vehicles &&
                UpdatedRow == updated)
            {
                return;
            }

            EnforcementRow = enforcement;
            ManualRow = manual;
            ShareRow = parkingUse;
            SupplyRow = parkingRating;
            VehicleRow = vehicles;
            UpdatedRow = updated;
            s_Version++;
        }

        private static void PublishMessage(string message)
        {
            // One temporary message is enough; repeating it in every row clutters Options.
            PublishRows(
                message,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
        }
    }
}

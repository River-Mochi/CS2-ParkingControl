// <copyright file="ParkingStatusCache.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Caches on-demand parking status text for the Options UI.

namespace ParkingControl
{
    using System;
    using System.Globalization;
    using Game;
    using Game.SceneFlow;
    using Unity.Entities;
    using UnityEngine;

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
        private const string kEnforcementFormatFallback =
            "{0} parked ({1} lanes) | {2}/{3} disabled | {4}";
        private const string kVehicleFormatFallback =
            "{0} street | {1} visible | {2} hidden | {3} OC";
        private const string kSupplyFormatFallback =
            "{0}  {1} / {2} public | {3}  {4} / {5} building";
        private const string kShareFormatFallback =
            "{0} street parked | {1} moving | updated {2}";

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
        /// Gets the cached personal-vehicle location summary.
        /// </summary>
        internal static string VehicleRow { get; private set; } = kLoadCityFallback;

        /// <summary>
        /// Gets the cached parking-supply summary.
        /// </summary>
        internal static string SupplyRow { get; private set; } = kLoadCityFallback;

        /// <summary>
        /// Gets the cached street share and update time.
        /// </summary>
        internal static string ShareRow { get; private set; } = kLoadCityFallback;

        /// <summary>
        /// Requests one snapshot after the simulation advances and returns the published UI version.
        /// </summary>
        /// <returns>The version of the currently published strings.</returns>
        public static int GetUiVersion()
        {
            // Four status widgets ask for the version each UI frame; perform the check only once.
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
            bool districtScope = snapshot.Scope == PCSettings.ParkingScope.ByDistrict;
            int parked = districtScope ? snapshot.TargetStreetParked : snapshot.StreetParked;
            int occupiedLanes = districtScope
                ? snapshot.OccupiedTargetCurbLanes
                : snapshot.OccupiedCurbLanes;
            int disabledLanes = districtScope
                ? snapshot.DisabledTargetCurbLanes
                : snapshot.DisabledCurbLanes;
            int targetLanes = districtScope ? snapshot.TargetCurbLanes : snapshot.CurbLanes;
            string status = LocalizeOwnershipStatus(
                ParkingStatusSystem.GetOwnershipStatus(
                    snapshot.RestrictionEnabled,
                    targetLanes,
                    disabledLanes,
                    snapshot.TrackedCurbLanes));

            string enforcement = ParkingStatusLocale.Format(
                ParkingStatusLocale.kEnforcementFormat,
                kEnforcementFormatFallback,
                Format(parked),
                Format(occupiedLanes),
                Format(disabledLanes),
                Format(targetLanes),
                status);
            string vehicles = ParkingStatusLocale.Format(
                ParkingStatusLocale.kVehicleFormat,
                kVehicleFormatFallback,
                Format(snapshot.StreetParked),
                Format(snapshot.VisibleOffStreet),
                Format(snapshot.HiddenInBuildings),
                Format(snapshot.OutsideConnection));
            string supply = ParkingStatusLocale.Format(
                ParkingStatusLocale.kSupplyFormat,
                kSupplyFormatFallback,
                FormatPercent(snapshot.OfficialParkingOccupied, snapshot.OfficialParkingCapacity),
                FormatSupply(snapshot.OfficialParkingOccupied),
                FormatSupply(snapshot.OfficialParkingCapacity),
                FormatPercent(snapshot.BuildingParkingOccupied, snapshot.BuildingParkingCapacity),
                FormatSupply(snapshot.BuildingParkingOccupied),
                FormatSupply(snapshot.BuildingParkingCapacity));
            string share = ParkingStatusLocale.Format(
                ParkingStatusLocale.kShareFormat,
                kShareFormatFallback,
                FormatPercent(snapshot.StreetParked, snapshot.KnownInCityParking),
                Format(snapshot.ActiveVehicles),
                snapshot.CapturedAtLocal.ToString("HH:mm:ss", CultureInfo.CurrentCulture));

            PublishRows(enforcement, vehicles, supply, share);
        }

        private static string LocalizeOwnershipStatus(string status)
        {
            return status switch
            {
                "OK" => ParkingStatusLocale.Get(ParkingStatusLocale.kStatusOk, "OK"),
                "OFF" => ParkingStatusLocale.Get(ParkingStatusLocale.kStatusOff, "OFF"),
                "CHECK" => ParkingStatusLocale.Get(ParkingStatusLocale.kStatusCheck, "CHECK"),
                _ => status,
            };
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

        private static string FormatSupply(int value)
        {
            long magnitude = Math.Abs((long)value);
            if (magnitude >= 1_000_000)
            {
                return FormatTruncated(value, 1_000_000, "m");
            }

            if (magnitude >= 1_000)
            {
                // The Options row is deliberately approximate; the manual log keeps exact totals.
                return FormatTruncated(value, 1_000, "k");
            }

            return value.ToString("N0", CultureInfo.CurrentCulture);
        }

        private static string FormatTruncated(int value, int divisor, string suffix)
        {
            double scaled = Math.Truncate(value * 10d / divisor) / 10d;
            return scaled.ToString("0.0", CultureInfo.CurrentCulture) + suffix;
        }

        private static string FormatPercent(int numerator, int denominator)
        {
            if (denominator <= 0)
            {
                return "0.0%";
            }

            double value = numerator * 100d / denominator;
            return value.ToString("0.0", CultureInfo.CurrentCulture) + "%";
        }

        private static void PublishLocalizedMessage(string localeId, string fallback)
        {
            s_LastMessageId = localeId;
            s_LastMessageFallback = fallback;
            PublishMessage(ParkingStatusLocale.Get(localeId, fallback));
        }

        private static void PublishRows(
            string enforcement,
            string vehicles,
            string supply,
            string share)
        {
            if (EnforcementRow == enforcement &&
                VehicleRow == vehicles &&
                SupplyRow == supply &&
                ShareRow == share)
            {
                return;
            }

            EnforcementRow = enforcement;
            VehicleRow = vehicles;
            SupplyRow = supply;
            ShareRow = share;
            s_Version++;
        }

        private static void PublishMessage(string message)
        {
            // One temporary message is enough; repeating it in all four rows clutters Options.
            PublishRows(message, string.Empty, string.Empty, string.Empty);
        }
    }
}

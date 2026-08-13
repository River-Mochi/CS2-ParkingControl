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
        private const string kLoadCity = "Load or start a city to view parking status.";
        private const string kPending = "Parking status is being collected...";

        private static bool s_ForceRefresh = true;
        private static bool s_HasRequestedSimulationFrame;
        private static bool s_HasSnapshot;
        private static bool s_RequestPending;
        private static bool s_WasInGame;
        private static int s_LastUiFrame = -1;
        private static uint s_LastRequestedSimulationFrame;
        private static int s_Version;

        /// <summary>
        /// Gets the cached enforcement summary.
        /// </summary>
        internal static string EnforcementRow { get; private set; } = kLoadCity;

        /// <summary>
        /// Gets the cached personal-vehicle location summary.
        /// </summary>
        internal static string VehicleRow { get; private set; } = kLoadCity;

        /// <summary>
        /// Gets the cached parking-supply summary.
        /// </summary>
        internal static string SupplyRow { get; private set; } = kLoadCity;

        /// <summary>
        /// Gets the cached street share and update time.
        /// </summary>
        internal static string ShareRow { get; private set; } = kLoadCity;

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

            if (!isGame)
            {
                PublishMessage(kLoadCity);
                return s_Version;
            }

            try
            {
                World? world = World.DefaultGameObjectInjectionWorld;
                if (world == null || !world.IsCreated)
                {
                    PublishMessage("Parking status is unavailable.");
                    return s_Version;
                }

                Game.Simulation.SimulationSystem? simulationSystem =
                    world.GetExistingSystemManaged<Game.Simulation.SimulationSystem>();
                ParkingStatusSystem? statusSystem = world.GetExistingSystemManaged<ParkingStatusSystem>();
                if (simulationSystem == null || statusSystem == null)
                {
                    PublishMessage("Parking status is unavailable.");
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
                    PublishMessage(kPending);
                }

                statusSystem.ScheduleStatus();
            }
            catch
            {
                // The attempted simulation frame remains cached to prevent a retry every UI frame.
                s_RequestPending = false;
                PublishMessage("Parking status is unavailable.");
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
            PublishMessage(kLoadCity);
        }

        /// <summary>
        /// Publishes a completed parking snapshot to the Options UI cache.
        /// </summary>
        /// <param name="snapshot">The snapshot built in the scheduled ECS system.</param>
        internal static void Publish(ParkingSnapshot snapshot)
        {
            string status = ParkingStatusSystem.GetOwnershipStatus(
                snapshot.RestrictionEnabled,
                snapshot.CurbLanes,
                snapshot.DisabledCurbLanes,
                snapshot.TrackedCurbLanes);
            EnforcementRow =
                $"{Format(snapshot.DisabledCurbLanes)}/{Format(snapshot.CurbLanes)} lanes disabled | {status}";
            VehicleRow =
                $"{Format(snapshot.StreetParked)} street | {Format(snapshot.VisibleOffStreet)} visible | " +
                $"{Format(snapshot.HiddenInBuildings)} hidden | {Format(snapshot.OutsideConnection)} outside";
            SupplyRow =
                $"{Format(snapshot.OfficialParkingOccupied)}/{Format(snapshot.OfficialParkingCapacity)} public | " +
                $"{Format(snapshot.GarageOccupied)}/{Format(snapshot.GarageCapacity)} garages";
            ShareRow =
                $"{FormatPercent(snapshot.StreetParked, snapshot.ParkedVehicles)} street | " +
                $"{Format(snapshot.ActiveVehicles)} active | {snapshot.CapturedAtLocal:HH:mm:ss}";

            s_HasSnapshot = true;
            s_ForceRefresh = false;
            s_RequestPending = false;
            s_HasRequestedSimulationFrame = true;
            s_LastRequestedSimulationFrame = snapshot.SimulationFrame;
            s_Version++;
        }

        /// <summary>
        /// Publishes a safe fallback after a scheduled status scan fails.
        /// </summary>
        internal static void PublishFailure()
        {
            s_RequestPending = false;
            PublishMessage("Parking status could not be collected; see ParkingControl.log.");
        }

        private static string Format(int value)
        {
            return value.ToString("N0", CultureInfo.CurrentCulture);
        }

        private static string FormatPercent(int numerator, int denominator)
        {
            if (denominator <= 0)
            {
                return "0.00%";
            }

            double value = numerator * 100d / denominator;
            return value.ToString("0.00", CultureInfo.CurrentCulture) + "%";
        }

        private static void PublishMessage(string message)
        {
            if (EnforcementRow == message &&
                VehicleRow == message &&
                SupplyRow == message &&
                ShareRow == message)
            {
                return;
            }

            EnforcementRow = message;
            VehicleRow = message;
            SupplyRow = message;
            ShareRow = message;
            s_Version++;
        }
    }
}

// <copyright file="ParkingStatusLocale.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Defines localization IDs and safe lookups for Parking Control status text.

namespace ParkingControl
{
    using System;
    using System.Globalization;
    using Colossal.Localization;
    using Game.SceneFlow;

    /// <summary>
    /// Supplies localized strings used by the live parking status rows.
    /// </summary>
    internal static class ParkingStatusLocale
    {
        internal const string kLoadCity = Mod.ModId + ".Status.LoadCity";
        internal const string kCollecting = Mod.ModId + ".Status.Collecting";
        internal const string kUnavailable = Mod.ModId + ".Status.Unavailable";
        internal const string kCollectionFailed = Mod.ModId + ".Status.CollectionFailed";
        internal const string kEnforcementFormat = Mod.ModId + ".Status.EnforcementFormat";
        internal const string kCompactEnforcementFormat = Mod.ModId + ".Status.CompactEnforcementFormat";
        internal const string kDistrictEnforcementFormat = Mod.ModId + ".Status.DistrictEnforcementFormat";
        internal const string kVehicleFormat = Mod.ModId + ".Status.VehicleFormat";
        internal const string kSupplyFormat = Mod.ModId + ".Status.SupplyFormat";
        internal const string kShareFormat = Mod.ModId + ".Status.ShareFormat";
        internal const string kStatusOk = Mod.ModId + ".Status.OK";
        internal const string kStatusOff = Mod.ModId + ".Status.OFF";
        internal const string kStatusCheck = Mod.ModId + ".Status.CHECK";

        /// <summary>
        /// Gets the currently active game locale ID.
        /// </summary>
        internal static string ActiveLocaleId
        {
            get
            {
                LocalizationManager? manager = GameManager.instance?.localizationManager;
                return manager?.activeLocaleId ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets one localized value, falling back to English if localization is unavailable.
        /// </summary>
        internal static string Get(string localeId, string fallback)
        {
            LocalizationManager? manager = GameManager.instance?.localizationManager;
            if (manager != null &&
                manager.activeDictionary.TryGetValue(localeId, out string value))
            {
                return value;
            }

            return fallback;
        }

        /// <summary>
        /// Formats one localized status row and safely falls back to its English format.
        /// </summary>
        internal static string Format(string localeId, string fallback, params object[] values)
        {
            string format = Get(localeId, fallback);
            try
            {
                return string.Format(CultureInfo.CurrentCulture, format, values);
            }
            catch (FormatException)
            {
                return string.Format(CultureInfo.CurrentCulture, fallback, values);
            }
        }
    }
}

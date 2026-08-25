// <copyright file="ParkingStatusLocale.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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
        internal const string kCompactEnforcementFormat = Mod.ModId + ".Status.CompactEnforcementFormat";
        internal const string kDistrictEnforcementFormat = Mod.ModId + ".Status.DistrictEnforcementFormat";
        internal const string kVehicleFormat = Mod.ModId + ".Status.VehicleFormat";
        internal const string kSupplyFormat = Mod.ModId + ".Status.SupplyFormat";
        internal const string kShareFormat = Mod.ModId + ".Status.ShareFormat";
        internal const string kStatusOk = Mod.ModId + ".Status.OK";
        internal const string kStatusOff = Mod.ModId + ".Status.OFF";
        internal const string kStatusCheck = Mod.ModId + ".Status.CHECK";
        internal const string kRatingPoor = Mod.ModId + ".Status.RatingPoor";
        internal const string kRatingGood = Mod.ModId + ".Status.RatingGood";
        internal const string kRatingNA = Mod.ModId + ".Status.RatingNA";

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

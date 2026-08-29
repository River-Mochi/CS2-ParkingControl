// <copyright file="ParkingStatusSystem.ScopeManualReport.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Adds a compact scope/manual target split beside the detailed parking report.

using CS2Shared.RiverMochi;

namespace ParkingControl
{

    public sealed partial class ParkingStatusSystem
    {
        private static void WriteScopeManualReportSummary(ParkingSnapshot snapshot)
        {
            LogUtils.Info(
                $"{Mod.ModTag} Parking target split: " +
                $"Scope={snapshot.ScopeTargetStreetParked} parked, " +
                $"{snapshot.DisabledScopeTargetCurbLanes}/{snapshot.ScopeTargetCurbLanes} disabled | " +
                $"Manual={snapshot.ManualTargetStreetParked} parked, " +
                $"{snapshot.DisabledManualTargetCurbLanes}/{snapshot.ManualTargetCurbLanes} disabled | " +
                $"Combined={snapshot.TargetStreetParked} parked, " +
                $"{snapshot.DisabledTargetCurbLanes}/{snapshot.TargetCurbLanes} disabled " +
                "(Scope and Manual can overlap).");
        }
    }
}

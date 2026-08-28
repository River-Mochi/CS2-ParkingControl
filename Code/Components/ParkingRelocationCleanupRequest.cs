// <copyright file="ParkingRelocationCleanupRequest.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Retains a newly banned curb lane for one delayed, targeted relocation retry.

namespace ParkingControl
{
    using Unity.Entities;

    /// <summary>
    /// One-shot marker consumed by the delayed cleanup in
    /// <see cref="ParkingRelocationSystem"/>.
    /// </summary>
    public struct ParkingRelocationCleanupRequest : IComponentData, IQueryTypeParameter
    {
    }
}

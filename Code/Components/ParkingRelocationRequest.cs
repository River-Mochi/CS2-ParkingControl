// <copyright file="ParkingRelocationRequest.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Marks a newly banned curb lane whose existing parked cars should be queued once.
    using Unity.Entities;

namespace ParkingControl
{


    /// <summary>
    /// One-shot marker consumed by <see cref="ParkingRelocationSystem"/>.
    /// </summary>
    public struct ParkingRelocationRequest : IComponentData, IQueryTypeParameter
    {
    }
}

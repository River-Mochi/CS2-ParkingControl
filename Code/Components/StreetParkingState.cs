// <copyright file="StreetParkingState.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Marks parking lanes whose vanilla flag was changed by Parking Control.

using Unity.Entities;

namespace ParkingControl
{

    /// <summary>
    /// Non-serialized marker added only when this mod changes an enabled curb lane.
    /// </summary>
    public struct StreetParkingState : IComponentData, IQueryTypeParameter
    {
    }
}

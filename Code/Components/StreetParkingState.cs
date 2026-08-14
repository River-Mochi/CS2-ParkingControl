// <copyright file="StreetParkingState.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Marks parking lanes whose vanilla flag was changed by Parking Control.

namespace ParkingControl
{
    using Unity.Entities;

    /// <summary>
    /// Non-serialized marker added only when this mod changes an enabled curb lane.
    /// </summary>
    public struct StreetParkingState : IComponentData, IQueryTypeParameter
    {
    }
}

// <copyright file="NoStreetParkingDistrict.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Persists a district's Roadside Parking Ban without saving the runtime policy prefab.

namespace ParkingControl
{
    using Colossal.Serialization.Entities;
    using Unity.Entities;

    /// <summary>
    /// Serializable marker stored on districts that have Roadside Parking Ban enabled.
    /// Without this mod, CS2 treats the component type as obsolete and omits it from the next save.
    /// </summary>
    public struct NoStreetParkingDistrict :
        IComponentData,
        IQueryTypeParameter,
        IEmptySerializable
    {
    }
}

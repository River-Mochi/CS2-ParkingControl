// <copyright file="NoStreetParkingDistrict.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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

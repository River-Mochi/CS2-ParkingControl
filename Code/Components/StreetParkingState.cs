// <copyright file="StreetParkingState.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Remembers the original parking-disabled state of lanes changed by this mod.

namespace ParkingControl
{
    using Colossal.Serialization.Entities;
    using Unity.Entities;

/// <summary>
/// Stores the value that <see cref="Game.Net.ParkingLaneFlags.ParkingDisabled"/> had before Parking Control changed a lane.
/// </summary>
    public struct StreetParkingState : IComponentData, IQueryTypeParameter, ISerializable
    {
    /// <summary>
    /// The original value of the parking-disabled flag.
    /// </summary>
        public bool m_WasParkingDisabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreetParkingState"/> struct.
    /// </summary>
    /// <param name="wasParkingDisabled">The parking-disabled value before this mod changed the lane.</param>
        public StreetParkingState(bool wasParkingDisabled)
        {
            m_WasParkingDisabled = wasParkingDisabled;
        }

    /// <inheritdoc/>
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(m_WasParkingDisabled);
        }

    /// <inheritdoc/>
        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out m_WasParkingDisabled);
        }
    }
}

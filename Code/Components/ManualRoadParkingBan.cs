// <copyright file="ManualRoadParkingBan.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Persists player-selected left/right roadside parking bans on individual roads.

namespace ParkingControl
{
    using System;
    using Colossal.Serialization.Entities;
    using Unity.Entities;

    /// <summary>
    /// Identifies road sides with a manual No Parking override.
    /// </summary>
    [Flags]
    public enum ManualParkingSides : uint
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

    /// <summary>
    /// Serializable per-road state for the Roads Services No Parking tool.
    /// </summary>
    public struct ManualRoadParkingBan :
        IComponentData,
        IQueryTypeParameter,
        ISerializable
    {
        public ManualParkingSides m_Sides;

        /// <summary>
        /// Gets whether the requested side has a manual parking ban.
        /// </summary>
        public readonly bool IsBanned(bool rightSide)
        {
            ManualParkingSides side = rightSide
                ? ManualParkingSides.Right
                : ManualParkingSides.Left;

            return (m_Sides & side) != 0;
        }

        /// <summary>
        /// Adds or removes a manual parking ban on one side.
        /// </summary>
        public void SetBanned(bool rightSide, bool banned)
        {
            ManualParkingSides side = rightSide
                ? ManualParkingSides.Right
                : ManualParkingSides.Left;

            if (banned)
            {
                m_Sides |= side;
            }
            else
            {
                m_Sides &= ~side;
            }
        }

        /// <summary>
        /// Gets whether neither side has a manual ban.
        /// </summary>
        public readonly bool IsEmpty => m_Sides == ManualParkingSides.None;

        /// <inheritdoc/>
        public readonly void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write((uint)m_Sides);
        }

        /// <inheritdoc/>
        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out uint value);
            m_Sides = (ManualParkingSides)value;
        }
    }
}

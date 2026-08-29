// <copyright file="ParkingStatusSystem.ReportSamples.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Collects bounded report samples and writes Scene Explorer transition details.

    using System.Collections.Generic;
    using System.Text;
    using Unity.Entities;

namespace ParkingControl
{

    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Records one vehicle in bounded manual-report samples.
        /// </summary>
        private void AddReportVehicle(
            ParkingReportDetails details,
            Entity vehicle,
            VehicleLocation location,
            bool isParked)
        {
            RecordSampleTransition(details, vehicle, location);

            if (location == VehicleLocation.StreetCurb)
            {
                AddBoundedSample(details.StreetSamples, vehicle);
            }

            // These samples are capped while scanning; large cities never build a
            // thousands-entry list merely to print a few Scene Explorer IDs.
            switch (location)
            {
                case VehicleLocation.VisibleOffStreet:
                    AddBoundedSample(details.VisibleSamples, vehicle);
                    break;
                case VehicleLocation.HiddenInBuilding:
                    AddBoundedSample(details.HiddenSamples, vehicle);
                    break;
                case VehicleLocation.OutsideConnection:
                    AddBoundedSample(details.OutsideSamples, vehicle);
                    break;
                default:
                    if (isParked && location != VehicleLocation.StreetCurb)
                    {
                        AddBoundedSample(details.UnknownSamples, vehicle);
                    }

                    break;
            }
        }

        private static void AddBoundedSample(List<Entity> samples, Entity vehicle)
        {
            if (samples.Count < kVehicleSampleLimit)
            {
                samples.Add(vehicle);
            }
        }

        private void RecordSampleTransition(
            ParkingReportDetails details,
            Entity vehicle,
            VehicleLocation currentLocation)
        {
            if (!m_HasPreviousReport)
            {
                return;
            }

            if (m_PreviousStreetSamples.Contains(vehicle))
            {
                details.SampleTransitions.Add(new VehicleSampleTransition(
                    vehicle,
                    VehicleSampleSource.Street,
                    currentLocation));
            }
            else if (m_PreviousOutsideSamples.Contains(vehicle))
            {
                details.SampleTransitions.Add(new VehicleSampleTransition(
                    vehicle,
                    VehicleSampleSource.OutsideConnection,
                    currentLocation));
            }
            else if (m_PreviousUnknownSamples.Contains(vehicle))
            {
                details.SampleTransitions.Add(new VehicleSampleTransition(
                    vehicle,
                    VehicleSampleSource.Unknown,
                    currentLocation));
            }
        }


        /// <summary>
        /// Lists a small sample from each parked location for Scene Explorer inspection.
        /// </summary>
        private static void AppendVehicleEntitySamples(
            StringBuilder text,
            ParkingSnapshot snapshot,
            ParkingReportDetails details)
        {
            text.AppendLine(
                $"Samples=<up to {kVehicleSampleLimit} per parked location; " +
                "enter Index:Version in Scene Explorer>");
            AppendEntitySampleLine(text, "Street", snapshot.StreetParked, details.StreetSamples);
            AppendEntitySampleLine(text, "Visible", snapshot.VisibleOffStreet, details.VisibleSamples);
            AppendEntitySampleLine(text, "Hidden", snapshot.HiddenInBuildings, details.HiddenSamples);
            AppendEntitySampleLine(text, "Outside", snapshot.OutsideConnection, details.OutsideSamples);
            AppendEntitySampleLine(
                text,
                "Unknown",
                snapshot.UnassignedOrUnknownParked,
                details.UnknownSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-OCSource",
                snapshot.UnknownTripSourceOutside,
                details.UnknownOutsideSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-BuildingSource",
                snapshot.UnknownTripSourceBuilding,
                details.UnknownBuildingSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-OtherSource",
                snapshot.UnknownTripSourceOther,
                details.UnknownOtherSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-MissingSource",
                snapshot.UnknownTripSourceMissing,
                details.UnknownMissingSourceSamples);
            AppendEntitySampleLine(
                text,
                "Unknown-NoTripSource",
                snapshot.UnknownWithoutTripSource,
                details.UnknownNoSourceSamples);
        }

        private static void AppendEntitySampleLine(
            StringBuilder text,
            string label,
            int total,
            List<Entity> vehicles)
        {
            vehicles.Sort(CompareEntities);
            text.Append($"  {label}={total} total; samples=");
            if (vehicles.Count == 0)
            {
                text.AppendLine("<none>");
                return;
            }

            for (int i = 0; i < vehicles.Count; i++)
            {
                if (i != 0)
                {
                    text.Append(", ");
                }

                text.Append(FormatEntity(vehicles[i]));
            }

            text.AppendLine();
        }

        private static int CompareEntities(Entity left, Entity right)
        {
            int indexOrder = left.Index.CompareTo(right.Index);
            return indexOrder != 0
                ? indexOrder
                : left.Version.CompareTo(right.Version);
        }

        private void AppendSampleTransitions(StringBuilder text, ParkingReportDetails details)
        {
            if (!m_HasPreviousReport)
            {
                text.AppendLine(
                    $"SampleTransitions=<first report; storing up to {kVehicleSampleLimit} " +
                    "Street, OutsideConnection, and Unknown Entity IDs for the next report>");
                return;
            }

            text.AppendLine(
                $"SampleTransitions=<up to {kVehicleSampleLimit} sampled IDs per group; " +
                "these results do not cover every vehicle>");
            AppendSampleTransitionGroup(
                text,
                "Street",
                VehicleSampleSource.Street,
                m_PreviousStreetSamples,
                details);
            AppendSampleTransitionGroup(
                text,
                "OutsideConnection",
                VehicleSampleSource.OutsideConnection,
                m_PreviousOutsideSamples,
                details);
            AppendSampleTransitionGroup(
                text,
                "Unknown",
                VehicleSampleSource.Unknown,
                m_PreviousUnknownSamples,
                details);
        }

        private void AppendSampleTransitionGroup(
            StringBuilder text,
            string label,
            VehicleSampleSource source,
            List<Entity> previousSamples,
            ParkingReportDetails details)
        {
            int[] counts = new int[8];
            foreach (Entity vehicle in previousSamples)
            {
                counts[(int)GetCurrentSampleState(vehicle, source, details)]++;
            }

            text.AppendLine(
                $"Previous{label}Samples={previousSamples.Count} | " +
                $"Street={counts[(int)CurrentSampleState.Street]} | " +
                $"Active={counts[(int)CurrentSampleState.Active]} | " +
                $"Visible={counts[(int)CurrentSampleState.Visible]} | " +
                $"Hidden={counts[(int)CurrentSampleState.Hidden]} | " +
                $"Outside={counts[(int)CurrentSampleState.Outside]} | " +
                $"Unknown={counts[(int)CurrentSampleState.Unknown]} | " +
                $"NoLongerTracked={counts[(int)CurrentSampleState.NoLongerTracked]} | " +
                $"NoLongerExists={counts[(int)CurrentSampleState.NoLongerExists]}");

            foreach (Entity vehicle in previousSamples)
            {
                CurrentSampleState current = GetCurrentSampleState(vehicle, source, details);
                text.AppendLine(
                    $"  {FormatEntity(vehicle)}: {label} -> {FormatSampleState(current)}");
            }
        }

        private CurrentSampleState GetCurrentSampleState(
            Entity vehicle,
            VehicleSampleSource source,
            ParkingReportDetails details)
        {
            foreach (VehicleSampleTransition transition in details.SampleTransitions)
            {
                if (transition.Vehicle == vehicle && transition.Source == source)
                {
                    return transition.CurrentLocation switch
                    {
                        VehicleLocation.Active => CurrentSampleState.Active,
                        VehicleLocation.StreetCurb => CurrentSampleState.Street,
                        VehicleLocation.VisibleOffStreet => CurrentSampleState.Visible,
                        VehicleLocation.HiddenInBuilding => CurrentSampleState.Hidden,
                        VehicleLocation.OutsideConnection => CurrentSampleState.Outside,
                        _ => CurrentSampleState.Unknown,
                    };
                }
            }

            return EntityManager.Exists(vehicle)
                ? CurrentSampleState.NoLongerTracked
                : CurrentSampleState.NoLongerExists;
        }

        private static string FormatSampleState(CurrentSampleState state)
        {
            return state switch
            {
                CurrentSampleState.NoLongerTracked => "No longer tracked as a personal vehicle",
                CurrentSampleState.NoLongerExists => "No longer exists",
                _ => state.ToString(),
            };
        }

        private enum CurrentSampleState
        {
            Street,
            Active,
            Visible,
            Hidden,
            Outside,
            Unknown,
            NoLongerTracked,
            NoLongerExists,
        }

    }
}

// <copyright file="ParkingStatusSystem.ParkingFeeDebug.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Read-only diagnostics for the vanilla district Roadside Parking Fee slider.

namespace ParkingControl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using CS2Shared.RiverMochi;
    using Game.Areas;
    using Game.Policies;
    using Game.Prefabs;
    using Unity.Collections;
    using Unity.Entities;

    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Logs the vanilla Parking Fee policy metadata and each district's stored value.
        /// </summary>
        private void WriteParkingFeePolicyDiagnostic()
        {
            BufferLookup<DistrictModifierData> modifierLookup =
                SystemAPI.GetBufferLookup<DistrictModifierData>(true);

            BufferLookup<Policy> policyLookup =
                SystemAPI.GetBufferLookup<Policy>(true);

            List<Entity> parkingFeePolicies = new();

            using (NativeArray<Entity> policyEntities =
                m_DistrictPolicyPrefabQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity policyEntity in policyEntities)
                {
                    if (!modifierLookup.TryGetBuffer(
                            policyEntity,
                            out DynamicBuffer<DistrictModifierData> modifiers))
                    {
                        continue;
                    }

                    foreach (DistrictModifierData modifier in modifiers)
                    {
                        if (modifier.m_Type == DistrictModifierType.ParkingFee)
                        {
                            parkingFeePolicies.Add(policyEntity);
                            break;
                        }
                    }
                }
            }

            StringBuilder text = new(2048);
            text.AppendLine();
            text.AppendLine(
                $"==================== {Mod.ModTag} VANILLA PARKING FEE DIAGNOSTIC ====================");
            text.AppendLine(
                $"ParkingFeePolicyCandidates={parkingFeePolicies.Count}");

            if (parkingFeePolicies.Count == 0)
            {
                text.AppendLine(
                    "Result=CHECK | No district policy with DistrictModifierType.ParkingFee was found.");
                text.Append(
                    $"==================== {Mod.ModTag} END PARKING FEE DIAGNOSTIC ====================");
                LogUtils.Info(text.ToString());
                return;
            }

            List<Entity> districts = new();
            using (NativeArray<Entity> districtEntities =
                m_DistrictQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity district in districtEntities)
                {
                    districts.Add(district);
                }
            }

            districts.Sort((left, right) => string.Compare(
                GetDistrictName(left),
                GetDistrictName(right),
                StringComparison.CurrentCultureIgnoreCase));

            for (int policyIndex = 0;
                policyIndex < parkingFeePolicies.Count;
                policyIndex++)
            {
                Entity policyEntity = parkingFeePolicies[policyIndex];

                bool hasSliderData =
                    EntityManager.HasComponent<PolicySliderData>(policyEntity);

                PolicySliderData sliderData = hasSliderData
                    ? EntityManager.GetComponentData<PolicySliderData>(policyEntity)
                    : default;

                text.AppendLine();
                text.AppendLine(
                    $"Policy[{policyIndex + 1}]={FormatEntity(policyEntity)} | " +
                    $"PolicySliderData={(hasSliderData ? "YES" : "NO")} | " +
                    $"FrontendDataExpected={(hasSliderData ? "SLIDER" : "NULL")}");

                if (hasSliderData)
                {
                    bool rangeValid =
                        sliderData.m_Range.max > sliderData.m_Range.min;

                    text.AppendLine(
                        $"  SliderRange={FormatDiagnosticFloat(sliderData.m_Range.min)}.." +
                        $"{FormatDiagnosticFloat(sliderData.m_Range.max)} | " +
                        $"Default={FormatDiagnosticFloat(sliderData.m_Default)} | " +
                        $"Step={FormatDiagnosticFloat(sliderData.m_Step)} | " +
                        $"Unit={sliderData.m_Unit} | " +
                        $"RangeValid={(rangeValid ? "YES" : "NO")}");
                }

                text.AppendLine(
                    "  District values (UIValue mirrors vanilla PoliciesUISystem: first stored entry, otherwise slider default):");

                foreach (Entity district in districts)
                {
                    bool hasStoredEntry = false;
                    int entryCount = 0;
                    Policy firstEntry = default;

                    if (policyLookup.TryGetBuffer(
                            district,
                            out DynamicBuffer<Policy> policies))
                    {
                        foreach (Policy policy in policies)
                        {
                            if (policy.m_Policy != policyEntity)
                            {
                                continue;
                            }

                            entryCount++;
                            if (!hasStoredEntry)
                            {
                                firstEntry = policy;
                                hasStoredEntry = true;
                            }
                        }
                    }

                    bool active =
                        hasStoredEntry &&
                        (firstEntry.m_Flags & PolicyFlags.Active) != 0;

                    float uiValue = hasStoredEntry
                        ? firstEntry.m_Adjustment
                        : hasSliderData
                            ? sliderData.m_Default
                            : 0f;

                    string inRange = "N/A";
                    if (hasSliderData)
                    {
                        bool valueInRange =
                            uiValue >= sliderData.m_Range.min &&
                            uiValue <= sliderData.m_Range.max;

                        inRange = valueInRange ? "YES" : "NO";
                    }

                    text.AppendLine(
                        $"    {GetDistrictName(district)} [{FormatEntity(district)}] | " +
                        $"StoredEntry={(hasStoredEntry ? "YES" : "NO")} | " +
                        $"EntryCount={entryCount} | " +
                        $"Active={(active ? "YES" : "NO")} | " +
                        $"StoredAdjustment={(hasStoredEntry ? FormatDiagnosticFloat(firstEntry.m_Adjustment) : "<none>")} | " +
                        $"UIValue={FormatDiagnosticFloat(uiValue)} | " +
                        $"InRange={inRange}" +
                        (entryCount > 1 ? " | DUPLICATE" : string.Empty));
                }
            }

            text.AppendLine();
            text.AppendLine(
                "Interpretation: PolicySliderData=YES means vanilla C# should send slider metadata to the UI. " +
                "If the district panel still shows no slider, compare its UIValue/Range and Player.log.");
            text.Append(
                $"==================== {Mod.ModTag} END PARKING FEE DIAGNOSTIC ====================");

            LogUtils.Info(text.ToString());
        }

        private static string FormatDiagnosticFloat(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}

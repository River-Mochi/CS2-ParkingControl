// <copyright file="ParkingFeeDebug.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Read-only debug output for the vanilla district Roadside Parking Fee slider.

    using CS2Shared.RiverMochi;
    using Unity.Entities;

namespace ParkingControl
{
    public sealed partial class ParkingStatusSystem
    {
        /// <summary>
        /// Logs the vanilla Parking Fee policy metadata and each district's stored value.
        /// </summary>
        private void WriteParkingFeeDebug()
        {
            // SystemAPI methods are source-generated. Keep every non-BCL type in this
            // method fully qualified so the generated partial does not depend on file usings.
            Unity.Entities.BufferLookup<Game.Prefabs.DistrictModifierData> modifierLookup =
                SystemAPI.GetBufferLookup<Game.Prefabs.DistrictModifierData>(true);
            Unity.Entities.BufferLookup<Game.Policies.Policy> policyLookup =
                SystemAPI.GetBufferLookup<Game.Policies.Policy>(true);
            Unity.Entities.ComponentLookup<Game.Prefabs.PolicySliderData> sliderLookup =
                SystemAPI.GetComponentLookup<Game.Prefabs.PolicySliderData>(true);

            System.Collections.Generic.List<Unity.Entities.Entity> parkingFeePolicies = new();

            using (Unity.Collections.NativeArray<Unity.Entities.Entity> policyEntities =
                m_DistrictPolicyPrefabQuery.ToEntityArray(Unity.Collections.Allocator.Temp))
            {
                foreach (Unity.Entities.Entity policyEntity in policyEntities)
                {
                    if (!modifierLookup.TryGetBuffer(
                            policyEntity,
                            out Unity.Entities.DynamicBuffer<Game.Prefabs.DistrictModifierData> modifiers))
                    {
                        continue;
                    }

                    foreach (Game.Prefabs.DistrictModifierData modifier in modifiers)
                    {
                        if (modifier.m_Type == Game.Areas.DistrictModifierType.ParkingFee)
                        {
                            parkingFeePolicies.Add(policyEntity);
                            break;
                        }
                    }
                }
            }

            Game.UI.InGame.SelectedInfoUISystem selectedInfo =
                World.GetExistingSystemManaged<Game.UI.InGame.SelectedInfoUISystem>();
            Unity.Entities.Entity selectedEntity =
                selectedInfo?.selectedEntity ?? Unity.Entities.Entity.Null;

            bool selectedExists =
                selectedEntity != Unity.Entities.Entity.Null &&
                SystemAPI.Exists(selectedEntity);
            bool selectedIsDistrict =
                selectedExists &&
                SystemAPI.HasComponent<Game.Areas.District>(selectedEntity);

            System.Text.StringBuilder text = new(3072);
            text.AppendLine();
            text.AppendLine(
                $"==================== {Mod.ModTag} VANILLA PARKING FEE DEBUG ====================");
            text.AppendLine($"ParkingFeePolicyCandidates={parkingFeePolicies.Count}");
            text.AppendLine(
                $"SelectedEntity={FormatEntity(selectedEntity)} | " +
                $"SelectedDistrict={(selectedIsDistrict ? GetDistrictName(selectedEntity) : "NO")}");

            if (parkingFeePolicies.Count == 0)
            {
                text.AppendLine(
                    "Result=CHECK | No district policy with ParkingFee modifier was found.");
                text.Append(
                    $"==================== {Mod.ModTag} END PARKING FEE DEBUG ====================");
                LogUtils.Info(text.ToString());
                return;
            }

            System.Collections.Generic.List<Unity.Entities.Entity> districts = new();

            using (Unity.Collections.NativeArray<Unity.Entities.Entity> districtEntities =
                m_DistrictQuery.ToEntityArray(Unity.Collections.Allocator.Temp))
            {
                foreach (Unity.Entities.Entity district in districtEntities)
                {
                    districts.Add(district);
                }
            }

            districts.Sort((left, right) => string.Compare(
                GetDistrictName(left),
                GetDistrictName(right),
                System.StringComparison.CurrentCultureIgnoreCase));

            int invalidDistrictValues = 0;

            for (int policyIndex = 0;
                policyIndex < parkingFeePolicies.Count;
                policyIndex++)
            {
                Unity.Entities.Entity policyEntity = parkingFeePolicies[policyIndex];

                bool hasSliderData = sliderLookup.HasComponent(policyEntity);

                Game.Prefabs.PolicySliderData sliderData = hasSliderData
                    ? sliderLookup[policyEntity]
                    : default;

                text.AppendLine();
                text.AppendLine(
                    $"Policy[{policyIndex + 1}]={FormatEntity(policyEntity)} | " +
                    $"PolicySliderData={(hasSliderData ? "YES" : "NO")} | " +
                    $"FrontendDataExpected={(hasSliderData ? "SLIDER" : "NULL")}");

                if (hasSliderData)
                {
                    bool rangeValid = sliderData.m_Range.max > sliderData.m_Range.min;

                    text.AppendLine(
                        $"  SliderRange={FormatDebugFloat(sliderData.m_Range.min)}.." +
                        $"{FormatDebugFloat(sliderData.m_Range.max)} | " +
                        $"Default={FormatDebugFloat(sliderData.m_Default)} | " +
                        $"Step={FormatDebugFloat(sliderData.m_Step)} | " +
                        $"Unit={sliderData.m_Unit} | " +
                        $"RangeValid={(rangeValid ? "YES" : "NO")}");
                }

                text.AppendLine("  District values:");

                foreach (Unity.Entities.Entity district in districts)
                {
                    bool hasStoredEntry = false;
                    int entryCount = 0;
                    Game.Policies.Policy firstEntry = default;

                    if (policyLookup.TryGetBuffer(
                            district,
                            out Unity.Entities.DynamicBuffer<Game.Policies.Policy> policies))
                    {
                        foreach (Game.Policies.Policy policy in policies)
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
                        (firstEntry.m_Flags & Game.Policies.PolicyFlags.Active) != 0;

                    // Vanilla PoliciesUISystem uses the stored adjustment whenever
                    // a matching Policy buffer entry exists, even when inactive.
                    float uiValue = hasStoredEntry
                        ? firstEntry.m_Adjustment
                        : hasSliderData
                            ? sliderData.m_Default
                            : 0f;

                    string inRange = "N/A";
                    string issue = string.Empty;

                    if (hasSliderData)
                    {
                        bool valueInRange =
                            uiValue >= sliderData.m_Range.min &&
                            uiValue <= sliderData.m_Range.max;

                        inRange = valueInRange ? "YES" : "NO";

                        if (!valueInRange)
                        {
                            invalidDistrictValues++;
                            issue = " | INVALID_VALUE";
                        }
                    }

                    string selected =
                        district == selectedEntity ? " | SELECTED" : string.Empty;

                    text.AppendLine(
                        $"    {GetDistrictName(district)} [{FormatEntity(district)}] | " +
                        $"StoredEntry={(hasStoredEntry ? "YES" : "NO")} | " +
                        $"EntryCount={entryCount} | " +
                        $"Active={(active ? "YES" : "NO")} | " +
                        $"StoredAdjustment={(hasStoredEntry ? FormatDebugFloat(firstEntry.m_Adjustment) : "<none>")} | " +
                        $"UIValue={FormatDebugFloat(uiValue)} | " +
                        $"InRange={inRange}" +
                        (entryCount > 1 ? " | DUPLICATE" : string.Empty) +
                        issue +
                        selected);
                }
            }

            text.AppendLine();
            text.AppendLine($"InvalidDistrictValues={invalidDistrictValues}");
            text.AppendLine(
                "Interpretation: PolicySliderData=YES means vanilla C# has slider metadata. " +
                "A stored UIValue outside SliderRange can survive in the district Policy buffer " +
                "and is a strong suspect when the panel shows 0 or loses the slider.");
            text.Append(
                $"==================== {Mod.ModTag} END PARKING FEE DEBUG ====================");

            LogUtils.Info(text.ToString());
        }

        private static string FormatDebugFloat(float value)
        {
            return value.ToString(
                "0.###",
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

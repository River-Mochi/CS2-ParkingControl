// <copyright file="LocaleKO.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Korean text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Korean localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleKO : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleKO"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleKO(PCSettings settings)
        {
            m_Settings = settings;
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Options tabs and groups.
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "작업" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "정보" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "노상 주차" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "개인 차량 상태" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "모드 정보" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "링크" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "진단" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "노상 주차 금지" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "**도시 전체**, **구역별**, **꺼짐** 중에서 선택합니다.\n" +
                    "- 대상 주차 차선을 비활성화해 새 노상 주차를 막습니다.\n" +
                    "- 이미 주차된 차량은 다음에 사용될 때 자연스럽게 이동합니다.\n" +
                    "- 유료 주차장과 일반 건물 주차는 계속 이용할 수 있습니다.\n" +
                    "**고속도로와 비대칭 3차선 도로는 원래 노상 주차를 허용하지 않습니다.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "도시 전체" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "구역별" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "꺼짐" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "사용법 표시" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "<구역별> 모드 사용법을 표시합니다.\n" +
                    "꺼짐 = 노상 주차 제한이 비활성화됩니다.\n" +
                    "도시 전체 = 대상 노상 주차가 도시 전체에서 차단됩니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "상태 표시" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<현재 주차 합계를 아래에 표시합니다.>\n" +
                    "상태는 옵션 메뉴가 열려 있을 때만 수집되며, 도시 " +
                    "플레이 중에는 백그라운드 상태 스캔을 하지 않습니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<구역 모드>\n" +
                    "1. 위에서 <구역별>을 선택합니다.\n" +
                    "2. 도시에서 구역을 만들거나 선택합니다.\n" +
                    "3. <정책>을 열고 **도로변 주차 금지 [✓]**를 켭니다.\n" +
                    "선택한 구역 밖의 도로는 일반 노상 주차를 유지합니다." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "도로변 주차 금지" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "이 구역에서는 <자동차와 오토바이>가 도로변에 주차하지 못하게 " +
                    "합니다. 이미 주차된 차량은 소유자가 다음에 사용할 때 이동합니다." },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "노상 주차" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<주차됨> = 선택한 모드에서 주차가 금지된 도로에 아직 주차된 차량.\n" +
                    "<차선> = 해당 차량이 있는 도로변 주차 구간. 한 차선에 여러 차량이 주차될 수 있습니다.\n" +
                    "<비활성> = 새 차량 주차가 금지된 노상 주차 차선.\n" +
                    "<구역별>에서는 다음을 표시합니다:\n" +
                    "- 금지 구역의 점유 차선 / 도시 전체 점유 차선.\n" +
                    "- 비활성 차선 / 도시의 대상 차선.\n" +
                    "- 활성화된 구역 / 전체 구역.\n" +
                    "<새로 만든 도로나 재건한 도로>는 차선이 업데이트되는 동안 잠깐 몇 대가 주차할 수 있습니다. " +
                    "이미 주차된 차량은 시민이 사용할 때 자연스럽게 이동합니다.\n" +
                    "<확인> = 선택한 도로 일부가 아직 차단되지 않았습니다. 도시를 잠시 실행한 뒤 " +
                    "다시 확인하세요. <확인>이 계속되면 도움을 요청할 때 주차 로그 보고서를 포함하세요." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "도로 사용" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "이 행은 구역뿐 아니라 <도시 전체>를 포함합니다.\n" +
                    "<노상 주차> = 공공 또는 건물 주차 대신 도로에 주차된 비율.\n" +
                    "<활성> = 주행 중이거나 교통에서 대기 중인 개인 차량.\n" +
                    "<공식> = 도로 ÷ (도로 + 사용 중 공공 + 사용 중 건물 주차).\n" +
                    "**외부 연결(OC) 보관소와 주차 차선이 지정되지 않은 차량은 제외됩니다.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "주차 공간" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "도시 전체 주차 공간 점유를 표시합니다.\n" +
                    "<공공> 사용 = 기본 게임 주차 정보 보기에서 집계하는 시설.\n" +
                    "<건물> 사용 = 주택, 직장, 상점에 포함된 주차 공간.\n" +
                    "**사용률이 높을수록 추가 주차 공간이 필요할 수 있습니다.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "차량 위치" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "이 행은 금지 구역뿐 아니라 도시 전체 데이터를 표시합니다.\n" +
                    "<도로> = 공공 도로에 주차됨.\n" +
                    "<표시> = 야외 주차장이나 건물에 포함된 야외 주차 공간에서 보고 클릭할 수 있는 차량.\n" +
                    "<숨김> = 건물이나 차고 내부.\n" +
                    "<OC> = 도시 경계의 외부 연결 보관소. 일부 유입 가구 차량은 대기 구역으로 여기에서 시작합니다.\n" +
                    "주차 차선이 지정되지 않은 차량은 여기에서 제외되고 정보 탭의 로그 보고서에만 표시됩니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "업데이트" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "이 도시 전체 상태 값이 마지막으로 새로 고침된 시간입니다." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "모드 이름" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "버전" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 링크" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods에서 제작자 페이지를 엽니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "주차 보고서 작성" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "노상 주차 및 관련 정보를 \n" +
                    "<Logs/ParkingControl.log>에 기록합니다.\n" +
                    "궁금하다면 같은 도시가 로드된 상태에서 나중에 두 번째 보고서를 작성하세요.\n" +
                    "- 서로 다른 범주에서 최대 20개의 샘플 Entity ID를 비교합니다.\n" +
                    "- 각 샘플이 그대로인지, 운전을 시작했는지, 다른 곳에 주차했는지, 사라졌는지 보여 줍니다.\n" +
                    "- 도시에서 Entity ID 번호를 추적하려면 Scene Explorer 모드가 필요합니다."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "로그 열기" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "<Logs/ParkingControl.log>를 열거나, 파일이 아직 없으면 Logs 폴더를 엽니다." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "아직 불러온 도시가 없습니다." },
                { ParkingStatusLocale.kCollecting, "주차 상태를 수집하는 중..." },
                { ParkingStatusLocale.kUnavailable, "주차 상태를 사용할 수 없습니다." },
                { ParkingStatusLocale.kCollectionFailed, "주차 상태를 수집하지 못했습니다. ParkingControl.log를 확인하세요." },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} 주차 ({1} 차선) | {2}/{3} 비활성{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} 주차 ({1}/{2} 차선) | {3}/{4} 비활성 | {5}/{6} 구역{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 도로 | {1} 표시 | {2} 숨김 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} 공공 {1}/{2} | {3} 건물 {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} 노상 주차 {1} | {2} 활성" },
                { ParkingStatusLocale.kStatusOk, "정상" },
                { ParkingStatusLocale.kStatusOff, "꺼짐" },
                { ParkingStatusLocale.kStatusCheck, "확인" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

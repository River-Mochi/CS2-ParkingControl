// <copyright file="LocaleKO.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "진단" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "노상 주차 금지" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "선택:\n" +
                    "<1. 구역별>\n" +
                    "<2. 도시 전체>\n" +
                    "<3. 꺼짐>\n" +
                    "- 대상 차선을 비활성화해 새 노상 주차를 막습니다.\n" +
                    "- 금지 후 기존 주차 차량은 점차 이동합니다. 넓은 금지 구역은 더 오래 걸립니다.\n" +
                    "- 유료 주차장과 일반 건물 주차는 계속 이용할 수 있습니다.\n" +
                    "**고속도로와 작은 양방향 골목처럼 일부 도로는 원래 노상 주차가 불가합니다.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "도시 전체" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "구역별" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "꺼짐" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "사용법 표시" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "<구역별> 모드 사용법을 표시합니다.\n" +
                    "1.a. 꺼짐 = 도시 전체 및 구역 제한을 끄고 대부분 게임 기본 상태로 돌아갑니다.\n" +
                    "1.b. 도로 서비스의 개별 도로 <주차 금지> 버튼은 횡단보도처럼 계속 사용할 수 있습니다.\n" +
                    "2. 도시 전체 = 도시의 모든 대상 공공 노상 주차를 차단합니다."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "상태 표시" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<현재 주차 합계를 아래에 표시합니다.>\n" +
                    "상태는 옵션 메뉴가 열려 있을 때만 수집됩니다.\n" +
                    "도시 플레이 중에는 백그라운드 스캔을 하지 않습니다."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<구역 모드>\n" +
                    "1. 위에서 <구역별>을 선택합니다.\n" +
                    "2. 구역을 만들거나 선택합니다.\n" +
                    "3. <정책>을 열고 **도로변 주차 금지 [✓]**를 켭니다.\n" +
                    "4. 주차 금지와 주차 요금을 함께 켜도 됩니다. 아직 남아 있거나 금지 후에도 주차한 차량에는 요금이 부과됩니다.\n" +
                    "금지 구역 밖의 도로는 일반 노상 주차를 유지합니다."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "주차 금지" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "도로 한쪽의 도로변 주차를 켜거나 끕니다. 여러 쪽은 왼쪽 마우스 버튼을 놓기 전에 그 위로 드래그하세요." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "도로변 주차 금지" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "이 구역에서 자동차와 오토바이의 도로변 주차를 막습니다.\n" +
                    "- 기존 주차 차량은 점차 이동합니다. 넓은 금지 구역은 더 오래 걸립니다."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "추가" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "해제" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "노상 주차" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "선택한 <도시 전체> 또는 <구역별> 금지 범위만 표시합니다. 수동 주차 금지는 별도 표시됩니다.\n" +
                    "<꺼짐> = 도시 전체/구역 금지는 꺼짐. 수동 <주차 금지> 도로는 계속 적용됩니다.\n" +
                    "<주차됨> = 선택 범위의 도로에 아직 주차된 차량.\n" +
                    "<비활성> = 비활성 도로변 차선 구간 / 대상 구간.\n" +
                    "<구역> = 주차 금지 구역 / 전체 구역.\n" +
                    "<확인> = 일부 대상 구간이 아직 선택한 금지 상태와 맞지 않습니다.\n" +
                    "<---------------------->\n" +
                    "**도로 변경이나 재건 후 [확인]이 보이면 도시를 잠시 실행한 뒤 옵션 > 상태를 다시 여세요. 계속 남으면 정보 > 진단 > 보고서 작성을 사용하세요.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "수동 주차 금지" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "수동 <주차 금지> 도구로 설정한 도로만 표시합니다.\n" +
                    "<주차됨> = 수동 금지 도로에 아직 주차된 차량.\n" +
                    "<비활성> = 비활성 도로변 차선 구간 / 수동 대상 구간.\n" +
                    "수동 금지는 도시 전체 또는 구역 금지와 겹칠 수 있으므로 이 행을 <노상 주차> 합계에 더하지 마세요.\n" +
                    "**도시를 잠시 실행해도 [확인]이 보이면 정보 > 진단 > 보고서 작성을 사용하고 도움을 요청할 때 제출하세요.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "주차 이용" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<도시 전체> 주차 이용을 표시합니다. 전체 도시 / 구역별 주차 금지 범위를 따르지 않습니다.\n" +
                    "<공공> = 공공 주차 시설의 사용 / 전체 공간.\n" +
                    "CS2 도로 주차 정보 보기와 같은 주차 시설 데이터를 사용합니다.\n" +
                    "<건물> = 건물이나 차고에 주차된 차량.\n" +
                    "<도로> = 도로에 주차된 차량.\n" +
                    "<합계> = 도시 안에서 확인된 주차 차량 합계(도로 + 공공 + 건물).\n" +
                    "**외부 연결과 위치 불명 대기 차량은 합계에서 제외됩니다.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "주차 평가" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<도시 전체> 공공 주차 여유를 표시합니다.\n" +
                    "<부족> = 15% 미만 여유.\n" +
                    "<OK> = 15% 이상 30% 미만 여유.\n" +
                    "<좋음> = 30% 이상 여유.\n" +
                    "<공공 여유> = 현재 비어 있는 공공 주차 공간.\n" +
                    "게임의 도로 주차 정보 보기와 같은 주차 시설을 집계합니다."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "차량 위치" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "이 행은 금지 구역뿐 아니라 도시 전체 데이터를 표시합니다.\n" +
                    "<도로> = 공공 도로에 주차됨.\n" +
                    "<표시> = 야외 주차장이나 건물의 야외 주차 공간에서 보고 클릭할 수 있는 차량.\n" +
                    "<실내> = 건물이나 차고 내부.\n" +
                    "<OC> = 도시 경계의 외부 연결 보관소. 일부 유입 가구 차량은 여기서 시작합니다(대기 구역).\n" +
                    "주차 차선이 지정되지 않은 차량은 여기서 제외되고 로그 보고서(정보 탭)에만 표시됩니다."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "업데이트" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "이 도시 전체 상태 값이 마지막으로 새로 고침된 시간입니다." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "모드 이름" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "버전" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 링크" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods에서 제작자 페이지를 엽니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "보고서 작성" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "노상 주차 및 관련 정보를 \n" +
                    "<Logs/ParkingControl.log>에 기록합니다.\n" +
                    "필요하면 같은 도시를 불러온 상태에서 나중에 두 번째 보고서를 작성하세요.\n" +
                    "- 서로 다른 범주에서 최대 20개의 샘플 Entity ID를 비교합니다.\n" +
                    "- 각 샘플이 남아 있는지, 주행을 시작했는지, 다른 곳에 주차했는지, 사라졌는지 보여 줍니다.\n" +
                    "- 도시에서 Entity ID를 추적하려면 Scene Explorer가 필요합니다."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "로그 열기" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "<Logs/ParkingControl.log>를 열거나 파일이 아직 없으면 Logs 폴더를 엽니다." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "상세 로그" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "자동 DEBUG 정보.\n" +
                    "일반 플레이용이 아닙니다. 디버깅하지 않으면 꺼두세요.\n" +
                    "보고서 작성은 꺼짐 상태에서도 작동합니다."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "아직 불러온 도시가 없습니다." },
                { ParkingStatusLocale.kCollecting, "주차 상태를 수집하는 중..." },
                { ParkingStatusLocale.kUnavailable, "주차 상태를 사용할 수 없습니다." },
                { ParkingStatusLocale.kCollectionFailed, "주차 상태를 수집하지 못했습니다. ParkingControl.log를 확인하세요." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} 주차 | {1}/{2} 비활성{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} 주차 | {1}/{2} 차선 비활성{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} 주차 | {1}/{2} 비활성 | {3}/{4} 구역{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 도로 | {1} 표시 | {2} 실내 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1}, 공공 여유 {2}" },
                { ParkingStatusLocale.kShareFormat, "{0} 공공 | {1} 건물 | {2} 도로 | {3} 합계" },
                { ParkingStatusLocale.kStatusOk, "정상" },
                { ParkingStatusLocale.kStatusOff, "꺼짐 = 도시/구역 금지 해제 | 수동 도로는 유지" },
                { ParkingStatusLocale.kManualNone, "설정 없음" },
                { ParkingStatusLocale.kStatusCheck, "확인" },
                { ParkingStatusLocale.kRatingPoor, "부족" },
                { ParkingStatusLocale.kRatingGood, "좋음" },
                { ParkingStatusLocale.kRatingNA, "N/A" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

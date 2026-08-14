// <copyright file="LocaleKO.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Korean text for Parking Control's Options UI.

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
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
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
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "작업" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "정보" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "노상 주차" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "개인 차량 상태" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "모드 정보" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "링크" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "진단" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "노상 주차 금지 (도시 전체)" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)),
                    "- 앞으로 개인 차량과 오토바이가 노상 주차를 사용하지 못하게 합니다.\n" +
                    "- 주차장과 건물에 포함된 주차 공간은 계속 사용할 수 있습니다.\n" +
                    "- 이미 주차된 차량은 제거되지 않습니다. 시민이 다음에 해당 차량을 사용하면 자연스럽게 떠납니다.\n" +
                    "- 도시의 노상 외 주차 공간이 충분한지 확인하세요. 부족하면 차량이 빈자리를 찾느라 오래 돌아다닐 수 있습니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "노상 주차" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<주차됨> = 아직 도로에 주차되어 있는 차량.\n" +
                    "<차선> = 해당 차량이 있는 도로변 주차 구간. 한 차선에 여러 차량이 주차될 수 있습니다.\n" +
                    "<비활성> = 새 차량이 주차할 수 없도록 닫힌 노상 주차 차선.\n" +
                    "<정상> = 노상 주차 금지가 켜져 있고 정상 작동 중입니다.\n" +
                    "<꺼짐> = 노상 주차 금지가 꺼져 있어 일반 도로에 자유롭게 주차할 수 있습니다.\n" +
                    "<확인> = 도로가 아직 업데이트 중일 수 있습니다. 잠시 기다린 뒤 계속되면 로그 보고서를 작성하세요.\n" +
                    "**노상 주차 금지를 켜거나 도로를 변경한 뒤에도 일부 차량이 남아 있을 수 있습니다. 시민이 해당 차량을 사용하면 자연스럽게 떠납니다.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "차량 위치" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<도로> = 공공 도로에 주차됨.\n" +
                    "<표시> = 야외 주차장 또는 건물에 포함된 야외 주차 공간에서 보고 클릭할 수 있는 차량.\n" +
                    "<숨김> = 건물이나 차고 내부.\n" +
                    "<OC> = 도시 경계의 외부 연결 보관소. 일부 유입 가구의 차량은 여기에서 시작합니다.\n" +
                    "**할당되지 않은 기본 게임 대기 영역은 로그에만 기록됩니다.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "주차" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<점유율> 및 <사용 / 전체>는 주차 공간 점유 상태를 표시합니다.\n" +
                    "<공공> = 기본 게임의 주차 정보 보기에서 집계하는 시설.\n" +
                    "<건물> = 주택과 직장에 포함된 주차 공간.\n" +
                    "**건물 주차에는 표시되는 야외 공간과 내부 주차 공간이 모두 포함됩니다.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "도로 사용" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<노상 주차> = 알려진 노상, 공공 또는 건물 주차를 이용하는 차량 중 노상에 주차된 비율.\n" +
                    "<이동 중> = 주행 중이거나 교통 정체에서 대기 중인 개인 차량.\n" +
                    "<업데이트> = 상태의 마지막 새로 고침.\n" +
                    "**외부 연결(OC)과 할당되지 않은 대기 영역은 제외됩니다.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "모드 이름" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "버전" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 링크" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods에서 제작자 페이지를 엽니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "주차 보고서 작성" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "노상 주차, 주차 공급, 소유권 및 차량 위치 세부 정보를\n" +
                    "<ParkingControl.log>에 기록합니다\n" +
                    "같은 도시에서 두 번째 보고서를 작성하면 동일한 노상 주차 차량 엔티티 ID를 추적합니다." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "로그 열기" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "<ParkingControl.log>를 열거나, 파일이 아직 없으면 Logs 폴더를 엽니다." },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "아직 불러온 도시가 없습니다." },
                { ParkingStatusLocale.kCollecting, "주차 상태를 수집하는 중..." },
                { ParkingStatusLocale.kUnavailable, "주차 상태를 사용할 수 없습니다." },
                { ParkingStatusLocale.kCollectionFailed, "주차 상태를 수집하지 못했습니다. ParkingControl.log를 확인하세요." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} 주차됨 ({1} 차선) | {2}/{3} 비활성 | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 도로 | {1} 표시 | {2} 숨김 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} 공공 | {3}  {4} / {5} 건물" },
                { ParkingStatusLocale.kShareFormat, "{0} 노상 주차 | {1} 이동 중 | 업데이트 {2}" },
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

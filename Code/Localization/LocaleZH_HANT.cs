// <copyright file="LocaleZH_HANT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Traditional Chinese text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Traditional Chinese localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleZH_HANT : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleZH_HANT"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleZH_HANT(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "操作" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "關於" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "路邊停車" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "私人車輛狀態" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "模組資訊" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "連結" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "診斷" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "禁止路邊停車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "選擇 **全城**、**按行政區** 或 **關閉**。\n" +
                    "- 符合條件的停車車道會被停用，以阻止新的路邊停車。\n" +
                    "- 已停放車輛會在下次使用時自然駛離。\n" +
                    "- 收費停車場和一般建築停車位仍可使用。\n" +
                    "**有些道路本來就不允許路邊停車，例如高速公路和小型雙向巷道。**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "全城" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "按行政區" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "關閉" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "顯示說明" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "顯示<按行政區>模式的使用方法。\n" +
                    "1.a. 關閉 = 關閉全城與行政區限制，基本恢復遊戲預設行為。\n" +
                    "1.b. 道路服務面板中的單一道路<禁止停車>按鈕仍可使用，就像新增行人穿越道一樣。\n" +
                    "2. 全城 = 禁止全城所有符合條件的公共路邊停車。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "顯示狀態" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<在下方顯示目前停車統計。>\n" +
                    "僅在開啟「選項」選單時收集狀態；正常城市遊玩期間不會在背景掃描狀態。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<行政區模式>\n" +
                    "1. 在上方選擇<按行政區>。\n" +
                    "2. 在城市中建立或選擇行政區。\n" +
                    "3. 開啟<政策>並啟用**路邊停車禁令 [✓]**。\n" +
                    "4. 禁停和停車費可以同時啟用。仍停在路邊或禁停後仍然停車的車輛都會被收費。\n" +
                    "禁停行政區之外的道路仍保留一般路邊停車。" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "禁止停車" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "切換道路一側的路邊停車。要處理多個路側，請按住滑鼠左鍵拖過這些路側後再放開。" },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "路邊停車禁令" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]", "禁止汽車和摩托車在此行政區的路邊停車。已停放車輛會在車主下次使用時離開。" },
                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "新增" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "移除" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路邊停車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<已停放> = 仍停在被 Parking Control 設為禁止停車路側的車輛。\n" +
                    "<車道> = 停放這些車輛的路邊停車路段。一個路段可停多輛車。\n" +
                    "<已停用> = 禁止新車輛停車的停車車道路段。一條道路可包含多個路段。\n" +
                    "<關閉 + 手動禁止停車> = 關閉會停用全城和行政區禁停，但手動設為禁止停車的路側仍保持生效。此行隨後只顯示這些手動禁停。\n" +
                    "<---------------------->\n" +
                    "如果選擇<按行政區>，則顯示：\n" +
                    "- 禁停行政區內的已占用車道 / 全城已占用車道。\n" +
                    "- 已停用車道 / 全城符合條件的車道。\n" +
                    "- 已啟用行政區 / 行政區總數。\n" +
                    "<---------------------->\n" +
                    "注意：更改或重建道路後，CS2 重建停車車道期間，已停用數量可能需要一點時間才能更新。讓城市運行一會兒，然後重新開啟選項。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "道路使用" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "此列包含<全城>，不只是行政區。\n" +
                    "<路邊停車> = 停在道路而不是公共或建築停車位的車輛百分比。\n" +
                    "<活動> = 正在行駛或在交通中等待的私人車輛。\n" +
                    "<公式> = 道路 ÷（道路 + 已占用公共 + 已占用建築）。\n" +
                    "**不包括外部連接 (OC) 儲存和未分配停車車道的車輛。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "停車位" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "顯示全城停車位占用情況。\n" +
                    "<公共> 已用 = 遊戲原版停車資訊檢視統計的設施。\n" +
                    "<建築> 已用 = 住宅、工作場所和商店附帶的停車位。\n" +
                    "**使用率越高 = 可能需要更多停車位。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "車輛位置" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "此列顯示全城資料，不只包含禁停行政區。\n" +
                    "<道路> = 停在公共道路上。\n" +
                    "<可見> = 可在露天停車場或建築附帶的室外停車位中看到並點選的車輛。\n" +
                    "<室內> = 位於建築或車庫內。\n" +
                    "<OC> = 城市邊界的外部連接車輛儲存；部分進入城市的家庭車輛會從那裡開始（暫存區）。\n" +
                    "未分配停車車道的車輛不會顯示在這裡，只會出現在日誌報告中（關於分頁）。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "已更新" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "這些全城狀態值上次重新整理的時間。" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "模組名稱" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "版本" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 連結" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "開啟作者的 Paradox Mods 頁面。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "寫入停車報告" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "將路邊停車和相關詳細資訊寫入\n" +
                    "<Logs/ParkingControl.log>。\n" +
                    "如需進一步查看，可在同一個已載入城市中稍後再寫入第 2 份報告。\n" +
                    "- 比較不同類別中最多 20 個範例 Entity ID。\n" +
                    "- 顯示每個範例是仍停留、開始行駛、停到別處還是消失。\n" +
                    "- 需要 Scene Explorer 模組才能在城市中追蹤 Entity ID 編號。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "開啟日誌" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "開啟 <Logs/ParkingControl.log>；如果檔案尚不存在，則開啟 Logs 資料夾。" },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "尚未載入城市。" },
                { ParkingStatusLocale.kCollecting, "正在收集停車狀態..." },
                { ParkingStatusLocale.kUnavailable, "停車狀態無法使用。" },
                { ParkingStatusLocale.kCollectionFailed, "無法收集停車狀態；請查看 ParkingControl.log。" },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} 已停放（{1} 車道）| {2}/{3} 已停用{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} 已停放（{1}/{2} 車道）| {3}/{4} 已停用 | {5}/{6} 行政區{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 道路 | {1} 可見 | {2} 室內 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} 公共 {1}/{2} | {3} 建築 {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} 路邊停車 {1} | {2} 活動" },
                { ParkingStatusLocale.kStatusOk, "正常" },
                { ParkingStatusLocale.kStatusOff, "關閉" },
                { ParkingStatusLocale.kStatusCheck, "檢查" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

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
                    "選擇：\n" +
                    "<1. 按行政區>\n" +
                    "<2. 全城>\n" +
                    "<3. 關閉>\n" +
                    "- 符合條件的車道會被停用，以阻止新的路邊停車。\n" +
                    "- 禁停後，已停放車輛會逐步移走；禁停範圍越大，所需時間越長。\n" +
                    "- 收費停車場和一般建築停車位仍可使用。\n" +
                    "**有些道路本來就不允許路邊停車，例如高速公路和小型雙向巷道。**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "全城" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "按行政區" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "關閉" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "顯示說明" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "顯示<按行政區>模式的使用方法。\n" +
                    "1.a. 關閉 = 關閉全城和行政區禁停，基本恢復遊戲預設狀態。\n" +
                    "1.b. 道路服務中的單一道路<禁止停車>按鈕仍可像新增行人穿越道一樣使用。\n" +
                    "2. 全城 = 禁止全城所有符合條件的公共路邊停車。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "顯示狀態" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<在下方顯示目前停車統計。>\n" +
                    "僅在開啟「選項」選單時收集狀態；\n" +
                    "正常遊玩期間不會在背景掃描。"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<行政區模式>\n" +
                    "1. 在上方選擇<按行政區>。\n" +
                    "2. 建立/選擇一個行政區。\n" +
                    "3. 開啟<政策>並啟用**路邊停車禁令 [✓]**。\n" +
                    "4. 禁停和停車費可以同時啟用。仍停在那裡或禁停後仍能停車的車輛都會被收費。\n" +
                    "禁停行政區之外的道路仍保留一般路邊停車。"
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "禁止停車" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "切換道路一側的路邊停車。要處理多個路側，請按住滑鼠左鍵拖過後再放開。" },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "路邊停車禁令" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "禁止汽車和摩托車在此行政區路邊停車。\n" +
                    "- 已停放車輛會逐步移走；禁停範圍越大，所需時間越長。"
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "新增" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "移除" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路邊停車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "僅顯示所選<全城>或<按行政區>禁停範圍。手動禁停道路分開顯示。\n" +
                    "<關閉> = 全城/行政區禁停已關閉；手動<禁止停車>道路仍生效。\n" +
                    "<已停放> = 仍停在所選範圍道路上的車輛。\n" +
                    "<已停用> = 已停用的路緣車道區段 / 目標區段。\n" +
                    "<行政區> = 啟用禁停的行政區 / 行政區總數。\n" +
                    "<檢查> = 部分目標區段尚未與所選禁停狀態一致。\n" +
                    "<---------------------->\n" +
                    "**如果更改或重建道路後出現[檢查]，讓城市運行一會兒，再重新開啟 選項 > 狀態。如果仍存在，請使用 關於 > 診斷 > 寫入報告。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "手動禁止停車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "僅顯示用手動<禁止停車>工具設定的道路。\n" +
                    "<已停放> = 仍停在這些手動禁停道路上的車輛。\n" +
                    "<已停用> = 已停用的路緣車道區段 / 手動目標區段。\n" +
                    "手動禁停可能與全城或行政區禁停重疊，請勿將此列加入<路邊停車>總數。\n" +
                    "**如果城市運行一會兒後仍出現[檢查]，請使用 關於 > 診斷 > 寫入報告，並在求助時提交該報告。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "停車使用" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<公共> = 停車設施中的已占用車位。\n" +
                    "大致對應 CS2 道路停車資訊面板。\n" +
                    "<建築> = 停在建築或車庫內的車輛。\n" +
                    "<道路> = 停在道路上的車輛。\n" +
                    "<總計> = 城內已知停放車輛總數（道路 + 公共 + 建築）。\n" +
                    "**外部連接和未知暫存車輛不計入總數。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "停車評級" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "評估有精確容量的停車位還剩多少<空閒>。\n" +
                    "<差> = 空閒少於 15%。\n" +
                    "<正常> = 空閒 15% 至不足 30%。\n" +
                    "<良好> = 空閒 30% 或以上。\n" +
                    "<公共>使用 CS2 道路停車資訊面板統計的設施。\n" +
                    "<建築>使用建築和車庫中有精確容量的停車位。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "車輛位置" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "此列顯示全城資料，不只包含禁停行政區。\n" +
                    "<道路> = 停在公共道路上。\n" +
                    "<可見> = 可在露天停車場或建築室外停車位中看到並點選的車輛。\n" +
                    "<室內> = 位於建築或車庫內。\n" +
                    "<OC> = 城市邊界的外部連接車輛儲存；部分進入城市的家庭車輛從那裡開始（暫存區）。\n" +
                    "未分配停車車道的車輛不會顯示在這裡，只會出現在日誌報告中（關於分頁）。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "已更新" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "這些全城狀態值上次重新整理的時間。" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "模組名稱" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "版本" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 連結" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "開啟作者的 Paradox Mods 頁面。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "寫入報告" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "將路邊停車和相關詳細資訊寫入 \n" +
                    "<Logs/ParkingControl.log>。\n" +
                    "如需進一步查看，可稍後在同一已載入城市中再寫入第 2 份報告。\n" +
                    "- 比較不同類別中最多 20 個範例 Entity ID。\n" +
                    "- 顯示每個範例是仍停留、開始行駛、停到別處還是消失。\n" +
                    "- 需要 Scene Explorer 才能在城市中追蹤 Entity ID。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "開啟日誌" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "開啟 <Logs/ParkingControl.log>；如果檔案尚不存在，則開啟 Logs 資料夾。" },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "詳細日誌" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "自動 DEBUG 詳細資訊。\n" +
                    "不適合正常遊玩；不除錯時請關閉。\n" +
                    "關閉時仍可使用「寫入報告」。"
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "尚未載入城市。" },
                { ParkingStatusLocale.kCollecting, "正在收集停車狀態..." },
                { ParkingStatusLocale.kUnavailable, "停車狀態無法使用。" },
                { ParkingStatusLocale.kCollectionFailed, "無法收集停車狀態；請查看 ParkingControl.log。" },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} 已停放 | {1}/{2} 已停用{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} 已停放 | {1}/{2} 車道已停用{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} 已停放 | {1}/{2} 已停用 | {3}/{4} 行政區{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 道路 | {1} 可見 | {2} 室內 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = 公共空閒 {1} | {2} = 建築空閒 {3}" },
                { ParkingStatusLocale.kShareFormat, "{0} 公共 | {1} 建築 | {2} 道路 | {3} 總計" },
                { ParkingStatusLocale.kStatusOk, "正常" },
                { ParkingStatusLocale.kStatusOff, "關閉 = 全城/行政區禁停關閉 | 手動道路仍生效" },
                { ParkingStatusLocale.kManualNone, "未設定" },
                { ParkingStatusLocale.kStatusCheck, "檢查" },
                { ParkingStatusLocale.kRatingPoor, "差" },
                { ParkingStatusLocale.kRatingGood, "良好" },
                { ParkingStatusLocale.kRatingNA, "N/A" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

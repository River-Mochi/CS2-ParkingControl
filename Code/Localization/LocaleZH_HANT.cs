// <copyright file="LocaleZH_HANT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Traditional Chinese text for Parking Control's Options UI.

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
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
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
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "操作" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "關於" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "路邊停車" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "私人車輛狀態" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "模組資訊" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "連結" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "診斷" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "禁止路邊停車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "選擇禁止新車輛路邊停車的範圍。已停放車輛會自然駛離；停車場和建築停車位仍可使用。" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "全城" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "按行政區" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "關閉" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "顯示說明" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)), "顯示行政區模式的使用方法。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<行政區模式>\n" +
                    "1. 在上方選擇<按行政區>。\n" +
                    "2. 在城市中建立或選擇行政區。\n" +
                    "3. 開啟<政策>並啟用<禁止路邊停車>。\n" +
                    "未選擇的行政區仍保留一般路邊停車。" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "禁止路邊停車" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路邊停車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<已停放> = 停在所選模式涵蓋道路上的車輛。\n" +
                    "<車道> = 停放這些車輛的路邊停車路段。一條停車車道可停多輛車。\n" +
                    "<已停用> = 已禁止新車輛停車的路邊停車車道。\n" +
                    "<正常> = 禁止路邊停車已開啟並正常運作。\n" +
                    "<關閉> = 禁止路邊停車已關閉；車輛可以在一般道路上自由停車。\n" +
                    "<檢查> = 道路可能仍在更新。請稍候；如果一直如此，請寫入一份日誌報告。\n" +
                    "**啟用禁止路邊停車或修改道路後，部分車輛仍可能暫時保留。市民使用車輛後，它們會自然駛離。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "車輛位置" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<路邊> = 停在公共道路上。\n" +
                    "<可見> = 可在露天停車場或建築物附帶的室外停車位中看到並點選的車輛。\n" +
                    "<隱藏> = 位於建築物或車庫內部。\n" +
                    "<OC> = 位於城市邊界的外部連接儲存區；部分遷入家庭的車輛會從這裡開始。\n" +
                    "**原版遊戲中未分配的暫存區域僅記錄在日誌中。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "停車位" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<佔用率> 和 <已用 / 總數> 顯示停車位佔用情況。\n" +
                    "<公共> = 原版「停車」資訊檢視統計的設施。\n" +
                    "<建築> = 住宅和工作場所附帶的停車位。\n" +
                    "**建築停車位包括可見的室外車位和內部停車位。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "路邊使用情況" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<路邊停放> = 使用已知路邊、公共或建築停車位的車輛中，停在路邊的比例。\n" +
                    "<行駛中> = 正在行駛或在交通中等待的私人車輛。\n" +
                    "<更新> = 狀態上次重新整理的時間。\n" +
                    "**外部連接（OC）和未分配暫存區域不計入統計。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "模組名稱" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "版本" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 連結" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "開啟作者在 Paradox Mods 上的頁面。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "寫入停車報告" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "將路邊停車、停車供給、車輛歸屬和車輛位置的詳細資訊\n" +
                    "寫入 <ParkingControl.log>\n" +
                    "在同一個已載入城市中產生第二份報告時，會追蹤相同的路邊停車車輛實體 ID。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "開啟日誌" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "開啟 <ParkingControl.log>；如果檔案尚不存在，則開啟 Logs 資料夾。" },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "尚未載入城市。" },
                { ParkingStatusLocale.kCollecting, "正在收集停車狀態..." },
                { ParkingStatusLocale.kUnavailable, "停車狀態無法使用。" },
                { ParkingStatusLocale.kCollectionFailed, "無法收集停車狀態；請查看 ParkingControl.log。" },
                { ParkingStatusLocale.kEnforcementFormat, "{0} 已停放（{1} 車道） | {2}/{3} 已停用 | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 路邊 | {1} 可見 | {2} 隱藏 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} 公共 | {3}  {4} / {5} 建築" },
                { ParkingStatusLocale.kShareFormat, "{0} 路邊停放 | {1} 行駛中 | 更新 {2}" },
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

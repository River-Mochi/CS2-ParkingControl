// <copyright file="LocaleZH_HANS.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Simplified Chinese text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Simplified Chinese localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleZH_HANS : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleZH_HANS"/> class.
        /// </summary>
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
        public LocaleZH_HANS(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "关于" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "路边停车" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "私人车辆状态" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "模组信息" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "链接" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "诊断" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "全城禁止路边停车" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)),
                    "- 阻止之后的私人车辆和摩托车使用路边停车位。\n" +
                    "- 停车场和建筑物附带的停车位仍可使用。\n" +
                    "- 已经停放的车辆不会被移除。市民下次使用车辆时，它们会自然驶离。\n" +
                    "- 请确保城市有足够的非路边停车位，否则车辆可能会长时间寻找空位。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路边停车" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<已停放> = 仍停在道路上的车辆。\n" +
                    "<车道> = 停放这些车辆的路边停车路段。一条停车车道可停多辆车。\n" +
                    "<已禁用> = 已禁止新车辆停车的路边停车车道。\n" +
                    "<正常> = 禁止路边停车已开启并正常工作。\n" +
                    "<关闭> = 禁止路边停车已关闭；车辆可以在普通道路上自由停车。\n" +
                    "<检查> = 道路可能仍在更新。请稍等；如果一直如此，请写入一份日志报告。\n" +
                    "**启用禁止路边停车或修改道路后，部分车辆仍可能暂时保留。市民使用车辆后，它们会自然驶离。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "车辆位置" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<路边> = 停在公共道路上。\n" +
                    "<可见> = 可在露天停车场或建筑物附带的室外停车位中看到并点击的车辆。\n" +
                    "<隐藏> = 位于建筑物或车库内部。\n" +
                    "<OC> = 位于城市边界的外部连接存储区；部分迁入家庭的车辆会从这里开始。\n" +
                    "**原版游戏中未分配的暂存区域仅记录在日志中。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "停车位" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<占用率> 和 <已用 / 总数> 显示停车位占用情况。\n" +
                    "<公共> = 原版“停车”信息视图统计的设施。\n" +
                    "<建筑> = 住宅和工作场所附带的停车位。\n" +
                    "**建筑停车位包括可见的室外车位和内部停车位。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "路边使用情况" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<路边停放> = 使用已知路边、公共或建筑停车位的车辆中，停在路边的比例。\n" +
                    "<行驶中> = 正在行驶或在交通中等待的私人车辆。\n" +
                    "<更新> = 状态上次刷新的时间。\n" +
                    "**外部连接（OC）和未分配暂存区域不计入统计。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "模组名称" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "版本" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 链接" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "打开作者在 Paradox Mods 上的页面。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "写入停车报告" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "将路边停车、停车供给、车辆归属和车辆位置的详细信息\n" +
                    "写入 <ParkingControl.log>\n" +
                    "在同一已加载城市中生成第二份报告时，会跟踪相同的路边停车车辆实体 ID。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "打开日志" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "打开 <ParkingControl.log>；如果文件尚不存在，则打开 Logs 文件夹。" },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "加载或开始一个城市以查看停车状态。" },
                { ParkingStatusLocale.kCollecting, "正在收集停车状态..." },
                { ParkingStatusLocale.kUnavailable, "停车状态不可用。" },
                { ParkingStatusLocale.kCollectionFailed, "无法收集停车状态；请查看 ParkingControl.log。" },
                { ParkingStatusLocale.kEnforcementFormat, "{0} 已停放（{1} 车道） | {2}/{3} 已禁用 | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 路边 | {1} 可见 | {2} 隐藏 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} 公共 | {3}  {4} / {5} 建筑" },
                { ParkingStatusLocale.kShareFormat, "{0} 路边停放 | {1} 行驶中 | 更新 {2}" },
                { ParkingStatusLocale.kStatusOk, "正常" },
                { ParkingStatusLocale.kStatusOff, "关闭" },
                { ParkingStatusLocale.kStatusCheck, "检查" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

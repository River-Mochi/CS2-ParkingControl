// <copyright file="LocaleZH_HANS.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Simplified Chinese text for Parking Control's Options UI.

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
        /// <param name="settings">Options settings whose localization IDs are used.</param>
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
                // Options tabs and groups.
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "操作" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "关于" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "路边停车" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "私人车辆状态" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "模组信息" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "链接" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "诊断" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "禁止路边停车" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "选择 **全城**、**按行政区** 或 **关闭**。\n" +
                    "- 符合条件的停车车道会被禁用，以阻止新的路边停车。\n" +
                    "- 已停放车辆会在下次使用时自然驶离。\n" +
                    "- 收费停车场和普通建筑停车位仍可使用。\n" +
                    "**高速公路和非对称三车道道路本来就不允许路边停车。**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "全城" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "按行政区" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "关闭" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "显示说明" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "显示<按行政区>模式的使用方法。\n" +
                    "1.a. 关闭 = 关闭全城和行政区限制，基本恢复游戏默认行为。\n" +
                    "1.b. 道路服务面板中的单条道路<禁止停车>按钮仍可使用，就像添加人行横道一样。\n" +
                    "2. 全城 = 禁止全城所有符合条件的公共路边停车。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "显示状态" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<在下方显示当前停车统计。>\n" +
                    "仅在打开“选项”菜单时收集状态；正常城市游戏期间不会后台扫描状态。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<行政区模式>\n" +
                    "1. 在上方选择<按行政区>。\n" +
                    "2. 在城市中创建或选择行政区。\n" +
                    "3. 打开<政策>并启用**路边停车禁令 [✓]**。\n" +
                    "4. 禁停和停车费可以同时启用。仍停在路边或禁停后仍然停车的车辆都会被收费。\n" +
                    "禁停行政区之外的道路仍保留正常路边停车。" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "禁止停车" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "切换道路一侧的路边停车。要处理多个路侧，请按住鼠标左键拖过这些路侧后再松开。" },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "路边停车禁令" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]", "禁止汽车和摩托车在此行政区的路边停车。已停放车辆会在车主下次使用时离开。" },
                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "添加" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "移除" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路边停车" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<已停放> = 仍停在所选模式禁止停车道路上的车辆。\n" +
                    "<车道> = 停放这些车辆的路边停车路段。一条车道可停多辆车。\n" +
                    "<已禁用> = 禁止新车辆停车的路边停车车道。\n" +
                    "<按行政区>显示：\n" +
                    "- 禁停行政区内的已占用车道 / 全城已占用车道。\n" +
                    "- 已禁用车道 / 全城符合条件的车道。\n" +
                    "- 已启用行政区 / 行政区总数。\n" +
                    "<新建或重建道路>在车道更新时可能短暂允许少量车辆停车。已停车辆会在市民使用时自然离开。\n" +
                    "<检查> = 某些选定道路尚未被阻止停车。让城市运行一会儿后再检查。如果<检查>仍然存在，求助时请附上停车日志报告。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "道路使用" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "此行包含<全城>，不只是行政区。\n" +
                    "<路边停车> = 停在道路而不是公共或建筑停车位的车辆百分比。\n" +
                    "<活动> = 正在行驶或在交通中等待的私人车辆。\n" +
                    "<公式> = 道路 ÷（道路 + 已占用公共 + 已占用建筑）。\n" +
                    "**不包括外部连接 (OC) 存储和未分配停车车道的车辆。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "停车位" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "显示全城停车位占用情况。\n" +
                    "<公共> 已用 = 游戏原版停车信息视图统计的设施。\n" +
                    "<建筑> 已用 = 住宅、工作场所和商店附带的停车位。\n" +
                    "**使用率越高 = 可能需要更多停车位。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "车辆位置" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "此行显示全城数据，不只包含禁停行政区。\n" +
                    "<道路> = 停在公共道路上。\n" +
                    "<可见> = 可在露天停车场或建筑附带的室外停车位中看到并点击的车辆。\n" +
                    "<室内> = 位于建筑或车库内。\n" +
                    "<OC> = 城市边界的外部连接车辆存储；部分进入城市的家庭车辆会从那里开始（暂存区）。\n" +
                    "未分配停车车道的车辆不会显示在这里，只会出现在日志报告中（关于选项卡）。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "已更新" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "这些全城状态值上次刷新的时间。" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "模组名称" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "版本" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 链接" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "打开作者的 Paradox Mods 页面。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "写入停车报告" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "将路边停车和相关详细信息写入\n" +
                    "<Logs/ParkingControl.log>。\n" +
                    "如需进一步查看，可在同一个已加载城市中稍后再写入第 2 份报告。\n" +
                    "- 比较不同类别中最多 20 个示例 Entity ID。\n" +
                    "- 显示每个示例是仍停留、开始行驶、停到别处还是消失。\n" +
                    "- 需要 Scene Explorer 模组才能在城市中跟踪 Entity ID 编号。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "打开日志" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "打开 <Logs/ParkingControl.log>；如果文件尚不存在，则打开 Logs 文件夹。" },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "尚未加载城市。" },
                { ParkingStatusLocale.kCollecting, "正在收集停车状态..." },
                { ParkingStatusLocale.kUnavailable, "停车状态不可用。" },
                { ParkingStatusLocale.kCollectionFailed, "无法收集停车状态；请查看 ParkingControl.log。" },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} 已停放（{1} 车道）| {2}/{3} 已禁用{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} 已停放（{1}/{2} 车道）| {3}/{4} 已禁用 | {5}/{6} 行政区{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 道路 | {1} 可见 | {2} 室内 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} 公共 {1}/{2} | {3} 建筑 {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} 路边停车 {1} | {2} 活动" },
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

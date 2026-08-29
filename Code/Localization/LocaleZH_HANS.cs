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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "诊断" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "禁止路边停车" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "选择：\n" +
                    "<1. 按行政区>\n" +
                    "<2. 仅手动>\n" +
                    "<3. 全城>\n" +
                    "- 符合条件的车道会被禁用，以阻止新的路边停车。\n" +
                    "- 禁停后，已停放车辆会逐步移走；禁停范围越大，所需时间越长。\n" +
                    "- 收费停车场和普通建筑停车位仍可使用。\n" +
                    "**有些道路本来就不允许路边停车，例如高速公路和小型双向巷道。**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "1. 按行政区" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "2. 仅手动" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "3. 全城" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "显示说明" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "显示<按行政区>模式的使用方法。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "显示状态" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<在下方显示当前停车统计。>\n" +
                    "仅在打开“选项”菜单时收集状态；\n" +
                    "正常游戏期间不会后台扫描。"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<行政区模式>\n" +
                    "1. 在上方选择<按行政区>。\n" +
                    "2. 创建/选择一个行政区。\n" +
                    "3. 打开<政策>并启用**路边停车禁令 [✓]**。\n" +
                    "4. 禁停和停车费可以同时启用。仍停在那里或禁停后仍能停车的车辆都会被收费。\n" +
                    "禁停行政区之外的道路仍保留正常路边停车。"
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "禁止停车" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "切换道路一侧的路边停车。要处理多个路侧，请按住鼠标左键拖过后再松开。" },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "路边停车禁令" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "禁止汽车和摩托车在此行政区路边停车。\n" +
                    "- 已停放车辆会逐步移走；禁停范围越大，所需时间越长。"
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "添加" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "移除" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路边停车" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "仅显示所选<全城>或<按行政区>禁停范围。手动禁停道路单独显示。\n" +
                    "<仅手动> = 全城/行政区禁停已关闭；手动<禁止停车>道路仍生效。\n" +
                    "<已停放> = 仍停在所选范围道路上的车辆。\n" +
                    "<已禁用> = 已禁用的路缘车道段 / 目标车道段。\n" +
                    "<行政区> = 启用禁停的行政区 / 行政区总数。\n" +
                    "<检查> = 部分目标车道段尚未与所选禁停状态一致。\n" +
                    "<---------------------->\n" +
                    "**如果更改或重建道路后出现[检查]，让城市运行一会儿，再重新打开 选项 > 状态。如果仍存在，请使用 关于 > 诊断 > 写入报告。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "手动禁止停车" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "仅显示用手动<禁止停车>工具设置的道路。\n" +
                    "<已停放> = 仍停在这些手动禁停道路上的车辆。\n" +
                    "<已禁用> = 已禁用的路缘车道段 / 手动目标车道段。\n" +
                    "手动禁停可能与全城或行政区禁停重叠，请勿将此行加入<路边停车>总数。\n" +
                    "**如果城市运行一会儿后仍出现[检查]，请使用 关于 > 诊断 > 写入报告，并在求助时提交该报告。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "停车使用" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "显示<全城>停车使用情况，不随“全城 / 按行政区”禁停范围变化。\n" +
                    "<公共> = 公共停车设施已占用 / 总车位。\n" +
                    "使用与 CS2 道路停车信息面板相同的停车设施数据。\n" +
                    "<建筑> = 停在建筑或车库内的汽车。\n" +
                    "<道路> = 停在道路上的车辆。\n" +
                    "<总计> = 城内已知停放车辆总数（道路 + 公共 + 建筑）。\n" +
                    "**外部连接和未知暂存车辆不计入总数。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "停车评级" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "显示<全城>公共停车空闲情况。\n" +
                    "<差> = 空闲少于 15%。\n" +
                    "<正常> = 空闲 15% 至不足 30%。\n" +
                    "<良好> = 空闲 30% 或以上。\n" +
                    "<公共空闲> = 当前未使用的公共停车位。\n" +
                    "统计与游戏道路停车信息面板相同的停车设施。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "车辆位置" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "此行显示全城数据，不只包含禁停行政区。\n" +
                    "<道路> = 停在公共道路上。\n" +
                    "<可见> = 可在露天停车场或建筑室外停车位中看到并点击的车辆。\n" +
                    "<室内> = 位于建筑或车库内。\n" +
                    "<OC> = 城市边界的外部连接车辆存储；部分进入城市的家庭车辆从那里开始（暂存区）。\n" +
                    "未分配停车车道的车辆不会显示在这里，只会出现在日志报告中（关于选项卡）。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "已更新" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "这些全城状态值上次刷新的时间。" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "模组名称" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "版本" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods 链接" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "打开作者的 Paradox Mods 页面。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "写入报告" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "将路边停车和相关详细信息写入 \n" +
                    "<Logs/ParkingControl.log>。\n" +
                    "如需进一步查看，可稍后在同一已加载城市中再写入第 2 份报告。\n" +
                    "- 比较不同类别中最多 20 个示例 Entity ID。\n" +
                    "- 显示每个示例是仍停留、开始行驶、停到别处还是消失。\n" +
                    "- 需要 Scene Explorer 才能在城市中跟踪 Entity ID。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "打开日志" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "打开 <Logs/ParkingControl.log>；如果文件尚不存在，则打开 Logs 文件夹。" },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "详细日志" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "自动 DEBUG 详细信息。\n" +
                    "不适合正常游戏；不调试时请关闭。\n" +
                    "关闭时仍可使用“写入报告”。"
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "尚未加载城市。" },
                { ParkingStatusLocale.kCollecting, "正在收集停车状态..." },
                { ParkingStatusLocale.kUnavailable, "停车状态不可用。" },
                { ParkingStatusLocale.kCollectionFailed, "无法收集停车状态；请查看 ParkingControl.log。" },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} 已停放 | {1}/{2} 已禁用{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} 已停放 | {1}/{2} 车道已禁用{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} 已停放 | {1}/{2} 已禁用 | {3}/{4} 行政区{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 道路 | {1} 可见 | {2} 室内 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1}，公共空闲 {2}" },
                { ParkingStatusLocale.kShareFormat, "{0} 公共 | {1} 建筑 | {2} 道路 | {3} 总计" },
                { ParkingStatusLocale.kStatusOk, "正常" },
                { ParkingStatusLocale.kStatusOff, "仅手动 = 全城/行政区禁停关闭 | 手动道路仍生效" },
                { ParkingStatusLocale.kManualNone, "未设置" },
                { ParkingStatusLocale.kStatusCheck, "检查" },
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

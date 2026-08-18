// <copyright file="LocaleJA.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Japanese text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Japanese localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleJA : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleJA"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleJA(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "情報" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "路上駐車" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "自家用車の状態" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "MOD情報" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "リンク" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "診断" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "路上駐車禁止" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "**市全体**、**地区ごと**、**オフ**から選びます。\n" +
                    "- 対象の駐車車線を無効にして、新たな路上駐車を防ぎます。\n" +
                    "- すでに駐車中の車は、次に使われたとき自然に移動します。\n" +
                    "- 有料駐車場と通常の建物内駐車は引き続き利用できます。\n" +
                    "**高速道路と非対称3車線道路は、もともと路上駐車できません。**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "市全体" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "地区ごと" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "オフ" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "手順を表示" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "<地区ごと>モードの使い方を表示します。\n" +
                    "オフ = 路上駐車の制限を無効にします。\n" +
                    "市全体 = 対象となる路上駐車を市全体で禁止します。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "状況を表示" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<現在の駐車状況を下に表示します。>\n" +
                    "状況はオプション画面を開いている間だけ集計され、通常" +
                    "の都市プレイ中にバックグラウンドスキャンは行いません。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<地区モード>\n" +
                    "1. 上で<地区ごと>を選びます。\n" +
                    "2. 都市内で地区を作成または選択します。\n" +
                    "3. <条例>を開き、**路上駐車禁止 [✓]**を有効にします。\n" +
                    "4. 駐車禁止と駐車料金は同時に有効にできます。残っている車や、禁止後も駐車した車には料金がかかります。\n" +
                    "駐車禁止地区の外では通常の路上駐車ができます。" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "路上駐車禁止" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "この地区では、自動車とオートバイが道路脇に駐車できないように" +
                    "します。すでに駐車中の車両は、所有者が次に使ったとき移動します。" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路上駐車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<駐車中> = 選択したモードで駐車禁止になっている道路にまだ駐車している車。\n" +
                    "<車線> = それらの車がある道路脇の駐車区間。1つの車線に複数台駐車できます。\n" +
                    "<無効> = 新たな駐車を受け付けない路上駐車車線。\n" +
                    "<地区ごと>では次を表示します:\n" +
                    "- 禁止地区内の駐車中車線 / 市全体の駐車中車線。\n" +
                    "- 無効な車線 / 市内の対象車線。\n" +
                    "- 有効な地区 / 全地区。\n" +
                    "<新設・再建した道路>は、車線の更新中に一時的に数台の車を受け入れることがあります。 " +
                    "すでに駐車中の車は、市民が使うと自然に移動します。\n" +
                    "<確認> = " +
                    "選択した道路の一部がまだ禁止されていません。少し都市を動かして再確認してください。<確認>が続く場合は、問い合わせ時に駐車ログレポートを添付してください。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "路上利用" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "この行は地区だけでなく<市全体>を集計します。\n" +
                    "<路上駐車> = 公共・建物駐車ではなく路上に駐車している割合。\n" +
                    "<稼働中> = 走行中または渋滞で待機中の自家用車。\n" +
                    "<計算式> = 路上 ÷ (路上 + 使用中の公共 + 使用中の建物駐車)。\n" +
                    "**外部接続 (OC) の保管領域と、駐車車線が割り当てられていない車は除外されます。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "駐車スペース" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "市全体の駐車スペース使用状況を表示します。\n" +
                    "<公共> 使用 = ゲーム本体の駐車情報ビューで集計される施設。\n" +
                    "<建物> 使用 = 住宅、職場、店舗に付属する駐車スペース。\n" +
                    "**使用率が高いほど、追加の駐車スペースが必要になる場合があります。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "車の場所" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "この行は、禁止地区だけでなく市全体のデータを表示します。\n" +
                    "<路上> = 公道に駐車中。\n" +
                    "<表示> = 屋外駐車場や建物に付属する屋外駐車スペースで、見たりクリックしたりできる車。\n" +
                    "<屋内> = 建物やガレージの中。\n" +
                    "<OC> = 市境の外部接続保管エリア。一部の流入世帯の車は待機場所としてここから始まります。\n" +
                    "駐車車線が割り当てられていない車はここでは省略され、情報タブのログレポートにのみ表示されます。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "更新" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "これらの市全体ステータス値が最後に更新された時刻です。" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "MOD名" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "バージョン" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods リンク" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods で作者のページを開きます。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "駐車レポートを書き出す" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "路上駐車と関連情報を \n" +
                    "<Logs/ParkingControl.log> に書き出します。\n" +
                    "気になる場合は、同じ都市を読み込んだまま後で2回目のレポートを作成できます。\n" +
                    "- 異なるカテゴリから最大20件のサンプルEntity IDを比較します。\n" +
                    "- 各サンプルがそのまま、走行開始、別の場所に駐車、消滅したかを示します。\n" +
                    "- 都市内でEntity IDの番号を追跡するには、Scene Explorer modが必要です。"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "ログを開く" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "<Logs/ParkingControl.log> を開きます。まだ存在しない場合は Logs フォルダーを開きます。" },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "都市はまだ読み込まれていません。" },
                { ParkingStatusLocale.kCollecting, "駐車状況を収集中..." },
                { ParkingStatusLocale.kUnavailable, "駐車状況を取得できません。" },
                { ParkingStatusLocale.kCollectionFailed, "駐車状況を収集できませんでした。ParkingControl.log を確認してください。" },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} 駐車中 ({1} 車線) | {2}/{3} 無効{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} 駐車中 ({1}/{2} 車線) | {3}/{4} 無効 | {5}/{6} 地区{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 路上 | {1} 表示 | {2} 屋内 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} 公共 {1}/{2} | {3} 建物 {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} 路上駐車 {1} | {2} 稼働中" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "オフ" },
                { ParkingStatusLocale.kStatusCheck, "確認" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

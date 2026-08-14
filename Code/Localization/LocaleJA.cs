// <copyright file="LocaleJA.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Japanese text for Parking Control's Options UI.

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
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
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
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "操作" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "情報" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "路上駐車" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "自家用車の状態" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "MOD情報" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "リンク" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "診断" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "路上駐車禁止（市全体）" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)),
                    "- 今後、自家用車とオートバイが路上駐車を利用できないようにします。\n" +
                    "- 駐車場や建物に付属する駐車スペースは引き続き利用できます。\n" +
                    "- すでに駐車中の車両は削除されません。市民が次にその車を使用すると自然に移動します。\n" +
                    "- 路外駐車が十分にないと、空きを探して車が長時間走り回ることがあります。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路上駐車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<駐車中> = 道路上にまだ駐車している車。\n" +
                    "<車線> = それらの車が駐車している道路脇の駐車区画。1つの車線に複数の車を駐車できます。\n" +
                    "<無効> = 新たな駐車を受け付けない路上駐車車線。\n" +
                    "<OK> = 路上駐車禁止が有効で正常に動作しています。\n" +
                    "<オフ> = 路上駐車禁止が無効です。通常の道路には自由に駐車できます。\n" +
                    "<確認> = 道路がまだ更新中の可能性があります。少し待ち、続く場合はログレポートを書き出してください。\n" +
                    "**路上駐車禁止を有効にした後や道路を変更した後も、一部の車が残ることがあります。市民がその車を使用すると自然に移動します。**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "車の場所" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<路上> = 公道に駐車中。\n" +
                    "<表示> = 屋外駐車場や建物に付属する屋外駐車スペースで、見たりクリックしたりできる車。\n" +
                    "<非表示> = 建物やガレージの中。\n" +
                    "<OC> = 市境の外部接続ストレージ。一部の流入世帯の車はここから始まります。\n" +
                    "**未割り当てのゲーム本体の待機領域はログにのみ記録されます。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "駐車場" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<使用率> と <使用中 / 合計> は駐車場の利用状況を示します。\n" +
                    "<公共> = ゲーム本体の「駐車場」情報ビューで集計される施設。\n" +
                    "<建物> = 住宅や職場に付属する駐車場。\n" +
                    "**建物の駐車場には、表示される屋外スペースと内部駐車場の両方が含まれます。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "路上利用率" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<路上駐車> = 既知の路上・公共・建物駐車場を利用している車のうち、路上駐車している割合。\n" +
                    "<移動中> = 走行中または渋滞で待機中の自家用車。\n" +
                    "<更新> = 状態の最終更新時刻。\n" +
                    "**外部接続（OC）と未割り当ての待機領域は除外されます。**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "MOD名" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "バージョン" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods リンク" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods で作者のページを開きます。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "駐車レポートを書き出す" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "路上駐車、駐車供給、所有状況、車両位置の詳細を\n" +
                    "<ParkingControl.log> に書き出します\n" +
                    "同じ都市で2回目のレポートを作成すると、同じ路上駐車車両のエンティティIDを追跡します。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "ログを開く" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "<ParkingControl.log> を開きます。まだ存在しない場合は Logs フォルダーを開きます。" },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "都市はまだ読み込まれていません。" },
                { ParkingStatusLocale.kCollecting, "駐車状況を収集中..." },
                { ParkingStatusLocale.kUnavailable, "駐車状況を取得できません。" },
                { ParkingStatusLocale.kCollectionFailed, "駐車状況を収集できませんでした。ParkingControl.log を確認してください。" },
                { ParkingStatusLocale.kEnforcementFormat, "{0} 駐車中（{1} 車線） | {2}/{3} 無効 | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 路上 | {1} 表示 | {2} 非表示 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} 公共 | {3}  {4} / {5} 建物" },
                { ParkingStatusLocale.kShareFormat, "{0} 路上駐車 | {1} 移動中 | 更新 {2}" },
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

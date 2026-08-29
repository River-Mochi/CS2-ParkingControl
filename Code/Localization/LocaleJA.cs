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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "診断" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "路上駐車禁止" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "選択:\n" +
                    "<1. 地区ごと>\n" +
                    "<2. 市全体>\n" +
                    "<3. オフ>\n" +
                    "- 対象車線を無効にして、新たな路上駐車を防ぎます。\n" +
                    "- 禁止後、駐車中の車は徐々に移動します。広い禁止区域ほど時間がかかります。\n" +
                    "- 有料駐車場と通常の建物駐車は引き続き利用できます。\n" +
                    "**高速道路や小型の双方向路地など、一部の道路はもともと路上駐車できません。**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "市全体" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "地区ごと" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "オフ" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "手順を表示" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "<地区ごと>モードの使い方を表示します。\n" +
                    "1.a. オフ = 市全体と地区の制限を無効にし、ほぼゲーム標準に戻します。\n" +
                    "1.b. 道路サービスの道路単位<駐車禁止>ボタンは、横断歩道と同じように引き続き使えます。\n" +
                    "2. 市全体 = 市内の対象となる公共の路上駐車をすべて禁止します。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "状況を表示" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<現在の駐車状況を下に表示します。>\n" +
                    "状況はオプション画面を開いている間だけ集計されます。\n" +
                    "通常の都市プレイ中にバックグラウンドスキャンは行いません。"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<地区モード>\n" +
                    "1. 上で<地区ごと>を選びます。\n" +
                    "2. 地区を作成/選択します。\n" +
                    "3. <条例>を開き、**路上駐車禁止 [✓]**を有効にします。\n" +
                    "4. 禁止と駐車料金は同時に有効にできます。残っている車や、禁止後も駐車した車には料金がかかります。\n" +
                    "禁止地区の外では通常の路上駐車ができます。"
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "駐車禁止" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "道路の片側の路上駐車を切り替えます。複数の側を変更するには、左マウスボタンを離す前にその上をドラッグします。" },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "路上駐車禁止" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "この地区では、自動車とオートバイの路上駐車を禁止します。\n" +
                    "- 駐車中の車は徐々に移動します。広い禁止区域ほど時間がかかります。"
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "追加" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "解除" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "路上駐車" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "選択中の<市全体>または<地区ごと>の禁止範囲だけを表示します。手動の駐車禁止は別表示です。\n" +
                    "<オフ> = 市全体/地区の禁止は無効。手動<駐車禁止>道路は有効のままです。\n" +
                    "<駐車中> = 選択範囲の道路にまだ駐車している車。\n" +
                    "<無効> = 無効な路肩車線区間 / 対象区間。\n" +
                    "<地区> = 禁止が有効な地区 / 全地区。\n" +
                    "<確認> = 一部の対象区間がまだ選択中の禁止状態と一致していません。\n" +
                    "<---------------------->\n" +
                    "**道路変更や再建後に[確認]が出たら、都市をしばらく進めてから オプション > 状況 を開き直してください。残る場合は 情報 > 診断 > レポート出力 を使ってください。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "手動駐車禁止" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "手動<駐車禁止>ツールで設定した道路だけを表示します。\n" +
                    "<駐車中> = 手動で禁止した道路にまだ駐車している車。\n" +
                    "<無効> = 無効な路肩車線区間 / 手動対象区間。\n" +
                    "手動禁止は市全体や地区の禁止と重なる場合があります。この行を<路上駐車>の合計に足さないでください。\n" +
                    "**都市をしばらく進めても[確認]が出る場合は、情報 > 診断 > レポート出力 を使い、問い合わせ時に提出してください。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "駐車利用" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<市全体>の駐車利用状況を表示します。全市 / 地区別の駐車禁止範囲には従いません。\n" +
                    "<公共> = 公共駐車施設の使用中 / 総スペース数。\n" +
                    "CS2の道路・駐車情報ビューと同じ駐車施設データを使用します。\n" +
                    "<建物> = 建物やガレージに駐車している車。\n" +
                    "<路上> = 道路に駐車している車。\n" +
                    "<合計> = 市内で把握できる駐車車両の合計（路上 + 公共 + 建物）。\n" +
                    "**外部接続と場所不明の待機車両は合計から除外されます。**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "駐車評価" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<市全体>の公共駐車場の空き状況を表示します。\n" +
                    "<不足> = 空き15%未満。\n" +
                    "<OK> = 空き15%以上30%未満。\n" +
                    "<良好> = 空き30%以上。\n" +
                    "<公共空き> = 現在未使用の公共駐車スペース。\n" +
                    "ゲームの道路・駐車情報ビューと同じ駐車施設を数えます。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "車の場所" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "この行は、禁止地区だけでなく市全体のデータを表示します。\n" +
                    "<路上> = 公道に駐車中。\n" +
                    "<表示> = 屋外駐車場や建物の屋外駐車スペースで見たりクリックしたりできる車。\n" +
                    "<屋内> = 建物やガレージの中。\n" +
                    "<OC> = 市境の外部接続保管エリア。一部の流入世帯の車はここから始まります（待機場所）。\n" +
                    "駐車車線が割り当てられていない車はここでは省略され、ログレポート（情報タブ）にのみ表示されます。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "更新" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "これらの市全体ステータス値が最後に更新された時刻です。" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "MOD名" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "バージョン" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods リンク" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods で作者のページを開きます。" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "レポート出力" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "路上駐車と関連情報を \n" +
                    "<Logs/ParkingControl.log> に書き出します。\n" +
                    "必要なら、同じ都市を読み込んだまま後で2回目のレポートを作成できます。\n" +
                    "- 異なるカテゴリから最大20件のサンプルEntity IDを比較します。\n" +
                    "- 各サンプルが残った、走行開始、別の場所に駐車、消滅したかを示します。\n" +
                    "- 都市内でEntity IDを追跡するにはScene Explorerが必要です。"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "ログを開く" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "<Logs/ParkingControl.log> を開きます。まだない場合は Logs フォルダーを開きます。" },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "詳細ログ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "自動DEBUG情報。\n" +
                    "通常プレイ用ではありません。デバッグしない場合はオフにしてください。\n" +
                    "レポート出力はオフでも使えます。"
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "都市はまだ読み込まれていません。" },
                { ParkingStatusLocale.kCollecting, "駐車状況を収集中..." },
                { ParkingStatusLocale.kUnavailable, "駐車状況を取得できません。" },
                { ParkingStatusLocale.kCollectionFailed, "駐車状況を収集できませんでした。ParkingControl.log を確認してください。" },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} 駐車中 | {1}/{2} 無効{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} 駐車中 | {1}/{2} 車線無効{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} 駐車中 | {1}/{2} 無効 | {3}/{4} 地区{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} 路上 | {1} 表示 | {2} 屋内 | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1}、公共空き {2}" },
                { ParkingStatusLocale.kShareFormat, "{0} 公共 | {1} 建物 | {2} 路上 | {3} 合計" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "オフ = 市全体/地区の禁止なし | 手動道路は有効" },
                { ParkingStatusLocale.kManualNone, "設定なし" },
                { ParkingStatusLocale.kStatusCheck, "確認" },
                { ParkingStatusLocale.kRatingPoor, "不足" },
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

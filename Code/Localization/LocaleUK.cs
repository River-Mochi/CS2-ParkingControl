// <copyright file="LocaleUK.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Ukrainian text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Ukrainian localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleUK : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleUK"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleUK(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Дії" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Про мод" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Вуличне паркування" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Стан особистих авто" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Інформація про мод" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Посилання" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Діагностика" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Без вуличного паркування" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Виберіть:\n" +
                    "<1. за районами>\n" +
                    "<2. Усе місто>\n" +
                    "<3. ВИМК.>\n" +
                    "- Доступні смуги блокуються для нового вуличного паркування.\n" +
                    "- Уже припарковані авто поступово переміщуються після заборони; великі зони потребують більше часу.\n" +
                    "- Платні паркінги та звичайні місця біля будівель залишаються доступними.\n" +
                    "**Деякі дороги вже не дозволяють вуличне паркування, наприклад автомагістралі та невеликі двосторонні провулки.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Усе місто" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "за районами" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "ВИМК." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Показати інструкції" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Показує режим <за районами>.\n" +
                    "1.a. ВИМК. = заборони для всього міста й районів вимкнено; переважно повернення до стандартної гри.\n" +
                    "1.b. Кнопка <Стоянку заборонено> для окремої дороги в дорожніх службах усе одно працює як пішохідний перехід.\n" +
                    "2. Усе місто = блокує все доступне громадське вуличне паркування."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Показати стан" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Показує нижче поточні підсумки паркування.>\n" +
                    "Дані збираються лише поки відкрите меню налаштувань;\n" +
                    "під час гри фонове сканування не виконується."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Режим за районами>\n" +
                    "1. Виберіть <за районами> вище.\n" +
                    "2. Створіть/виберіть район.\n" +
                    "3. Відкрийте <Політики> та ввімкніть **Заборона вуличного паркування [✓]**.\n" +
                    "4. Заборону й плату за паркування можна ввімкнути одночасно. Плата стягується з авто, які ще залишилися або все ж припаркувалися.\n" +
                    "Дороги поза районами із забороною зберігають звичайне вуличне паркування."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Стоянку заборонено" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Вмикає або вимикає паркування з одного боку дороги. Для кількох сторін проведіть по них до відпускання лівої кнопки миші." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Заборона вуличного паркування" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Не дозволяє автомобілям і мотоциклам паркуватися вздовж доріг у цьому районі.\n" +
                    "- Уже припарковані авто поступово переміщуються; великі зони потребують більше часу."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Додати" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Прибрати" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Вуличне паркування" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Показує лише вибрану область <Усе місто> або <за районами>. Ручні заборони показуються окремо.\n" +
                    "<ВИМК.> = заборони міста/районів вимкнено; дороги з ручним <Стоянку заборонено> залишаються активними.\n" +
                    "<Припарковано> = авто, які ще стоять на вулицях вибраної області.\n" +
                    "<Вимкнено> = вимкнені ділянки смуги біля бордюру / цільові ділянки.\n" +
                    "<Райони> = райони із забороною / усі райони.\n" +
                    "<ПЕРЕВІРИТИ> = деякі цільові ділянки ще не відповідають вибраній забороні.\n" +
                    "<---------------------->\n" +
                    "**Якщо [ПЕРЕВІРИТИ] з’явиться після зміни або перебудови доріг, запустіть місто на деякий час і знову відкрийте Налаштування > Стан. Якщо лишиться, використайте Про мод > Діагностика > Записати звіт.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Ручна заборона" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Показує лише дороги, задані ручним інструментом <Стоянку заборонено>.\n" +
                    "<Припарковано> = авто, які ще стоять на цих вручну заборонених дорогах.\n" +
                    "<Вимкнено> = вимкнені ділянки смуги біля бордюру / вручну вибрані ділянки.\n" +
                    "Ручні заборони можуть накладатися на Усе місто або райони; не додавайте цей рядок до підсумків <Вуличне паркування>.\n" +
                    "**Якщо [ПЕРЕВІРИТИ] з’являється після роботи міста деякий час, використайте Про мод > Діагностика > Записати звіт і надішліть його, коли звертаєтесь по допомогу.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Використання паркінгу" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Показує використання паркування в <усьому місті>. Не залежить від області заборони Все місто / за районами.\n" +
                    "<Громадські> = зайняті / усі місця на громадських паркінгах.\n" +
                    "Використовує ті самі дані паркінгів, що й панель паркування Доріг у CS2.\n" +
                    "<Будинки> = авто, припарковані в будівлях або гаражах.\n" +
                    "<Вулиця> = авто, припарковані на вулицях.\n" +
                    "<Усього> = відомі припарковані авто в місті (вулиця + громадські + будинки).\n" +
                    "**Зовнішні з’єднання та невідомі зони очікування не враховуються.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Оцінка паркування" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Показує доступність громадського паркування в <усьому місті>.\n" +
                    "<ПОГАНО> = вільно менше 15%.\n" +
                    "<OK> = вільно від 15% до менше 30%.\n" +
                    "<ДОБРЕ> = вільно 30% або більше.\n" +
                    "<Громадські вільні> = наразі невикористані громадські паркомісця.\n" +
                    "Рахує ті самі паркінги, що й панель паркування Доріг у грі."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Розташування авто" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Цей рядок показує дані по всьому місту, а не лише по районах із забороною.\n" +
                    "<Вулиця> = припарковані на громадських дорогах.\n" +
                    "<Видимі> = авто, які видно й можна вибрати на відкритих паркінгах або зовнішніх місцях біля будівель.\n" +
                    "<Всередині> = у будівлях або гаражах.\n" +
                    "<OC> = сховище авто на зовнішньому з’єднанні біля межі міста; деякі авто нових домогосподарств починають там (зона очікування).\n" +
                    "Авто без призначеної паркувальної смуги тут не показуються й є лише у звіті журналу (вкладка Про мод)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Оновлено" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Час останнього оновлення цих загальноміських значень." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Назва моду" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Версія" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Посилання Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Відкрити сторінку автора на Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Записати звіт" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Записує дані про вуличне паркування та пов’язані подробиці до \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "За потреби запишіть 2-й звіт пізніше в тому самому завантаженому місті.\n" +
                    "- Порівнює до 20 зразків Entity ID з різних категорій.\n" +
                    "- Показує, чи кожен зразок залишився, поїхав, припаркувався деінде або зник.\n" +
                    "- Для відстеження Entity ID у місті потрібен Scene Explorer."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Відкрити журнал" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Відкриває <Logs/ParkingControl.log> або папку Logs, якщо файл ще не існує." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Докладний журнал" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Автоматичні подробиці DEBUG.\n" +
                    "Не для звичайної гри; вимкніть, якщо не виконуєте діагностику.\n" +
                    "Записати звіт працює і коли вимкнено."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Місто ще не завантажено." },
                { ParkingStatusLocale.kCollecting, "Збирається стан паркування..." },
                { ParkingStatusLocale.kUnavailable, "Стан паркування недоступний." },
                { ParkingStatusLocale.kCollectionFailed, "Не вдалося зібрати стан паркування; див. ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} припарк. | {1}/{2} вимкн.{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} припарк. | {1}/{2} смуг вимкн.{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} припарк. | {1}/{2} вимкн. | {3}/{4} районів{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} вулиця | {1} видимі | {2} всередині | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1}, гром. вільно {2}" },
                { ParkingStatusLocale.kShareFormat, "{0} гром. | {1} буд. | {2} вулиця | {3} усього" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "ВИМК. = заборони міста/районів вимк. | ручні дороги активні" },
                { ParkingStatusLocale.kManualNone, "Немає" },
                { ParkingStatusLocale.kStatusCheck, "ПЕРЕВІРИТИ" },
                { ParkingStatusLocale.kRatingPoor, "ПОГАНО" },
                { ParkingStatusLocale.kRatingGood, "ДОБРЕ" },
                { ParkingStatusLocale.kRatingNA, "Н/Д" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

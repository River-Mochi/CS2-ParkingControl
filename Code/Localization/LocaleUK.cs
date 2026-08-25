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
                    "**за районами**\n" +
                    "**Усе місто**\n" +
                    " або **ВИМК.**.\n" +
                    "- Доступні смуги блокуються для нового паркування на вулиці.\n" +
                    "- Уже припарковані авто виїдуть природно, коли ними скористаються наступного разу.\n" +
                    "- Платні паркінги та звичайні місця біля будівель залишаються доступними.\n" +
                    "**Деякі дороги вже не дозволяють вуличне паркування, наприклад автомагістралі та невеликі двосторонні провулки.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Усе місто" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "за районами" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "ВИМК." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Показати інструкції" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Показує, як користуватися режимом <за районами>.\n" +
                    "1.a. ВИМК. = обмеження для всього міста й районів вимкнено; переважно повернення до стандартної поведінки гри.\n" +
                    "1.b. Кнопка <Стоянку заборонено> для окремої дороги в панелі дорожніх служб усе одно працює, як додавання пішохідного переходу.\n" +
                    "2. Усе місто = блокує все доступне громадське вуличне паркування в місті."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Показати стан" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Показує нижче поточні підсумки паркування.>\n" +
                    "Дані збираються лише поки відкрите меню налаштувань; " +
                    "під час гри фонове сканування не виконується." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Режим за районами>\n" +
                    "1. Виберіть <за районами> вище.\n" +
                    "2. Створіть або виберіть район у місті.\n" +
                    "3. Відкрийте <Політики> та ввімкніть **Заборона вуличного паркування [✓]**.\n" +
                    "4. Заборону й плату за паркування можна ввімкнути одночасно. Плата стягується з авто, які ще залишилися або все ж змогли припаркуватися.\n" +
                    "Дороги поза районами із забороною зберігають звичайне вуличне паркування." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Стоянку заборонено" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]",
                    "Вмикає або вимикає паркування з одного боку дороги. Для кількох сторін проведіть по них, утримуючи ліву кнопку миші, і відпустіть її в кінці." },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Заборона вуличного паркування" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Не дозволяє автомобілям і мотоциклам паркуватися вздовж доріг у цьому районі. " +
                    "Уже припарковані авто виїдуть, коли власники скористаються ними наступного разу."
                },
                // Native mouse action hints for the No Parking road tool.
                {
                    $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]",
                    "Додати"
                },
                {
                    $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]",
                    "Прибрати"
                },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Вуличне паркування" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Припарковано> = авто, які ще стоять на сторонах доріг, де Parking Control встановив заборону паркування.\n" +
                    "<Смуги> = ділянки паркування вздовж дороги, де стоять ці авто. Одна ділянка може вміщати багато авто.\n" +
                    "<Вимкнено> = ділянки паркувальних смуг, закриті для нових авто. Одна дорога може містити кілька ділянок.\n" +
                    "<ВИМК. + ручна заборона> = ВИМК. вимикає заборони для всього міста й районів, але вручну встановлені заборони на сторонах доріг залишаються активними. Тоді цей рядок показує лише ці ручні заборони.\n" +
                    "<---------------------->\n" +
                    "Якщо вибрано <за районами>, показує:\n" +
                    "- Зайняті смуги в районах із забороною / зайняті смуги по всьому місту.\n" +
                    "- Вимкнені смуги / доступні смуги міста.\n" +
                    "- Райони з увімкненою політикою / усі райони.\n" +
                    "<---------------------->\n" +
                    "Примітка: після зміни або перебудови доріг кількості вимкнених ділянок може знадобитися трохи часу, поки CS2 перебудовує паркувальні смуги. Запустіть місто ненадовго й знову відкрийте Налаштування." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Використання вулиць" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Цей рядок охоплює <все місто>, а не лише райони.\n" +
                    "<На вулиці> = відсоток авто, припаркованих на вулицях замість громадських або будинкових паркінгів.\n" +
                    "<Активні> = особисті авто, що їдуть або стоять у заторі.\n" +
                    "<Формула> = вулиця ÷ (вулиця + зайняті громадські + зайняті будинкові).\n" +
                    "**Сховище на зовнішніх з'єднаннях (OC) та авто без призначеної паркувальної смуги не враховуються.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Паркомісця" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Показує зайнятість паркування по всьому місту.\n" +
                    "<Громадські> зайняті = місця в об'єктах, які рахує стандартний Parking InfoView гри.\n" +
                    "<Будинки> зайняті = паркування при житлі, робочих місцях і магазинах.\n" +
                    "**Вищий % використання = місту може знадобитися більше паркомісць.**" },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Розташування авто" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Цей рядок показує дані по всьому місту, а не лише по районах із забороною.\n" +
                    "<Вулиця> = припарковані на громадських дорогах.\n" +
                    "<Видимі> = авто, які можна побачити й вибрати на відкритих паркінгах або зовнішніх місцях біля будівель.\n" +
                    "<Всередині> = у будівлях або гаражах.\n" +
                    "<OC> = сховище авто на зовнішньому з'єднанні біля межі міста; деякі авто нових домогосподарств починають там як у зоні очікування.\n" +
                    "Авто без призначеної паркувальної смуги тут не показуються й є лише у звіті журналу (вкладка Про мод)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Оновлено" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "Час останнього оновлення цих загальноміських значень." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Назва моду" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Версія" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Посилання Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Відкрити сторінку автора на Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Записати звіт про паркування" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Записує дані про вуличне паркування та пов'язані подробиці до \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "За потреби запишіть 2-й звіт пізніше в тому самому завантаженому місті.\n" +
                    "- Порівнює до 20 прикладів Entity ID з різних категорій.\n" +
                    "- Показує, чи кожен приклад залишився, поїхав, припаркувався деінде або зник.\n" +
                    "- Для відстеження номерів Entity ID у місті потрібен мод Scene Explorer."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Відкрити журнал" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Відкрити <Logs/ParkingControl.log> або папку Logs, якщо файл ще не існує." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Місто ще не завантажено." },
                { ParkingStatusLocale.kCollecting, "Збір даних про паркування..." },
                { ParkingStatusLocale.kUnavailable, "Дані про паркування недоступні." },
                { ParkingStatusLocale.kCollectionFailed, "Не вдалося зібрати дані про паркування; див. ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} припарк. ({1} смуг) | {2}/{3} вимкн.{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} припарк. ({1}/{2} смуг) | {3}/{4} вимкн. | {5}/{6} районів{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} вулиця | {1} видимі | {2} всередині | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} громад. {1}/{2} | {3} будинки {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} на вулиці {1} | {2} активні" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "ВИМК." },
                { ParkingStatusLocale.kStatusCheck, "ПЕРЕВ." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

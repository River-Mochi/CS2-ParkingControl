// <copyright file="LocalePL.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Polish text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Polish localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocalePL : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalePL"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocalePL(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Działania" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "O modzie" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Parkowanie przy ulicy" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Stan pojazdów prywatnych" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informacje o modzie" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Linki" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostyka" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Zakaz parkowania przy ulicy" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Wybierz **Całe miasto**, **Według dzielnic** albo **WYŁ.**.\n" +
                    "- Odpowiednie pasy są blokowane, aby uniemożliwić nowe parkowanie przy ulicy.\n" +
                    "- Już zaparkowane auta odjadą, gdy zostaną ponownie użyte.\n" +
                    "- Płatne parkingi i zwykłe miejsca przy budynkach pozostają dostępne.\n" +
                    "**Autostrady i asymetryczne drogi 3-pasmowe już nie pozwalają na parkowanie przy ulicy.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Całe miasto" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Według dzielnic" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "WYŁ." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Pokaż instrukcje" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Pokazuje, jak korzystać z trybu <Według dzielnic>.\n" +
                    "WYŁ. = ograniczenia parkowania przy ulicy są wyłączone.\n" +
                    "Całe miasto = odpowiednie miejsca przy ulicy są zablokowane w całym mieście." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Pokaż stan" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Pokaż poniżej bieżące dane o parkowaniu.>\n" +
                    "Stan jest zbierany tylko wtedy, gdy menu Opcje jest " +
                    "otwarte; podczas gry nie działa żaden skan stanu w tle." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Tryb dzielnic>\n" +
                    "1. Wybierz wyżej <Według dzielnic>.\n" +
                    "2. Utwórz lub wybierz dzielnicę w mieście.\n" +
                    "3. Otwórz <Polityki> i włącz **Zakaz parkowania przy drodze [✓]**.\n" +
                    "Poza wybranymi dzielnicami pozostaje zwykłe parkowanie." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Zakaz parkowania przy drodze" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "Uniemożliwia <samochodom i motocyklom> parkowanie przy drodze w tej " +
                    "dzielnicy. Już zaparkowane pojazdy odjadą, gdy właściciele ponownie ich użyją." },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Parking uliczny" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Zaparkowane> = auta nadal stojące na ulicach objętych wybranym trybem.\n" +
                    "<Pasy> = odcinki parkingowe przy drodze zajęte przez te auta. Jeden pas może mieścić kilka aut.\n" +
                    "<Wyłączone> = pasy parkowania przy ulicy zamknięte dla nowych pojazdów.\n" +
                    "<Według dzielnic> pokazuje:\n" +
                    "- Zajęte pasy w dzielnicach z zakazem / zajęte pasy w całym mieście.\n" +
                    "- Wyłączone pasy / odpowiednie pasy w mieście.\n" +
                    "- Włączone dzielnice / wszystkie dzielnice.\n" +
                    "<Nowe lub przebudowane drogi> mogą przez chwilę przyjąć kilka aut podczas aktualizacji pasów.\n" +
                    "Już zaparkowane auta odjadą naturalnie.\n" +
                    "<SPRAWDŹ> = niektóre wybrane drogi nie są jeszcze zablokowane. Uruchom miasto na chwilę i " +
                    "sprawdź ponownie. Jeśli <SPRAWDŹ> pozostaje, dołącz raport parkowania przy prośbie o pomoc." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Użycie ulic" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Ten wiersz obejmuje <całe miasto>, nie tylko dzielnice.\n" +
                    "<Zaparkowane na ulicy> = procent aut zaparkowanych na ulicach zamiast na parkingach publicznych lub przy budynkach.\n" +
                    "<Aktywne> = pojazdy prywatne jadące lub stojące w ruchu.\n" +
                    "<Wzór> = ulica ÷ (ulica + zajęte publiczne + zajęte przy budynkach).\n" +
                    "**Magazyn połączeń zewnętrznych (OC) i auta bez przypisanego pasa parkingowego są wykluczone.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Miejsca" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Pokazuje zajętość parkingów w całym mieście.\n" +
                    "<Publ.> zajęte = obiekty liczone przez podstawowy widok informacji o parkowaniu.\n" +
                    "<Budynek> zajęte = miejsca przy domach, miejscach pracy i sklepach.\n" +
                    "**Wyższy % wykorzystania = może być potrzebnych więcej miejsc parkingowych.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Położenie aut" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Ten wiersz pokazuje dane całego miasta, nie tylko dzielnic z zakazem.\n" +
                    "<Ulica> = zaparkowane na drogach publicznych.\n" +
                    "<Widoczne> = auta widoczne i klikalne na otwartych parkingach lub zewnętrznych miejscach przy budynkach.\n" +
                    "<Ukryte> = wewnątrz budynków lub garaży.\n" +
                    "<OC> = magazyn połączenia zewnętrznego na granicy miasta; niektóre auta przybywających gospodarstw zaczynają tam jako strefa oczekiwania.\n" +
                    "Auta bez przypisanego pasa parkingowego są tu pomijane i pokazywane tylko w raporcie logu na karcie O modzie." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Zaktualizowano" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "Czas ostatniego odświeżenia tych wartości stanu dla całego miasta." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nazwa moda" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Wersja" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Otwiera stronę autora w Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Zapisz raport parkowania" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Zapisuje szczegóły parkowania przy ulicy i powiązane dane do \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Jeśli chcesz sprawdzić szczegóły, utwórz później drugi raport w tym samym wczytanym mieście.\n" +
                    "- Porównuje do 20 przykładowych ID encji z różnych kategorii.\n" +
                    "- Pokazuje, czy każdy przykład pozostał, ruszył, zaparkował gdzie indziej czy zniknął.\n" +
                    "- Mod Scene Explorer jest potrzebny do śledzenia numerów ID encji w mieście."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Otwórz log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Otwiera <Logs/ParkingControl.log> albo folder Logs, jeśli plik jeszcze nie istnieje." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Nie wczytano miasta." },
                { ParkingStatusLocale.kCollecting, "Trwa zbieranie stanu parkowania..." },
                { ParkingStatusLocale.kUnavailable, "Stan parkowania jest niedostępny." },
                { ParkingStatusLocale.kCollectionFailed, "Nie udało się zebrać stanu parkowania; zobacz ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} zapark. ({1} pasy) | {2}/{3} wył.{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} zapark. ({1}/{2} pasy) | {3}/{4} wył. | {5}/{6} dzielnic{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} ulica | {1} widoczne | {2} ukryte | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} publ. {1}/{2} | {3} budynki {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} na ulicy {1} | {2} aktywne" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "WYŁ." },
                { ParkingStatusLocale.kStatusCheck, "SPRAWDŹ" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

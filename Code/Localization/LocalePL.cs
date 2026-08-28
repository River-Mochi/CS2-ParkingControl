// <copyright file="LocalePL.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Diagnostyka" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Zakaz parkowania przy ulicy" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Wybierz:\n" +
                    "<1. Według dzielnic>\n" +
                    "<2. Całe miasto>\n" +
                    "<3. WYŁ.>\n" +
                    "- Odpowiednie pasy są blokowane, aby uniemożliwić nowe parkowanie przy ulicy.\n" +
                    "- Już zaparkowane auta przenoszą się stopniowo po wprowadzeniu zakazu; duże obszary potrzebują więcej czasu.\n" +
                    "- Płatne parkingi i zwykłe miejsca przy budynkach pozostają dostępne.\n" +
                    "**Niektóre drogi już nie pozwalają na parkowanie przy ulicy, np. autostrady i małe dwukierunkowe alejki.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Całe miasto" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Według dzielnic" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "WYŁ." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Pokaż instrukcje" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Pokazuje tryb <Według dzielnic>.\n" +
                    "1.a. WYŁ. = ograniczenia miasta i dzielnic są wyłączone; w większości wraca zachowanie gry.\n" +
                    "1.b. Przycisk <Zakaz parkowania> dla pojedynczej drogi w Usługach drogowych nadal działa jak przejście dla pieszych.\n" +
                    "2. Całe miasto = blokuje wszystkie odpowiednie publiczne miejsca przy ulicach."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Pokaż stan" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Pokaż poniżej bieżące dane o parkowaniu.>\n" +
                    "Stan jest zbierany tylko wtedy, gdy menu Opcje jest otwarte;\n" +
                    "podczas gry nie działa skan w tle."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Tryb dzielnic>\n" +
                    "1. Wybierz wyżej <Według dzielnic>.\n" +
                    "2. Utwórz/wybierz dzielnicę.\n" +
                    "3. Otwórz <Polityki> i włącz **Zakaz parkowania przy drodze [✓]**.\n" +
                    "4. Zakaz i opłata za parkowanie mogą być włączone razem. Opłata obejmuje auta, które jeszcze zostały lub mimo zakazu zaparkują.\n" +
                    "Poza dzielnicami z zakazem pozostaje zwykłe parkowanie przy ulicy."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Zakaz parkowania" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Włącza lub wyłącza parkowanie po jednej stronie drogi. Aby zmienić kilka stron, przeciągnij po nich przed puszczeniem lewego przycisku myszy." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Zakaz parkowania przy drodze" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Uniemożliwia samochodom i motocyklom parkowanie przy drodze w tej dzielnicy.\n" +
                    "- Już zaparkowane auta przenoszą się stopniowo; duże obszary potrzebują więcej czasu."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Dodaj" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Usuń" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Parking uliczny" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Pokazuje tylko wybrany zakres <Całe miasto> lub <Według dzielnic>. Ręczne zakazy są osobno.\n" +
                    "<WYŁ.> = zakazy miasta/dzielnic są wyłączone; ręczne drogi <Zakaz parkowania> pozostają aktywne.\n" +
                    "<Zaparkowane> = auta nadal stojące na ulicach w wybranym zakresie.\n" +
                    "<Wyłączone> = wyłączone odcinki pasa przy krawężniku / odcinki docelowe.\n" +
                    "<Dzielnice> = dzielnice z zakazem / wszystkie dzielnice.\n" +
                    "<SPRAWDŹ> = część odcinków docelowych nie odpowiada jeszcze wybranemu zakazowi.\n" +
                    "<---------------------->\n" +
                    "**Jeśli [SPRAWDŹ] pojawi się po zmianie lub przebudowie dróg, uruchom miasto na chwilę i ponownie otwórz Opcje > Stan. Jeśli nadal jest, użyj O modzie > Diagnostyka > Zapisz raport.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Ręczny zakaz" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Pokazuje tylko drogi ustawione ręcznym narzędziem <Zakaz parkowania>.\n" +
                    "<Zaparkowane> = auta nadal stojące na tych ręcznie zabronionych drogach.\n" +
                    "<Wyłączone> = wyłączone odcinki pasa przy krawężniku / ręcznie wybrane odcinki.\n" +
                    "Ręczne zakazy mogą nakładać się na Całe miasto lub dzielnice; nie dodawaj tego wiersza do sumy <Parking uliczny>.\n" +
                    "**Jeśli [SPRAWDŹ] pojawia się po krótkim uruchomieniu miasta, użyj O modzie > Diagnostyka > Zapisz raport i dołącz go przy prośbie o pomoc.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Użycie parkingów" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Publiczne> = zajęte miejsca w obiektach parkingowych.\n" +
                    "W przybliżeniu odpowiada panelowi parkowania Dróg w CS2.\n" +
                    "<Budynki> = pojazdy zaparkowane w budynkach lub garażach.\n" +
                    "<Ulica> = auta zaparkowane na ulicach.\n" +
                    "<Suma> = znane zaparkowane auta w mieście (ulica + publiczne + budynki).\n" +
                    "**Połączenia zewnętrzne i nieznane miejsca oczekiwania są wykluczone.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Ocena parkingów" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Ocenia, ile parkingu o dokładnej pojemności jest nadal <wolne>.\n" +
                    "<SŁABO> = mniej niż 15% wolne.\n" +
                    "<OK> = od 15% do mniej niż 30% wolne.\n" +
                    "<DOBRZE> = 30% lub więcej wolne.\n" +
                    "<Publiczne> używa obiektów liczonych przez panel parkowania Dróg w CS2.\n" +
                    "<Budynki> używa parkingów o dokładnej pojemności w budynkach i garażach."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Położenie aut" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Ten wiersz pokazuje dane całego miasta, nie tylko dzielnic z zakazem.\n" +
                    "<Ulica> = zaparkowane na drogach publicznych.\n" +
                    "<Widoczne> = auta widoczne i klikalne na otwartych parkingach lub zewnętrznych miejscach przy budynkach.\n" +
                    "<Wewnątrz> = w budynkach lub garażach.\n" +
                    "<OC> = magazyn połączenia zewnętrznego na granicy miasta; niektóre auta przybywających gospodarstw zaczynają tam (strefa oczekiwania).\n" +
                    "Auta bez przypisanego pasa parkingowego są tu pomijane i pokazywane tylko w raporcie logu (karta O modzie)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Zaktualizowano" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Czas ostatniego odświeżenia tych wartości dla całego miasta." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nazwa moda" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Wersja" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Otwiera stronę autora w Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Zapisz raport" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Zapisuje szczegóły parkowania przy ulicy i powiązane dane do \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "W razie potrzeby utwórz później drugi raport w tym samym wczytanym mieście.\n" +
                    "- Porównuje do 20 przykładowych ID encji z różnych kategorii.\n" +
                    "- Pokazuje, czy każdy przykład pozostał, ruszył, zaparkował gdzie indziej czy zniknął.\n" +
                    "- Scene Explorer jest potrzebny do śledzenia ID encji w mieście."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Otwórz log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Otwiera <Logs/ParkingControl.log> albo folder Logs, jeśli plik jeszcze nie istnieje." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Szczegółowy log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Automatyczne szczegóły DEBUG.\n" +
                    "Nie do normalnej gry; wyłącz, jeśli nie diagnozujesz.\n" +
                    "Zapisz raport działa także po wyłączeniu."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Nie wczytano miasta." },
                { ParkingStatusLocale.kCollecting, "Trwa zbieranie stanu parkowania..." },
                { ParkingStatusLocale.kUnavailable, "Stan parkowania jest niedostępny." },
                { ParkingStatusLocale.kCollectionFailed, "Nie udało się zebrać stanu parkowania; zobacz ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} zapark. | {1}/{2} wył.{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} zapark. | {1}/{2} pasów wył.{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} zapark. | {1}/{2} wył. | {3}/{4} dzielnic{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} ulica | {1} widoczne | {2} wewnątrz | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1} publ. wolne | {2} = {3} budynki wolne" },
                { ParkingStatusLocale.kShareFormat, "{0} publ. | {1} budynki | {2} ulica | {3} suma" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "WYŁ. = zakazy miasta/dzielnic wył. | ręczne drogi działają" },
                { ParkingStatusLocale.kManualNone, "Brak" },
                { ParkingStatusLocale.kStatusCheck, "SPRAWDŹ" },
                { ParkingStatusLocale.kRatingPoor, "SŁABO" },
                { ParkingStatusLocale.kRatingGood, "DOBRZE" },
                { ParkingStatusLocale.kRatingNA, "N/D" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

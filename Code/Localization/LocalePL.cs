// <copyright file="LocalePL.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Polish text for Parking Control's Options UI.

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
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
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
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Działania" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "O modzie" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Parkowanie przy ulicy" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Stan pojazdów prywatnych" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informacje o modzie" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Linki" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostyka" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Zakaz parkowania przy ulicy" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Wybierz, gdzie blokować nowe parkowanie przy ulicy. Już zaparkowane auta odjadą naturalnie; parkingi i miejsca w budynkach pozostaną dostępne." },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Całe miasto" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Według dzielnic" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "Wyłączone" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Pokaż instrukcje" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)), "Pokaż sposób użycia trybu dzielnic." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Pokaż stan" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "Pokaż dane o parkowaniu poniżej. Są zbierane tylko po wyświetleniu." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Tryb dzielnic>\n" +
                    "1. Wybierz wyżej <Według dzielnic>.\n" +
                    "2. Utwórz lub wybierz dzielnicę w mieście.\n" +
                    "3. Otwórz <Polityki> i włącz <Zakaz parkowania przy ulicy>.\n" +
                    "Poza wybranymi dzielnicami pozostaje zwykłe parkowanie." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Zakaz parkowania przy ulicy" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "W trybie Według dzielnic blokuje tu nowe parkowanie przy ulicy. " +
                    "Już zaparkowane auta odjadą po użyciu." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Parkowanie przy ulicy" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Zaparkowane> = auta na ulicach objętych wybranym trybem.\n" +
                    "<Pasy> = odcinki parkingowe przy drodze zajęte przez te samochody. Jeden pas może pomieścić kilka samochodów.\n" +
                    "<Wyłączone> = pasy parkowania przy ulicy zamknięte dla nowych pojazdów.\n" +
                    "<OK> = Zakaz parkowania przy ulicy jest włączony i działa.\n" +
                    "<WYŁ.> = Zakaz parkowania przy ulicy jest wyłączony; samochody mogą swobodnie parkować na zwykłych ulicach.\n" +
                    "<SPRAWDŹ> = drogi mogą być jeszcze aktualizowane. Poczekaj chwilę; jeśli stan się utrzymuje, zapisz raport do logu.\n" +
                    "**Niektóre samochody mogą pozostać po włączeniu zakazu parkowania przy ulicy lub po zmianie dróg. Odjadą naturalnie, gdy obywatel użyje samochodu.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Położenie samochodów" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Ulica> = zaparkowane na drogach publicznych.\n" +
                    "<Widoczne> = samochody, które można zobaczyć i kliknąć na otwartych parkingach lub zewnętrznych miejscach parkingowych przy budynkach.\n" +
                    "<Ukryte> = wewnątrz budynków lub garaży.\n" +
                    "<OC> = magazyn połączenia zewnętrznego na granicy miasta; niektóre samochody przybywających gospodarstw domowych zaczynają tam.\n" +
                    "**Nieprzypisany obszar oczekiwania gry bazowej jest rejestrowany tylko w logu.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parkowanie" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<Procent> i <użyte / łącznie> pokazują zajętość miejsc parkingowych.\n" +
                    "<Publiczne> = obiekty liczone przez podstawowy widok informacji o parkowaniu.\n" +
                    "<Budynek> = miejsca parkingowe należące do domów i miejsc pracy.\n" +
                    "**Parking budynków obejmuje widoczne miejsca zewnętrzne i parking wewnętrzny.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Wykorzystanie ulic" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Zaparkowane na ulicy> = udział samochodów korzystających ze znanego parkowania ulicznego, publicznego lub przy budynkach.\n" +
                    "<W ruchu> = pojazdy prywatne jadące lub czekające w ruchu ulicznym.\n" +
                    "<Zaktualizowano> = ostatnie odświeżenie stanu.\n" +
                    "**Połączenia zewnętrzne (OC) i nieprzypisany obszar oczekiwania są wykluczone.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nazwa moda" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Wersja" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Otwórz stronę autora w Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Zapisz raport parkowania" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Zapisz szczegóły parkowania przy ulicy, podaży miejsc, własności i położenia pojazdów\n" +
                    "do <ParkingControl.log>\n" +
                    "Drugi raport w tym samym wczytanym mieście śledzi te same identyfikatory encji samochodów zaparkowanych przy ulicy." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Otwórz log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Otwórz <ParkingControl.log> albo folder Logs, jeśli plik jeszcze nie istnieje." },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "Nie wczytano miasta." },
                { ParkingStatusLocale.kCollecting, "Trwa zbieranie stanu parkowania..." },
                { ParkingStatusLocale.kUnavailable, "Stan parkowania jest niedostępny." },
                { ParkingStatusLocale.kCollectionFailed, "Nie udało się zebrać stanu parkowania; zobacz ParkingControl.log." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} zaparkowane ({1} pasy) | {2}/{3} wyłączone | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} ulica | {1} widoczne | {2} ukryte | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} publiczne | {3}  {4} / {5} budynek" },
                { ParkingStatusLocale.kShareFormat, "{0} zaparkowane na ulicy | {1} w ruchu | zaktualizowano {2}" },
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

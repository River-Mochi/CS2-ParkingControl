// <copyright file="LocaleDE.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the German text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// German localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleDE : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleDE"/> class.
        /// </summary>
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
        public LocaleDE(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Aktionen" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Über" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Straßenparken" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Status privater Fahrzeuge" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Mod-Informationen" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Links" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnose" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "Kein Straßenparken (gesamte Stadt)" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)),
                    "- Verhindert, dass private Fahrzeuge und Motorräder künftig Straßenparkplätze nutzen.\n" +
                    "- Parkplätze und Stellplätze in Gebäuden bleiben verfügbar.\n" +
                    "- Bereits geparkte Fahrzeuge werden nicht entfernt. Sie fahren auf natürliche Weise weg, sobald ein Bürger das Auto das nächste Mal benutzt.\n" +
                    "- Stelle sicher, dass die Stadt genügend Parkmöglichkeiten abseits der Straße hat, sonst fahren Autos möglicherweise lange auf der Suche nach einem Stellplatz herum." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Straßenparken" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Geparkt> = Autos, die noch auf Straßen geparkt sind.\n" +
                    "<Spuren> = Straßenrand-Parkabschnitte mit diesen Autos. Eine Spur kann mehrere Autos aufnehmen.\n" +
                    "<Deaktiviert> = Straßenparkspuren, die für neues Parken gesperrt sind.\n" +
                    "<OK> = Kein Straßenparken ist aktiviert und funktioniert.\n" +
                    "<AUS> = Kein Straßenparken ist deaktiviert; Autos dürfen auf normalen Straßen frei parken.\n" +
                    "<PRÜFEN> = Straßen werden möglicherweise noch aktualisiert. Warte einen Moment; schreibe einen Log-Bericht, falls dies bestehen bleibt.\n" +
                    "**Einige Autos können nach dem Aktivieren der Regel gegen Straßenparken oder nach Straßenänderungen verbleiben. Sie fahren auf natürliche Weise weg, wenn der Bürger das Auto benutzt.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Auto-Standorte" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Straße> = auf öffentlichen Straßen geparkt.\n" +
                    "<Sichtbar> = Autos, die du auf offenen Parkplätzen oder auf Außenparkplätzen von Gebäuden sehen und anklicken kannst.\n" +
                    "<Versteckt> = in Gebäuden oder Garagen.\n" +
                    "<OC> = Speicher einer Außenverbindung am Stadtrand; einige Autos einziehender Haushalte beginnen dort.\n" +
                    "**Nicht zugewiesene Bereitstellung des Grundspiels wird nur im Log erfasst.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parkplätze" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<Prozent> und <belegt / gesamt> zeigen die Parkplatzbelegung.\n" +
                    "<Öffentlich> = Einrichtungen, die von der Parkplatz-Infoansicht des Grundspiels gezählt werden.\n" +
                    "<Gebäude> = Parkplätze, die zu Wohnungen und Arbeitsplätzen gehören.\n" +
                    "**Gebäudeparkplätze umfassen sichtbare Außenstellplätze und interne Parkplätze.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Straßennutzung" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Auf Straße geparkt> = Anteil der Autos, die bekannte Straßen-, öffentliche oder Gebäudeparkplätze nutzen.\n" +
                    "<In Bewegung> = private Fahrzeuge, die fahren oder im Verkehr warten.\n" +
                    "<Aktualisiert> = letzte Aktualisierung des Status.\n" +
                    "**Außenverbindungen (OC) und nicht zugewiesene Bereitstellung sind ausgeschlossen.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod-Name" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox-Mods-Link" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Die Seite des Autors auf Paradox Mods öffnen." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Parkbericht schreiben" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Details zu Straßenparken, Angebot, Besitz und Fahrzeugstandorten\n" +
                    "in <ParkingControl.log> schreiben\n" +
                    "Ein zweiter Bericht in derselben geladenen Stadt verfolgt dieselben Entitäts-IDs der auf der Straße geparkten Autos." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Log öffnen" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "<ParkingControl.log> öffnen oder den Logs-Ordner, falls die Datei noch nicht vorhanden ist." },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "Lade oder starte eine Stadt, um den Parkstatus anzuzeigen." },
                { ParkingStatusLocale.kCollecting, "Parkstatus wird erfasst..." },
                { ParkingStatusLocale.kUnavailable, "Der Parkstatus ist nicht verfügbar." },
                { ParkingStatusLocale.kCollectionFailed, "Der Parkstatus konnte nicht erfasst werden; siehe ParkingControl.log." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} geparkt ({1} Spuren) | {2}/{3} deaktiviert | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} Straße | {1} sichtbar | {2} versteckt | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} öffentlich | {3}  {4} / {5} Gebäude" },
                { ParkingStatusLocale.kShareFormat, "{0} auf Straße geparkt | {1} in Bewegung | aktualisiert {2}" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "AUS" },
                { ParkingStatusLocale.kStatusCheck, "PRÜFEN" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

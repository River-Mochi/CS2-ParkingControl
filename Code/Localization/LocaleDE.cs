// <copyright file="LocaleDE.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: German text for Parking Control's Options UI.

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
        /// <param name="settings">Options settings whose localization IDs are used.</param>
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
                // Options tabs and groups.
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Aktionen" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Über" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Straßenparken" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Status privater Fahrzeuge" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Mod-Informationen" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Links" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnose" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Kein Straßenparken" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Wähle **Ganze Stadt**, **Nach Bezirk** oder **AUS**.\n" +
                    "- Geeignete Straßenparkspuren werden für neues Parken gesperrt.\n" +
                    "- Bereits geparkte Autos fahren weg, wenn sie das nächste Mal benutzt werden.\n" +
                    "- Kostenpflichtige Parkplätze und normale Gebäudeparkplätze bleiben nutzbar.\n" +
                    "**Autobahnen und asymmetrische 3-spurige Straßen erlauben bereits kein Straßenparken.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Ganze Stadt" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Nach Bezirk" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "AUS" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Anweisungen anzeigen" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Zeigt, wie der Modus <Nach Bezirk> verwendet wird.\n" +
                    "1.a. AUS = stadtweite und Bezirksbeschränkungen sind deaktiviert; weitgehend zurück zu den Spielstandards.\n" +
                    "1.b. Die <Parkverbot>-Schaltfläche für einzelne Straßen im Straßenservices-Panel funktioniert weiterhin, ähnlich wie das Anwenden eines Zebrastreifens.\n" +
                    "2. Ganze Stadt = sperrt alle geeigneten öffentlichen Straßenparkplätze der Stadt." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Status anzeigen" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Aktuelle Parkzahlen unten anzeigen.>\n" +
                    "Der Status wird nur erfasst, solange das Optionsmenü geöffnet ist; während des normalen Stadtspiels läuft kein Hintergrundscan." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Bezirksmodus>\n" +
                    "1. Wähle oben <Nach Bezirk>.\n" +
                    "2. Erstelle oder wähle einen Bezirk in der Stadt.\n" +
                    "3. Öffne <Richtlinien> und aktiviere **Parkverbot am Straßenrand [✓]**.\n" +
                    "4. Parkverbot und Parkgebühr können gleichzeitig aktiv sein. Die Gebühr gilt für Autos, die noch dort stehen oder trotz Verbot dort parken.\n" +
                    "Straßen außerhalb von Bezirken mit Parkverbot behalten normales Straßenparken." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Parkverbot" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]",
                    "Schaltet das Parken am Straßenrand auf einer Straßenseite um. Für mehrere Seiten darüber ziehen, bevor die linke Maustaste losgelassen wird." },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Parkverbot am Straßenrand" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Verhindert, dass Autos und Motorräder in diesem Bezirk am Straßenrand parken. Bereits geparkte Fahrzeuge fahren weg, wenn ihre Besitzer sie das nächste Mal benutzen." },
                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Hinzufügen" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Entfernen" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Straßenparken" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Geparkt> = Autos, die noch auf Straßen parken, auf denen der gewählte Modus das Parken verbietet.\n" +
                    "<Spuren> = Straßenrand-Parkabschnitte mit diesen Autos. Eine Spur kann mehrere Autos aufnehmen.\n" +
                    "<Gesperrt> = Straßenparkspuren, die kein neues Parken zulassen.\n" +
                    "<Nach Bezirk> zeigt:\n" +
                    "- Belegte Spuren in Bezirken mit Verbot / belegte Spuren stadtweit.\n" +
                    "- Deaktivierte Spuren / geeignete Spuren der Stadt.\n" +
                    "- Aktivierte Bezirke / gesamte Bezirke.\n" +
                    "<Neue oder umgebaute Straßen> können kurzzeitig einige Autos aufnehmen, während ihre Spuren aktualisiert werden. Bereits geparkte Autos fahren weg, wenn Bürger sie benutzen.\n" +
                    "<PRÜFEN> = einige ausgewählte Straßen sind noch nicht blockiert. Lass die Stadt kurz laufen und prüfe erneut. Bleibt <PRÜFEN>, füge bei einer Support-Anfrage einen Parkbericht aus dem Log bei." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Straßennutzung" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Diese Zeile umfasst die <ganze Stadt>, nicht nur Bezirke.\n" +
                    "<Auf Straße geparkt> = Anteil der Fahrzeuge, die auf Straßen statt auf öffentlichen oder Gebäudeparkplätzen parken.\n" +
                    "<Aktiv> = private Fahrzeuge, die fahren oder im Verkehr warten.\n" +
                    "<Formel> = Straße ÷ (Straße + belegte öffentliche + belegte Gebäudeplätze).\n" +
                    "**Speicher von Außenverbindungen (OC) und Autos ohne zugewiesene Parkspur sind ausgeschlossen.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parkplätze" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Zeigt die stadtweite Parkplatzbelegung.\n" +
                    "<Öffentlich> belegt = Einrichtungen, die von der Vanilla-Parkplatz-Infoansicht gezählt werden.\n" +
                    "<Gebäude> belegt = Parkplätze bei Wohnungen, Arbeitsplätzen und Geschäften.\n" +
                    "**Je höher die %-Nutzung, desto eher werden zusätzliche Parkplätze benötigt.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Auto-Standorte" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Diese Zeile zeigt stadtweite Daten, nicht nur Bezirke mit Parkverbot.\n" +
                    "<Straße> = auf öffentlichen Straßen geparkt.\n" +
                    "<Sichtbar> = Autos, die du auf offenen Parkplätzen oder Außenparkplätzen von Gebäuden sehen und anklicken kannst.\n" +
                    "<Innen> = in Gebäuden oder Garagen.\n" +
                    "<OC> = Speicher einer Außenverbindung am Stadtrand; einige Autos einziehender Haushalte starten dort als Bereitstellung.\n" +
                    "Autos ohne zugewiesene Parkspur werden hier ausgelassen und nur im Log-Bericht im Tab Über angezeigt." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Aktualisiert" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Zeitpunkt, zu dem diese stadtweiten Statuswerte zuletzt aktualisiert wurden." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod-Name" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox-Mods-Link" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Öffnet die Seite des Autors auf Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Parkbericht schreiben" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Schreibt Details zu Straßenparken und verwandten Daten nach \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Wenn du neugierig bist, schreibe später in derselben geladenen Stadt einen 2. Bericht.\n" +
                    "- Vergleicht bis zu 20 Beispiel-Entity-IDs aus verschiedenen Kategorien.\n" +
                    "- Zeigt, ob jedes Beispiel blieb, losfuhr, anderswo parkte oder verschwand.\n" +
                    "- Der Mod Scene Explorer wird benötigt, um die Entity-ID-Nummern in der Stadt zu verfolgen." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Log öffnen" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Öffnet <Logs/ParkingControl.log> oder den Logs-Ordner, falls die Datei noch nicht existiert." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Noch keine Stadt geladen." },
                { ParkingStatusLocale.kCollecting, "Parkstatus wird erfasst..." },
                { ParkingStatusLocale.kUnavailable, "Parkstatus ist nicht verfügbar." },
                { ParkingStatusLocale.kCollectionFailed, "Parkstatus konnte nicht erfasst werden; siehe ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} geparkt ({1} Spuren) | {2}/{3} gesperrt{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} geparkt ({1}/{2} Spuren) | {3}/{4} gesperrt | {5}/{6} Bezirke{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} Straße | {1} sichtbar | {2} innen | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} öff. {1}/{2} | {3} Gebäude {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} Straße {1} | {2} aktiv" },
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

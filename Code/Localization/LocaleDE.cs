// <copyright file="LocaleDE.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: German text for Parking Control's Options UI.
    using System.Collections.Generic;
    using Colossal;

namespace ParkingControl
{

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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Diagnose" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Kein Straßenparken" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Wähle:\n" +
                    "<1. Nach Bezirk>\n" +
                    "<2. Ganze Stadt>\n" +
                    "<3. AUS>\n" +
                    "- Geeignete Spuren werden gesperrt, damit dort niemand neu parkt.\n" +
                    "- Bereits geparkte Autos ziehen nach dem Verbot nach und nach um; große Verbotsbereiche brauchen länger.\n" +
                    "- Kostenpflichtige Parkplätze und normale Gebäudeparkplätze bleiben nutzbar.\n" +
                    "**Einige Straßen erlauben ohnehin kein Straßenparken, z. B. Autobahnen und kleine zweispurige Gassen.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Ganze Stadt" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Nach Bezirk" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "AUS" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Anweisungen anzeigen" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Zeigt die Nutzung von <Nach Bezirk>.\n" +
                    "1.a. AUS = Stadt- und Bezirksverbote sind deaktiviert; weitgehend zurück zum Spielstandard.\n" +
                    "1.b. Der <Parkverbot>-Button für einzelne Straßen im Straßenservices-Panel funktioniert weiterhin wie ein Zebrastreifen.\n" +
                    "2. Ganze Stadt = sperrt alle geeigneten öffentlichen Straßenparkplätze der Stadt."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Status anzeigen" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Aktuelle Parkzahlen unten anzeigen.>\n" +
                    "Der Status wird nur erfasst, solange das Optionsmenü geöffnet ist;\n" +
                    "während des Stadtspiels läuft kein Hintergrundscan."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Bezirksmodus>\n" +
                    "1. Wähle oben <Nach Bezirk>.\n" +
                    "2. Erstelle/wähle einen Bezirk in der Stadt.\n" +
                    "3. Öffne <Richtlinien> und aktiviere **Parkverbot am Straßenrand [✓]**.\n" +
                    "4. Parkverbot und Parkgebühr können gleichzeitig aktiv sein. Die Gebühr gilt für Autos, die noch dort stehen oder trotzdem hineinparken.\n" +
                    "Straßen außerhalb gesperrter Bezirke behalten normales Straßenparken."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Parkverbot" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Schaltet das Parken am Straßenrand auf einer Straßenseite um. Für mehrere Seiten darüber ziehen, bevor die linke Maustaste losgelassen wird." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Parkverbot am Straßenrand" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Verhindert, dass Autos und Motorräder in diesem Bezirk am Straßenrand parken.\n" +
                    "- Bereits geparkte Autos ziehen nach und nach um; große Verbotsbereiche brauchen länger."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Hinzufügen" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Entfernen" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Straßenparken" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Zeigt nur das gewählte Verbot <Ganze Stadt> oder <Nach Bezirk>. Manuelle Parkverbote stehen separat.\n" +
                    "<AUS> = Stadt-/Bezirksverbote sind aus; manuelle <Parkverbot>-Straßen bleiben aktiv.\n" +
                    "<Geparkt> = Autos, die noch auf Straßen im gewählten Bereich parken.\n" +
                    "<Gesperrt> = gesperrte Bordsteinabschnitte / Zielabschnitte.\n" +
                    "<Bezirke> = Bezirke mit Parkverbot / alle Bezirke.\n" +
                    "<PRÜFEN> = einige Zielabschnitte entsprechen dem gewählten Verbot noch nicht.\n" +
                    "<---------------------->\n" +
                    "**Wenn [PRÜFEN] nach Straßenänderungen oder Umbauten erscheint, lass die Stadt eine Weile laufen und öffne Optionen > Status erneut. Bleibt es bestehen, nutze Über > Diagnose > Bericht schreiben.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Manuelles Parkverbot" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Zeigt nur Straßen, die mit dem manuellen <Parkverbot>-Tool gesetzt wurden.\n" +
                    "<Geparkt> = Autos, die noch auf diesen manuell gesperrten Straßen parken.\n" +
                    "<Gesperrt> = gesperrte Bordsteinabschnitte / manuell gewählte Zielabschnitte.\n" +
                    "Manuelle Verbote können Stadt- oder Bezirksverbote überlappen; diese Zeile nicht zu <Straßenparken> addieren.\n" +
                    "**Wenn [PRÜFEN] nach längerem Stadtlauf erscheint, nutze Über > Diagnose > Bericht schreiben und reiche ihn bei einer Support-Anfrage ein.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Parknutzung" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Öffentlich> = belegte Plätze in Parkeinrichtungen.\n" +
                    "Entspricht ungefähr CS2s Straßen-Parkinfo.\n" +
                    "<Gebäude> = Fahrzeuge in Gebäuden oder Garagen.\n" +
                    "<Straße> = Autos auf Straßen.\n" +
                    "<Gesamt> = bekannte geparkte Autos in der Stadt (Straße + öffentlich + Gebäude).\n" +
                    "**Außenverbindungen und unbekannte Bereitstellung sind nicht enthalten.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parkbewertung" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Bewertet, wie viel exakt erfassbares Parken noch <frei> ist.\n" +
                    "<SCHLECHT> = unter 15 % frei.\n" +
                    "<OK> = 15 % bis unter 30 % frei.\n" +
                    "<GUT> = 30 % oder mehr frei.\n" +
                    "<Öffentlich> nutzt Parkeinrichtungen aus CS2s Straßen-Parkinfo.\n" +
                    "<Gebäude> nutzt Parkplätze mit exakter Kapazität in Gebäuden und Garagen."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Auto-Standorte" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Diese Zeile zeigt stadtweite Daten, nicht nur Bezirke mit Verbot.\n" +
                    "<Straße> = auf öffentlichen Straßen geparkt.\n" +
                    "<Sichtbar> = sichtbare und anklickbare Autos auf offenen Parkplätzen oder Außenplätzen von Gebäuden.\n" +
                    "<Innen> = in Gebäuden oder Garagen.\n" +
                    "<OC> = Speicher an der Außenverbindung am Stadtrand; einige Autos einziehender Haushalte starten dort (Bereitstellung).\n" +
                    "Autos ohne zugewiesene Parkspur werden hier ausgelassen und nur im Log-Bericht (Tab Über) gezeigt."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Aktualisiert" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Zeitpunkt der letzten Aktualisierung dieser stadtweiten Statuswerte." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod-Name" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox-Mods-Link" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Öffnet die Seite des Autors auf Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Bericht schreiben" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Schreibt Details zu Straßenparken und verwandten Daten nach \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Bei Bedarf später in derselben geladenen Stadt einen 2. Bericht schreiben.\n" +
                    "- Vergleicht bis zu 20 Beispiel-Entity-IDs aus verschiedenen Kategorien.\n" +
                    "- Zeigt, ob jedes Beispiel blieb, losfuhr, anderswo parkte oder verschwand.\n" +
                    "- Scene Explorer wird benötigt, um Entity-ID-Nummern in der Stadt zu verfolgen."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Log öffnen" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Öffnet <Logs/ParkingControl.log> oder den Logs-Ordner, falls die Datei noch nicht existiert." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Ausführliches Log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Automatische DEBUG-Details.\n" +
                    "Nicht für normales Spielen; AUS, wenn nicht debuggt wird.\n" +
                    "Bericht schreiben funktioniert auch bei AUS."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Noch keine Stadt geladen." },
                { ParkingStatusLocale.kCollecting, "Parkstatus wird erfasst..." },
                { ParkingStatusLocale.kUnavailable, "Parkstatus ist nicht verfügbar." },
                { ParkingStatusLocale.kCollectionFailed, "Parkstatus konnte nicht erfasst werden; siehe ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} geparkt | {1}/{2} gesperrt{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} geparkt | {1}/{2} Spuren gesperrt{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} geparkt | {1}/{2} gesperrt | {3}/{4} Bezirke{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} Straße | {1} sichtbar | {2} innen | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1} öff. frei | {2} = {3} Gebäude frei" },
                { ParkingStatusLocale.kShareFormat, "{0} öff. | {1} Gebäude | {2} Straße | {3} gesamt" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "AUS = Stadt-/Bezirksverbote aus | manuelle Straßen bleiben aktiv" },
                { ParkingStatusLocale.kManualNone, "Keine gesetzt" },
                { ParkingStatusLocale.kStatusCheck, "PRÜFEN" },
                { ParkingStatusLocale.kRatingPoor, "SCHLECHT" },
                { ParkingStatusLocale.kRatingGood, "GUT" },
                { ParkingStatusLocale.kRatingNA, "N/A" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

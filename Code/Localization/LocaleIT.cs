// <copyright file="LocaleIT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Italian text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Italian localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleIT : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleIT"/> class.
        /// </summary>
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
        public LocaleIT(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Azioni" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Informazioni" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Parcheggio su strada" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Stato veicoli personali" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informazioni sulla mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Collegamenti" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostica" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "Nessun parcheggio su strada (intera città)" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)),
                    "- Impedisce ai veicoli personali e alle motociclette di usare il parcheggio su strada in futuro.\n" +
                    "- I parcheggi e i posti auto inclusi negli edifici restano disponibili.\n" +
                    "- I veicoli già parcheggiati non vengono rimossi. Se ne vanno naturalmente la prossima volta che un cittadino usa l'auto.\n" +
                    "- Assicurati che la città disponga di abbastanza parcheggi fuori strada, altrimenti le auto potrebbero girare a lungo in cerca di un posto." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Parcheggio su strada" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Parcheggiate> = auto ancora parcheggiate sulle strade.\n" +
                    "<Corsie> = sezioni di parcheggio a bordo strada che contengono quelle auto. Una corsia può contenere diverse auto.\n" +
                    "<Disattivate> = corsie di parcheggio su strada chiuse a nuovi parcheggi.\n" +
                    "<OK> = Nessun parcheggio su strada è attivo e funziona.\n" +
                    "<DISATTIVATO> = Nessun parcheggio su strada è disattivato; le auto possono parcheggiare liberamente sulle strade normali.\n" +
                    "<CONTROLLA> = le strade potrebbero essere ancora in aggiornamento. Attendi un momento; scrivi un rapporto nel log se il problema persiste.\n" +
                    "**Alcune auto possono rimanere dopo aver attivato la regola senza parcheggio su strada o dopo aver modificato le strade. Se ne vanno naturalmente se il cittadino usa l'auto.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Posizione delle auto" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Strada> = parcheggiate su strade pubbliche.\n" +
                    "<Visibili> = auto che puoi vedere e selezionare nei parcheggi all'aperto o nei posti auto esterni inclusi negli edifici.\n" +
                    "<Nascoste> = all'interno di edifici o garage.\n" +
                    "<OC> = deposito della connessione esterna al confine della città; alcune auto delle famiglie in arrivo iniziano lì.\n" +
                    "**L'area di attesa non assegnata del gioco base viene registrata solo nel log.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Parcheggi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<Percentuale> e <usati / totali> mostrano l'occupazione dei parcheggi.\n" +
                    "<Pubblici> = strutture conteggiate dalla visualizzazione Informazioni parcheggi del gioco base.\n" +
                    "<Edificio> = parcheggi inclusi con abitazioni e luoghi di lavoro.\n" +
                    "**I parcheggi degli edifici includono posti esterni visibili e parcheggi interni.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso della strada" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Parcheggiate in strada> = quota di auto che usa parcheggi noti su strada, pubblici o degli edifici.\n" +
                    "<In movimento> = veicoli personali in marcia o in attesa nel traffico.\n" +
                    "<Aggiornato> = ultimo aggiornamento dello stato.\n" +
                    "**Le connessioni esterne (OC) e l'area di attesa non assegnata sono escluse.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nome mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versione" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Apri la pagina dell'autore su Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Scrivi rapporto parcheggi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Scrivi i dettagli su parcheggio su strada, disponibilità, proprietà e posizione dei veicoli\n" +
                    "in <ParkingControl.log>\n" +
                    "Un secondo rapporto nella stessa città caricata segue gli stessi ID entità delle auto parcheggiate in strada." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Apri log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Apri <ParkingControl.log>, oppure la cartella Logs se il file non esiste ancora." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

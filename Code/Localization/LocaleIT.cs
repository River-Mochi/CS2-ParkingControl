// <copyright file="LocaleIT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Italian text for Parking Control's Options UI.

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
        /// <param name="settings">Options settings whose localization IDs are used.</param>
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
                // Options tabs and groups.
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Azioni" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Informazioni" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Parcheggio su strada" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Stato veicoli personali" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informazioni sulla mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Collegamenti" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostica" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Niente parcheggio su strada" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Scegli **Intera città**, **Per distretto** oppure **DISATTIVATO**.\n" +
                    "- Le corsie idonee vengono bloccate per impedire nuovi parcheggi su strada.\n" +
                    "- Le auto già parcheggiate se ne vanno quando vengono usate di nuovo.\n" +
                    "- I parcheggi a pagamento e quelli normali degli edifici restano utilizzabili.\n" +
                    "**Autostrade e strade asimmetriche a 3 corsie escludono già il parcheggio su strada.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Intera città" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Per distretto" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DISATTIVATO" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostra istruzioni" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Mostra come usare la modalità <Per distretto>.\n" +
                    "DISATTIVATO = le restrizioni al parcheggio su strada sono disattivate.\n" +
                    "Intera città = il parcheggio su strada idoneo è bloccato in tutta la città." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostra stato" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Mostra qui sotto i totali attuali dei parcheggi.>\n" +
                    "Lo stato viene raccolto solo mentre il menu Opzioni è aperto; " +
                    "durante il gioco non viene eseguita alcuna scansione in background." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modalità per distretto>\n" +
                    "1. Scegli <Per distretto> qui sopra.\n" +
                    "2. Crea o seleziona un distretto nella città.\n" +
                    "3. Apri <Politiche> e attiva **Divieto di parcheggio a bordo strada [✓]**.\n" +
                    "Fuori dai distretti selezionati resta il parcheggio normale." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Divieto di parcheggio a bordo strada" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Impedisce ad auto e moto di parcheggiare a bordo strada in questo distretto. " +
                    "I veicoli già parcheggiati se ne vanno quando i proprietari li usano di nuovo." },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Sosta su strada" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<In sosta> = auto ancora parcheggiate sulle strade dove la modalità scelta vieta il parcheggio.\n" +
                    "<Corsie> = tratti di parcheggio a bordo strada che contengono quelle auto. Una corsia può contenerne diverse.\n" +
                    "<Chiuse> = corsie di parcheggio su strada chiuse ai nuovi veicoli.\n" +
                    "<Per distretto> mostra:\n" +
                    "- Corsie occupate nei distretti con divieto / corsie occupate in tutta la città.\n" +
                    "- Corsie disattivate / corsie idonee della città.\n" +
                    "- Distretti attivati / distretti totali.\n" +
                    "<Strade nuove o ricostruite> possono accettare brevemente qualche auto mentre le corsie si aggiornano. " +
                    "Le auto già parcheggiate se ne vanno quando i cittadini le usano.\n" +
                    "<CONTROLLA> = alcune strade selezionate non sono ancora bloccate. Fai andare la città per un " +
                    "po’ e ricontrolla. Se <CONTROLLA> resta, includi un rapporto parcheggi quando chiedi assistenza." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso strade" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Questa riga include <l’intera città>, non solo i distretti.\n" +
                    "<Parcheggiate in strada> = percentuale parcheggiata in strada invece che in parcheggi pubblici o degli edifici.\n" +
                    "<Attive> = veicoli personali in marcia o fermi nel traffico.\n" +
                    "<Formula> = strada ÷ (strada + pubblico occupato + edificio occupato).\n" +
                    "**Il deposito delle connessioni esterne (OC) e le auto senza corsia di parcheggio assegnata sono esclusi.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Posti auto" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Mostra l’occupazione dei parcheggi in tutta la città.\n" +
                    "<Pubblico> usato = strutture conteggiate dalla visualizzazione Informazioni parcheggi del gioco base.\n" +
                    "<Edificio> usato = parcheggi inclusi con abitazioni, luoghi di lavoro e negozi.\n" +
                    "**Una % di utilizzo più alta = potrebbero servire più parcheggi.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Posizione auto" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Questa riga mostra dati di tutta la città, non solo dei distretti con il divieto.\n" +
                    "<Strada> = parcheggiate su strade pubbliche.\n" +
                    "<Visibili> = auto visibili e cliccabili nei parcheggi all’aperto o nei posti auto esterni degli edifici.\n" +
                    "<Nascoste> = dentro edifici o garage.\n" +
                    "<OC> = deposito della connessione esterna al confine della città; alcune auto delle famiglie in arrivo iniziano lì come area di attesa.\n" +
                    "Le auto senza corsia di parcheggio assegnata sono omesse qui e mostrate solo nel rapporto del log nella scheda Informazioni." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Aggiornato" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "Ora dell’ultimo aggiornamento di questi valori di stato dell’intera città." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nome mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versione" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Apre la pagina dell’autore su Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Scrivi rapporto parcheggi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Scrive i dettagli del parcheggio su strada e dati correlati in \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Se ti interessa, crea un 2° rapporto più tardi nella stessa città caricata.\n" +
                    "- Confronta fino a 20 ID entità di esempio da categorie diverse.\n" +
                    "- Mostra se ogni esempio è rimasto, ha iniziato a guidare, ha parcheggiato altrove o è scomparso.\n" +
                    "- Il mod Scene Explorer è necessario per seguire i numeri degli ID entità nella città."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Apri log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Apre <Logs/ParkingControl.log>, oppure la cartella Logs se il file non esiste ancora." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Nessuna città caricata." },
                { ParkingStatusLocale.kCollecting, "Raccolta dello stato dei parcheggi..." },
                { ParkingStatusLocale.kUnavailable, "Lo stato dei parcheggi non è disponibile." },
                { ParkingStatusLocale.kCollectionFailed, "Impossibile raccogliere lo stato dei parcheggi; consulta ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} in sosta ({1} corsie) | {2}/{3} chiuse{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} in sosta ({1}/{2} corsie) | {3}/{4} chiuse | {5}/{6} distretti{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} strada | {1} visibili | {2} nascoste | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} pubblico {1}/{2} | {3} edificio {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} in strada {1} | {2} attive" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DISATT." },
                { ParkingStatusLocale.kStatusCheck, "VERIF." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

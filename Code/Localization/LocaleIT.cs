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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kRoadSizeGroup), "Intera città per dimensione stradale" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Stato veicoli personali" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informazioni sulla mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Collegamenti" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Diagnostica" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Niente parcheggio su strada" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Scegli:\n" +
                    "Consigliato: <1. Per distretto> - mostra nel gioco la politica di distretto **[Divieto di parcheggio a bordo strada]**.\n" +
                    "<2. Solo manuale> - i divieti città/distretto sono tutti DISATTIVATI. Il pulsante manuale [Divieto di sosta] funziona con qualsiasi scelta.\n" +
                    "<3. Intera città> - vieta il parcheggio su strada in tutta la città. Esclude le strade con posti segnati a spina o perpendicolari.\n" +
                    "- Le corsie vengono disattivate per impedire nuovi parcheggi su strada.\n" +
                    "- Le auto già parcheggiate si spostano gradualmente dopo il divieto; le aree grandi richiedono più tempo.\n" +
                    "- I parcheggi a pagamento e quelli normali degli edifici restano utilizzabili.\n" +
                    "**Alcune strade escludono già il parcheggio su strada, come autostrade, piccoli vicoli a doppio senso e strade asimmetriche a 3 corsie.**"
                },

                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "1. Per distretto" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "2. Solo manuale" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "3. Intera città" },

                // Citywide road-size bans.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.BanFourLaneRoads)), "Vieta parcheggio su strade a 4 corsie" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.BanFourLaneRoads)),
                    "Vieta il parcheggio sulla maggior parte delle <strade a 4 corsie> in tutta la città.\n" +
                    "- Funziona anche con strade personalizzate di Road Builder (RB) con 4 corsie di marcia.\n" +
                    "- Eccezione: le strade con posti segnati, come parcheggi a spina o perpendicolari, non vengono toccate.\n" +
                    "- Si somma al menu sopra e alle strade con <Divieto di sosta> manuale; non annulla mai un altro divieto.\n" +
                    "**Le strade senza parcheggio, come le autostrade, non sono interessate.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.BanSixLaneRoads)), "Vieta parcheggio su strade a 6 corsie" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.BanSixLaneRoads)),
                    "Vieta il parcheggio sulla maggior parte delle <strade a 6 corsie> in tutta la città.\n" +
                    "- Funziona anche con strade personalizzate di Road Builder (RB) con 6 corsie di marcia.\n" +
                    "- Eccezione: le strade con posti segnati, come parcheggi a spina o perpendicolari, non vengono toccate.\n" +
                    "- Si somma al menu sopra e alle strade con <Divieto di sosta> manuale; non annulla mai un altro divieto.\n" +
                    "**Le strade senza parcheggio, come le autostrade, non sono interessate.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostra istruzioni" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Mostra come usare la modalità <Per distretto>."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostra stato" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Mostra qui sotto i totali attuali dei parcheggi.>\n" +
                    "Lo stato viene raccolto solo mentre il menu Opzioni è aperto;\n" +
                    "durante il gioco non viene eseguita alcuna scansione in background."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modalità per distretto>\n" +
                    "1. Scegli <Per distretto> qui sopra.\n" +
                    "2. Crea/seleziona un distretto.\n" +
                    "3. Apri <Politiche> e attiva **Divieto di parcheggio a bordo strada [✓]**.\n" +
                    "4. Divieto e tariffa possono essere attivi insieme. La tariffa viene applicata alle auto ancora presenti o che riescono comunque a parcheggiare.\n" +
                    "Fuori dai distretti con divieto resta il parcheggio normale su strada."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Divieto di sosta" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Attiva o disattiva il parcheggio su un lato della strada. Per più lati, trascina su di essi prima di rilasciare il pulsante sinistro del mouse." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Divieto di parcheggio a bordo strada" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Impedisce ad auto e moto di parcheggiare a bordo strada in questo distretto.\n" +
                    "- Le auto già parcheggiate si spostano gradualmente; le aree grandi richiedono più tempo."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Aggiungi" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Rimuovi" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Sosta su strada" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Mostra solo l’ambito selezionato <Intera città> o <Per distretto>. I divieti manuali sono elencati a parte.\n" +
                    "Anche i divieti per le strade a 4 e 6 corsie sono conteggiati qui.\n" +
                    "<Solo manuale> = divieti città/distretto disattivati; le strade con <Divieto di sosta> manuale restano attive.\n" +
                    "<In sosta> = auto ancora parcheggiate nelle strade dell’ambito selezionato.\n" +
                    "<Chiuse> = sezioni di corsia a bordo strada disattivate / sezioni obiettivo.\n" +
                    "<Distretti> = distretti con divieto / distretti totali.\n" +
                    "<VERIF.> = alcune sezioni obiettivo non corrispondono ancora al divieto selezionato.\n" +
                    "<---------------------->\n" +
                    "**Se [VERIF.] appare dopo aver modificato o ricostruito strade, fai andare la città per un po’ e riapri Opzioni > Stato. Se resta, usa Informazioni > Diagnostica > Scrivi rapporto.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Divieto manuale" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Mostra solo le strade impostate con lo strumento manuale <Divieto di sosta>.\n" +
                    "<In sosta> = auto ancora parcheggiate su quelle strade vietate manualmente.\n" +
                    "<Chiuse> = sezioni di corsia a bordo strada disattivate / selezionate manualmente.\n" +
                    "I divieti manuali possono sovrapporsi a Intera città o distretti; non aggiungere questa riga ai totali <Sosta su strada>.\n" +
                    "**Se [VERIF.] appare dopo aver fatto andare la città per un po’, usa Informazioni > Diagnostica > Scrivi rapporto e allegalo quando chiedi aiuto.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso parcheggi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Mostra l’uso dei parcheggi in <tutta la città>. Non segue l’ambito del divieto Intera città / per distretto.\n" +
                    "<Pubblico> = posti occupati / totali nelle strutture pubbliche.\n" +
                    "Usa gli stessi dati del pannello parcheggi di Strade in CS2.\n" +
                    "<Strada> = auto parcheggiate su strada. Intera città lascia attivi i posti segnati, quindi quelle auto compaiono ancora qui.\n" +
                    "<Edif.> = auto parcheggiate in edifici o garage.\n" +
                    "<Totale> = auto parcheggiate note in città (pubblico + strada + edificio).\n" +
                    "**Connessioni esterne e aree di attesa sconosciute sono escluse.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Valutazione parcheggi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Mostra quanto sono pieni i <parcheggi della città>.\n" +
                    "<Parcheggiate> = posti occupati / totali, gli stessi numeri della InfoView parcheggi del gioco.\n" +
                    "La percentuale indica quanti di quei posti sono ancora liberi.\n" +
                    "<POOR> = meno del 15% libero.\n" +
                    "<OK> = dal 15% a meno del 30% libero.\n" +
                    "<GOOD> = 30% o più libero.\n" +
                    "**Sono auto nei parcheggi, non lungo la strada.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Posizione auto" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Questa riga mostra dati di tutta la città, non solo dei distretti con divieto.\n" +
                    "<Strada> = parcheggiate su strade pubbliche.\n" +
                    "<Visibili> = auto visibili e cliccabili nei parcheggi all’aperto o nei posti esterni degli edifici.\n" +
                    "<Interno> = in edifici o garage.\n" +
                    "<OC> = deposito della connessione esterna al confine della città; alcune auto delle famiglie in arrivo iniziano lì (area di attesa).\n" +
                    "Le auto senza corsia di parcheggio assegnata sono omesse qui e mostrate solo nel rapporto del log (scheda Informazioni)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Aggiornato" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Ora dell’ultimo aggiornamento di questi valori di stato dell’intera città." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nome mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versione" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Apre la pagina dell’autore su Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Scrivi rapporto" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Scrive i dettagli del parcheggio su strada e dati correlati in \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Se serve, crea un 2° rapporto più tardi nella stessa città caricata.\n" +
                    "- Confronta fino a 20 ID entità di esempio da categorie diverse.\n" +
                    "- Mostra se ogni esempio è rimasto, ha iniziato a guidare, ha parcheggiato altrove o è scomparso.\n" +
                    "- Scene Explorer è necessario per seguire gli ID entità nella città."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Apri log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Apre <Logs/ParkingControl.log>, oppure la cartella Logs se il file non esiste ancora." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Log dettagliato" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Dettagli DEBUG automatici.\n" +
                    "Non è per il gioco normale; DISATTIVA se non stai facendo debug.\n" +
                    "Scrivi rapporto funziona anche se DISATTIVATO."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Nessuna città caricata." },
                { ParkingStatusLocale.kCollecting, "Raccolta dello stato dei parcheggi..." },
                { ParkingStatusLocale.kUnavailable, "Lo stato dei parcheggi non è disponibile." },
                { ParkingStatusLocale.kCollectionFailed, "Impossibile raccogliere lo stato dei parcheggi; consulta ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} in sosta | {1}/{2} chiuse{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} in sosta | {1}/{2} corsie chiuse{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} in sosta | {1}/{2} chiuse | {3}/{4} distretti{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} strada | {1} visibili | {2} interno | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1} | Parcheggiate {2}/{3}" },
                { ParkingStatusLocale.kShareFormat, "{0} pubblico | {2} strada | {1} edif. | {3} totale" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "Solo manuale = divieti città/distretto off | strade manuali attive" },
                { ParkingStatusLocale.kManualNone, "Nessuno" },
                { ParkingStatusLocale.kStatusCheck, "VERIF." },
                { ParkingStatusLocale.kRatingPoor, "SCARSO" },
                { ParkingStatusLocale.kRatingGood, "BUONO" },
                { ParkingStatusLocale.kRatingNA, "N/D" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

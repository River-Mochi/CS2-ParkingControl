// <copyright file="LocaleFR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: French text for Parking Control's Options UI.

using System.Collections.Generic;
using Colossal;

namespace ParkingControl
{

    /// <summary>
    /// French localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleFR : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleFR"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleFR(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Actions" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "À propos" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Stationnement sur rue" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "État des véhicules personnels" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informations sur le mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Liens" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Diagnostic" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Stationnement sur rue interdit" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Choisissez :\n" +
                    "<1. Par quartier>\n" +
                    "<2. Ville entière>\n" +
                    "<3. DÉSACTIVÉ>\n" +
                    "- Les voies admissibles sont bloquées pour empêcher tout nouveau stationnement sur rue.\n" +
                    "- Les voitures déjà garées se déplacent progressivement après l’interdiction ; les grandes zones prennent plus de temps.\n" +
                    "- Les parkings payants et le stationnement normal des bâtiments restent disponibles.\n" +
                    "**Certaines routes excluent déjà le stationnement sur rue, comme les autoroutes et les petites ruelles à double sens.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Ville entière" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Par quartier" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DÉSACTIVÉ" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Afficher les instructions" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Explique le mode <Par quartier>.\n" +
                    "1.a. DÉSACTIVÉ = les restrictions de ville et de quartiers sont coupées ; retour en grande partie aux règles normales.\n" +
                    "1.b. Le bouton <Interdiction de stationner> pour une route dans Services routiers fonctionne toujours comme un passage piéton.\n" +
                    "2. Ville entière = bloque tout le stationnement public sur rue admissible."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Afficher l’état" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Affiche ci-dessous les totaux actuels de stationnement.>\n" +
                    "Le statut n’est collecté que lorsque le menu Options est ouvert ;\n" +
                    "aucun scan en arrière-plan ne tourne pendant la partie."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Mode par quartier>\n" +
                    "1. Choisissez <Par quartier> ci-dessus.\n" +
                    "2. Créez/sélectionnez un quartier.\n" +
                    "3. Ouvrez <Politiques> et activez **Interdiction de stationner en bord de route [✓]**.\n" +
                    "4. L’interdiction et le tarif peuvent être actifs ensemble. Le tarif s’applique aux voitures encore présentes ou qui réussissent à se garer.\n" +
                    "Les routes hors des quartiers interdits gardent le stationnement normal."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Interdiction de stationner" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Active ou désactive le stationnement d’un côté de la route. Pour plusieurs côtés, faites glisser dessus avant de relâcher le bouton gauche de la souris." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Interdiction de stationner en bord de route" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Empêche les voitures et motos de stationner en bord de route dans ce quartier.\n" +
                    "- Les voitures déjà garées se déplacent progressivement ; les grandes zones prennent plus de temps."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Ajouter" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Retirer" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Parking sur rue" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Affiche uniquement la portée choisie <Ville entière> ou <Par quartier>. Les interdictions manuelles sont séparées.\n" +
                    "<DÉSACT.> = interdictions ville/quartiers coupées ; les routes <Interdiction de stationner> manuelles restent actives.\n" +
                    "<Garées> = voitures encore garées dans la portée choisie.\n" +
                    "<Fermées> = sections de voie en bordure fermées / sections ciblées.\n" +
                    "<Quartiers> = quartiers avec interdiction / total des quartiers.\n" +
                    "<VÉRIF.> = certaines sections ciblées ne correspondent pas encore à l’interdiction choisie.\n" +
                    "<---------------------->\n" +
                    "**Si [VÉRIF.] apparaît après une modification ou reconstruction de route, faites tourner la ville un moment puis rouvrez Options > État. Si cela reste, utilisez À propos > Diagnostic > Écrire rapport.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Interdiction manuelle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Affiche uniquement les routes réglées avec l’outil manuel <Interdiction de stationner>.\n" +
                    "<Garées> = voitures encore garées sur ces routes interdites manuellement.\n" +
                    "<Fermées> = sections de voie en bordure fermées / ciblées manuellement.\n" +
                    "Les interdictions manuelles peuvent chevaucher Ville entière ou quartiers ; ne pas ajouter cette ligne aux totaux <Parking sur rue>.\n" +
                    "**Si [VÉRIF.] apparaît après avoir fait tourner la ville un moment, utilisez À propos > Diagnostic > Écrire rapport et joignez-le à votre demande d’aide.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Usage du parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Public> = places occupées dans les installations de stationnement.\n" +
                    "Correspond approximativement au panneau Stationnement des Routes de CS2.\n" +
                    "<Bât.> = véhicules garés dans les bâtiments ou garages.\n" +
                    "<Rue> = voitures garées sur rue.\n" +
                    "<Total> = voitures garées connues en ville (rue + public + bâtiment).\n" +
                    "**Les connexions extérieures et les zones d’attente inconnues sont exclues.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Évaluation du parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Évalue la part de stationnement à capacité exacte encore <libre>.\n" +
                    "<MAUVAIS> = moins de 15 % libre.\n" +
                    "<OK> = de 15 % à moins de 30 % libre.\n" +
                    "<BON> = 30 % ou plus libre.\n" +
                    "<Public> utilise les installations comptées par le panneau Stationnement des Routes de CS2.\n" +
                    "<Bât.> utilise les places à capacité exacte des bâtiments et garages."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Position autos" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Cette ligne affiche les données de toute la ville, pas seulement des quartiers interdits.\n" +
                    "<Rue> = stationnées sur les voies publiques.\n" +
                    "<Visibles> = voitures visibles et cliquables dans les parkings ouverts ou les places extérieures des bâtiments.\n" +
                    "<Intérieur> = dans les bâtiments ou garages.\n" +
                    "<OC> = stockage de connexion extérieure à la limite de la ville ; certaines voitures de ménages entrants commencent là (zone d’attente).\n" +
                    "Les voitures sans voie de stationnement attribuée sont omises ici et visibles seulement dans le rapport du journal (onglet À propos)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Mis à jour" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Heure de la dernière actualisation de ces valeurs à l’échelle de la ville." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nom du mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Lien Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Ouvre la page de l’auteur sur Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Écrire rapport" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Écrit les détails du stationnement sur rue et les données associées dans \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Si besoin, écrivez un 2e rapport plus tard dans la même ville chargée.\n" +
                    "- Compare jusqu’à 20 ID d’entité échantillonnés de différentes catégories.\n" +
                    "- Indique si chaque échantillon est resté, a roulé, s’est garé ailleurs ou a disparu.\n" +
                    "- Scene Explorer est nécessaire pour suivre les ID d’entité dans la ville."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Ouvrir le journal" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Ouvre <Logs/ParkingControl.log>, ou le dossier Logs si le fichier n’existe pas encore." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Journal détaillé" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Détails DEBUG automatiques.\n" +
                    "Pas pour une partie normale ; désactivez-le hors diagnostic.\n" +
                    "Écrire rapport fonctionne même désactivé."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Aucune ville chargée." },
                { ParkingStatusLocale.kCollecting, "Collecte de l’état du stationnement..." },
                { ParkingStatusLocale.kUnavailable, "L’état du stationnement n’est pas disponible." },
                { ParkingStatusLocale.kCollectionFailed, "Impossible de collecter l’état du stationnement ; consultez ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} garées | {1}/{2} fermées{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} garées | {1}/{2} voies fermées{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} garées | {1}/{2} fermées | {3}/{4} quartiers{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} rue | {1} visibles | {2} intérieur | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1} public libre | {2} = {3} bât. libre" },
                { ParkingStatusLocale.kShareFormat, "{0} public | {1} bât. | {2} rue | {3} total" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DÉSACT. = interdictions ville/quartiers coupées | routes manuelles actives" },
                { ParkingStatusLocale.kManualNone, "Aucune" },
                { ParkingStatusLocale.kStatusCheck, "VÉRIF." },
                { ParkingStatusLocale.kRatingPoor, "MAUVAIS" },
                { ParkingStatusLocale.kRatingGood, "BON" },
                { ParkingStatusLocale.kRatingNA, "N/D" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

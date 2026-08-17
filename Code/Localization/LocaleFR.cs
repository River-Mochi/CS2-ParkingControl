// <copyright file="LocaleFR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: French text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// French localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleFR : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// Initializes a new instance of the <see cref="LocaleFR"/> class.
        /// Initializes a new instance of the <see cref="LocaleEN"/> class.
        /// </summary>
        public LocaleFR(PCSettings settings)
        public LocaleEN(PCSettings settings)
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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostic" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Stationnement sur rue interdit" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Choisissez **Ville entière**, **Par quartier** ou **DÉSACTIVÉ**.\n" +
                    "- Les voies admissibles sont bloquées pour empêcher tout nouveau stationnement sur rue.\n" +
                    "- Les voitures déjà garées partent naturellement lorsqu’elles sont réutilisées.\n" +
                    "- Les parkings payants et le stationnement normal des bâtiments restent disponibles.\n" +
                    "**Les autoroutes et les routes asymétriques à 3 voies excluent déjà le stationnement sur rue.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Ville entière" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Par quartier" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DÉSACTIVÉ" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Afficher les instructions" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Explique comment utiliser le mode <Par quartier>.\n" +
                    "DÉSACTIVÉ = les restrictions de stationnement sur rue sont désactivées.\n" +
                    "Ville entière = le stationnement admissible sur rue est bloqué dans toute la ville." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Afficher l’état" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Affiche ci-dessous les totaux actuels de stationnement.>\n" +
                    "Le statut n’est collecté que lorsque le menu Options est " +
                    "ouvert ; aucun scan en arrière-plan ne tourne pendant la partie." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Mode par quartier>\n" +
                    "1. Choisissez <Par quartier> ci-dessus.\n" +
                    "2. Créez ou sélectionnez un quartier dans la ville.\n" +
                    "3. Ouvrez <Politiques> et activez **Interdiction de stationner en bord de route [✓]**.\n" +
                    "Les routes hors des quartiers sélectionnés gardent le stationnement normal." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Interdiction de stationner en bord de route" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "Empêche les <voitures et motos> de stationner en bord de route dans ce quartier. " +
                    "Les véhicules déjà garés partent lorsque leurs propriétaires les réutilisent." },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Parking sur rue" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Garées> = voitures encore garées dans les rues où le mode choisi interdit le stationnement.\n" +
                    "<Voies> = sections de stationnement en bord de route occupées par ces voitures. Une voie peut en contenir plusieurs.\n" +
                    "<Fermées> = voies de stationnement sur rue fermées aux nouveaux véhicules.\n" +
                    "<Par quartier> affiche :\n" +
                    "- Voies occupées dans les quartiers interdits / voies occupées dans toute la ville.\n" +
                    "- Voies fermées / voies admissibles de la ville.\n" +
                    "- Quartiers activés / quartiers totaux.\n" +
                    "<Rues nouvelles ou reconstruites> peuvent accepter brièvement quelques voitures pendant la mise à jour de leurs voies. " +
                    "Les voitures déjà garées partent lorsque les citoyens les utilisent.\n" +
                    "<VÉRIF.> = certaines routes sélectionnées ne sont pas encore bloquées. Faites tourner la ville un moment puis " +
                    "revérifiez. Si <VÉRIF.> reste affiché, joignez un rapport de stationnement quand vous demandez de l’aide." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Usage des rues" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Cette ligne couvre <toute la ville>, pas seulement les quartiers.\n" +
                    "<Garées sur rue> = pourcentage garé dans la rue plutôt que dans les parkings publics ou de bâtiments.\n" +
                    "<Actifs> = véhicules personnels roulant ou attendant dans la circulation.\n" +
                    "<Formule> = rue ÷ (rue + public occupé + bâtiment occupé).\n" +
                    "**Le stockage des connexions extérieures (OC) et les voitures sans voie de stationnement attribuée sont exclus.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Places" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Affiche l’occupation des parkings dans toute la ville.\n" +
                    "<Public> utilisé = installations comptées par la vue d’information Stationnement du jeu de base.\n" +
                    "<Bâtiment> utilisé = stationnement inclus avec logements, lieux de travail et commerces.\n" +
                    "**Un % d’utilisation plus élevé = davantage de stationnement peut être nécessaire.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Position autos" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Cette ligne affiche les données de toute la ville, pas seulement des quartiers concernés par l’interdiction.\n" +
                    "<Rue> = stationnées sur la voie publique.\n" +
                    "<Visibles> = voitures visibles et cliquables dans les parkings à ciel ouvert ou les places extérieures des bâtiments.\n" +
                    "<Cachées> = dans les bâtiments ou garages.\n" +
                    "<OC> = stockage de connexion extérieure à la limite de la ville ; certaines voitures de ménages entrants commencent là comme zone de transit.\n" +
                    "Les voitures sans voie de stationnement attribuée sont omises ici et visibles uniquement dans le rapport du journal, onglet À propos." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Mis à jour" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "Heure de la dernière actualisation de ces valeurs de statut à l’échelle de la ville." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nom du mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Lien Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Ouvre la page de l’auteur sur Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Écrire un rapport de stationnement" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Écrit les détails du stationnement sur rue et les informations associées dans \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Si vous êtes curieux, écrivez un 2e rapport plus tard dans la même ville chargée.\n" +
                    "- Compare jusqu’à 20 ID d’entité échantillonnés de différentes catégories.\n" +
                    "- Indique si chaque échantillon est resté, a commencé à rouler, s’est garé ailleurs ou a disparu.\n" +
                    "- Le mod Scene Explorer est nécessaire pour suivre les numéros d’ID d’entité dans la ville."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Ouvrir le journal" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Ouvre <Logs/ParkingControl.log>, ou le dossier Logs si le fichier n’existe pas encore." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Aucune ville chargée." },
                { ParkingStatusLocale.kCollecting, "Collecte de l’état du stationnement..." },
                { ParkingStatusLocale.kUnavailable, "L’état du stationnement n’est pas disponible." },
                { ParkingStatusLocale.kCollectionFailed, "Impossible de collecter l’état du stationnement ; consultez ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} garées ({1} voies) | {2}/{3} fermées{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} garées ({1}/{2} voies) | {3}/{4} fermées | {5}/{6} quartiers{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} rue | {1} visibles | {2} cachées | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} public {1}/{2} | {3} bâtiment {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} sur rue {1} | {2} actifs" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DÉSACT." },
                { ParkingStatusLocale.kStatusCheck, "VÉRIF." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

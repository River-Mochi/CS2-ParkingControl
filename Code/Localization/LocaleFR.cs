// <copyright file="LocaleFR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the French text for Parking Control's Options UI.

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

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleFR"/> class.
        /// </summary>
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
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
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Actions" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "À propos" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Stationnement sur rue" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "État des véhicules personnels" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informations sur le mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Liens" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnostic" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NoStreetParking)), "Aucun stationnement sur rue (ville entière)" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.NoStreetParking)),
                    "- Empêche les véhicules personnels et les motos de se garer dans la rue à l'avenir.\n" +
                    "- Les parkings et le stationnement inclus dans les bâtiments restent disponibles.\n" +
                    "- Les véhicules déjà stationnés ne sont pas supprimés. Ils partent naturellement la prochaine fois qu'un citoyen utilise la voiture.\n" +
                    "- Assurez-vous que la ville dispose de suffisamment de stationnement hors rue, sinon les voitures risquent de beaucoup rouler pour trouver une place." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Stationnement sur rue" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Stationnés> = voitures encore stationnées dans les rues.\n" +
                    "<Voies> = sections de stationnement en bord de route contenant ces voitures. Une voie peut contenir plusieurs voitures.\n" +
                    "<Désactivées> = voies de stationnement sur rue fermées aux nouveaux stationnements.\n" +
                    "<OK> = Aucun stationnement sur rue est activé et fonctionne.\n" +
                    "<DÉSACTIVÉ> = Aucun stationnement sur rue est désactivé ; les voitures peuvent se garer librement dans les rues ordinaires.\n" +
                    "<VÉRIFIER> = les routes sont peut-être encore en cours de mise à jour. Attendez un moment ; écrivez un rapport dans le journal si cela persiste.\n" +
                    "**Certaines voitures peuvent rester après l'activation de la règle sans stationnement sur rue ou après une modification des routes. Elles partent naturellement si le citoyen utilise la voiture.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Emplacement des voitures" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Rue> = stationnées sur la voie publique.\n" +
                    "<Visibles> = voitures que vous pouvez voir et sélectionner dans les parkings à ciel ouvert ou les stationnements extérieurs inclus avec les bâtiments.\n" +
                    "<Cachées> = à l'intérieur de bâtiments ou de garages.\n" +
                    "<OC> = stockage de connexion extérieure à la limite de la ville ; certaines voitures de ménages entrants commencent ici.\n" +
                    "**La zone d'attente non attribuée du jeu de base est uniquement enregistrée dans le journal.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Stationnement" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<Pourcentage> et <utilisé / total> indiquent l'occupation du stationnement.\n" +
                    "<Public> = installations comptées par la vue d'information Stationnement du jeu de base.\n" +
                    "<Bâtiment> = stationnement inclus avec les logements et les lieux de travail.\n" +
                    "**Le stationnement des bâtiments comprend les places extérieures visibles et le stationnement intérieur.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Utilisation des rues" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Stationnées dans la rue> = part des voitures utilisant un stationnement connu sur rue, public ou de bâtiment.\n" +
                    "<En déplacement> = véhicules personnels qui roulent ou attendent dans la circulation.\n" +
                    "<Mis à jour> = dernière actualisation de l'état.\n" +
                    "**Les connexions extérieures (OC) et la zone d'attente non attribuée sont exclues.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nom du mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Version" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Lien Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Ouvrir la page de l'auteur sur Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Écrire un rapport de stationnement" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Écrire les détails du stationnement sur rue, de l'offre, de la propriété et de l'emplacement des véhicules\n" +
                    "dans <ParkingControl.log>\n" +
                    "Un second rapport dans la même ville chargée suit les mêmes ID d'entités des voitures stationnées dans la rue." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Ouvrir le journal" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Ouvrir <ParkingControl.log>, ou le dossier Logs si le fichier n'existe pas encore." },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "Chargez ou démarrez une ville pour afficher l'état du stationnement." },
                { ParkingStatusLocale.kCollecting, "Collecte de l'état du stationnement..." },
                { ParkingStatusLocale.kUnavailable, "L'état du stationnement n'est pas disponible." },
                { ParkingStatusLocale.kCollectionFailed, "Impossible de collecter l'état du stationnement ; consultez ParkingControl.log." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} stationnées ({1} voies) | {2}/{3} désactivées | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} rue | {1} visibles | {2} cachées | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} public | {3}  {4} / {5} bâtiment" },
                { ParkingStatusLocale.kShareFormat, "{0} stationnées dans la rue | {1} en déplacement | mis à jour {2}" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DÉSACTIVÉ" },
                { ParkingStatusLocale.kStatusCheck, "VÉRIFIER" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

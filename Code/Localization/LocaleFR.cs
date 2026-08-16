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
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Stationnement sur rue interdit" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Choisissez où interdire tout nouveau stationnement sur rue. Les voitures déjà garées partent naturellement ; les parkings et bâtiments restent disponibles." },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Ville entière" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Par quartier" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "Désactivé" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Afficher les instructions" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)), "Afficher comment utiliser le mode par quartier." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Afficher l’état" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "Affiche les totaux de stationnement. Données collectées seulement si affichées." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Mode par quartier>\n" +
                    "1. Choisissez <Par quartier> ci-dessus.\n" +
                    "2. Créez ou sélectionnez un quartier dans la ville.\n" +
                    "3. Ouvrez <Politiques> et activez <Stationnement sur rue interdit>.\n" +
                    "Le stationnement normal reste permis hors des quartiers sélectionnés." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Stationnement sur rue interdit" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "En mode Par quartier, empêche tout nouveau stationnement sur rue ici. " +
                    "Les voitures déjà garées partent naturellement." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Stationnement sur rue" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Garées> = voitures dans les rues couvertes par le mode choisi.\n" +
                    "<Voies> = sections de stationnement en bord de route contenant ces voitures. Une voie peut contenir plusieurs voitures.\n" +
                    "<Fermées> = voies de stationnement sur rue fermées aux nouveaux stationnements.\n" +
                    "<OK> = Aucun stationnement sur rue est activé et fonctionne.\n" +
                    "<OFF> = Aucun stationnement sur rue est désactivé ; les voitures peuvent se garer librement dans les rues ordinaires.\n" +
                    "<VÉRIF.> = les routes sont peut-être encore en cours de mise à jour. Attendez un moment ; écrivez un rapport dans le journal si cela persiste.\n" +
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
                    "<Rue> = part des voitures utilisant un stationnement connu sur rue, public ou de bâtiment.\n" +
                    "<Roulent> = véhicules personnels qui roulent ou attendent dans la circulation.\n" +
                    "<Màj> = dernière actualisation de l'état.\n" +
                    "**Les connexions extérieures (OC) et la zone d'attente non attribuée sont exclues.**"
                },
                // Translate these two new entries from LocaleEN.cs.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Updated" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "Time when these citywide status values were last refreshed." },
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
                { ParkingStatusLocale.kLoadCity, "Aucune ville chargée." },
                { ParkingStatusLocale.kCollecting, "Collecte de l'état du stationnement..." },
                { ParkingStatusLocale.kUnavailable, "L'état du stationnement n'est pas disponible." },
                { ParkingStatusLocale.kCollectionFailed, "Impossible de collecter l'état du stationnement ; consultez ParkingControl.log." },
                // Translate these formats without changing the numbered placeholders.
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} parked ({1} lanes) | {2}/{3} disabled{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} parked ({1}/{2} lanes) | {3}/{4} disabled | {5}/{6} districts{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} rue | {1} visibles | {2} cachées | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} public {1}/{2} | {3} building {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} street parked {1} | {2} active" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "OFF" },
                { ParkingStatusLocale.kStatusCheck, "VÉRIF." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

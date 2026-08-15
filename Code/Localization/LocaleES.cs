// <copyright file="LocaleES.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Spanish text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Spanish localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleES : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleES"/> class.
        /// </summary>
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
        public LocaleES(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Acciones" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Acerca de" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Estacionamiento en la calle" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Estado de vehículos personales" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Información del mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Enlaces" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnóstico" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Sin estacionamiento en la calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Elige dónde se bloquea el estacionamiento nuevo en la calle. Los coches ya estacionados se van de forma natural; los estacionamientos y edificios siguen disponibles." },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Toda la ciudad" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Por distrito" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "Desactivado" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar instrucciones" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar cómo usar el modo por distrito." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostrar estado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "Muestra los totales de estacionamiento. Solo se recopilan al mostrarlos." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modo por distrito>\n" +
                    "1. Elige <Por distrito> arriba.\n" +
                    "2. Crea o selecciona un distrito en la ciudad.\n" +
                    "3. Abre <Políticas> y activa <Sin estacionamiento en la calle>.\n" +
                    "Las calles fuera de los distritos seleccionados conservan el estacionamiento normal." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Sin estacionamiento en la calle" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "En el modo Por distrito, impide nuevos estacionamientos en la calle aquí. " +
                    "Los coches ya aparcados se van de forma natural." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Estacionamiento en la calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Aparc.> = coches en calles cubiertas por el modo seleccionado.\n" +
                    "<Carr.> = secciones de estacionamiento junto a la calle que contienen esos coches. Un carril puede contener varios coches.\n" +
                    "<Cerrados> = carriles de estacionamiento en la calle cerrados a nuevos vehículos.\n" +
                    "<OK> = Sin estacionamiento en la calle está activado y funcionando.\n" +
                    "<OFF> = Sin estacionamiento en la calle está desactivado; los coches pueden estacionar libremente en calles normales.\n" +
                    "<REVISAR> = es posible que las carreteras aún se estén actualizando. Espera un momento; escribe un informe en el registro si continúa.\n" +
                    "**Algunos coches pueden permanecer después de activar la regla sin estacionamiento en la calle o de cambiar carreteras. Se van de forma natural si el ciudadano usa el coche.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Ubicación de los coches" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Calle> = estacionados en vías públicas.\n" +
                    "<Visibles> = coches que puedes ver y seleccionar en estacionamientos al aire libre o en estacionamiento exterior incluido con edificios.\n" +
                    "<Ocultos> = dentro de edificios o garajes.\n" +
                    "<OC> = almacenamiento de conexión exterior en el límite de la ciudad; algunos coches de hogares que llegan empiezan allí.\n" +
                    "**La zona de espera sin asignar del juego base solo aparece en el registro.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Estacionamiento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<Porcentaje> y <usado / total> muestran la ocupación del estacionamiento.\n" +
                    "<Público> = instalaciones contabilizadas por la vista de información de Estacionamiento del juego base.\n" +
                    "<Edificio> = estacionamiento incluido con viviendas y lugares de trabajo.\n" +
                    "**El estacionamiento de edificios incluye espacios exteriores visibles y estacionamiento interno.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso de la calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Calle> = proporción de coches que usan estacionamiento conocido en la calle, público o de edificios.\n" +
                    "<En marcha> = vehículos personales circulando o esperando en el tráfico.\n" +
                    "<Act.> = última actualización del estado.\n" +
                    "**Se excluyen la conexión exterior (OC) y la zona de espera sin asignar.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nombre del mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versión" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Enlace de Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Abrir la página del autor en Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Escribir informe de estacionamiento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Escribir detalles del estacionamiento en la calle, la oferta, la propiedad y la ubicación de vehículos\n" +
                    "en <ParkingControl.log>\n" +
                    "Un segundo informe en la misma ciudad cargada sigue los mismos ID de entidad de los coches estacionados en la calle." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Abrir registro" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Abrir <ParkingControl.log>, o la carpeta Logs si el archivo todavía no existe." },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "No hay ciudad cargada." },
                { ParkingStatusLocale.kCollecting, "Recopilando el estado del estacionamiento..." },
                { ParkingStatusLocale.kUnavailable, "El estado del estacionamiento no está disponible." },
                { ParkingStatusLocale.kCollectionFailed, "No se pudo recopilar el estado del estacionamiento; consulta ParkingControl.log." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} aparc. ({1} carr.) | {2}/{3} cerrados | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} calle | {1} visibles | {2} ocultos | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} {1}/{2} público | {3} {4}/{5} edificio" },
                { ParkingStatusLocale.kShareFormat, "{0} calle | {1} en marcha | act. {2}" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "OFF" },
                { ParkingStatusLocale.kStatusCheck, "REVISAR" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

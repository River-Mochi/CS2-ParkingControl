// <copyright file="LocaleES.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Spanish text for Parking Control's Options UI.

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
        /// <param name="settings">Options settings whose localization IDs are used.</param>
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
                // Options tabs and groups.
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Acciones" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Acerca de" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Estacionamiento en la calle" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Estado de vehículos personales" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Información del mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Enlaces" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnóstico" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Sin estacionamiento en la calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Elige **Toda la ciudad**, **Por distrito** o **DESACTIVADO**.\n" +
                    "- Los carriles aptos se bloquean para impedir nuevos estacionamientos en la calle.\n" +
                    "- Los coches ya estacionados se van cuando vuelven a usarse.\n" +
                    "- Los estacionamientos de pago y el estacionamiento normal de edificios siguen disponibles.\n" +
                    "**Las autopistas y las carreteras asimétricas de 3 carriles ya excluyen el estacionamiento en la calle.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Toda la ciudad" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Por distrito" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DESACTIVADO" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar instrucciones" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Muestra cómo usar el modo <Por distrito>.\n" +
                    "DESACTIVADO = se desactivan las restricciones de estacionamiento en la calle.\n" +
                    "Toda la ciudad = se bloquea el estacionamiento apto en la calle en toda la ciudad." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostrar estado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Muestra abajo los totales actuales de estacionamiento.>\n" +
                    "El estado solo se recopila mientras el menú Opciones está abierto; " +
                    "no hay un escaneo de estado en segundo plano durante el juego." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modo por distrito>\n" +
                    "1. Elige <Por distrito> arriba.\n" +
                    "2. Crea o selecciona un distrito en la ciudad.\n" +
                    "3. Abre <Políticas> y activa **Prohibición de estacionamiento en la calle [✓]**.\n" +
                    "Las calles fuera de los distritos seleccionados mantienen el estacionamiento normal." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Prohibición de estacionamiento en la calle" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "Impide que <coches y motocicletas> aparquen junto a la calle en este distrito. " +
                    "Los vehículos ya estacionados se irán cuando sus dueños vuelvan a usarlos." },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Aparc. en calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Aparc.> = coches que siguen estacionados en calles cubiertas por el modo seleccionado.\n" +
                    "<Carr.> = tramos de estacionamiento junto a la vía que contienen esos coches. Un carril puede contener varios.\n" +
                    "<Desact.> = carriles de estacionamiento en la calle cerrados a nuevos vehículos.\n" +
                    "<Por distrito> muestra:\n" +
                    "- Carriles ocupados en distritos con prohibición / carriles ocupados en toda la ciudad.\n" +
                    "- Carriles desactivados / carriles aptos de la ciudad.\n" +
                    "- Distritos activados / distritos totales.\n" +
                    "<Carreteras nuevas o reconstruidas> pueden aceptar unos pocos coches brevemente mientras se actualizan sus carriles.\n" +
                    "Los coches ya estacionados se van de forma natural.\n" +
                    "<REVISAR> = algunas calles seleccionadas aún no están bloqueadas. Deja correr la ciudad un poco " +
                    "y vuelve a comprobar. Si sigue <REVISAR>, incluye un informe de estacionamiento al pedir ayuda." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso de calles" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Esta fila incluye <toda la ciudad>, no solo los distritos.\n" +
                    "<Estacionados en la calle> = porcentaje aparcado en calles en vez de estacionamientos públicos o de edificios.\n" +
                    "<Activos> = vehículos personales circulando o esperando en tráfico.\n" +
                    "<Fórmula> = calle ÷ (calle + público ocupado + edificio ocupado).\n" +
                    "**Se excluyen el almacenamiento de conexiones exteriores (OC) y los coches sin carril de estacionamiento asignado.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Plazas" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Muestra la ocupación de estacionamiento de toda la ciudad.\n" +
                    "<Público> usado = instalaciones contadas por la vista de información de Estacionamiento del juego base.\n" +
                    "<Edificio> usado = estacionamiento incluido con viviendas, trabajos y tiendas.\n" +
                    "**Un % de uso más alto = puede hacer falta más estacionamiento.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Dónde aparcan" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Esta fila muestra datos de toda la ciudad, no solo de distritos con la prohibición.\n" +
                    "<Calle> = estacionados en vías públicas.\n" +
                    "<Visibles> = coches que puedes ver y seleccionar en estacionamientos al aire libre o plazas exteriores de edificios.\n" +
                    "<Ocultos> = dentro de edificios o garajes.\n" +
                    "<OC> = almacenamiento de conexión exterior en el límite de la ciudad; algunos coches de hogares entrantes empiezan allí como zona de espera.\n" +
                    "Los coches sin carril de estacionamiento asignado se omiten aquí y solo aparecen en el informe del registro de la pestaña Acerca de." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Actualizado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)),
                    "Hora de la última actualización de estos valores de estado de toda la ciudad." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nombre del mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versión" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Enlace de Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Abre la página del autor en Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Escribir informe de estacionamiento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Escribe detalles del estacionamiento en la calle y datos relacionados en \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Si te interesa, escribe un 2.º informe más tarde en la misma ciudad cargada.\n" +
                    "- Compara hasta 20 ID de entidad de muestra de distintas categorías.\n" +
                    "- Muestra si cada muestra permaneció, empezó a circular, aparcó en otro lugar o desapareció.\n" +
                    "- Se necesita el mod Scene Explorer para seguir los números de ID de entidad dentro de la ciudad."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Abrir registro" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)),
                    "Abre <Logs/ParkingControl.log>, o la carpeta Logs si el archivo aún no existe." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "No hay ciudad cargada." },
                { ParkingStatusLocale.kCollecting, "Recopilando el estado del estacionamiento..." },
                { ParkingStatusLocale.kUnavailable, "El estado del estacionamiento no está disponible." },
                { ParkingStatusLocale.kCollectionFailed, "No se pudo recopilar el estado del estacionamiento; consulta ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat,
                    "{0} aparc. ({1} carr.) | {2}/{3} desact.{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat,
                    "{0} aparc. ({1}/{2} carr.) | {3}/{4} desact. | {5}/{6} dist.{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} calle | {1} visibles | {2} ocultos | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} público {1}/{2} | {3} edificio {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} en calle {1} | {2} activos" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DESACTIVADO" },
                { ParkingStatusLocale.kStatusCheck, "REVISAR" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

// <copyright file="LocaleES.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Aparcamiento en calle" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Estado de vehículos personales" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Información del mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Enlaces" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnóstico" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Sin aparcamiento en calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Elige **Toda la ciudad**, **Por distrito** o **DESACTIVADO**.\n" +
                    "- Los carriles aptos se bloquean para impedir nuevos aparcamientos en la calle.\n" +
                    "- Los coches ya aparcados se van cuando vuelven a usarse.\n" +
                    "- Los aparcamientos de pago y el aparcamiento normal de edificios siguen disponibles.\n" +
                    "**Algunas carreteras ya excluyen el aparcamiento en la calle, como las autopistas y los callejones pequeños de doble sentido.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Toda la ciudad" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Por distrito" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DESACTIVADO" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar instrucciones" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Muestra cómo usar el modo <Por distrito>.\n" +
                    "1.a. DESACTIVADO = se desactivan las restricciones de toda la ciudad y de los distritos; vuelve en gran parte a los valores normales del juego.\n" +
                    "1.b. El botón <Prohibido aparcar> para una sola carretera en el panel Servicios de carretera sigue funcionando, igual que al aplicar un paso de peatones.\n" +
                    "2. Toda la ciudad = bloquea todo el aparcamiento público en calle apto de la ciudad." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostrar estado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Muestra abajo los totales actuales de aparcamiento.>\n" +
                    "El estado solo se recopila mientras el menú Opciones está abierto; no hay un escaneo de estado en segundo plano durante el juego." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modo por distrito>\n" +
                    "1. Elige <Por distrito> arriba.\n" +
                    "2. Crea o selecciona un distrito en la ciudad.\n" +
                    "3. Abre <Políticas> y activa **Prohibido aparcar en la calle [✓]**.\n" +
                    "4. Puedes activar a la vez la prohibición y la tarifa de aparcamiento. La tarifa se cobra a los coches que aún sigan allí o consigan aparcar.\n" +
                    "Las calles fuera de los distritos con prohibición mantienen el aparcamiento normal." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Prohibido aparcar" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]",
                    "Activa o desactiva el aparcamiento junto a la vía en un lado de la carretera. Para varios lados, arrastra sobre ellos antes de soltar el botón izquierdo del ratón." },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Prohibido aparcar en la calle" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Impide que coches y motos aparquen junto a la vía en este distrito. Los vehículos ya aparcados se irán cuando sus dueños vuelvan a usarlos." },
                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Añadir" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Quitar" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Aparc. en calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Aparc.> = coches que siguen aparcados en lados de la carretera marcados como Prohibido aparcar por Parking Control.\n" +
                    "<Carr.> = tramos de aparcamiento junto a la vía que contienen esos coches. Un tramo puede contener muchos coches.\n" +
                    "<Desact.> = tramos de carril de aparcamiento cerrados a nuevos vehículos. Una carretera puede contener varios tramos.\n" +
                    "<DESACTIVADO + Prohibido aparcar manual> = DESACTIVADO desactiva las prohibiciones de toda la ciudad y por distrito, pero los lados de carretera marcados manualmente como Prohibido aparcar siguen activos. Esta fila muestra entonces solo esas prohibiciones manuales.\n" +
                    "<---------------------->\n" +
                    "Si está seleccionado <Por distrito>, muestra:\n" +
                    "- Carriles ocupados en distritos con prohibición / carriles ocupados en toda la ciudad.\n" +
                    "- Carriles desactivados / carriles aptos de la ciudad.\n" +
                    "- Distritos activados / distritos totales.\n" +
                    "<---------------------->\n" +
                    "Nota: después de cambiar o reconstruir carreteras, el recuento de tramos desactivados puede tardar un poco mientras CS2 reconstruye los carriles de aparcamiento. Deja correr la ciudad un poco y vuelve a abrir Opciones." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso de calles" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Esta fila incluye <toda la ciudad>, no solo los distritos.\n" +
                    "<Aparcados en la calle> = porcentaje aparcado en calles en vez de aparcamientos públicos o de edificios.\n" +
                    "<Activos> = vehículos personales circulando o esperando en tráfico.\n" +
                    "<Fórmula> = calle ÷ (calle + público ocupado + edificio ocupado).\n" +
                    "**Se excluyen el almacenamiento de conexiones exteriores (OC) y los coches sin carril de aparcamiento asignado.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Plazas" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Muestra la ocupación de aparcamiento de toda la ciudad.\n" +
                    "<Público> usado = instalaciones contadas por la vista de información de Aparcamiento del juego base.\n" +
                    "<Edificio> usado = aparcamiento incluido con viviendas, trabajos y tiendas.\n" +
                    "**Un % de uso más alto = puede hacer falta más aparcamiento.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Dónde aparcan" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Esta fila muestra datos de toda la ciudad, no solo de distritos con la prohibición.\n" +
                    "<Calle> = aparcados en vías públicas.\n" +
                    "<Visibles> = coches que puedes ver y seleccionar en aparcamientos al aire libre o plazas exteriores de edificios.\n" +
                    "<Dentro> = dentro de edificios o garajes.\n" +
                    "<OC> = almacenamiento de conexión exterior en el límite de la ciudad; algunos coches de hogares entrantes empiezan allí como zona de espera.\n" +
                    "Los coches sin carril de aparcamiento asignado se omiten aquí y solo aparecen en el informe del registro de la pestaña Acerca de." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Actualizado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Hora de la última actualización de estos valores de estado de toda la ciudad." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nombre del mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versión" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Enlace de Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Abre la página del autor en Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Escribir informe de aparcamiento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Escribe detalles del aparcamiento en la calle y datos relacionados en \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Si te interesa, escribe un 2.º informe más tarde en la misma ciudad cargada.\n" +
                    "- Compara hasta 20 ID de entidad de muestra de distintas categorías.\n" +
                    "- Muestra si cada muestra permaneció, empezó a circular, aparcó en otro lugar o desapareció.\n" +
                    "- Se necesita el mod Scene Explorer para seguir los números de ID de entidad dentro de la ciudad." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Abrir registro" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Abre <Logs/ParkingControl.log>, o la carpeta Logs si el archivo aún no existe." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "No hay ciudad cargada." },
                { ParkingStatusLocale.kCollecting, "Recopilando el estado del aparcamiento..." },
                { ParkingStatusLocale.kUnavailable, "El estado del aparcamiento no está disponible." },
                { ParkingStatusLocale.kCollectionFailed, "No se pudo recopilar el estado del aparcamiento; consulta ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} aparc. ({1} carr.) | {2}/{3} desact.{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} aparc. ({1}/{2} carr.) | {3}/{4} desact. | {5}/{6} dist.{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} calle | {1} visibles | {2} dentro | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} público {1}/{2} | {3} edificio {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} en calle {1} | {2} activos" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DESACT." },
                { ParkingStatusLocale.kStatusCheck, "REVISAR" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

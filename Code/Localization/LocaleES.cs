// <copyright file="LocaleES.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Spanish text for Parking Control's Options UI.

using System.Collections.Generic;
using Colossal;

namespace ParkingControl
{

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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Diagnóstico" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Sin aparcamiento en calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Elige:\n" +
                    "<1. Por distrito>\n" +
                    "<2. Toda la ciudad>\n" +
                    "<3. DESACTIVADO>\n" +
                    "- Los carriles aptos se bloquean para impedir nuevos aparcamientos en la calle.\n" +
                    "- Los coches ya aparcados se van desplazando tras la prohibición; las zonas grandes tardan más.\n" +
                    "- Los aparcamientos de pago y el aparcamiento normal de edificios siguen disponibles.\n" +
                    "**Algunas carreteras ya excluyen el aparcamiento en calle, como autopistas y callejones pequeños de doble sentido.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Toda la ciudad" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Por distrito" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DESACTIVADO" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar instrucciones" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Muestra cómo usar <Por distrito>.\n" +
                    "1.a. DESACTIVADO = se desactivan las restricciones de toda la ciudad y distritos; vuelve en gran parte al juego normal.\n" +
                    "1.b. El botón <Prohibido aparcar> para una carretera en Servicios de carretera sigue funcionando como un paso de peatones.\n" +
                    "2. Toda la ciudad = bloquea todo el aparcamiento público en calle apto."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostrar estado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Muestra abajo los totales actuales de aparcamiento.>\n" +
                    "El estado solo se recopila mientras el menú Opciones está abierto;\n" +
                    "no hay escaneo en segundo plano durante el juego."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modo por distrito>\n" +
                    "1. Elige <Por distrito> arriba.\n" +
                    "2. Crea/selecciona un distrito.\n" +
                    "3. Abre <Políticas> y activa **Prohibido aparcar en la calle [✓]**.\n" +
                    "4. La prohibición y la tarifa pueden estar activas a la vez. Se cobra a los coches que aún estén allí o consigan aparcar.\n" +
                    "Las calles fuera de distritos con prohibición mantienen el aparcamiento normal."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Prohibido aparcar" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Activa o desactiva el aparcamiento en un lado de la carretera. Para varios lados, arrastra sobre ellos antes de soltar el botón izquierdo del ratón." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Prohibido aparcar en la calle" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Impide que coches y motos aparquen junto a la vía en este distrito.\n" +
                    "- Los coches ya aparcados se van desplazando poco a poco; las zonas grandes tardan más."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Añadir" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Quitar" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Aparc. en calle" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Muestra solo el ámbito seleccionado <Toda la ciudad> o <Por distrito>. Las prohibiciones manuales se muestran aparte.\n" +
                    "<DESACT.> = se desactivan las prohibiciones de ciudad/distritos; las carreteras con <Prohibido aparcar> manual siguen activas.\n" +
                    "<Aparc.> = coches aún aparcados en calles del ámbito seleccionado.\n" +
                    "<Desact.> = tramos de carril junto al bordillo desactivados / objetivo.\n" +
                    "<Distritos> = distritos con prohibición / distritos totales.\n" +
                    "<REVISAR> = algunos tramos objetivo aún no coinciden con la prohibición seleccionada.\n" +
                    "<---------------------->\n" +
                    "**Si aparece [REVISAR] tras cambiar o reconstruir carreteras, deja correr la ciudad un rato y vuelve a abrir Opciones > Estado. Si sigue, usa Acerca de > Diagnóstico > Escribir informe.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Prohibición manual" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Muestra solo carreteras marcadas con la herramienta manual <Prohibido aparcar>.\n" +
                    "<Aparc.> = coches aún aparcados en esas carreteras prohibidas manualmente.\n" +
                    "<Desact.> = tramos de carril junto al bordillo desactivados / marcados manualmente.\n" +
                    "Las prohibiciones manuales pueden solaparse con Toda la ciudad o distritos; no sumes esta fila a <Aparc. en calle>.\n" +
                    "**Si aparece [REVISAR] después de dejar correr la ciudad un rato, usa Acerca de > Diagnóstico > Escribir informe y envíalo al pedir ayuda.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso de aparcamiento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Público> = plazas ocupadas en instalaciones de aparcamiento.\n" +
                    "Coincide aproximadamente con el panel de aparcamiento de Carreteras de CS2.\n" +
                    "<Edif.> = vehículos aparcados en edificios o garajes.\n" +
                    "<Calle> = coches aparcados en calles.\n" +
                    "<Total> = coches aparcados conocidos en la ciudad (calle + público + edificio).\n" +
                    "**Se excluyen las conexiones exteriores y la espera de ubicación desconocida.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Valoración de parking" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Valora cuánto aparcamiento con capacidad exacta queda <libre>.\n" +
                    "<MALO> = menos del 15 % libre.\n" +
                    "<OK> = del 15 % a menos del 30 % libre.\n" +
                    "<BUENO> = 30 % o más libre.\n" +
                    "<Público> usa instalaciones contadas por el panel de aparcamiento de Carreteras de CS2.\n" +
                    "<Edif.> usa aparcamiento de capacidad exacta en edificios y garajes."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Dónde aparcan" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Esta fila muestra datos de toda la ciudad, no solo distritos con prohibición.\n" +
                    "<Calle> = aparcados en vías públicas.\n" +
                    "<Visibles> = coches visibles y seleccionables en aparcamientos al aire libre o plazas exteriores de edificios.\n" +
                    "<Dentro> = en edificios o garajes.\n" +
                    "<OC> = almacenamiento de conexión exterior en el límite de la ciudad; algunos coches de hogares entrantes empiezan allí (zona de espera).\n" +
                    "Los coches sin carril de aparcamiento asignado se omiten aquí y solo aparecen en el informe del registro (pestaña Acerca de)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Actualizado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Hora de la última actualización de estos valores de toda la ciudad." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nombre del mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versión" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Enlace de Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Abre la página del autor en Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Escribir informe" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Escribe detalles del aparcamiento en calle y datos relacionados en \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Si te interesa, escribe un 2.º informe más tarde en la misma ciudad cargada.\n" +
                    "- Compara hasta 20 ID de entidad de muestra de distintas categorías.\n" +
                    "- Muestra si cada muestra siguió allí, empezó a circular, aparcó en otro lugar o desapareció.\n" +
                    "- Se necesita Scene Explorer para seguir los ID de entidad en la ciudad."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Abrir registro" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Abre <Logs/ParkingControl.log>, o la carpeta Logs si el archivo aún no existe." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Registro detallado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Detalles DEBUG automáticos.\n" +
                    "No es para el juego normal; DESACTÍVALO si no estás depurando.\n" +
                    "Escribir informe sigue funcionando aunque esté DESACTIVADO."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "No hay ciudad cargada." },
                { ParkingStatusLocale.kCollecting, "Recopilando el estado del aparcamiento..." },
                { ParkingStatusLocale.kUnavailable, "El estado del aparcamiento no está disponible." },
                { ParkingStatusLocale.kCollectionFailed, "No se pudo recopilar el estado del aparcamiento; consulta ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} aparc. | {1}/{2} desact.{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} aparc. | {1}/{2} carriles desact.{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} aparc. | {1}/{2} desact. | {3}/{4} distritos{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} calle | {1} visibles | {2} dentro | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1} público libre | {2} = {3} edif. libre" },
                { ParkingStatusLocale.kShareFormat, "{0} público | {1} edif. | {2} calle | {3} total" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DESACT. = sin prohibiciones de ciudad/distrito | manual sigue activo" },
                { ParkingStatusLocale.kManualNone, "Ninguna" },
                { ParkingStatusLocale.kStatusCheck, "REVISAR" },
                { ParkingStatusLocale.kRatingPoor, "MALO" },
                { ParkingStatusLocale.kRatingGood, "BUENO" },
                { ParkingStatusLocale.kRatingNA, "N/D" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

// <copyright file="LocalePT_BR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Brazilian Portuguese text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Brazilian Portuguese localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocalePT_BR : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalePT_BR"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocalePT_BR(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Ações" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Sobre" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Estacionamento na rua" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Status dos veículos particulares" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informações do mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Links" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Diagnóstico" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Sem estacionamento na rua" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Escolha:\n" +
                    "<1. Por distrito>\n" +
                    "<2. Cidade inteira>\n" +
                    "<3. DESLIGADO>\n" +
                    "- As faixas elegíveis são bloqueadas para impedir novos estacionamentos na rua.\n" +
                    "- Carros já estacionados se mudam aos poucos após a proibição; áreas grandes levam mais tempo.\n" +
                    "- Estacionamentos pagos e vagas normais de edifícios continuam disponíveis.\n" +
                    "**Algumas vias já não permitem estacionamento na rua, como rodovias e pequenas vielas de mão dupla.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Cidade inteira" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Por distrito" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DESLIGADO" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar instruções" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Mostra como usar <Por distrito>.\n" +
                    "1.a. DESLIGADO = proibições da cidade e distritos ficam desativadas; volta em grande parte ao padrão do jogo.\n" +
                    "1.b. O botão <Proibido estacionar> para uma via em Serviços de estrada continua funcionando como uma faixa de pedestres.\n" +
                    "2. Cidade inteira = bloqueia todo o estacionamento público elegível na rua."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostrar status" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Mostra abaixo os totais atuais de estacionamento.>\n" +
                    "O status só é coletado enquanto o menu Opções está aberto;\n" +
                    "não há varredura em segundo plano durante o jogo."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modo por distrito>\n" +
                    "1. Escolha <Por distrito> acima.\n" +
                    "2. Crie/selecione um distrito.\n" +
                    "3. Abra <Políticas> e ative **Proibição de estacionamento à beira da via [✓]**.\n" +
                    "4. A proibição e a tarifa podem ficar ativas juntas. A tarifa é cobrada dos carros que ainda estiverem lá ou conseguirem estacionar.\n" +
                    "Ruas fora dos distritos com proibição mantêm o estacionamento normal."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Proibido estacionar" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Ativa ou desativa o estacionamento em um lado da via. Para vários lados, arraste sobre eles antes de soltar o botão esquerdo do mouse." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Proibição de estacionamento à beira da via" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Impede carros e motos de estacionarem à beira da via neste distrito.\n" +
                    "- Carros já estacionados se mudam aos poucos; áreas grandes levam mais tempo."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Adicionar" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Remover" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Estac. na rua" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Mostra só o escopo <Cidade inteira> ou <Por distrito> selecionado. Proibições manuais ficam separadas.\n" +
                    "<DESLIGADO> = proibições da cidade/distritos desligadas; vias com <Proibido estacionar> manual continuam ativas.\n" +
                    "<Estac.> = carros ainda estacionados nas ruas do escopo selecionado.\n" +
                    "<Desat.> = trechos de faixa junto ao meio-fio desativados / trechos alvo.\n" +
                    "<Distritos> = distritos com proibição / total de distritos.\n" +
                    "<VERIF.> = alguns trechos alvo ainda não correspondem à proibição selecionada.\n" +
                    "<---------------------->\n" +
                    "**Se [VERIF.] aparecer após mudar ou reconstruir vias, deixe a cidade rodar um pouco e reabra Opções > Status. Se continuar, use Sobre > Diagnóstico > Gravar relatório.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Proibição manual" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Mostra só vias definidas com a ferramenta manual <Proibido estacionar>.\n" +
                    "<Estac.> = carros ainda estacionados nessas vias proibidas manualmente.\n" +
                    "<Desat.> = trechos de faixa junto ao meio-fio desativados / alvos manuais.\n" +
                    "Proibições manuais podem se sobrepor à Cidade inteira ou distritos; não some esta linha aos totais de <Estac. na rua>.\n" +
                    "**Se [VERIF.] aparecer depois de deixar a cidade rodar um pouco, use Sobre > Diagnóstico > Gravar relatório e envie ao pedir ajuda.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso do estacionamento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Público> = vagas ocupadas em estacionamentos públicos.\n" +
                    "Aproxima o painel de estacionamento das Estradas do CS2.\n" +
                    "<Edif.> = veículos estacionados em edifícios ou garagens.\n" +
                    "<Rua> = carros estacionados nas ruas.\n" +
                    "<Total> = carros estacionados conhecidos na cidade (rua + público + edifício).\n" +
                    "**Conexões externas e áreas de espera desconhecidas ficam fora do total.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Avaliação do estacionamento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Avalia quanto estacionamento com capacidade exata ainda está <livre>.\n" +
                    "<RUIM> = menos de 15% livre.\n" +
                    "<OK> = 15% a menos de 30% livre.\n" +
                    "<BOM> = 30% ou mais livre.\n" +
                    "<Público> usa estacionamentos contados pelo painel de estacionamento das Estradas do CS2.\n" +
                    "<Edif.> usa estacionamento com capacidade exata em edifícios e garagens."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Onde estacionam" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Esta linha mostra dados da cidade inteira, não só dos distritos com proibição.\n" +
                    "<Rua> = estacionados em vias públicas.\n" +
                    "<Visíveis> = carros visíveis e clicáveis em estacionamentos abertos ou vagas externas de edifícios.\n" +
                    "<Dentro> = em edifícios ou garagens.\n" +
                    "<OC> = armazenamento de conexão externa na divisa da cidade; alguns carros de famílias que chegam começam ali (área de espera).\n" +
                    "Carros sem faixa de estacionamento atribuída são omitidos aqui e aparecem só no relatório do log (aba Sobre)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Atualizado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Horário da última atualização destes valores da cidade inteira." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nome do mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versão" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link do Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Abre a página do autor no Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Gravar relatório" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Grava detalhes do estacionamento na rua e dados relacionados em \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Se quiser, grave um 2º relatório mais tarde na mesma cidade carregada.\n" +
                    "- Compara até 20 IDs de entidade de amostra de categorias diferentes.\n" +
                    "- Mostra se cada amostra ficou, começou a dirigir, estacionou em outro lugar ou desapareceu.\n" +
                    "- Scene Explorer é necessário para acompanhar IDs de entidade na cidade."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Abrir log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Abre <Logs/ParkingControl.log> ou a pasta Logs se o arquivo ainda não existir." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Log detalhado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Detalhes DEBUG automáticos.\n" +
                    "Não é para jogo normal; deixe DESLIGADO se não estiver depurando.\n" +
                    "Gravar relatório funciona mesmo DESLIGADO."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Nenhuma cidade carregada." },
                { ParkingStatusLocale.kCollecting, "Coletando o status do estacionamento..." },
                { ParkingStatusLocale.kUnavailable, "O status do estacionamento não está disponível." },
                { ParkingStatusLocale.kCollectionFailed, "Não foi possível coletar o status do estacionamento; consulte ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} estac. | {1}/{2} desat.{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} estac. | {1}/{2} faixas desat.{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} estac. | {1}/{2} desat. | {3}/{4} distr.{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} rua | {1} visíveis | {2} dentro | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1} público livre | {2} = {3} edif. livre" },
                { ParkingStatusLocale.kShareFormat, "{0} público | {1} edif. | {2} rua | {3} total" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DESLIGADO = sem proibição cidade/distritos | vias manuais ativas" },
                { ParkingStatusLocale.kManualNone, "Nenhuma" },
                { ParkingStatusLocale.kStatusCheck, "VERIF." },
                { ParkingStatusLocale.kRatingPoor, "RUIM" },
                { ParkingStatusLocale.kRatingGood, "BOM" },
                { ParkingStatusLocale.kRatingNA, "N/D" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

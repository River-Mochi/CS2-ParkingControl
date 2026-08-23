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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnóstico" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Sem estacionamento na rua" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Escolha **Cidade inteira**, **Por distrito** ou **DESLIGADO**.\n" +
                    "- As faixas elegíveis são bloqueadas para impedir novos estacionamentos na rua.\n" +
                    "- Carros já estacionados saem naturalmente quando voltam a ser usados.\n" +
                    "- Estacionamentos pagos e vagas normais de edifícios continuam disponíveis.\n" +
                    "**Algumas vias já não permitem estacionamento na rua, como rodovias e pequenas vielas de mão dupla.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Cidade inteira" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Por distrito" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "DESLIGADO" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar instruções" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Mostra como usar o modo <Por distrito>.\n" +
                    "1.a. DESLIGADO = as restrições da cidade inteira e dos distritos ficam desativadas; volta em grande parte ao padrão do jogo.\n" +
                    "1.b. O botão <Proibido estacionar> para uma única via no painel Serviços de estrada continua funcionando, como aplicar uma faixa de pedestres.\n" +
                    "2. Cidade inteira = bloqueia todo o estacionamento público elegível na rua da cidade." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostrar status" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Mostra abaixo os totais atuais de estacionamento.>\n" +
                    "O status só é coletado enquanto o menu Opções está aberto; não há varredura de status em segundo plano durante o jogo." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modo por distrito>\n" +
                    "1. Escolha <Por distrito> acima.\n" +
                    "2. Crie ou selecione um distrito na cidade.\n" +
                    "3. Abra <Políticas> e ative **Proibição de estacionamento à beira da via [✓]**.\n" +
                    "4. A proibição e a tarifa de estacionamento podem ficar ativas juntas. A tarifa é cobrada dos carros que ainda estiverem lá ou conseguirem estacionar.\n" +
                    "As ruas fora dos distritos com proibição mantêm o estacionamento normal." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Proibido estacionar" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]",
                    "Ativa ou desativa o estacionamento junto à via em um lado da estrada. Para vários lados, arraste sobre eles antes de soltar o botão esquerdo do mouse." },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Proibição de estacionamento à beira da via" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Impede que carros e motocicletas estacionem à beira da via neste distrito. Veículos já estacionados saem quando seus donos os usam novamente." },
                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Adicionar" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Remover" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Estac. na rua" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Estac.> = carros que ainda estão em lados da via marcados como Proibido estacionar pelo Parking Control.\n" +
                    "<Faixas> = trechos de estacionamento junto à via onde esses carros estão. Um trecho pode conter vários carros.\n" +
                    "<Desat.> = trechos de faixa de estacionamento fechados para novos veículos. Uma via pode ter vários trechos.\n" +
                    "<DESLIGADO + Proibido estacionar manual> = DESLIGADO desativa as proibições da cidade inteira e por distrito, mas os lados da via marcados manualmente como Proibido estacionar continuam ativos. Esta linha mostra então apenas essas proibições manuais.\n" +
                    "<---------------------->\n" +
                    "Se <Por distrito> estiver selecionado, mostra:\n" +
                    "- Faixas ocupadas em distritos com proibição / faixas ocupadas na cidade inteira.\n" +
                    "- Faixas desativadas / faixas elegíveis da cidade.\n" +
                    "- Distritos ativados / distritos totais.\n" +
                    "<---------------------->\n" +
                    "Nota: depois de alterar ou reconstruir vias, a contagem de trechos desativados pode levar um pouco de tempo enquanto o CS2 reconstrói as faixas de estacionamento. Deixe a cidade rodar um pouco e reabra as Opções." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso das ruas" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Esta linha inclui a <cidade inteira>, não apenas os distritos.\n" +
                    "<Estacionados na rua> = porcentagem estacionada nas ruas em vez de estacionamentos públicos ou de edifícios.\n" +
                    "<Ativos> = veículos particulares dirigindo ou aguardando no trânsito.\n" +
                    "<Fórmula> = rua ÷ (rua + público ocupado + edifício ocupado).\n" +
                    "**Armazenamento de conexão externa (OC) e carros sem faixa de estacionamento atribuída são excluídos.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Vagas" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Mostra a ocupação do estacionamento em toda a cidade.\n" +
                    "<Público> usado = instalações contadas pela visualização de informações de Estacionamento do jogo base.\n" +
                    "<Edifício> usado = estacionamento incluído em residências, locais de trabalho e lojas.\n" +
                    "**% de uso maior = pode ser necessário ter mais vagas.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Onde estacionam" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Esta linha mostra dados da cidade inteira, não apenas dos distritos com proibição.\n" +
                    "<Rua> = estacionados em vias públicas.\n" +
                    "<Visíveis> = carros que você pode ver e clicar em estacionamentos abertos ou vagas externas de edifícios.\n" +
                    "<Dentro> = dentro de edifícios ou garagens.\n" +
                    "<OC> = armazenamento de conexão externa na divisa da cidade; alguns carros de famílias que chegam começam ali como área de espera.\n" +
                    "Carros sem faixa de estacionamento atribuída são omitidos aqui e aparecem apenas no relatório do log na aba Sobre." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Atualizado" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Horário da última atualização destes valores de status da cidade inteira." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nome do mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versão" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link do Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Abre a página do autor no Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Gravar relatório de estacionamento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Grava detalhes do estacionamento na rua e informações relacionadas em \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Se tiver curiosidade, grave um 2º relatório mais tarde na mesma cidade carregada.\n" +
                    "- Compara até 20 IDs de entidade de amostra de categorias diferentes.\n" +
                    "- Mostra se cada amostra ficou, começou a dirigir, estacionou em outro lugar ou desapareceu.\n" +
                    "- O mod Scene Explorer é necessário para acompanhar os números de ID das entidades na cidade." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Abrir log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Abre <Logs/ParkingControl.log> ou a pasta Logs se o arquivo ainda não existir." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Nenhuma cidade carregada." },
                { ParkingStatusLocale.kCollecting, "Coletando o status do estacionamento..." },
                { ParkingStatusLocale.kUnavailable, "O status do estacionamento não está disponível." },
                { ParkingStatusLocale.kCollectionFailed, "Não foi possível coletar o status do estacionamento; consulte ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} estac. ({1} faixas) | {2}/{3} desat.{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} estac. ({1}/{2} faixas) | {3}/{4} desat. | {5}/{6} distr.{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} rua | {1} visíveis | {2} dentro | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} público {1}/{2} | {3} edifício {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} na rua {1} | {2} ativos" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DESLIGADO" },
                { ParkingStatusLocale.kStatusCheck, "VERIF." },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

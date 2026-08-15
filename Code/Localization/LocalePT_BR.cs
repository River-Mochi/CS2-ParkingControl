// <copyright file="LocalePT_BR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Provides the Brazilian Portuguese text for Parking Control's Options UI.

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
        /// <param name="settings"> Options settings whose localization IDs are used.</param>
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
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Ações" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Sobre" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Estacionamento na rua" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Status dos veículos particulares" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Informações do mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Links" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Diagnóstico" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Sem estacionamento na rua" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Escolha onde bloquear novos estacionamentos na rua. Carros já estacionados saem naturalmente; estacionamentos e vagas de edifícios continuam disponíveis." },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Cidade inteira" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Por distrito" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "Desligado" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar instruções" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)), "Mostrar como usar o modo por distrito." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Mostrar status" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "Mostra os totais de estacionamento. Os dados só são coletados quando visíveis." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Modo por distrito>\n" +
                    "1. Escolha <Por distrito> acima.\n" +
                    "2. Crie ou selecione um distrito na cidade.\n" +
                    "3. Abra <Políticas> e ative <Sem estacionamento na rua>.\n" +
                    "As ruas fora dos distritos selecionados mantêm o estacionamento normal." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Policy.TITLE[{ParkingPolicySystem.PrefabName}]", "Sem estacionamento na rua" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.PrefabName}]",
                    "No modo Por distrito, impede novos estacionamentos na rua aqui. " +
                    "Os carros já estacionados saem naturalmente." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Estacionamento na rua" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Estacionados> = carros nas ruas cobertas pelo modo selecionado.\n" +
                    "<Faixas> = trechos de estacionamento junto à via onde esses carros estão. Uma faixa pode comportar vários carros.\n" +
                    "<Desativadas> = faixas de estacionamento na rua fechadas para novos veículos.\n" +
                    "<OK> = Sem estacionamento na rua está ativado e funcionando.\n" +
                    "<DESLIGADO> = Sem estacionamento na rua está desativado; os carros podem estacionar livremente em ruas comuns.\n" +
                    "<VERIFICAR> = as vias ainda podem estar sendo atualizadas. Aguarde um momento; gere um relatório no log se isso continuar.\n" +
                    "**Alguns carros podem permanecer depois de ativar a regra sem estacionamento na rua ou alterar vias. Eles saem naturalmente se o cidadão usar o carro.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Localização dos carros" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "<Rua> = estacionados em vias públicas.\n" +
                    "<Visíveis> = carros que você pode ver e clicar em estacionamentos abertos ou em vagas externas incluídas em edifícios.\n" +
                    "<Ocultos> = dentro de edifícios ou garagens.\n" +
                    "<OC> = armazenamento de conexão externa na divisa da cidade; alguns carros de famílias que chegam começam ali.\n" +
                    "**A área de espera não atribuída do jogo base aparece somente no log.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Estacionamento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "<Percentual> e <usado / total> mostram a ocupação do estacionamento.\n" +
                    "<Público> = instalações contadas pela visualização de informações de Estacionamento do jogo base.\n" +
                    "<Edifício> = estacionamento incluído em residências e locais de trabalho.\n" +
                    "**O estacionamento de edifícios inclui vagas externas visíveis e estacionamento interno.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Uso das ruas" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Estacionados na rua> = proporção de carros que usam estacionamento conhecido na rua, público ou de edifícios.\n" +
                    "<Em movimento> = veículos particulares dirigindo ou aguardando no trânsito.\n" +
                    "<Atualizado> = última atualização do status.\n" +
                    "**Conexões externas (OC) e a área de espera não atribuída são excluídas.**"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Nome do mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Versão" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Link do Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Abra a página do autor no Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Gravar relatório de estacionamento" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Grave detalhes de estacionamento na rua, oferta de vagas, propriedade e localização de veículos\n" +
                    "em <ParkingControl.log>\n" +
                    "Um segundo relatório na mesma cidade carregada acompanha os mesmos IDs de entidade dos carros estacionados na rua." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Abrir log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Abra <ParkingControl.log> ou a pasta Logs se o arquivo ainda não existir." },
                // Live status rows use these localized strings.
                { ParkingStatusLocale.kLoadCity, "Nenhuma cidade carregada." },
                { ParkingStatusLocale.kCollecting, "Coletando o status do estacionamento..." },
                { ParkingStatusLocale.kUnavailable, "O status do estacionamento não está disponível." },
                { ParkingStatusLocale.kCollectionFailed, "Não foi possível coletar o status do estacionamento; consulte ParkingControl.log." },
                { ParkingStatusLocale.kEnforcementFormat, "{0} estacionados ({1} faixas) | {2}/{3} desativadas | {4}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} rua | {1} visíveis | {2} ocultos | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0}  {1} / {2} público | {3}  {4} / {5} edifício" },
                { ParkingStatusLocale.kShareFormat, "{0} estacionados na rua | {1} em movimento | atualizado {2}" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "DESLIGADO" },
                { ParkingStatusLocale.kStatusCheck, "VERIFICAR" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

// <copyright file="LocaleTR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Turkish text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Turkish localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleTR : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleTR"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleTR(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Eylemler" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Hakkında" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Yol kenarı parkı" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Kişisel araç durumu" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Mod bilgileri" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Bağlantılar" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Tanılama" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Yol kenarı park yasağı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)), "Seçin:\n**Bölgeye göre**\n**Tüm Şehir**\nveya **KAPALI**\n- Uygun yol kenarı park şeritleri yeni araçların park etmesini önlemek için kapatılır.\n- Halihazırda park etmiş araçlar, sahipleri aracı bir sonraki kullandığında doğal olarak ayrılır.\n- Ücretli otoparklar ve binaların normal park alanları kullanılabilir durumda kalır.\n**Otoyollar ve asimetrik 3 şeritli yollar gibi bazı yollar zaten yol kenarı parkına izin vermez.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Tüm Şehir" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Bölgeye göre" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "KAPALI" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Talimatları göster" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)), "<Bölgeye göre> modunun nasıl kullanılacağını gösterir.\nKAPALI = şehir geneli ve bölge kısıtlamaları kapatılır; davranış büyük ölçüde oyunun varsayılanına döner.\n- Ancak tek tek yollar için <Park Yasak> düğmesini kullanmaya devam edebilirsiniz ve bu yasaklar uygulanır.\nTüm Şehir = uygun yol kenarı parkı şehir genelinde engellenir." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Durumu göster" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)), "<Geçerli park toplamlarını aşağıda gösterir.>\nDurum yalnızca Seçenekler menüsü açıkken toplanır; şehir oynanırken arka planda durum taraması yapılmaz." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)), "<Bölge modu>\n1. Yukarıdan <Bölgeye göre> seçin.\n2. Şehirde bir bölge oluşturun/seçin.\n3. <Politikalar> panelini açın ve **Yol Kenarı Park Yasağı [✓]** seçeneğini etkinleştirin.\n4. Park yasağı ile park ücretini aynı anda etkinleştirebilirsiniz. Ücret, hâlâ kalan veya içeri sızan araçlardan alınır.\nYasaklı bölgelerin dışındaki yollar normal yol kenarı parkını korur." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },
                { $"Assets.NAME[{NoParkingRoadToolSystem.kToolId}]", "Park Yasak" },
                { $"Assets.DESCRIPTION[{NoParkingRoadToolSystem.kToolId}]", "Bir yolun tek tarafındaki yol kenarı parkını açıp kapatır." },
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Yol Kenarı Park Yasağı" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]", "Bu bölgede otomobillerin ve motosikletlerin yol kenarına park etmesini engeller. Halihazırda park etmiş araçlar, sahipleri aracı bir sonraki kullandığında ayrılır." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Yol Kenarı Parkı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)), "<Park etmiş> = seçili mod tarafından yasaklanan yollarda hâlâ park etmiş araçlar.\n<Şeritler> = bu araçların bulunduğu yol kenarı park bölümleri. Bir şerit birden fazla araç tutabilir.\n<Devre dışı> = yeni park etmeye kapatılmış yol kenarı park şeritleri.\n<Bölgeye göre> şunları gösterir:\n- Yasaklı bölgelerde dolu şeritler / şehir genelinde dolu şeritler.\n- Devre dışı şeritler / şehirdeki uygun şeritler.\n- Etkin bölgeler / toplam bölgeler.\n<Yeni veya yeniden yapılan yollar>, şeritleri güncellenirken kısa süreliğine birkaç aracı kabul edebilir. Zaten park etmiş araçlar, vatandaşlar onları kullandığında doğal olarak ayrılır.\n<CHECK> = seçili bazı yollar henüz engellenmemiş. Şehri kısa süre çalıştırıp tekrar kontrol edin. <CHECK> devam ederse yardım isterken park günlüğü raporunu ekleyin." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Yol kullanımı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)), "Bu satır yalnızca bölgeleri değil <tüm şehri> kapsar.\n<Yolda park> = halka açık veya bina otoparkı yerine sokakta park eden araçların yüzdesi.\n<Aktif> = trafikte hareket eden veya bekleyen kişisel araçlar.\n<Formül> = yol ÷ (yol + dolu halka açık + dolu bina).\n**Şehir dışı bağlantı (OC) depolaması ve atanmış park şeridi olmayan araçlar hariç tutulur.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Park yerleri" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)), "Şehir genelindeki park doluluğunu gösterir.\n<Halka açık> kullanılan = oyunun standart Parking InfoView görünümünün saydığı tesisler.\n<Bina> kullanılan = evler, işyerleri ve mağazalarla birlikte gelen park yerleri.\n**Daha yüksek kullanım yüzdesi = daha fazla park yeri gerekebilir.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Araç Konumları" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)), "Bu satır yalnızca yasağın olduğu bölgeleri değil, şehir geneli verilerini gösterir.\n<Yol> = kamu yollarında park etmiş.\n<Görünür> = açık hava otoparklarında veya binalara ait dış park alanlarında görüp tıklayabildiğiniz araçlar.\n<İçeride> = binalarda veya garajlarda.\n<OC> = şehir sınırındaki dış bağlantı araç depolaması; bazı gelen hane araçları burada başlar (bekleme alanı).\nAtanmış park şeridi olmayan araçlar burada gösterilmez ve yalnızca günlük raporunda (Hakkında sekmesi) görünür." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Güncellendi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Bu şehir geneli durum değerlerinin en son yenilendiği zaman." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod adı" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Sürüm" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods bağlantısı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Yazarın Paradox Mods sayfasını açar." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Park raporu yaz" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)), "Yol kenarı parkı ve ilgili ayrıntıları\n<Logs/ParkingControl.log> dosyasına yazar.\nİsterseniz aynı yüklü şehirde daha sonra 2. bir rapor yazın.\n- Farklı kategorilerden en fazla 20 örnek Entity ID karşılaştırır.\n- Her örneğin kaldığını, sürmeye başladığını, başka yere park ettiğini veya kaybolduğunu gösterir.\n- Şehirde Entity ID numaralarını izlemek için Scene Explorer modu gerekir." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Günlüğü aç" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "<Logs/ParkingControl.log> dosyasını veya dosya henüz yoksa Logs klasörünü açar." },
                { ParkingStatusLocale.kLoadCity, "Henüz şehir yüklenmedi." },
                { ParkingStatusLocale.kCollecting, "Park durumu toplanıyor..." },
                { ParkingStatusLocale.kUnavailable, "Park durumu kullanılamıyor." },
                { ParkingStatusLocale.kCollectionFailed, "Park durumu toplanamadı; ParkingControl.log dosyasına bakın." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} park etmiş ({1} şerit) | {2}/{3} devre dışı{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} park etmiş ({1}/{2} şerit) | {3}/{4} devre dışı | {5}/{6} bölge{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} yol | {1} görünür | {2} içeride | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} halka açık {1}/{2} | {3} bina {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} yolda park {1} | {2} aktif" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "KAPALI" },
                { ParkingStatusLocale.kStatusCheck, "KONTROL" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

// <copyright file="LocaleTR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Turkish text for Parking Control's Options UI.
    using System.Collections.Generic;
    using Colossal;

namespace ParkingControl
{


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
                // Options tabs and groups.
                { m_Settings.GetSettingsLocaleID(), Mod.ModName },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Eylemler" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Hakkında" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Yol kenarı parkı" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Kişisel araç durumu" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Mod bilgileri" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Bağlantılar" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Tanılama" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Yol kenarı park yasağı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Seçin:\n" +
                    "<1. Bölgeye göre>\n" +
                    "<2. Tüm Şehir>\n" +
                    "<3. KAPALI>\n" +
                    "- Uygun şeritler yeni yol kenarı parkını önlemek için kapatılır.\n" +
                    "- Yasaktan sonra park etmiş araçlar zamanla taşınır; büyük alanlar daha uzun sürer.\n" +
                    "- Ücretli otoparklar ve binaların normal park alanları kullanılabilir kalır.\n" +
                    "**Otoyollar ve küçük çift yönlü ara sokaklar gibi bazı yollar zaten yol kenarı parkına izin vermez.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Tüm Şehir" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Bölgeye göre" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "KAPALI" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Talimatları göster" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "<Bölgeye göre> modunu açıklar.\n" +
                    "1.a. KAPALI = şehir geneli ve bölge yasakları kapalıdır; büyük ölçüde oyun varsayılanına döner.\n" +
                    "1.b. Yol Hizmetlerindeki tek yol <Park Yasak> düğmesi, yaya geçidi gibi çalışmaya devam eder.\n" +
                    "2. Tüm Şehir = şehirdeki tüm uygun halka açık yol kenarı parkını engeller."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Durumu göster" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Geçerli park toplamlarını aşağıda gösterir.>\n" +
                    "Durum yalnızca Seçenekler menüsü açıkken toplanır;\n" +
                    "şehir oynanırken arka planda tarama yapılmaz."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Bölge modu>\n" +
                    "1. Yukarıdan <Bölgeye göre> seçin.\n" +
                    "2. Bir bölge oluşturun/seçin.\n" +
                    "3. <Politikalar>ı açıp **Yol Kenarı Park Yasağı [✓]** seçeneğini etkinleştirin.\n" +
                    "4. Yasak ve park ücreti birlikte açık olabilir. Ücret, hâlâ kalan veya yine de park eden araçlardan alınır.\n" +
                    "Yasaklı bölgelerin dışındaki yollar normal yol kenarı parkını korur."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Park Yasak" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Yolun bir tarafındaki yol kenarı parkını açıp kapatır. Birden fazla taraf için sol fare düğmesini bırakmadan önce üzerlerinden sürükleyin." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Yol Kenarı Park Yasağı" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Bu bölgede otomobil ve motosikletlerin yol kenarına park etmesini engeller.\n" +
                    "- Park etmiş araçlar zamanla taşınır; büyük yasak alanları daha uzun sürer."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Ekle" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Kaldır" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Yol Kenarı Parkı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Yalnızca seçilen <Tüm Şehir> veya <Bölgeye göre> yasak kapsamını gösterir. Elle yasaklar ayrı listelenir.\n" +
                    "<KAPALI> = şehir/bölge yasakları kapalı; elle <Park Yasak> yapılan yollar etkin kalır.\n" +
                    "<Park etmiş> = seçilen kapsamdaki yollarda hâlâ park etmiş araçlar.\n" +
                    "<Devre dışı> = kapalı kaldırım kenarı şerit bölümleri / hedef bölümler.\n" +
                    "<Bölgeler> = park yasağı olan bölgeler / toplam bölgeler.\n" +
                    "<KONTROL> = bazı hedef bölümler henüz seçilen yasakla eşleşmiyor.\n" +
                    "<---------------------->\n" +
                    "**Yol değişikliği veya yeniden yapımdan sonra [KONTROL] görünürse şehri biraz çalıştırıp Seçenekler > Durum’u yeniden açın. Kalırsa Hakkında > Tanılama > Rapor yaz yolunu kullanın.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Elle Park Yasağı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Yalnızca elle <Park Yasak> aracıyla ayarlanan yolları gösterir.\n" +
                    "<Park etmiş> = elle yasaklanan bu yollarda hâlâ park etmiş araçlar.\n" +
                    "<Devre dışı> = kapalı kaldırım kenarı şerit bölümleri / elle hedeflenen bölümler.\n" +
                    "Elle yasaklar Tüm Şehir veya bölge yasaklarıyla çakışabilir; bu satırı <Yol Kenarı Parkı> toplamlarına eklemeyin.\n" +
                    "**Şehri biraz çalıştırdıktan sonra [KONTROL] görünürse Hakkında > Tanılama > Rapor yaz yolunu kullanın ve yardım isterken raporu gönderin.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Park kullanımı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Halka açık> = park tesislerindeki dolu yerler.\n" +
                    "CS2 Yol park bilgi paneliyle yaklaşık eşleşir.\n" +
                    "<Bina> = binalarda veya garajlarda park etmiş araçlar.\n" +
                    "<Yol> = yollarda park etmiş araçlar.\n" +
                    "<Toplam> = şehirde bilinen park etmiş araçlar (yol + halka açık + bina).\n" +
                    "**Dış bağlantılar ve bilinmeyen bekleme alanları toplama dahil değildir.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Park değerlendirmesi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Kesin kapasitesi olan park alanlarının ne kadarının <boş> olduğunu değerlendirir.\n" +
                    "<KÖTÜ> = %15’ten az boş.\n" +
                    "<OK> = %15 ile %30’dan az boş.\n" +
                    "<İYİ> = %30 veya daha fazla boş.\n" +
                    "<Halka açık>, CS2 Yol park bilgi panelinin saydığı tesisleri kullanır.\n" +
                    "<Bina>, bina ve garajlardaki kesin kapasiteli parkı kullanır."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Araç Konumları" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Bu satır yalnızca yasaklı bölgeleri değil, şehir geneli verileri gösterir.\n" +
                    "<Yol> = kamu yollarında park etmiş.\n" +
                    "<Görünür> = açık otoparklarda veya binaların dış park alanlarında görüp tıklayabildiğiniz araçlar.\n" +
                    "<İçeride> = binalarda veya garajlarda.\n" +
                    "<OC> = şehir sınırındaki dış bağlantı araç depolaması; bazı gelen hane araçları burada başlar (bekleme alanı).\n" +
                    "Atanmış park şeridi olmayan araçlar burada gösterilmez ve yalnızca günlük raporunda (Hakkında sekmesi) görünür."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Güncellendi" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Bu şehir geneli değerlerin en son yenilendiği zaman." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Mod adı" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Sürüm" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Paradox Mods bağlantısı" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Yazarın Paradox Mods sayfasını açar." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Rapor yaz" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Yol kenarı parkı ve ilgili ayrıntıları \n" +
                    "<Logs/ParkingControl.log> dosyasına yazar.\n" +
                    "İsterseniz aynı yüklü şehirde daha sonra 2. bir rapor yazın.\n" +
                    "- Farklı kategorilerden en fazla 20 örnek Entity ID karşılaştırır.\n" +
                    "- Her örneğin kaldığını, sürmeye başladığını, başka yere park ettiğini veya kaybolduğunu gösterir.\n" +
                    "- Şehirde Entity ID izlemek için Scene Explorer gerekir."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Günlüğü aç" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "<Logs/ParkingControl.log> dosyasını veya dosya yoksa Logs klasörünü açar." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Ayrıntılı günlük" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Otomatik DEBUG ayrıntıları.\n" +
                    "Normal oyun için değildir; hata ayıklamıyorsanız KAPALI tutun.\n" +
                    "Rapor yaz KAPALI iken de çalışır."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Henüz şehir yüklenmedi." },
                { ParkingStatusLocale.kCollecting, "Park durumu toplanıyor..." },
                { ParkingStatusLocale.kUnavailable, "Park durumu kullanılamıyor." },
                { ParkingStatusLocale.kCollectionFailed, "Park durumu toplanamadı; ParkingControl.log dosyasına bakın." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} park | {1}/{2} devre dışı{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} park | {1}/{2} şerit devre dışı{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} park | {1}/{2} devre dışı | {3}/{4} bölge{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} yol | {1} görünür | {2} içeride | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = {1} halka açık boş | {2} = {3} bina boş" },
                { ParkingStatusLocale.kShareFormat, "{0} halka açık | {1} bina | {2} yol | {3} toplam" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "KAPALI = şehir/bölge yasağı yok | elle yollar etkin" },
                { ParkingStatusLocale.kManualNone, "Ayarlı değil" },
                { ParkingStatusLocale.kStatusCheck, "KONTROL" },
                { ParkingStatusLocale.kRatingPoor, "KÖTÜ" },
                { ParkingStatusLocale.kRatingGood, "İYİ" },
                { ParkingStatusLocale.kRatingNA, "N/A" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

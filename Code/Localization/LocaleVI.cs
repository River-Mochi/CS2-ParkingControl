// <copyright file="LocaleVI.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Vietnamese text for Parking Control's Options UI.
    using System.Collections.Generic;
    using Colossal;

namespace ParkingControl
{


    /// <summary>
    /// Vietnamese localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleVI : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleVI"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleVI(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "Hành động" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "Giới thiệu" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "Đỗ xe ven đường" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "Trạng thái xe cá nhân" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "Thông tin mod" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "Liên kết" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "Chẩn đoán" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Cấm đỗ xe ven đường" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Chọn:\n" +
                    "<1. Theo khu vực>\n" +
                    "<2. Toàn thành phố>\n" +
                    "<3. TẮT>\n" +
                    "- Các làn đủ điều kiện bị khóa để ngăn xe mới đỗ ven đường.\n" +
                    "- Xe đang đỗ sẽ dần chuyển đi sau khi cấm; khu vực lớn cần lâu hơn.\n" +
                    "- Bãi đỗ có thu phí và chỗ đỗ thông thường của công trình vẫn dùng được.\n" +
                    "**Một số đường vốn đã không cho đỗ ven đường, như đường cao tốc và hẻm nhỏ hai chiều.**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Toàn thành phố" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Theo khu vực" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "TẮT" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Hiện hướng dẫn" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Hiện cách dùng chế độ <Theo khu vực>.\n" +
                    "1.a. TẮT = tắt lệnh cấm toàn thành phố và khu vực; phần lớn trở về mặc định của game.\n" +
                    "1.b. Nút <Cấm đỗ xe> cho từng đường trong Dịch vụ đường bộ vẫn hoạt động như thêm vạch qua đường.\n" +
                    "2. Toàn thành phố = chặn toàn bộ chỗ đỗ công cộng ven đường đủ điều kiện."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Hiện trạng thái" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Hiện tổng số đỗ xe hiện tại bên dưới.>\n" +
                    "Trạng thái chỉ được thu thập khi menu Tùy chọn đang mở;\n" +
                    "không quét nền trong lúc chơi."
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Chế độ theo khu vực>\n" +
                    "1. Chọn <Theo khu vực> ở trên.\n" +
                    "2. Tạo/chọn một khu vực.\n" +
                    "3. Mở <Chính sách> và bật **Cấm đỗ xe ven đường [✓]**.\n" +
                    "4. Có thể bật cùng lúc lệnh cấm và phí đỗ xe. Phí vẫn tính cho xe còn lại hoặc vẫn lọt vào đỗ.\n" +
                    "Các đường ngoài khu vực bị cấm vẫn giữ đỗ xe ven đường bình thường."
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Cấm đỗ xe" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Bật hoặc tắt đỗ xe ở một bên đường. Với nhiều bên, kéo qua chúng trước khi thả nút chuột trái." },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Cấm đỗ xe ven đường" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "Ngăn ô tô và xe máy đỗ ven đường trong khu vực này.\n" +
                    "- Xe đang đỗ sẽ dần chuyển đi; khu vực lớn cần lâu hơn."
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Thêm" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Gỡ" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Đỗ xe ven đường" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "Chỉ hiển thị phạm vi <Toàn thành phố> hoặc <Theo khu vực> đã chọn. Lệnh cấm thủ công hiển thị riêng.\n" +
                    "<TẮT> = tắt lệnh cấm thành phố/khu vực; các đường <Cấm đỗ xe> thủ công vẫn hoạt động.\n" +
                    "<Đang đỗ> = xe vẫn đỗ trên đường trong phạm vi đã chọn.\n" +
                    "<Đã khóa> = đoạn làn sát lề bị khóa / đoạn mục tiêu.\n" +
                    "<Khu vực> = khu vực có lệnh cấm / tổng khu vực.\n" +
                    "<KIỂM TRA> = một số đoạn mục tiêu chưa khớp với lệnh cấm đã chọn.\n" +
                    "<---------------------->\n" +
                    "**Nếu [KIỂM TRA] xuất hiện sau khi đổi hoặc xây lại đường, hãy chạy thành phố một lúc rồi mở lại Tùy chọn > Trạng thái. Nếu vẫn còn, dùng Giới thiệu > Chẩn đoán > Ghi báo cáo.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "Cấm đỗ thủ công" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "Chỉ hiển thị các đường được đặt bằng công cụ <Cấm đỗ xe> thủ công.\n" +
                    "<Đang đỗ> = xe vẫn đỗ trên các đường bị cấm thủ công đó.\n" +
                    "<Đã khóa> = đoạn làn sát lề bị khóa / đoạn mục tiêu thủ công.\n" +
                    "Lệnh cấm thủ công có thể trùng với Toàn thành phố hoặc khu vực; không cộng dòng này vào tổng <Đỗ xe ven đường>.\n" +
                    "**Nếu [KIỂM TRA] xuất hiện sau khi chạy thành phố một lúc, dùng Giới thiệu > Chẩn đoán > Ghi báo cáo và gửi khi cần trợ giúp.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Sử dụng chỗ đỗ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<Công cộng> = chỗ đã dùng trong các cơ sở đỗ xe.\n" +
                    "Gần khớp với bảng đỗ xe của Đường trong CS2.\n" +
                    "<Công trình> = xe đỗ trong công trình hoặc nhà để xe.\n" +
                    "<Đường> = xe đỗ trên đường.\n" +
                    "<Tổng> = tổng xe đỗ đã biết trong thành phố (đường + công cộng + công trình).\n" +
                    "**Không tính kết nối ngoài thành phố và khu chờ không rõ vị trí.**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Đánh giá chỗ đỗ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Đánh giá lượng chỗ đỗ có sức chứa chính xác còn <trống>.\n" +
                    "<KÉM> = trống dưới 15%.\n" +
                    "<OK> = trống từ 15% đến dưới 30%.\n" +
                    "<TỐT> = trống từ 30% trở lên.\n" +
                    "<Công cộng> dùng các cơ sở được bảng đỗ xe của Đường trong CS2 tính.\n" +
                    "<Công trình> dùng chỗ đỗ có sức chứa chính xác trong công trình và nhà để xe."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Vị trí xe" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Dòng này hiển thị dữ liệu toàn thành phố, không chỉ khu vực có lệnh cấm.\n" +
                    "<Đường> = đỗ trên đường công cộng.\n" +
                    "<Hiển thị> = xe có thể nhìn thấy và nhấp vào trong bãi ngoài trời hoặc chỗ đỗ ngoài trời của công trình.\n" +
                    "<Bên trong> = trong công trình hoặc nhà để xe.\n" +
                    "<OC> = kho xe tại kết nối ngoài thành phố ở rìa bản đồ; một số xe hộ gia đình đi vào bắt đầu tại đó (khu chờ).\n" +
                    "Xe chưa được gán làn đỗ không hiển thị ở đây và chỉ xuất hiện trong báo cáo nhật ký (tab Giới thiệu)."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Cập nhật" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Thời điểm các giá trị toàn thành phố này được làm mới gần nhất." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Tên mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Phiên bản" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Liên kết Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Mở trang của tác giả trên Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Ghi báo cáo" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Ghi chi tiết đỗ xe ven đường và dữ liệu liên quan vào \n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Nếu cần, hãy ghi báo cáo lần 2 sau đó trong cùng thành phố đang tải.\n" +
                    "- So sánh tối đa 20 Entity ID mẫu từ các nhóm khác nhau.\n" +
                    "- Cho biết mỗi mẫu vẫn ở đó, bắt đầu chạy, đỗ nơi khác hay biến mất.\n" +
                    "- Cần Scene Explorer để theo dõi Entity ID trong thành phố."
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Mở nhật ký" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Mở <Logs/ParkingControl.log>, hoặc thư mục Logs nếu tệp chưa tồn tại." },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Nhật ký chi tiết" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "Chi tiết DEBUG tự động.\n" +
                    "Không dành cho chơi bình thường; hãy TẮT nếu không gỡ lỗi.\n" +
                    "Ghi báo cáo vẫn hoạt động khi TẮT."
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Chưa tải thành phố." },
                { ParkingStatusLocale.kCollecting, "Đang thu thập trạng thái đỗ xe..." },
                { ParkingStatusLocale.kUnavailable, "Không có trạng thái đỗ xe." },
                { ParkingStatusLocale.kCollectionFailed, "Không thể thu thập trạng thái đỗ xe; xem ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} đang đỗ | khóa {1}/{2}{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} đang đỗ | khóa {1}/{2} làn{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} đang đỗ | khóa {1}/{2} | {3}/{4} khu vực{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} đường | {1} hiển thị | {2} bên trong | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = công cộng trống {1} | {2} = công trình trống {3}" },
                { ParkingStatusLocale.kShareFormat, "{0} công cộng | {1} công trình | {2} đường | {3} tổng" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "TẮT = không cấm thành phố/khu vực | đường thủ công vẫn hoạt động" },
                { ParkingStatusLocale.kManualNone, "Chưa đặt" },
                { ParkingStatusLocale.kStatusCheck, "KIỂM TRA" },
                { ParkingStatusLocale.kRatingPoor, "KÉM" },
                { ParkingStatusLocale.kRatingGood, "TỐT" },
                { ParkingStatusLocale.kRatingNA, "N/A" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

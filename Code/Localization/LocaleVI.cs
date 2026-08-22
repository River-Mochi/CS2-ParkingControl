// <copyright file="LocaleVI.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Vietnamese text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDiagnosticsGroup), "Chẩn đoán" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "Cấm đỗ xe ven đường" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "Chọn:\n" +
                    "**Theo khu vực**\n" +
                    "**Toàn thành phố**\n" +
                    "hoặc **TẮT**.\n" +
                    "- Các làn đỗ xe ven đường đủ điều kiện sẽ bị khóa để ngăn xe mới đỗ vào.\n" +
                    "- Xe đang đỗ sẽ rời đi tự nhiên khi chủ xe sử dụng xe lần tiếp theo.\n" +
                    "- Bãi đỗ xe có thu phí và chỗ đỗ xe thông thường của công trình vẫn sử dụng được.\n" +
                    "**Một số loại đường vốn đã không cho đỗ xe ven đường, như đường cao tốc và đường 3 làn bất đối xứng.**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "Toàn thành phố" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "Theo khu vực" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "TẮT" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "Hiện hướng dẫn" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "Hiện cách sử dụng chế độ <Theo khu vực>.\n" +
                    "1.a. TẮT = tắt các hạn chế toàn thành phố và theo khu vực, phần lớn trở về hành vi mặc định của game.\n" +
                    "1.b. Nút <Cấm đỗ xe> cho từng đoạn đường trong bảng Dịch vụ đường bộ vẫn hoạt động, giống như thêm vạch qua đường.\n" +
                    "2. Toàn thành phố = chặn toàn bộ chỗ đỗ xe công cộng ven đường đủ điều kiện trong thành phố." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "Hiện trạng thái" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<Hiện tổng số đỗ xe hiện tại bên dưới.>\n" +
                    "Trạng thái chỉ được thu thập khi menu Tùy chọn đang mở; không có quét trạng thái nền trong lúc chơi thành phố." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<Chế độ theo khu vực>\n" +
                    "1. Chọn <Theo khu vực> ở trên.\n" +
                    "2. Tạo/chọn một khu vực trong thành phố.\n" +
                    "3. Mở bảng <Chính sách> và bật **Cấm đỗ xe ven đường [✓]**.\n" +
                    "4. Có thể bật đồng thời lệnh cấm và phí đỗ xe. Phí vẫn được tính cho những xe còn lại hoặc lọt vào.\n" +
                    "Các đường ngoài khu vực bị cấm vẫn giữ đỗ xe ven đường bình thường." },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "Cấm đỗ xe" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "Bật hoặc tắt đỗ xe ven đường ở một bên đường. Với nhiều bên, hãy kéo qua chúng trước khi thả nút chuột trái." },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "Cấm đỗ xe ven đường" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]", "Ngăn ô tô và xe máy đỗ ven đường trong khu vực này. Các xe đang đỗ sẽ rời đi khi chủ xe sử dụng xe lần tiếp theo." },
                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "Thêm" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "Gỡ" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "Đỗ xe ven đường" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<Đang đỗ> = xe vẫn đang đỗ trên các đường bị cấm theo chế độ đã chọn.\n" +
                    "<Làn> = các đoạn đỗ xe ven đường đang chứa những xe đó. Một làn có thể chứa nhiều xe.\n" +
                    "<Đã khóa> = các làn đỗ xe ven đường đã đóng với xe mới.\n" +
                    "<Theo khu vực> hiển thị:\n" +
                    "- Làn có xe đỗ trong khu vực bị cấm / làn có xe đỗ toàn thành phố.\n" +
                    "- Làn đã khóa / làn đủ điều kiện toàn thành phố.\n" +
                    "- Khu vực đã bật / tổng số khu vực.\n" +
                    "<Đường mới hoặc vừa xây lại> có thể tạm thời nhận một vài xe khi các làn đang cập nhật. Xe đã đỗ sẽ rời đi tự nhiên khi người dân sử dụng xe.\n" +
                    "<KIỂM TRA> = một số đường đã chọn chưa bị chặn. Hãy chạy thành phố một lúc rồi kiểm tra lại. Nếu <KIỂM TRA> vẫn còn, hãy kèm báo cáo nhật ký đỗ xe khi yêu cầu trợ giúp." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "Sử dụng đường" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "Dòng này gồm dữ liệu của <toàn thành phố>, không chỉ các khu vực.\n" +
                    "<Đỗ trên đường> = tỷ lệ xe đỗ trên đường thay vì bãi đỗ công cộng hoặc chỗ đỗ của công trình.\n" +
                    "<Đang hoạt động> = xe cá nhân đang chạy hoặc chờ trong giao thông.\n" +
                    "<Công thức> = đường ÷ (đường + công cộng đang dùng + công trình đang dùng).\n" +
                    "**Không tính kho xe ở kết nối ngoài thành phố (OC) và xe chưa được gán làn đỗ.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "Chỗ đỗ xe" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "Hiển thị mức sử dụng chỗ đỗ xe toàn thành phố.\n" +
                    "<Công cộng> đã dùng = các cơ sở được Parking InfoView mặc định của game tính.\n" +
                    "<Công trình> đã dùng = chỗ đỗ đi kèm nhà ở, nơi làm việc và cửa hàng.\n" +
                    "**Tỷ lệ sử dụng càng cao = có thể cần thêm chỗ đỗ xe.**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "Vị trí xe" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "Dòng này hiển thị dữ liệu toàn thành phố, không chỉ các khu vực có lệnh cấm.\n" +
                    "<Đường> = đỗ trên đường công cộng.\n" +
                    "<Hiển thị> = xe bạn có thể nhìn thấy và nhấp vào trong bãi ngoài trời hoặc chỗ đỗ ngoài trời đi kèm công trình.\n" +
                    "<Bên trong> = trong công trình hoặc nhà để xe.\n" +
                    "<OC> = kho xe tại kết nối ngoài thành phố ở rìa bản đồ; một số xe hộ gia đình đi vào bắt đầu tại đó (khu chờ).\n" +
                    "Xe chưa được gán làn đỗ không hiển thị ở đây và chỉ xuất hiện trong báo cáo nhật ký (tab Giới thiệu)." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "Cập nhật" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "Thời điểm các giá trị trạng thái toàn thành phố này được làm mới gần nhất." },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "Tên mod" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "Phiên bản" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "Liên kết Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "Mở trang của tác giả trên Paradox Mods." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "Ghi báo cáo đỗ xe" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "Ghi chi tiết đỗ xe ven đường và dữ liệu liên quan vào\n" +
                    "<Logs/ParkingControl.log>.\n" +
                    "Nếu muốn theo dõi, hãy ghi báo cáo lần thứ 2 sau đó trong cùng thành phố đang tải.\n" +
                    "- So sánh tối đa 20 Entity ID mẫu từ các nhóm khác nhau.\n" +
                    "- Cho biết mỗi mẫu vẫn ở đó, bắt đầu chạy, đỗ nơi khác hay biến mất.\n" +
                    "- Cần mod Scene Explorer để theo dõi các số Entity ID trong thành phố." },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "Mở nhật ký" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "Mở <Logs/ParkingControl.log>, hoặc thư mục Logs nếu tệp chưa tồn tại." },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "Chưa tải thành phố." },
                { ParkingStatusLocale.kCollecting, "Đang thu thập trạng thái đỗ xe..." },
                { ParkingStatusLocale.kUnavailable, "Không có trạng thái đỗ xe." },
                { ParkingStatusLocale.kCollectionFailed, "Không thể thu thập trạng thái đỗ xe; xem ParkingControl.log." },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} xe đang đỗ ({1} làn) | khóa {2}/{3}{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} xe đang đỗ ({1}/{2} làn) | khóa {3}/{4} | {5}/{6} khu vực{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} đường | {1} hiển thị | {2} bên trong | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} công cộng {1}/{2} | {3} công trình {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} đỗ trên đường {1} | {2} hoạt động" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "TẮT" },
                { ParkingStatusLocale.kStatusCheck, "KIỂM TRA" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

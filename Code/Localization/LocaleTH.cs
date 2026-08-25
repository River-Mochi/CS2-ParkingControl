// <copyright file="LocaleTH.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Thai text for Parking Control's Options UI.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Thai localization entries for <see cref="PCSettings"/>.
    /// </summary>
    public sealed class LocaleTH : IDictionarySource
    {
        private readonly PCSettings m_Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleTH"/> class.
        /// </summary>
        /// <param name="settings">Options settings whose localization IDs are used.</param>
        public LocaleTH(PCSettings settings)
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
                { m_Settings.GetOptionTabLocaleID(PCSettings.kActionsTab), "การทำงาน" },
                { m_Settings.GetOptionTabLocaleID(PCSettings.kAboutTab), "เกี่ยวกับ" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStreetParkingGroup), "การจอดรถริมถนน" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kStatusGroup), "สถานะรถส่วนบุคคล" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutInfoGroup), "ข้อมูลม็อด" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutLinksGroup), "ลิงก์" },
                { m_Settings.GetOptionGroupLocaleID(PCSettings.kAboutDebugGroup), "การวินิจฉัย" },

                // Street-parking controls.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.Scope)), "ห้ามจอดรถริมถนน" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.Scope)),
                    "เลือก:\n" +
                    "**ตามเขต**\n" +
                    "**ทั้งเมือง**\n" +
                    "หรือ **ปิด**\n" +
                    "- ช่องจอดริมถนนที่เข้าเกณฑ์จะถูกปิดเพื่อไม่ให้รถเข้าจอดใหม่\n" +
                    "- รถที่จอดอยู่แล้วจะออกไปตามปกติเมื่อเจ้าของนำรถไปใช้ครั้งถัดไป\n" +
                    "- ลานจอดรถแบบเก็บค่าธรรมเนียมและที่จอดรถของอาคารยังใช้งานได้ตามปกติ\n" +
                    "**ถนนบางประเภทไม่อนุญาตให้จอดริมถนนอยู่แล้ว เช่น ทางหลวงและตรอกขนาดเล็กแบบสองทาง**" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "ทั้งเมือง" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "ตามเขต" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "ปิด" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "แสดงคำแนะนำ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "แสดงวิธีใช้โหมด <ตามเขต>\n" +
                    "1.a. ปิด = ปิดข้อจำกัดทั้งเมืองและตามเขต โดยส่วนใหญ่จะกลับสู่ค่าปกติของเกม\n" +
                    "1.b. ปุ่ม <ห้ามจอดรถ> สำหรับถนนแต่ละช่วงในแผงบริการถนนยังใช้ได้เหมือนเดิม คล้ายกับการเพิ่มทางม้าลาย\n" +
                    "2. ทั้งเมือง = ปิดการจอดรถสาธารณะริมถนนที่เข้าเกณฑ์ทั่วทั้งเมือง" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "แสดงสถานะ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<แสดงยอดรวมการจอดรถปัจจุบันด้านล่าง>\n" +
                    "สถานะจะถูกรวบรวมเฉพาะขณะที่เปิดเมนูตัวเลือกเท่านั้น ไม่มีการสแกนสถานะเบื้องหลังระหว่างเล่นเมือง" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<โหมดตามเขต>\n" +
                    "1. เลือก <ตามเขต> ด้านบน\n" +
                    "2. สร้าง/เลือกเขตในเมือง\n" +
                    "3. เปิดแผง <นโยบาย> แล้วเปิด **ห้ามจอดรถริมถนน [✓]**\n" +
                    "4. สามารถเปิดทั้งการห้ามจอดและค่าจอดรถพร้อมกันได้ ค่าจอดรถจะคิดกับรถที่ยังคงอยู่หรือเล็ดลอดเข้ามา\n" +
                    "ถนนนอกเขตที่ห้ามจอดยังคงมีการจอดริมถนนตามปกติ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "ห้ามจอดรถ" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "เปิดหรือปิดการจอดรถริมถนนบนด้านหนึ่งของถนน หากต้องการหลายด้าน ให้ลากผ่านด้านเหล่านั้นก่อนปล่อยปุ่มเมาส์ซ้าย" },
                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "ห้ามจอดรถริมถนน" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]", "ป้องกันรถยนต์และรถจักรยานยนต์ไม่ให้จอดริมถนนในเขตนี้ รถที่จอดอยู่แล้วจะออกไปเมื่อเจ้าของนำรถไปใช้ครั้งถัดไป" },
                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "เพิ่ม" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "นำออก" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "การจอดรถริมถนน" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "<จอดอยู่> = รถที่ยังจอดบนด้านถนนที่ Parking Control ตั้งเป็นห้ามจอด\n" +
                    "<ช่อง> = ส่วนของที่จอดริมถนนที่มีรถเหล่านั้นอยู่ หนึ่งส่วนอาจรองรับรถได้หลายคัน\n" +
                    "<ปิดใช้งาน> = ส่วนของช่องจอดที่ปิดไม่ให้รถใหม่เข้าจอด ถนนหนึ่งเส้นอาจมีหลายส่วน\n" +
                    "<ปิด + ห้ามจอดแบบกำหนดเอง> = ปิดจะยกเลิกการห้ามทั้งเมืองและตามเขต แต่ด้านถนนที่ตั้งห้ามจอดด้วยตนเองยังคงทำงาน แถวนี้จะแสดงเฉพาะการห้ามแบบกำหนดเองเหล่านั้น\n" +
                    "<---------------------->\n" +
                    "หากเลือก <ตามเขต> จะแสดง:\n" +
                    "- ช่องที่มีรถจอดในเขตห้ามจอด / ช่องที่มีรถจอดทั่วเมือง\n" +
                    "- ช่องที่ปิดใช้งาน / ช่องที่เข้าเกณฑ์ทั่วเมือง\n" +
                    "- เขตที่เปิดใช้งาน / เขตทั้งหมด\n" +
                    "<---------------------->\n" +
                    "หมายเหตุ: หลังจากเปลี่ยนหรือสร้างถนนใหม่ จำนวนส่วนที่ปิดใช้งานอาจต้องใช้เวลาสักเล็กน้อยระหว่างที่ CS2 สร้างช่องจอดใหม่ ให้รันเมืองสักครู่แล้วเปิดตัวเลือกอีกครั้ง" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "การใช้ถนน" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "แถวนี้รวมข้อมูลของ <ทั้งเมือง> ไม่ใช่เฉพาะเขต\n" +
                    "<จอดบนถนน> = เปอร์เซ็นต์ของรถที่จอดบนถนนแทนลานจอดสาธารณะหรือที่จอดของอาคาร\n" +
                    "<กำลังใช้งาน> = รถส่วนบุคคลที่กำลังขับหรือรอในการจราจร\n" +
                    "<สูตร> = ถนน ÷ (ถนน + สาธารณะที่ใช้อยู่ + อาคารที่ใช้อยู่)\n" +
                    "**ไม่รวมที่เก็บรถที่จุดเชื่อมต่อนอกเมือง (OC) และรถที่ยังไม่มีช่องจอดที่กำหนด**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "ที่จอดรถ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "แสดงการใช้ที่จอดรถทั่วเมือง\n" +
                    "<สาธารณะ> ใช้แล้ว = สถานที่ที่นับโดย Parking InfoView ของเกม\n" +
                    "<อาคาร> ใช้แล้ว = ที่จอดรถที่รวมอยู่กับบ้าน ที่ทำงาน และร้านค้า\n" +
                    "**เปอร์เซ็นต์การใช้งานที่สูงขึ้น = อาจต้องมีที่จอดรถเพิ่ม**" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "ตำแหน่งรถ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "แถวนี้แสดงข้อมูลทั่วเมือง ไม่ใช่เฉพาะเขตที่มีการห้ามจอด\n" +
                    "<ถนน> = จอดบนถนนสาธารณะ\n" +
                    "<มองเห็น> = รถที่มองเห็นและคลิกได้ในลานกลางแจ้งหรือที่จอดกลางแจ้งที่รวมกับอาคาร\n" +
                    "<ภายใน> = อยู่ในอาคารหรือโรงจอดรถ\n" +
                    "<OC> = ที่เก็บรถตรงจุดเชื่อมต่อนอกเมืองบริเวณขอบเมือง รถของครัวเรือนที่เข้ามาบางคันเริ่มต้นที่นั่น (พื้นที่พักรถ)\n" +
                    "รถที่ยังไม่มีช่องจอดที่กำหนดจะไม่แสดงที่นี่ และจะแสดงเฉพาะในรายงานล็อก (แท็บเกี่ยวกับ)" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "อัปเดต" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "เวลาที่ค่ารวมสถานะทั่วเมืองเหล่านี้ถูกรีเฟรชล่าสุด" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "ชื่อม็อด" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "เวอร์ชัน" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "ลิงก์ Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "เปิดหน้าของผู้สร้างบน Paradox Mods" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "เขียนรายงานการจอดรถ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "เขียนรายละเอียดการจอดริมถนนและข้อมูลที่เกี่ยวข้องไปยัง\n" +
                    "<Logs/ParkingControl.log>\n" +
                    "หากต้องการตรวจสอบเพิ่มเติม ให้เขียนรายงานครั้งที่ 2 ภายหลังในเมืองเดิมที่โหลดอยู่\n" +
                    "- เปรียบเทียบตัวอย่าง Entity ID สูงสุด 20 รายการจากหมวดต่าง ๆ\n" +
                    "- แสดงว่าแต่ละตัวอย่างยังอยู่ เริ่มขับ ไปจอดที่อื่น หรือหายไป\n" +
                    "- ต้องใช้ม็อด Scene Explorer เพื่อติดตามหมายเลข Entity ID ในเมือง" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "เปิดล็อก" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "เปิด <Logs/ParkingControl.log> หรือโฟลเดอร์ Logs หากยังไม่มีไฟล์" },
                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "ยังไม่ได้โหลดเมือง" },
                { ParkingStatusLocale.kCollecting, "กำลังรวบรวมสถานะการจอดรถ..." },
                { ParkingStatusLocale.kUnavailable, "ไม่สามารถดูสถานะการจอดรถได้" },
                { ParkingStatusLocale.kCollectionFailed, "ไม่สามารถรวบรวมสถานะการจอดรถได้ โปรดดู ParkingControl.log" },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} คันจอดอยู่ ({1} ช่อง) | ปิด {2}/{3}{4}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} คันจอดอยู่ ({1}/{2} ช่อง) | ปิด {3}/{4} | {5}/{6} เขต{7}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} ถนน | {1} มองเห็น | {2} ภายใน | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} สาธารณะ {1}/{2} | {3} อาคาร {4}/{5}" },
                { ParkingStatusLocale.kShareFormat, "{0} จอดบนถนน {1} | {2} ใช้งาน" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "ปิด" },
                { ParkingStatusLocale.kStatusCheck, "ตรวจสอบ" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

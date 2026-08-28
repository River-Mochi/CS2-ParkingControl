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
                    "<1. ตามเขต>\n" +
                    "<2. ทั้งเมือง>\n" +
                    "<3. ปิด>\n" +
                    "- ช่องจอดริมถนนที่เข้าเกณฑ์จะถูกปิดเพื่อไม่ให้รถใหม่เข้าจอด\n" +
                    "- รถที่จอดอยู่จะทยอยย้ายหลังเปิดการห้าม พื้นที่ใหญ่ใช้เวลานานกว่า\n" +
                    "- ลานจอดแบบเก็บค่าธรรมเนียมและที่จอดรถของอาคารยังใช้งานได้\n" +
                    "**ถนนบางประเภทไม่อนุญาตให้จอดริมถนนอยู่แล้ว เช่น ทางหลวงและตรอกเล็กแบบสองทาง**"
                },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.WholeCity), "ทั้งเมือง" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.ByDistrict), "ตามเขต" },
                { m_Settings.GetEnumValueLocaleID(PCSettings.ParkingScope.Off), "ปิด" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowInstructions)), "แสดงคำแนะนำ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowInstructions)),
                    "แสดงวิธีใช้โหมด <ตามเขต>\n" +
                    "1.a. ปิด = ปิดข้อจำกัดทั้งเมืองและตามเขต โดยส่วนใหญ่กลับสู่ค่าปกติของเกม\n" +
                    "1.b. ปุ่ม <ห้ามจอดรถ> สำหรับถนนแต่ละเส้นในบริการถนนยังใช้ได้เหมือนการเพิ่มทางม้าลาย\n" +
                    "2. ทั้งเมือง = ปิดการจอดรถสาธารณะริมถนนที่เข้าเกณฑ์ทั่วเมือง"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShowStatus)), "แสดงสถานะ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShowStatus)),
                    "<แสดงยอดรวมการจอดรถปัจจุบันด้านล่าง>\n" +
                    "สถานะจะถูกรวบรวมเฉพาะขณะที่เปิดเมนูตัวเลือก\n" +
                    "ไม่มีการสแกนเบื้องหลังระหว่างเล่นเมือง"
                },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.DistrictInstructions)),
                    "<โหมดตามเขต>\n" +
                    "1. เลือก <ตามเขต> ด้านบน\n" +
                    "2. สร้าง/เลือกเขตในเมือง\n" +
                    "3. เปิด <นโยบาย> แล้วเปิด **ห้ามจอดรถริมถนน [✓]**\n" +
                    "4. เปิดทั้งการห้ามจอดและค่าจอดพร้อมกันได้ ค่าจอดจะคิดกับรถที่ยังอยู่หรือยังเข้ามาจอดได้\n" +
                    "ถนนนอกเขตที่ห้ามจอดยังจอดริมถนนได้ตามปกติ"
                },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.DistrictInstructions)), string.Empty },

                // In-city Roads Services tool.
                { $"Assets.NAME[{ManualNoParkingToolSystem.kToolId}]", "ห้ามจอดรถ" },
                { $"Assets.DESCRIPTION[{ManualNoParkingToolSystem.kToolId}]", "เปิดหรือปิดการจอดริมถนนด้านหนึ่ง หากต้องการหลายด้าน ให้ลากผ่านก่อนปล่อยปุ่มเมาส์ซ้าย" },

                // In-city district policy.
                { $"Policy.TITLE[{ParkingPolicySystem.kPrefabName}]", "ห้ามจอดรถริมถนน" },
                { $"Policy.DESCRIPTION[{ParkingPolicySystem.kPrefabName}]",
                    "ห้ามรถยนต์และรถจักรยานยนต์จอดริมถนนในเขตนี้\n" +
                    "- รถที่จอดอยู่จะทยอยย้าย พื้นที่ใหญ่ใช้เวลานานกว่า"
                },

                // Native mouse action hints for the No Parking road tool.
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kUpgradeHintId}]", "เพิ่ม" },
                { $"Common.ACTION[{ManualNoParkingTooltipSystem.kDowngradeHintId}]", "นำออก" },

                // Live Options status rows, in display order.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.EnforcementStatus)), "การจอดรถริมถนน" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.EnforcementStatus)),
                    "แสดงเฉพาะขอบเขต <ทั้งเมือง> หรือ <ตามเขต> ที่เลือก การห้ามแบบกำหนดเองแสดงแยกกัน\n" +
                    "<ปิด> = ปิดการห้ามทั้งเมือง/ตามเขต แต่ถนนที่ตั้ง <ห้ามจอดรถ> เองยังทำงาน\n" +
                    "<จอดอยู่> = รถที่ยังจอดบนถนนในขอบเขตที่เลือก\n" +
                    "<ปิดใช้งาน> = ส่วนช่องริมขอบทางที่ปิด / ส่วนเป้าหมาย\n" +
                    "<เขต> = เขตที่เปิดการห้าม / เขตทั้งหมด\n" +
                    "<ตรวจสอบ> = บางส่วนเป้าหมายยังไม่ตรงกับการห้ามที่เลือก\n" +
                    "<---------------------->\n" +
                    "**หาก [ตรวจสอบ] แสดงหลังเปลี่ยนหรือสร้างถนนใหม่ ให้รันเมืองสักพักแล้วเปิด ตัวเลือก > สถานะ ใหม่ หากยังอยู่ ให้ใช้ เกี่ยวกับ > การวินิจฉัย > เขียนรายงาน**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ManualStatus)), "ห้ามจอดแบบกำหนดเอง" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ManualStatus)),
                    "แสดงเฉพาะถนนที่ตั้งด้วยเครื่องมือ <ห้ามจอดรถ> แบบกำหนดเอง\n" +
                    "<จอดอยู่> = รถที่ยังจอดบนถนนที่ห้ามเอง\n" +
                    "<ปิดใช้งาน> = ส่วนช่องริมขอบทางที่ปิด / ส่วนที่เลือกเอง\n" +
                    "การห้ามแบบกำหนดเองอาจซ้อนกับทั้งเมืองหรือเขต อย่านำแถวนี้ไปรวมกับ <การจอดรถริมถนน>\n" +
                    "**หาก [ตรวจสอบ] ยังแสดงหลังรันเมืองสักพัก ให้ใช้ เกี่ยวกับ > การวินิจฉัย > เขียนรายงาน และส่งเมื่อขอความช่วยเหลือ**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ShareStatus)), "การใช้ที่จอดรถ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ShareStatus)),
                    "<สาธารณะ> = ช่องจอดที่มีรถในลานจอดสาธารณะ\n" +
                    "ใกล้เคียงกับแผงข้อมูลที่จอดรถของถนนใน CS2\n" +
                    "<อาคาร> = รถที่จอดในอาคารหรือโรงจอด\n" +
                    "<ถนน> = รถที่จอดบนถนน\n" +
                    "<รวม> = รถที่ทราบว่าจอดอยู่ในเมือง (ถนน + สาธารณะ + อาคาร)\n" +
                    "**ไม่รวมจุดเชื่อมต่อนอกเมืองและพื้นที่พักรถที่ไม่ทราบตำแหน่ง**"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.SupplyStatus)), "ระดับที่จอดรถ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.SupplyStatus)),
                    "ประเมินที่จอดรถที่มีความจุแน่นอนว่ายัง <ว่าง> เท่าใด\n" +
                    "<แย่> = ว่างน้อยกว่า 15%\n" +
                    "<OK> = ว่าง 15% ถึงน้อยกว่า 30%\n" +
                    "<ดี> = ว่าง 30% ขึ้นไป\n" +
                    "<สาธารณะ> ใช้ลานจอดที่แผงข้อมูลที่จอดรถของถนนใน CS2 นับ\n" +
                    "<อาคาร> ใช้ที่จอดความจุแน่นอนในอาคารและโรงจอด"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VehicleStatus)), "ตำแหน่งรถ" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VehicleStatus)),
                    "แถวนี้แสดงข้อมูลทั่วเมือง ไม่ใช่เฉพาะเขตที่ห้ามจอด\n" +
                    "<ถนน> = จอดบนถนนสาธารณะ\n" +
                    "<มองเห็น> = รถที่มองเห็นและคลิกได้ในลานกลางแจ้งหรือที่จอดกลางแจ้งของอาคาร\n" +
                    "<ภายใน> = อยู่ในอาคารหรือโรงจอดรถ\n" +
                    "<OC> = ที่เก็บรถตรงจุดเชื่อมต่อนอกเมืองบริเวณขอบเมือง รถของครัวเรือนที่เข้ามาบางคันเริ่มที่นั่น (พื้นที่พักรถ)\n" +
                    "รถที่ไม่มีช่องจอดที่กำหนดจะไม่แสดงที่นี่ และมีเฉพาะในรายงาน log (แท็บเกี่ยวกับ)"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.UpdatedStatus)), "อัปเดต" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.UpdatedStatus)), "เวลาที่ค่าทั่วเมืองเหล่านี้ถูกรีเฟรชล่าสุด" },

                // About tab.
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.NameText)), "ชื่อม็อด" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VersionText)), "เวอร์ชัน" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenParadox)), "ลิงก์ Paradox Mods" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenParadox)), "เปิดหน้าผู้สร้างบน Paradox Mods" },
                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.ReportToLog)), "เขียนรายงาน" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.ReportToLog)),
                    "เขียนรายละเอียดการจอดริมถนนและข้อมูลที่เกี่ยวข้องไปที่ \n" +
                    "<Logs/ParkingControl.log>\n" +
                    "หากต้องการ ให้เขียนรายงานครั้งที่ 2 ภายหลังในเมืองเดิมที่โหลดอยู่\n" +
                    "- เปรียบเทียบ Entity ID ตัวอย่างสูงสุด 20 รายการจากหลายกลุ่ม\n" +
                    "- แสดงว่าแต่ละตัวอย่างยังอยู่ เริ่มขับ ไปจอดที่อื่น หรือหายไป\n" +
                    "- ต้องใช้ Scene Explorer เพื่อติดตาม Entity ID ในเมือง"
                },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.OpenLog)), "เปิด log" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.OpenLog)), "เปิด <Logs/ParkingControl.log> หรือโฟลเดอร์ Logs หากยังไม่มีไฟล์" },

                { m_Settings.GetOptionLabelLocaleID(nameof(PCSettings.VerboseLog)), "Log แบบละเอียด" },
                { m_Settings.GetOptionDescLocaleID(nameof(PCSettings.VerboseLog)),
                    "รายละเอียด DEBUG อัตโนมัติ\n" +
                    "ไม่เหมาะกับการเล่นปกติ ให้ปิดถ้าไม่ได้ดีบัก\n" +
                    "เขียนรายงานยังใช้ได้เมื่อปิด"
                },

                // Dynamic values used by the live status rows.
                { ParkingStatusLocale.kLoadCity, "ยังไม่ได้โหลดเมือง" },
                { ParkingStatusLocale.kCollecting, "กำลังรวบรวมสถานะการจอดรถ..." },
                { ParkingStatusLocale.kUnavailable, "ไม่มีสถานะการจอดรถ" },
                { ParkingStatusLocale.kCollectionFailed, "รวบรวมสถานะการจอดรถไม่ได้ โปรดดู ParkingControl.log" },
                { ParkingStatusLocale.kCompactEnforcementFormat, "{0} จอดอยู่ | ปิด {1}/{2}{3}" },
                { ParkingStatusLocale.kManualEnforcementFormat, "{0} จอดอยู่ | ปิด {1}/{2} ช่อง{3}" },
                { ParkingStatusLocale.kDistrictEnforcementFormat, "{0} จอดอยู่ | ปิด {1}/{2} | {3}/{4} เขต{5}" },
                { ParkingStatusLocale.kVehicleFormat, "{0} ถนน | {1} มองเห็น | {2} ภายใน | {3} OC" },
                { ParkingStatusLocale.kSupplyFormat, "{0} = สาธารณะว่าง {1} | {2} = อาคารว่าง {3}" },
                { ParkingStatusLocale.kShareFormat, "{0} สาธารณะ | {1} อาคาร | {2} ถนน | {3} รวม" },
                { ParkingStatusLocale.kStatusOk, "OK" },
                { ParkingStatusLocale.kStatusOff, "ปิด = ไม่ห้ามทั้งเมือง/เขต | ถนนที่ตั้งเองยังทำงาน" },
                { ParkingStatusLocale.kManualNone, "ยังไม่ได้ตั้ง" },
                { ParkingStatusLocale.kStatusCheck, "ตรวจสอบ" },
                { ParkingStatusLocale.kRatingPoor, "แย่" },
                { ParkingStatusLocale.kRatingGood, "ดี" },
                { ParkingStatusLocale.kRatingNA, "N/A" },
            };
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}

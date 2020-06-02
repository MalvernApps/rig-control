using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serialtest
{
    public enum EnumRadio
    {
        [Description("Null Radio")]
        NULL_RADIO = 0x00,
        [Description("IC-271")]
        IC_271 = 0x20,
        [Description("IC-275")]
        IC_275 = 0x10,
        [Description("IC-375")]
        IC_375 = 0x12,
        [Description("IC-471")]
        IC_471 = 0x22,
        [Description("IC-475")]
        IC_475 = 0x14,
        [Description("IC-575")]
        IC_575 = 0x16,
        [Description("IC-7000")]
        IC_7000 = 0x70,
        [Description("IC-703")]
        IC_703 = 0x68,
        [Description("IC-706")]
        IC_706 = 0x48,
        [Description("IC-706 MkII")]
        IC_706MkII = 0x4E,
        [Description("IC-706 MkII G")]
        IC_706MkIIG = 0x58,
        [Description("IC-707")]
        IC_707 = 0x3e,
        [Description("IC-7100")]
        IC_7100 = 0x88,
        [Description("IC-718")]
        IC_718 = 0x5E,
        [Description("IC-7200")]
        IC_7200 = 0x76,
        [Description("IC-725")]
        IC_725 = 0x28,
        [Description("IC-726")]
        IC_726 = 0x30,
        [Description("IC-728")]
        IC_728 = 0x38,
        [Description("IC-729")]
        IC_729 = 0x3A,
        [Description("IC-735")]
        IC_735 = 0x04,
        [Description("IC-736")]
        IC_736 = 0x40,
        [Description("IC-737")]
        IC_737 = 0x3C,
        [Description("IC-738")]
        IC_738 = 0x44,
        [Description("IC-7400 (IC-746Pro)")]
        IC_7400 = 0x66,
        [Description("IC-746")]
        IC_746 = 0x56,
        [Description("IC-751 A")]
        IC_751A = 0x1C,
        [Description("IC-756")]
        IC_756 = 0x50,
        [Description("IC-756 Pro")]
        IC_756Pro = 0x5C,
        [Description("IC-756 Pro II")]
        IC_756ProII = 0x64,
        [Description("IC-756 Pro III")]
        IC_756ProIII = 0x6e,
        [Description("IC-761")]
        IC_761 = 0x1E,
        [Description("IC-765")]
        IC_765 = 0x2C,
        [Description("IC-775")]
        IC_775 = 0x46,
        [Description("IC-7700")]
        IC_7700 = 0x74,
        [Description("IC-78")]
        IC_78 = 0x62,
        [Description("IC-7800")]
        IC_7800 = 0x6A,
        [Description("IC-781")]
        IC_781 = 0x26,
        [Description("IC-820")]
        IC_820 = 0x42,
        [Description("IC-821")]
        IC_821 = 0x4C,
        [Description("IC-910")]
        IC_910 = 0x60,
        [Description("IC-970")]
        IC_970 = 0x2E,
        [Description("IC-1271")]
        IC_1271 = 0x24,
        [Description("IC-1275")]
        IC_1275 = 0x18,
        [Description("IC-R10")]
        IC_R10 = 0x52,
        [Description("IC-R20")]
        IC_R20 = 0x6C,
        [Description("IC-R71")]
        IC_R71 = 0x1A,
        [Description("IC-R72")]
        IC_R72 = 0x32,
        [Description("IC-R75")]
        IC_R75 = 0x5A,
        [Description("IC-R7000")]
        IC_R7000 = 0x08,
        [Description("IC-R7100")]
        IC_R7100 = 0x34,
        [Description("IC-R8500")]
        IC_R8500 = 0x4A,
        [Description("IC-R9000")]
        IC_R9000 = 0x2A,
        [Description("IC-R9500")]
        IC_R9500 = 0x72,
        [Description("IC-RX7")]
        IC_RX7 = 0x78,
        [Description("Controller/PC")]
        PC_CONTROL = 0xE0
    }
}

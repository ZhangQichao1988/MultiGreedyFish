/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 * 
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */
using System;

namespace Jackpot
{
    partial class CountryCode
    {
#region Note

        // NOTE: Unity monoではまともに値がとれないので、.NET環境で生成
        // using System;
        // using System.Collections.Generic;
        // using System.Globalization;
        // using System.Linq;
        //
        // var regions = new Dictionary<int, RegionInfo>();
        // var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        // foreach (var culture in cultures)
        // {
        //     try
        //     {
        //         var region = new RegionInfo(culture.LCID);
        //         regions[culture.LCID] = region;
        //     }
        //     catch (CultureNotFoundException)
        //     {
        //         var consoleColor = Console.ForegroundColor;
        //         Console.ForegroundColor = ConsoleColor.Red;
        //         Console.WriteLine("Culture not found: " + culture.Name);
        //         Console.ForegroundColor = consoleColor;
        //     }
        // }
        // var builder = new StringBuilder();
        // var englishNameSpecialChars = new char[] { '.', ' ', '(', ')', '[', ']'};
        // foreach (var entry in regions.OrderBy(entry => entry.Key))
        // {
        //     int num = -1;
        //     var name = !int.TryParse(entry.Value.TwoLetterISORegionName, out num)
        //         ? entry.Value.TwoLetterISORegionName + entry.Key.ToString()
        //         : new String(entry.Value.EnglishName.Where(c =>!englishNameSpecialChars.Contains(c)).ToArray()) + entry.Key.ToString();
        //     builder.AppendFormat(
        //         "        public static readonly CountryCode {0} = new CountryCode({1}, \"{0}\", \"{2}\", \"{3}\");\n",
        //         name,
        //         entry.Key,
        //         entry.Value.TwoLetterISORegionName,
        //         entry.Value.ThreeLetterISORegionName
        //     );
        // }
        // Console.WriteLine(builder.ToString());

#endregion

#region Generated

        public static readonly CountryCode SA1025 = new CountryCode(1025, "SA1025", "SA", "SAU");
        public static readonly CountryCode BG1026 = new CountryCode(1026, "BG1026", "BG", "BGR");
        public static readonly CountryCode ES1027 = new CountryCode(1027, "ES1027", "ES", "ESP");
        public static readonly CountryCode TW1028 = new CountryCode(1028, "TW1028", "TW", "TWN");
        public static readonly CountryCode CZ1029 = new CountryCode(1029, "CZ1029", "CZ", "CZE");
        public static readonly CountryCode DK1030 = new CountryCode(1030, "DK1030", "DK", "DNK");
        public static readonly CountryCode DE1031 = new CountryCode(1031, "DE1031", "DE", "DEU");
        public static readonly CountryCode GR1032 = new CountryCode(1032, "GR1032", "GR", "GRC");
        public static readonly CountryCode US1033 = new CountryCode(1033, "US1033", "US", "USA");
        public static readonly CountryCode FI1035 = new CountryCode(1035, "FI1035", "FI", "FIN");
        public static readonly CountryCode FR1036 = new CountryCode(1036, "FR1036", "FR", "FRA");
        public static readonly CountryCode IL1037 = new CountryCode(1037, "IL1037", "IL", "ISR");
        public static readonly CountryCode HU1038 = new CountryCode(1038, "HU1038", "HU", "HUN");
        public static readonly CountryCode IS1039 = new CountryCode(1039, "IS1039", "IS", "ISL");
        public static readonly CountryCode IT1040 = new CountryCode(1040, "IT1040", "IT", "ITA");
        public static readonly CountryCode JP1041 = new CountryCode(1041, "JP1041", "JP", "JPN");
        public static readonly CountryCode KR1042 = new CountryCode(1042, "KR1042", "KR", "KOR");
        public static readonly CountryCode NL1043 = new CountryCode(1043, "NL1043", "NL", "NLD");
        public static readonly CountryCode NO1044 = new CountryCode(1044, "NO1044", "NO", "NOR");
        public static readonly CountryCode PL1045 = new CountryCode(1045, "PL1045", "PL", "POL");
        public static readonly CountryCode BR1046 = new CountryCode(1046, "BR1046", "BR", "BRA");
        public static readonly CountryCode CH1047 = new CountryCode(1047, "CH1047", "CH", "CHE");
        public static readonly CountryCode RO1048 = new CountryCode(1048, "RO1048", "RO", "ROU");
        public static readonly CountryCode RU1049 = new CountryCode(1049, "RU1049", "RU", "RUS");
        public static readonly CountryCode HR1050 = new CountryCode(1050, "HR1050", "HR", "HRV");
        public static readonly CountryCode SK1051 = new CountryCode(1051, "SK1051", "SK", "SVK");
        public static readonly CountryCode AL1052 = new CountryCode(1052, "AL1052", "AL", "ALB");
        public static readonly CountryCode SE1053 = new CountryCode(1053, "SE1053", "SE", "SWE");
        public static readonly CountryCode TH1054 = new CountryCode(1054, "TH1054", "TH", "THA");
        public static readonly CountryCode TR1055 = new CountryCode(1055, "TR1055", "TR", "TUR");
        public static readonly CountryCode PK1056 = new CountryCode(1056, "PK1056", "PK", "PAK");
        public static readonly CountryCode ID1057 = new CountryCode(1057, "ID1057", "ID", "IDN");
        public static readonly CountryCode UA1058 = new CountryCode(1058, "UA1058", "UA", "UKR");
        public static readonly CountryCode BY1059 = new CountryCode(1059, "BY1059", "BY", "BLR");
        public static readonly CountryCode SI1060 = new CountryCode(1060, "SI1060", "SI", "SVN");
        public static readonly CountryCode EE1061 = new CountryCode(1061, "EE1061", "EE", "EST");
        public static readonly CountryCode LV1062 = new CountryCode(1062, "LV1062", "LV", "LVA");
        public static readonly CountryCode LT1063 = new CountryCode(1063, "LT1063", "LT", "LTU");
        public static readonly CountryCode TJ1064 = new CountryCode(1064, "TJ1064", "TJ", "TAJ");
        public static readonly CountryCode IR1065 = new CountryCode(1065, "IR1065", "IR", "IRN");
        public static readonly CountryCode VN1066 = new CountryCode(1066, "VN1066", "VN", "VNM");
        public static readonly CountryCode AM1067 = new CountryCode(1067, "AM1067", "AM", "ARM");
        public static readonly CountryCode AZ1068 = new CountryCode(1068, "AZ1068", "AZ", "AZE");
        public static readonly CountryCode ES1069 = new CountryCode(1069, "ES1069", "ES", "ESP");
        public static readonly CountryCode DE1070 = new CountryCode(1070, "DE1070", "DE", "DEU");
        public static readonly CountryCode MK1071 = new CountryCode(1071, "MK1071", "MK", "MKD");
        public static readonly CountryCode ZA1072 = new CountryCode(1072, "ZA1072", "ZA", "ZAF");
        public static readonly CountryCode ZA1073 = new CountryCode(1073, "ZA1073", "ZA", "ZAF");
        public static readonly CountryCode ZA1074 = new CountryCode(1074, "ZA1074", "ZA", "ZAF");
        public static readonly CountryCode ZA1076 = new CountryCode(1076, "ZA1076", "ZA", "ZAF");
        public static readonly CountryCode ZA1077 = new CountryCode(1077, "ZA1077", "ZA", "ZAF");
        public static readonly CountryCode ZA1078 = new CountryCode(1078, "ZA1078", "ZA", "ZAF");
        public static readonly CountryCode GE1079 = new CountryCode(1079, "GE1079", "GE", "GEO");
        public static readonly CountryCode FO1080 = new CountryCode(1080, "FO1080", "FO", "FRO");
        public static readonly CountryCode IN1081 = new CountryCode(1081, "IN1081", "IN", "IND");
        public static readonly CountryCode MT1082 = new CountryCode(1082, "MT1082", "MT", "MLT");
        public static readonly CountryCode NO1083 = new CountryCode(1083, "NO1083", "NO", "NOR");
        public static readonly CountryCode MY1086 = new CountryCode(1086, "MY1086", "MY", "MYS");
        public static readonly CountryCode KZ1087 = new CountryCode(1087, "KZ1087", "KZ", "KAZ");
        public static readonly CountryCode KG1088 = new CountryCode(1088, "KG1088", "KG", "KGZ");
        public static readonly CountryCode KE1089 = new CountryCode(1089, "KE1089", "KE", "KEN");
        public static readonly CountryCode TM1090 = new CountryCode(1090, "TM1090", "TM", "TKM");
        public static readonly CountryCode UZ1091 = new CountryCode(1091, "UZ1091", "UZ", "UZB");
        public static readonly CountryCode RU1092 = new CountryCode(1092, "RU1092", "RU", "RUS");
        public static readonly CountryCode IN1093 = new CountryCode(1093, "IN1093", "IN", "IND");
        public static readonly CountryCode IN1094 = new CountryCode(1094, "IN1094", "IN", "IND");
        public static readonly CountryCode IN1095 = new CountryCode(1095, "IN1095", "IN", "IND");
        public static readonly CountryCode IN1096 = new CountryCode(1096, "IN1096", "IN", "IND");
        public static readonly CountryCode IN1097 = new CountryCode(1097, "IN1097", "IN", "IND");
        public static readonly CountryCode IN1098 = new CountryCode(1098, "IN1098", "IN", "IND");
        public static readonly CountryCode IN1099 = new CountryCode(1099, "IN1099", "IN", "IND");
        public static readonly CountryCode IN1100 = new CountryCode(1100, "IN1100", "IN", "IND");
        public static readonly CountryCode IN1101 = new CountryCode(1101, "IN1101", "IN", "IND");
        public static readonly CountryCode IN1102 = new CountryCode(1102, "IN1102", "IN", "IND");
        public static readonly CountryCode IN1103 = new CountryCode(1103, "IN1103", "IN", "IND");
        public static readonly CountryCode MN1104 = new CountryCode(1104, "MN1104", "MN", "MNG");
        public static readonly CountryCode CN1105 = new CountryCode(1105, "CN1105", "CN", "CHN");
        public static readonly CountryCode GB1106 = new CountryCode(1106, "GB1106", "GB", "GBR");
        public static readonly CountryCode KH1107 = new CountryCode(1107, "KH1107", "KH", "KHM");
        public static readonly CountryCode LA1108 = new CountryCode(1108, "LA1108", "LA", "LAO");
        public static readonly CountryCode MM1109 = new CountryCode(1109, "MM1109", "MM", "MMR");
        public static readonly CountryCode ES1110 = new CountryCode(1110, "ES1110", "ES", "ESP");
        public static readonly CountryCode IN1111 = new CountryCode(1111, "IN1111", "IN", "IND");
        public static readonly CountryCode SY1114 = new CountryCode(1114, "SY1114", "SY", "SYR");
        public static readonly CountryCode LK1115 = new CountryCode(1115, "LK1115", "LK", "LKA");
        public static readonly CountryCode US1116 = new CountryCode(1116, "US1116", "US", "USA");
        public static readonly CountryCode CA1117 = new CountryCode(1117, "CA1117", "CA", "CAN");
        public static readonly CountryCode ET1118 = new CountryCode(1118, "ET1118", "ET", "ETH");
        public static readonly CountryCode NP1121 = new CountryCode(1121, "NP1121", "NP", "NPL");
        public static readonly CountryCode NL1122 = new CountryCode(1122, "NL1122", "NL", "NLD");
        public static readonly CountryCode AF1123 = new CountryCode(1123, "AF1123", "AF", "AFG");
        public static readonly CountryCode PH1124 = new CountryCode(1124, "PH1124", "PH", "PHL");
        public static readonly CountryCode MV1125 = new CountryCode(1125, "MV1125", "MV", "MDV");
        public static readonly CountryCode NG1128 = new CountryCode(1128, "NG1128", "NG", "NGA");
        public static readonly CountryCode NG1130 = new CountryCode(1130, "NG1130", "NG", "NGA");
        public static readonly CountryCode BO1131 = new CountryCode(1131, "BO1131", "BO", "BOL");
        public static readonly CountryCode ZA1132 = new CountryCode(1132, "ZA1132", "ZA", "ZAF");
        public static readonly CountryCode RU1133 = new CountryCode(1133, "RU1133", "RU", "RUS");
        public static readonly CountryCode LU1134 = new CountryCode(1134, "LU1134", "LU", "LUX");
        public static readonly CountryCode GL1135 = new CountryCode(1135, "GL1135", "GL", "GRL");
        public static readonly CountryCode NG1136 = new CountryCode(1136, "NG1136", "NG", "NGA");
        public static readonly CountryCode ET1138 = new CountryCode(1138, "ET1138", "ET", "ETH");
        public static readonly CountryCode ET1139 = new CountryCode(1139, "ET1139", "ET", "ETH");
        public static readonly CountryCode PY1140 = new CountryCode(1140, "PY1140", "PY", "PRY");
        public static readonly CountryCode US1141 = new CountryCode(1141, "US1141", "US", "USA");
        public static readonly CountryCode SO1143 = new CountryCode(1143, "SO1143", "SO", "SOM");
        public static readonly CountryCode CN1144 = new CountryCode(1144, "CN1144", "CN", "CHN");
        public static readonly CountryCode CL1146 = new CountryCode(1146, "CL1146", "CL", "CHL");
        public static readonly CountryCode CA1148 = new CountryCode(1148, "CA1148", "CA", "CAN");
        public static readonly CountryCode FR1150 = new CountryCode(1150, "FR1150", "FR", "FRA");
        public static readonly CountryCode CN1152 = new CountryCode(1152, "CN1152", "CN", "CHN");
        public static readonly CountryCode NZ1153 = new CountryCode(1153, "NZ1153", "NZ", "NZL");
        public static readonly CountryCode FR1154 = new CountryCode(1154, "FR1154", "FR", "FRA");
        public static readonly CountryCode FR1155 = new CountryCode(1155, "FR1155", "FR", "FRA");
        public static readonly CountryCode FR1156 = new CountryCode(1156, "FR1156", "FR", "FRA");
        public static readonly CountryCode RU1157 = new CountryCode(1157, "RU1157", "RU", "RUS");
        public static readonly CountryCode GT1158 = new CountryCode(1158, "GT1158", "GT", "GTM");
        public static readonly CountryCode RW1159 = new CountryCode(1159, "RW1159", "RW", "RWA");
        public static readonly CountryCode SN1160 = new CountryCode(1160, "SN1160", "SN", "SEN");
        public static readonly CountryCode AF1164 = new CountryCode(1164, "AF1164", "AF", "AFG");
        public static readonly CountryCode GB1169 = new CountryCode(1169, "GB1169", "GB", "GBR");
        public static readonly CountryCode IQ1170 = new CountryCode(1170, "IQ1170", "IQ", "IRQ");
        public static readonly CountryCode IQ2049 = new CountryCode(2049, "IQ2049", "IQ", "IRQ");
        public static readonly CountryCode ES2051 = new CountryCode(2051, "ES2051", "ES", "ESP");
        public static readonly CountryCode CN2052 = new CountryCode(2052, "CN2052", "CN", "CHN");
        public static readonly CountryCode CH2055 = new CountryCode(2055, "CH2055", "CH", "CHE");
        public static readonly CountryCode GB2057 = new CountryCode(2057, "GB2057", "GB", "GBR");
        public static readonly CountryCode MX2058 = new CountryCode(2058, "MX2058", "MX", "MEX");
        public static readonly CountryCode BE2060 = new CountryCode(2060, "BE2060", "BE", "BEL");
        public static readonly CountryCode CH2064 = new CountryCode(2064, "CH2064", "CH", "CHE");
        public static readonly CountryCode BE2067 = new CountryCode(2067, "BE2067", "BE", "BEL");
        public static readonly CountryCode NO2068 = new CountryCode(2068, "NO2068", "NO", "NOR");
        public static readonly CountryCode PT2070 = new CountryCode(2070, "PT2070", "PT", "PRT");
        public static readonly CountryCode MD2072 = new CountryCode(2072, "MD2072", "MD", "MDA");
        public static readonly CountryCode CS2074 = new CountryCode(2074, "CS2074", "CS", "SCG");
        public static readonly CountryCode FI2077 = new CountryCode(2077, "FI2077", "FI", "FIN");
        public static readonly CountryCode IN2080 = new CountryCode(2080, "IN2080", "IN", "IND");
        public static readonly CountryCode AZ2092 = new CountryCode(2092, "AZ2092", "AZ", "AZE");
        public static readonly CountryCode DE2094 = new CountryCode(2094, "DE2094", "DE", "DEU");
        public static readonly CountryCode BW2098 = new CountryCode(2098, "BW2098", "BW", "BWA");
        public static readonly CountryCode SE2107 = new CountryCode(2107, "SE2107", "SE", "SWE");
        public static readonly CountryCode IE2108 = new CountryCode(2108, "IE2108", "IE", "IRL");
        public static readonly CountryCode BN2110 = new CountryCode(2110, "BN2110", "BN", "BRN");
        public static readonly CountryCode UZ2115 = new CountryCode(2115, "UZ2115", "UZ", "UZB");
        public static readonly CountryCode BD2117 = new CountryCode(2117, "BD2117", "BD", "BGD");
        public static readonly CountryCode PK2118 = new CountryCode(2118, "PK2118", "PK", "PAK");
        public static readonly CountryCode LK2121 = new CountryCode(2121, "LK2121", "LK", "LKA");
        public static readonly CountryCode CN2128 = new CountryCode(2128, "CN2128", "CN", "CHN");
        public static readonly CountryCode PK2137 = new CountryCode(2137, "PK2137", "PK", "PAK");
        public static readonly CountryCode CA2141 = new CountryCode(2141, "CA2141", "CA", "CAN");
        public static readonly CountryCode DZ2143 = new CountryCode(2143, "DZ2143", "DZ", "DZA");
        public static readonly CountryCode IN2145 = new CountryCode(2145, "IN2145", "IN", "IND");
        public static readonly CountryCode SN2151 = new CountryCode(2151, "SN2151", "SN", "SEN");
        public static readonly CountryCode EC2155 = new CountryCode(2155, "EC2155", "EC", "ECU");
        public static readonly CountryCode ER2163 = new CountryCode(2163, "ER2163", "ER", "ERI");
        public static readonly CountryCode EG3073 = new CountryCode(3073, "EG3073", "EG", "EGY");
        public static readonly CountryCode HK3076 = new CountryCode(3076, "HK3076", "HK", "HKG");
        public static readonly CountryCode AT3079 = new CountryCode(3079, "AT3079", "AT", "AUT");
        public static readonly CountryCode AU3081 = new CountryCode(3081, "AU3081", "AU", "AUS");
        public static readonly CountryCode ES3082 = new CountryCode(3082, "ES3082", "ES", "ESP");
        public static readonly CountryCode CA3084 = new CountryCode(3084, "CA3084", "CA", "CAN");
        public static readonly CountryCode CS3098 = new CountryCode(3098, "CS3098", "CS", "SCG");
        public static readonly CountryCode FI3131 = new CountryCode(3131, "FI3131", "FI", "FIN");
        public static readonly CountryCode MN3152 = new CountryCode(3152, "MN3152", "MN", "MNG");
        public static readonly CountryCode PE3179 = new CountryCode(3179, "PE3179", "PE", "PER");
        public static readonly CountryCode ID4096 = new CountryCode(4096, "ID4096", "ID", "IDN");
        public static readonly CountryCode LY4097 = new CountryCode(4097, "LY4097", "LY", "LBY");
        public static readonly CountryCode SG4100 = new CountryCode(4100, "SG4100", "SG", "SGP");
        public static readonly CountryCode LU4103 = new CountryCode(4103, "LU4103", "LU", "LUX");
        public static readonly CountryCode CA4105 = new CountryCode(4105, "CA4105", "CA", "CAN");
        public static readonly CountryCode GT4106 = new CountryCode(4106, "GT4106", "GT", "GTM");
        public static readonly CountryCode CH4108 = new CountryCode(4108, "CH4108", "CH", "CHE");
        public static readonly CountryCode BA4122 = new CountryCode(4122, "BA4122", "BA", "BIH");
        public static readonly CountryCode NO4155 = new CountryCode(4155, "NO4155", "NO", "NOR");
        public static readonly CountryCode MA4191 = new CountryCode(4191, "MA4191", "MA", "MAR");
        public static readonly CountryCode DZ5121 = new CountryCode(5121, "DZ5121", "DZ", "DZA");
        public static readonly CountryCode MO5124 = new CountryCode(5124, "MO5124", "MO", "MAC");
        public static readonly CountryCode LI5127 = new CountryCode(5127, "LI5127", "LI", "LIE");
        public static readonly CountryCode NZ5129 = new CountryCode(5129, "NZ5129", "NZ", "NZL");
        public static readonly CountryCode CR5130 = new CountryCode(5130, "CR5130", "CR", "CRI");
        public static readonly CountryCode LU5132 = new CountryCode(5132, "LU5132", "LU", "LUX");
        public static readonly CountryCode BA5146 = new CountryCode(5146, "BA5146", "BA", "BIH");
        public static readonly CountryCode SE5179 = new CountryCode(5179, "SE5179", "SE", "SWE");
        public static readonly CountryCode MA6145 = new CountryCode(6145, "MA6145", "MA", "MAR");
        public static readonly CountryCode IE6153 = new CountryCode(6153, "IE6153", "IE", "IRL");
        public static readonly CountryCode PA6154 = new CountryCode(6154, "PA6154", "PA", "PAN");
        public static readonly CountryCode MC6156 = new CountryCode(6156, "MC6156", "MC", "MCO");
        public static readonly CountryCode BA6170 = new CountryCode(6170, "BA6170", "BA", "BIH");
        public static readonly CountryCode NO6203 = new CountryCode(6203, "NO6203", "NO", "NOR");
        public static readonly CountryCode TN7169 = new CountryCode(7169, "TN7169", "TN", "TUN");
        public static readonly CountryCode ZA7177 = new CountryCode(7177, "ZA7177", "ZA", "ZAF");
        public static readonly CountryCode DO7178 = new CountryCode(7178, "DO7178", "DO", "DOM");
        public static readonly CountryCode BA7194 = new CountryCode(7194, "BA7194", "BA", "BIH");
        public static readonly CountryCode SE7227 = new CountryCode(7227, "SE7227", "SE", "SWE");
        public static readonly CountryCode OM8193 = new CountryCode(8193, "OM8193", "OM", "OMN");
        public static readonly CountryCode JM8201 = new CountryCode(8201, "JM8201", "JM", "JAM");
        public static readonly CountryCode VE8202 = new CountryCode(8202, "VE8202", "VE", "VEN");
        public static readonly CountryCode RE8204 = new CountryCode(8204, "RE8204", "RE", "REU");
        public static readonly CountryCode BA8218 = new CountryCode(8218, "BA8218", "BA", "BIH");
        public static readonly CountryCode FI8251 = new CountryCode(8251, "FI8251", "FI", "FIN");
        public static readonly CountryCode YE9217 = new CountryCode(9217, "YE9217", "YE", "YEM");
        public static readonly CountryCode Caribbean9225 = new CountryCode(9225, "Caribbean9225", "029", "029");
        public static readonly CountryCode CO9226 = new CountryCode(9226, "CO9226", "CO", "COL");
        public static readonly CountryCode CD9228 = new CountryCode(9228, "CD9228", "CD", "COD");
        public static readonly CountryCode RS9242 = new CountryCode(9242, "RS9242", "RS", "SRB");
        public static readonly CountryCode FI9275 = new CountryCode(9275, "FI9275", "FI", "FIN");
        public static readonly CountryCode SY10241 = new CountryCode(10241, "SY10241", "SY", "SYR");
        public static readonly CountryCode BZ10249 = new CountryCode(10249, "BZ10249", "BZ", "BLZ");
        public static readonly CountryCode PE10250 = new CountryCode(10250, "PE10250", "PE", "PER");
        public static readonly CountryCode SN10252 = new CountryCode(10252, "SN10252", "SN", "SEN");
        public static readonly CountryCode RS10266 = new CountryCode(10266, "RS10266", "RS", "SRB");
        public static readonly CountryCode JO11265 = new CountryCode(11265, "JO11265", "JO", "JOR");
        public static readonly CountryCode TT11273 = new CountryCode(11273, "TT11273", "TT", "TTO");
        public static readonly CountryCode AR11274 = new CountryCode(11274, "AR11274", "AR", "ARG");
        public static readonly CountryCode CM11276 = new CountryCode(11276, "CM11276", "CM", "CMR");
        public static readonly CountryCode ME11290 = new CountryCode(11290, "ME11290", "ME", "MNE");
        public static readonly CountryCode LB12289 = new CountryCode(12289, "LB12289", "LB", "LBN");
        public static readonly CountryCode ZW12297 = new CountryCode(12297, "ZW12297", "ZW", "ZWE");
        public static readonly CountryCode EC12298 = new CountryCode(12298, "EC12298", "EC", "ECU");
        public static readonly CountryCode CI12300 = new CountryCode(12300, "CI12300", "CI", "CIV");
        public static readonly CountryCode ME12314 = new CountryCode(12314, "ME12314", "ME", "MNE");
        public static readonly CountryCode KW13313 = new CountryCode(13313, "KW13313", "KW", "KWT");
        public static readonly CountryCode PH13321 = new CountryCode(13321, "PH13321", "PH", "PHL");
        public static readonly CountryCode CL13322 = new CountryCode(13322, "CL13322", "CL", "CHL");
        public static readonly CountryCode ML13324 = new CountryCode(13324, "ML13324", "ML", "MLI");
        public static readonly CountryCode AE14337 = new CountryCode(14337, "AE14337", "AE", "ARE");
        public static readonly CountryCode UY14346 = new CountryCode(14346, "UY14346", "UY", "URY");
        public static readonly CountryCode MA14348 = new CountryCode(14348, "MA14348", "MA", "MAR");
        public static readonly CountryCode BH15361 = new CountryCode(15361, "BH15361", "BH", "BHR");
        public static readonly CountryCode HK15369 = new CountryCode(15369, "HK15369", "HK", "HKG");
        public static readonly CountryCode PY15370 = new CountryCode(15370, "PY15370", "PY", "PRY");
        public static readonly CountryCode HT15372 = new CountryCode(15372, "HT15372", "HT", "HTI");
        public static readonly CountryCode QA16385 = new CountryCode(16385, "QA16385", "QA", "QAT");
        public static readonly CountryCode IN16393 = new CountryCode(16393, "IN16393", "IN", "IND");
        public static readonly CountryCode BO16394 = new CountryCode(16394, "BO16394", "BO", "BOL");
        public static readonly CountryCode MY17417 = new CountryCode(17417, "MY17417", "MY", "MYS");
        public static readonly CountryCode SV17418 = new CountryCode(17418, "SV17418", "SV", "SLV");
        public static readonly CountryCode SG18441 = new CountryCode(18441, "SG18441", "SG", "SGP");
        public static readonly CountryCode HN18442 = new CountryCode(18442, "HN18442", "HN", "HND");
        public static readonly CountryCode NI19466 = new CountryCode(19466, "NI19466", "NI", "NIC");
        public static readonly CountryCode PR20490 = new CountryCode(20490, "PR20490", "PR", "PRI");
        public static readonly CountryCode US21514 = new CountryCode(21514, "US21514", "US", "USA");
        public static readonly CountryCode LatinAmerica22538 = new CountryCode(
                                                                   22538,
                                                                   "LatinAmerica22538",
                                                                   "419",
                                                                   "419"
                                                               );

#endregion
    }
}

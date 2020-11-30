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
using UnityEngine;
using System;
using System.Collections.Generic;


namespace Jackpot
{
    /// <summary>
    /// WindowsTimeZone表記を, Olson形式に変換する関数を提供するクラスです
    /// </summary>
    public class OlsonMapper
    {
        /// <summary>
        /// timeZoneIdにマッチする, Olson形式の文字列を返却します.
        /// </summary>
        /// <param name="timeZoneId">timeZoneId</param>
        public static string Find(string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId))
            {
                throw new ArgumentNullException(string.Format("timeZoneId was null. timeZoneId:{0}", timeZoneId));
            }

            return olsonMap[timeZoneId];
        }

        static Dictionary<string, string> olsonMap = new Dictionary<string, string>()
        {
            { "Dateline Standard Time", "Etc/GMT+12" },
            { "UTC-11", "Etc/GMT+11" },
            { "Hawaiian Standard Time", "Pacific/Honolulu" },
            { "Alaskan Standard Time", "America/Anchorage" },
            { "Pacific Standard Time (Mexico)", "America/Santa_Isabel" },
            { "Pacific Standard Time", "America/Los_Angeles" },
            { "US Mountain Standard Time", "America/Phoenix" },
            { "Mountain Standard Time (Mexico)", "America/Chihuahua" },
            { "Mountain Standard Time", "America/Denver" },
            { "Central Standard Time (Mexico)", "America/Mexico_City" },
            { "Canada Central Standard Time", "America/Regina" },
            { "Central America Standard Time", "America/Guatemala" },
            { "Central Standard Time", "America/Chicago" },
            { "US Eastern Standard Time", "America/Indianapolis" },
            { "SA Pacific Standard Time", "America/Bogota" },
            { "Eastern Standard Time", "America/New_York" },
            { "Venezuela Standard Time", "America/Caracas" },
            { "Paraguay Standard Time", "America/Asuncion" },
            { "Central Brazilian Standard Time", "America/Cuiaba" },
            { "Pacific SA Standard Time", "America/Santiago" },
            { "SA Western Standard Time", "America/La_Paz" },
            { "Atlantic Standard Time", "America/Halifax" },
            { "Newfoundland Standard Time", "America/St_Johns" },
            { "SA Eastern Standard Time", "America/Cayenne" },
            { "Greenland Standard Time", "America/Godthab" },
            { "Bahia Standard Time", "America/Bahia" },
            { "Argentina Standard Time", "America/Buenos_Aires" },
            { "E. South America Standard Time", "America/Sao_Paulo" },
            { "Montevideo Standard Time", "America/Montevideo" },
            { "UTC-02", "Etc/GMT+2" },
            { "Azores Standard Time", "Atlantic/Azores" },
            { "Cape Verde Standard Time", "Atlantic/Cape_Verde" },
            { "Morocco Standard Time", "Africa/Casablanca" },
            { "GMT Standard Time", "Europe/London" },
            { "Greenwich Standard Time", "Atlantic/Reykjavik" },
            { "UTC", "Etc/GMT" },
            { "W. Europe Standard Time", "Europe/Berlin" },
            { "Namibia Standard Time", "Africa/Windhoek" },
            { "Central European Standard Time", "Europe/Warsaw" },
            { "Libya Standard Time", "Africa/Tripoli" },
            { "Romance Standard Time", "Europe/Paris" },
            { "Central Europe Standard Time", "Europe/Budapest" },
            { "W. Central Africa Standard Time", "Africa/Lagos" },
            { "GTB Standard Time", "Europe/Bucharest" },
            { "Turkey Standard Time", "Europe/Istanbul" },
            { "Israel Standard Time", "Asia/Jerusalem" },
            { "Egypt Standard Time", "Africa/Cairo" },
            { "Syria Standard Time", "Asia/Damascus" },
            { "South Africa Standard Time", "Africa/Johannesburg" },
            { "FLE Standard Time", "Europe/Kiev" },
            { "Middle East Standard Time", "Asia/Beirut" },
            { "E. Europe Standard Time", "Asia/Nicosia" },
            { "Jordan Standard Time", "Asia/Amman" },
            { "Kaliningrad Standard Time", "Europe/Kaliningrad" },
            { "Arab Standard Time", "Asia/Riyadh" },
            { "E. Africa Standard Time", "Africa/Nairobi" },
            { "Arabic Standard Time", "Asia/Baghdad" },
            { "Iran Standard Time", "Asia/Tehran" },
            { "Arabian Standard Time", "Asia/Dubai" },
            { "Caucasus Standard Time", "Asia/Yerevan" },
            { "Georgian Standard Time", "Asia/Tbilisi" },
            { "Azerbaijan Standard Time", "Asia/Baku" },
            { "Mauritius Standard Time", "Indian/Mauritius" },
            { "Russian Standard Time", "Europe/Moscow" },
            { "Afghanistan Standard Time", "Asia/Kabul" },
            { "West Asia Standard Time", "Asia/Tashkent" },
            { "Pakistan Standard Time", "Asia/Karachi" },
            { "Sri Lanka Standard Time", "Asia/Colombo" },
            { "India Standard Time", "Asia/Calcutta" },
            { "Nepal Standard Time", "Asia/Katmandu" },
            { "Central Asia Standard Time", "Asia/Almaty" },
            { "Ekaterinburg Standard Time", "Asia/Yekaterinburg" },
            { "Bangladesh Standard Time", "Asia/Dhaka" },
            { "Myanmar Standard Time", "Asia/Rangoon" },
            { "N. Central Asia Standard Time", "Asia/Novosibirsk" },
            { "SE Asia Standard Time", "Asia/Bangkok" },
            { "Ulaanbaatar Standard Time", "Asia/Ulaanbaatar" },
            { "Singapore Standard Time", "Asia/Singapore" },
            { "North Asia Standard Time", "Asia/Krasnoyarsk" },
            { "W. Australia Standard Time", "Australia/Perth" },
            { "China Standard Time", "Asia/Shanghai" },
            { "Taipei Standard Time", "Asia/Taipei" },
            { "North Asia East Standard Time", "Asia/Irkutsk" },
            { "Korea Standard Time", "Asia/Seoul" },
            { "Tokyo Standard Time", "Asia/Tokyo" },
            { "Cen. Australia Standard Time", "Australia/Adelaide" },
            { "AUS Central Standard Time", "Australia/Darwin" },
            { "AUS Eastern Standard Time", "Australia/Sydney" },
            { "West Pacific Standard Time", "Pacific/Port_Moresby" },
            { "E. Australia Standard Time", "Australia/Brisbane" },
            { "Tasmania Standard Time", "Australia/Hobart" },
            { "Yakutsk Standard Time", "Asia/Yakutsk" },
            { "Vladivostok Standard Time", "Asia/Vladivostok" },
            { "Central Pacific Standard Time", "Pacific/Guadalcanal" },
            { "New Zealand Standard Time", "Pacific/Auckland" },
            { "Fiji Standard Time", "Pacific/Fiji" },
            { "Magadan Standard Time", "Asia/Magadan" },
            { "UTC+12", "Etc/GMT-12" },
            { "Samoa Standard Time", "Pacific/Apia" },
            { "Tonga Standard Time", "Pacific/Tongatapu" }
        };
    }
}


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
namespace Jackpot
{
    /// <summary>
    /// Country codeを取り扱うEnumerationです
    /// </summary>
    public partial class CountryCode : Enumeration
    {
        /// <summary>
        /// 識別子となるLCIDを示します
        /// </summary>
        /// <value>The lcid.</value>
        public int Lcid { get { return Value; } }

        /// <summary>
        /// ISO3166-1 Alpha2形式のCountry Codeを示します
        /// </summary>
        /// <value><c>true</c> if this instance iso3166 p1 alpha2; otherwise, <c>false</c>.</value>
        public string Iso3166P1Alpha2 { get { return iso3166P1Alpha2; } }

        /// <summary>
        /// ISO3166-1 Alpha3形式のCountry Codeを示します
        /// </summary>
        /// <value><c>true</c> if this instance iso3166 p1 alpha2; otherwise, <c>false</c>.</value>
        public string Iso3166P1Alpha3 { get { return iso3166P1Alpha3; } }

        readonly string iso3166P1Alpha2;
        readonly string iso3166P1Alpha3;

#region Constructor

        CountryCode(int lcid, string name, string iso3166P1Alpha2, string iso3166P1Alpha3) : base(lcid, name)
        {
            this.iso3166P1Alpha2 = iso3166P1Alpha2;
            this.iso3166P1Alpha3 = iso3166P1Alpha3;
        }

#endregion

#region Public Methods

        /// <summary>
        /// LCIDからISO3166-1 Alpha2のCountry Codeに変換します
        /// </summary>
        /// <returns>The to iso3166 p1 alpha2.</returns>
        /// <param name="lcid">Lcid.</param>
        /// <param name="defaultValue">Default value.</param>
        public static string LcidToIso3166P1Alpha2(int lcid, string defaultValue)
        {
            var countryCode = Enumeration.FromValue<CountryCode>(lcid, null);
            return countryCode != null ? countryCode.Iso3166P1Alpha2 : defaultValue;
        }

        /// <summary>
        /// ISO3166-2 Alpha3のCountry CodeからISO3166-1 Alpha2のCountry Codeに変換します
        /// </summary>
        /// <returns><c>true</c> if iso3166 p1 alpha3 to iso3166 p1 alpha2 the specified iso3166P1Alpha3 defaultValue; otherwise, <c>false</c>.</returns>
        /// <param name="iso3166P1Alpha3">Iso3166 p1 alpha3.</param>
        /// <param name="defaultValue">Default value.</param>
        public static string Iso3166P1Alpha3ToIso3166P1Alpha2(string iso3166P1Alpha3, string defaultValue)
        {
            if (iso3166P1Alpha3 == null)
            {
                return defaultValue;
            }
            if (iso3166P1Alpha3.Length != 3)
            {
                return defaultValue;
            }
            var countryCode = Enumeration.Parse<CountryCode>(c => c.Iso3166P1Alpha3 == iso3166P1Alpha3, null);
            return countryCode != null ? countryCode.Iso3166P1Alpha2 : defaultValue;
        }

#endregion
    }
}


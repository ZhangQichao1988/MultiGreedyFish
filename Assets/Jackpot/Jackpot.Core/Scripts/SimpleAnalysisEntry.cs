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
    /// 解析結果を保持する事を目的とし、key-valueと正当性を示すプロパティを保持するクラスです
    /// </summary>
    public class SimpleAnalysisEntry
    {
        /// <summary>
        /// 解析内容を示すkeyを示します
        /// </summary>
        /// <value>The key.</value>
        public string Key { get { return key; } }

        /// <summary>
        /// 解析結果を示すvalueを示します
        /// </summary>
        /// <value>The value.</value>
        public string Value { get { return value; } }

        /// <summary>
        /// 解析結果として正当性があったか否かを示します
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get { return valid; } }

        readonly string key;
        readonly string value;
        readonly bool valid;

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.SimpleListingEntry`2"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="valid">If set to <c>true</c> valid.</param>
        public SimpleAnalysisEntry(string key, string value, bool valid)
        {
            this.key = key;
            this.value = value;
            this.valid = valid;
        }
    }
}

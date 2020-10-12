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
using System.Collections.Generic;
using System.Text;

namespace Jackpot
{
    /// <summary>
    /// クエリパラメータを管理生成するObjectです
    /// Not Supporting URL encodings other than UTF-8
    /// </summary>
    public class QueryParameter
    {
        #region Fields

        Dictionary<string, List<string>> parameters;
        List<string> notEscapeParameterKeys;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Network.QueryParameter"/> class.
        /// </summary>
        public QueryParameter()
            : this(new Dictionary<string, List<string>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Network.QueryParameter"/> class.
        /// </summary>
        /// <param name="dictionary">Dictionary.</param>
        public QueryParameter(Dictionary<string, List<string>> dictionary)
        {
            parameters = dictionary;
            notEscapeParameterKeys = new List<string>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 指定した文字列からQueryString Objectを生成します
        /// </summary>
        /// <returns>The query.</returns>
        /// <param name="queryString">Query string.</param>
        public static QueryParameter ParseQuery(string queryString)
        {
            var parameter = new QueryParameter();
            if (string.IsNullOrEmpty(queryString))
            {
                return parameter;
            }
            // Remove '?' if contains in the head
            var head = queryString.Substring(0, 1);
            if (head == "?")
            {
                queryString = queryString.Substring(1);
            }
            var paramStrings = queryString.Split('&');
            foreach (var paramString in paramStrings)
            {
                var paramSplit = paramString.Split('=');

                // Cannot splitting
                if (paramSplit.Length == 0)
                {
                    continue;
                }

                var name = paramSplit[0];
                var value = paramSplit[1];

                // Invalid parameter
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                // Set string.Empty if value was null.
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }

                // Decode URI components
                name = Uri.UnescapeDataString(name);
                value = Uri.UnescapeDataString(value);

                parameter.AddParam(name, value);
            }
            return parameter;
        }

        /// <summary>
        /// エスケープしないパラメータのkeyを追加します
        /// </summary>
        /// <param name="key">Name.</param>
        public void AddNotEscapeParameterKeys(string key)
        {
            if (!notEscapeParameterKeys.Contains(key))
            {
                notEscapeParameterKeys.Add(key);
            }
        }

        /// <summary>
        /// 全てのエスケープしないパラメータのkeyを取得します
        /// </summary>
        /// <returns>The all parameters.</returns>
        public List<string> GetAllotEscapeParameterKeys()
        {
            return new List<string>(notEscapeParameterKeys);
        }

        /// <summary>
        /// クエリパラメータを追加します
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="moreValues">More values.</param>
        public void AddParam(string name, string value, params string[] moreValues)
        {
            AddParam(name, value);

            if (moreValues == null)
            {
                return;
            }

            foreach (var moreValue in moreValues)
            {
                AddParam(name, moreValue);
            }
        }

        /// <summary>
        /// クエリパラメータをマージします
        /// </summary>
        /// <param name="queryParam">Query parameter.</param>
        public void AddParam(QueryParameter queryParam)
        {
            if (queryParam == null)
            {
                return;
            }
            foreach (var parameters in queryParam.GetAllParams())
            {
                foreach (var value in queryParam.GetParams(parameters.Key))
                {
                    AddParam(parameters.Key, value);
                }
            }
        }

        /// <summary>
        /// Dictionaryを指定してクエリパラメータを追加します
        /// </summary>
        /// <param name="dict">Dict.</param>
        public void AddParam(Dictionary<string, string> dict)
        {
            if (dict == null || dict.Count <= 0)
            {
                return;
            }
            foreach (var pair in dict)
            {
                AddParam(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Dictionaryを指定してクエリパラメータを追加します
        /// </summary>
        /// <param name="dict">Dict.</param>
        public void AddParam(Dictionary<string, List<string>> dict)
        {
            if (dict == null || dict.Count <= 0)
            {
                return;
            }
            foreach (var pair in dict)
            {
                var key = pair.Key;
                var values = pair.Value;
                foreach (var value in values)
                {
                    AddParam(key, value);
                }
            }
        }

        /// <summary>
        /// 指定したnameのパラメータを返却します
        /// </summary>
        /// <returns>The parameter.</returns>
        /// <param name="name">Name.</param>
        public string GetParam(string name)
        {
            if (!parameters.ContainsKey(name))
            {
                return string.Empty;
            }
            return parameters[name][0];
        }

        /// <summary>
        /// 指定したnameのパラメータをListで返却します
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="name">Name.</param>
        public List<string> GetParams(string name)
        {
            if (!parameters.ContainsKey(name))
            {
                return new List<string>();
            }
            return new List<string>(parameters[name]);
        }

        /// <summary>
        /// 全てのクエリパラメータを取得します
        /// </summary>
        /// <returns>The all parameters.</returns>
        public Dictionary<string, List<string>> GetAllParams()
        {
            return new Dictionary<string, List<string>>(parameters);
        }

        /// <summary>
        /// クエリパラメータが空である事を示します
        /// </summary>
        /// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty()
        {
            return parameters.Count == 0;
        }

        /// <summary>
        /// クエリパラメータを文字列出力します
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Network.QueryParameter"/>.</returns>
        public override string ToString()
        {
            return CreateQueryString(parameters);
        }

        /// <summary>
        /// ?付きクエリパラメータを文字列出力します
        /// クエリパラメータが空の場合は空文字を出力します
        /// </summary>
        /// <returns>The query.</returns>
        public string ToQuery()
        {
            return IsEmpty() ? string.Empty : string.Format("?{0}", CreateQueryString(parameters));
        }

        #endregion

        #region Private Methods

        void AddParam(string name, string value)
        {
            if (!parameters.ContainsKey(name))
            {
                parameters[name] = new List<string>();
            }
            parameters[name].Add(value);
        }

        string CreateQueryString(Dictionary<string, List<string>> parameters)
        {
            if (IsEmpty())
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                var keyStr = UriHelper.Escape(parameter.Key);
                foreach (var value in parameter.Value)
                {
                    if (notEscapeParameterKeys.Contains(parameter.Key))
                    {
                        var valStr = value == null ? string.Empty : value;
                        builder.AppendFormat("{0}={1}&", parameter.Key, valStr);
                    }
                    else
                    {
                        var valStr = value == null ? string.Empty : UriHelper.Escape(value);
                        builder.AppendFormat("{0}={1}&", keyStr, valStr);
                    }
                }
            }

            // Remove the ampersand in the end of a character string.
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        #endregion
    }
}

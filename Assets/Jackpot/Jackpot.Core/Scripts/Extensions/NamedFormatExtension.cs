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
using System.Reflection;

namespace Jackpot.Extensions
{
    /// <summary>
    /// string Objectに名前付きフォーマット機能を持たせるExtension Methodです
    /// </summary>
    public static class NamedFormatExtension
    {
        #region Constants

        static readonly char[] Literals = new char[2] { '{', '}' };
        static readonly char LeftLiteralChar = Literals[0];
        static readonly char RightLiteralChar = Literals[1];
        static readonly string LeftLiteral = LeftLiteralChar.ToString();
        static readonly string RightLiteral = RightLiteralChar.ToString();
        static readonly string EscapedLeftLiteral = LeftLiteral + LeftLiteral;
        static readonly string EscapedRightLiteral = RightLiteral + RightLiteral;

        #endregion

        #region Extension Methods

        /// <summary>
        /// 名前付きフォーマットを実行し、その結果を返却します
        /// </summary>
        /// <remarks>
        /// <code>
        /// using Jackpot.Extensions;
        /// var foo = "Hello, {name}!";
        /// var result = foo.NamedFormat(new { name = "Unity" });
        /// Debug.Log(result); // => "Hello, unity!";
        /// </code>
        /// </remarks>
        /// <returns>The format.</returns>
        /// <param name="self">Self.</param>
        /// <param name="source">Source.</param>
        public static string NamedFormat(this string self, object source = null)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }
            var propertyDict = new Dictionary<string, string>();
            var properties = source == null ? default(PropertyInfo[]) : source.GetType().GetProperties();
            if (properties != null)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    if (!property.CanRead)
                    {
                        continue;
                    }
                    var value = property.GetGetMethod().Invoke(source, null);
                    if (value == null)
                    {
                        value = "Null";
                    }
                    propertyDict.Add(property.Name, value.ToString());
                }
            }
            return self.NamedFormat(propertyDict);
        }

        /// <summary>
        /// 名前付きフォーマットを実行し、その結果を返却します
        /// </summary>
        /// <remarks>
        /// <code>
        /// using System.Collections.Generic;
        /// using Jackpot.Extensions;
        /// var foo = "Hello, {name}!";
        /// var result = foo.NamedFormat(new Dictionary<string, string>() { { "name", "Unity" } });
        /// Debug.Log(result); // => "Hello, unity!";
        /// </code>
        /// </remarks>
        /// <returns>The format.</returns>
        /// <param name="self">Self.</param>
        /// <param name="source">Source.</param>
        public static string NamedFormat(this string self, Dictionary<string, string> source)
        {
            if (string.IsNullOrEmpty(self))
            {
                return string.Empty;
            }
            var result = string.Empty;
            foreach (var expression in Expressions(self))
            {
                result += expression.Eval(source);
            }
            return result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// <see cref="Jackpot.Extensions.NamedFormatHandler"/>を登録します。prefixが重複した場合、Handlerは上書きされます
        /// </summary>
        /// <remarks>
        /// ハンドリングされた文字列を自由に変換して返却できます
        /// 実装Object内で例外がスローされた場合、代わりにフォーマットをかける前の文字列が出力されます
        /// 以下にSampleを示します
        /// <code>
        /// using System.Collections.Generic;
        /// using UnityEngine;
        /// using Jackpot.Extensions;
        /// 
        /// public class ExampleBehaviour : MonoBehaviour
        /// {
        ///     Dictionary<string, string> customDict;
        ///     void Awake()
        ///     {
        ///         // 辞書データを作成します
        ///         customDict = new Dictionary<string, string>()
        ///         {
        ///             { "yuno", "X/ _ /X" },
        ///         };
        ///
        ///         // 辞書にデータがなかったときは代わりにこの文字列を出力するとします
        ///         var unknown = "???";
        ///
        ///         // #から始まる場合の変換ルールを追加します
        ///         NamedFormatExtension.AddHandler("#", (prefix, formatString) =>
        ///         {
        ///             // prefixにはPrefix指定した#が
        ///             // formatStringはprefixが取り除かれた文字列が渡されます
        ///             // e.g. ) "{#hoge}".NamedFormat(); の場合、prefix = #, formatString = hoge
        ///             if (!customDict.ContainsKey(formatString))
        ///             {
        ///                 return unknown;
        ///             }
        ///             return customDict[formatString];
        ///         });
        ///     }
        ///
        ///     void Start()
        ///     {
        ///         // 辞書も引数もないのでフォーマット文字列が出力されます
        ///         Debug.Log("{yuno}".NamedFormat()); // => yuno
        ///
        ///         // 引数指定されたので、その通りに表示されます
        ///         Debug.Log("{yuno}".NamedFormat(new { yuno = "ゆのっち" });  // => ゆのっち
        ///
        ///         // #から始まったので、登録されたHandlerの変換ルールが適用されます
        ///         Debug.Log("{#yuno}".NamedFormat());  // => X/ _ /X
        ///
        ///         // Dictionaryを引数に指定する事でkeyをPrefixから始めることもできますが
        ///         // Handler登録されている場合、登録済みのHandlerの変換ルールが適用されます
        ///         Debug.Log("{#yuno}".NamedFormat(new Dictionary<string, string>() { { "#yuno", "ゆのっち" }}));      // => X/ _ /X
        ///         Debug.Log("{#ume}".NamedFormat(new Dictionary<string, string>() { { "#ume", "うめてんてー" }}));    // => ???
        ///
        ///         // 通常のNamedFormatと併用できます
        ///         Debug.Log("{#yuno} < {message}".NamedFormat(new { message = "来週もみてくださいね!" })); // => X/ _ /X < 来週も見てくださいね!
        ///     }
        /// }
        /// </code>
        /// </remarks>
        /// <param name="prefix">
        /// カスタムハンドリングする為のPrefix文字列です。string.StartsWith(prefix)で判定されます
        /// </param>
        /// <param name="handler">Handler.</param>
        public static void AddHandler(string prefix, NamedFormatDelegate handler)
        {
            FormatExpression.AddHandler(prefix, handler);
        }

        /// <summary>
        /// 指定されたprefixで始まるformat文字列を処理するHandlerが登録されているか否かを示します
        /// </summary>
        /// <returns><c>true</c>, if contains was handlered, <c>false</c> otherwise.</returns>
        /// <param name="prefix">Prefix.</param>
        public static bool HandlerContains(string prefix)
        {
            return FormatExpression.HandlerContains(prefix);
        }

        /// <summary>
        /// 指定されたprefixで始まるformat文字列を処理するHandlerの登録を解除します
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        public static void RemoveHandler(string prefix)
        {
            FormatExpression.RemoveHandler(prefix);
        }

        /// <summary>
        /// 全てのHandlerの登録を解除します
        /// </summary>
        public static void RemoveAllHandlers()
        {
            FormatExpression.RemoveAllHandlers();
        }

        #endregion

        #region Private Methods

        static IEnumerable<ITextExpression> Expressions(string format)
        {
            var yieldIndex = 0;
            var expressionStartIndex = 0;
            var expressionEndIndex = -1;
            var expressionHasBeenStarted = false;
            var cachedResult = new Dictionary<string, string>();

            for (var i = 0; i < format.Length; i++)
            {
                var currentChar = format[i];
                char? nextChar = null;
                if (i + 1 != format.Length)
                {
                    nextChar = format[i + 1];
                }

                // left literal
                if (!expressionHasBeenStarted)
                {
                    if (currentChar == RightLiteralChar)
                    {
                        if (nextChar != null && nextChar == RightLiteralChar)
                        {
                            yield return new LiteralExpression(format.Substring(yieldIndex, i + 1 - yieldIndex));
                            i++;
                            yieldIndex = i + 1;
                            continue;
                        }
                        throw new FormatException(string.Format("Invalid Format: {0}", format));
                    }
                    if (currentChar != LeftLiteralChar)
                    {
                        if (nextChar == null)
                        {
                            yield return new LiteralExpression(format.Substring(yieldIndex, format.Length - yieldIndex));
                            break;
                        }
                        continue;
                    }
                    if (nextChar == null)
                    {
                        throw new FormatException(string.Format("Invalid Format: {0}", format));
                    }
                    if (nextChar == LeftLiteralChar)
                    {
                        yield return new LiteralExpression(format.Substring(yieldIndex, i + 1 - yieldIndex));
                        i++;
                        yieldIndex = i + 1;
                        continue;
                    }
                    yield return new LiteralExpression(format.Substring(yieldIndex, i - yieldIndex));
                    expressionHasBeenStarted = true;
                    expressionStartIndex = i;
                    continue;
                }

                // right literal
                if (currentChar != RightLiteralChar)
                {
                    if (nextChar == null)
                    {
                        throw new FormatException(string.Format("Invalid Format: {0}", format));
                    }
                    continue;
                }
                expressionEndIndex = i;
                yield return new FormatExpression(format.Substring(expressionStartIndex, expressionEndIndex - expressionStartIndex + 1), cachedResult);
                expressionHasBeenStarted = false;
                if (nextChar == null)
                {
                    break;
                }
                yieldIndex = expressionEndIndex + 1;
            }
        }

        #endregion

        #region Inner Classes

        interface ITextExpression
        {
            string Eval(Dictionary<string, string> obj);
        }

        class FormatExpression : ITextExpression
        {
            static readonly Dictionary<string, NamedFormatDelegate> handlers = new Dictionary<string, NamedFormatDelegate>();
            readonly bool invalidExpression;
            readonly string expression;
            readonly string format;
            readonly Dictionary<string, string> cachedResult;

            #region Constructor

            public FormatExpression(string expression, Dictionary<string, string> cachedResult)
            {
                this.expression = expression;
                this.cachedResult = cachedResult;
                if (!expression.StartsWith(LeftLiteral) || !expression.EndsWith(RightLiteral))
                {
                    invalidExpression = true;
                    return;
                }
                invalidExpression = false;
                format = expression.Substring(1, expression.Length - 2);
            }

            #endregion

            #region ITextExpression

            public string Eval(Dictionary<string, string> source)
            {
                if (invalidExpression)
                {
                    throw new FormatException(string.Format("Invalid Expression: {0}", expression));
                }
                if (cachedResult.ContainsKey(format))
                {
                    return cachedResult[format];
                }
                try
                {
                    var prefix = string.Empty;
                    var localFormat = format;
                    NamedFormatDelegate handler = DefaultHandler;
                    foreach (var pair in handlers)
                    {
                        if (format.StartsWith(pair.Key) && pair.Value != null)
                        {
                            prefix = pair.Key;
                            localFormat = format.Substring(prefix.Length);
                            handler = pair.Value;
                            break;
                        }
                    }
                    cachedResult[format] = handler(prefix, localFormat, source);
                }
                catch (Exception e)
                {
                    Logger.Get("Jackpot.Extensions.NamedFormatExtension").Error(e);
                    cachedResult[format] = format;
                }
                return cachedResult[format];
            }

            string DefaultHandler(string prefix, string format, Dictionary<string, string> source)
            {
                if (source == null)
                {
                    return format;
                }
                return source[format];
            }

            #endregion

            #region Handler Methods

            public static void AddHandler(string prefix, NamedFormatDelegate handler)
            {
                if (string.IsNullOrEmpty(prefix) || prefix.Trim().Length == 0)
                {
                    throw new ArgumentException(string.Format("expected prefix is not empty, but was {0}", prefix));
                }
                if (prefix.Contains(LeftLiteral) || prefix.Contains(RightLiteral))
                {
                    throw new ArgumentException(string.Format("cannot set literal string to prefix, but was {0}", prefix));
                }
                if (handler == null)
                {
                    return;
                }
                RemoveHandler(prefix);
                handlers.Add(prefix, handler);
            }

            public static bool HandlerContains(string prefix)
            {
                return handlers.ContainsKey(prefix) && handlers[prefix] != null;
            }

            public static void RemoveHandler(string prefix)
            {
                if (!HandlerContains(prefix))
                {
                    return;
                }
                handlers.Remove(prefix);
            }

            public static void RemoveAllHandlers()
            {
                handlers.Clear();
            }

            #endregion
        }

        class LiteralExpression : ITextExpression
        {
            readonly string expression;

            public LiteralExpression(string expression)
            {
                this.expression = expression;
            }

            public string Eval(Dictionary<string, string> source)
            {
                return expression.Replace(EscapedLeftLiteral, LeftLiteral).Replace(EscapedRightLiteral, RightLiteral);
            }
        }

        #endregion
    }
}

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
using System.Collections;
using System.Text.RegularExpressions;

namespace Jackpot
{
    public class GpuUtility
    {
        static readonly Regex Reg = new Regex(@"[^T0-9]");

        /// <summary>
        /// 端末のGPUの種類を取得します.
        /// </summary>
        /// <returns>The gpu.</returns>
        public static GpuKind GetGpuKind()
        {
            return GetGpuKind(SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion);
        }

        /// <summary>
        /// 端末のGPUの種類を取得します.
        /// </summary>
        /// <returns>The gpu.</returns>
        /// <param name="graphicsDeviceName">Graphics device name.</param>
        /// <param name="graphicsDeviceVersion">Graphics device version.</param>
        public static GpuKind GetGpuKind(string graphicsDeviceName, string graphicsDeviceVersion)
        {
            if (string.IsNullOrEmpty(graphicsDeviceName))
            {
                return GpuKind.Unknown;
            }

            if (Platform.IsIos())
            {
                if (graphicsDeviceName.StartsWith("PowerVR"))
                {
                    return GpuKind.ApplePowerVR;
                }
                if (graphicsDeviceName.StartsWith("Apple A"))
                {
                    return GpuKind.Apple;
                }
            }
            else if (Platform.IsAndroid())
            {
                var openGlVersion = GetOpenGlESVersion(graphicsDeviceVersion);

                // PowerVRシリーズの判定
                if (graphicsDeviceName.StartsWith("PowerVR"))
                {
                    return GpuKind.AndroidPowerVR;
                }

                // Adrenoシリーズの判定
                if (graphicsDeviceName.StartsWith("Adreno"))
                {
                    var deviceNameWords = graphicsDeviceName.Split(' ');
                    foreach (var word in deviceNameWords)
                    {
                        if (word.IndexOf("Adreno") == -1 &&
                            word.IndexOf("(TM)") == -1)
                        {
                            // シリーズを取得し1桁目で該当シリーズを判定
                            var num = int.Parse(word.Substring(0, 1));
                            if (num <= 3)
                            {
                                return GpuKind.Adreno3xxAndUnder;
                            }
                            if (num >= 4)
                            {
                                return GpuKind.Adreno4xxAndOver;
                            }
                        }
                    }
                }

                // Maliシリーズの判定
                if (graphicsDeviceName.StartsWith("Mali"))
                {
                    // Txxxシリーズを取得
                    var series = Reg.Replace(graphicsDeviceName, string.Empty);
                    if (string.IsNullOrEmpty(series))
                    {
                        return GpuKind.Mali;
                    }

                    // 数値のみ取得
                    var no = int.Parse(series.Substring(1));

                    // T600シリーズ以降且つOpenGL ES 3.0以上か判定
                    if (no >= 600 && openGlVersion >= 3)
                    {
                        return GpuKind.MaliOther;
                    }
                    else
                    {
                        return GpuKind.Mali;
                    }
                }

                // Tegraシリーズの判定
                // OpenGL ES 3以上の場合はTegra K1 T1と判定する
                if (graphicsDeviceName.IndexOf("Tegra") != -1)
                {
                    if (openGlVersion <= 2)
                    {
                        return GpuKind.Tegra;
                    }
                    if (openGlVersion >= 3)
                    {
                        return GpuKind.TegraK1OrX1;
                    }
                }

                // 上記のどれにも当てはまらない場合はOpenGL ESのバージョンで判定する
                if (openGlVersion > 0 && openGlVersion <= 2)
                {
                    return GpuKind.OtherOpenGLES2xAndUnder;
                }
                if (openGlVersion >= 3)
                {
                    return GpuKind.OtherOpenGLES3xAndOver;
                }
            }
            return GpuKind.Other;
        }

        /// <summary>
        /// Open GL ES のメジャーバージョンを取得します.
        /// </summary>
        /// <returns>The open gl ES version.</returns>
        /// <param name="openGlVersion">Open gl version.</param>
        static int GetOpenGlESVersion(string openGlVersion)
        {
            if (!string.IsNullOrEmpty(openGlVersion) && openGlVersion.IndexOf("OpenGL") != -1)
            {
                var openGlVersionWords = openGlVersion.Split(' ');
                foreach (var word in openGlVersionWords)
                {
                    if (word.IndexOf("OpenGL") == -1 &&
                        word.IndexOf("ES") == -1)
                    {
                        var num = int.Parse(word.Substring(0, 1));
                        return num;
                    }
                }
            }

            return 0;
        }
    }
}

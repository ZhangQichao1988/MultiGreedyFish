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

namespace Jackpot
{
    public enum GpuKind
    {
        // 以下のどれにも当てはまらないもの
        Unknown = 0x000,
        // iOS Appleから始まる場合
        Apple = 0x001,
        // iOS PowerVRから始まる場合
        ApplePowerVR = 0x002,
        // Android PowerVRから始まる場合
        AndroidPowerVR = 0x004,
        // Android Adreno 3xx以前
        Adreno3xxAndUnder = 0x008,
        // Android Adreno 4xx以降
        Adreno4xxAndOver = 0x010,
        // Android Mali
        Mali = 0x020,
        // Android Mali T600シリーズ以降且つOpenGL ES 3.0以上
        MaliOther = 0x040,
        // Android Tegra
        Tegra = 0x080,
        // Android Tegra K1, Tegra X1の場合
        TegraK1OrX1 = 0x100,
        // Android OpenGL ES 2.0以前
        OtherOpenGLES2xAndUnder = 0x200,
        // Android OpenGL ES 3.0以降
        OtherOpenGLES3xAndOver = 0x400,
        // その他
        Other = 0x800
    }
}

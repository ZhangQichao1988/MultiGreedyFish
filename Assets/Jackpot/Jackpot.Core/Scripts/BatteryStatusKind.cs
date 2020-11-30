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
    /// バッテリーの充電状態を示します
    /// </summary>
    public enum BatteryStatusKind
    {
        /// <summary>
        /// 状態不明
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 放電中
        /// </summary>
        Discharging = 1,

        /// <summary>
        /// 充電中
        /// </summary>
        Charging = 2,

        /// <summary>
        /// フル充電
        /// </summary>
        Full = 3,
    }
}

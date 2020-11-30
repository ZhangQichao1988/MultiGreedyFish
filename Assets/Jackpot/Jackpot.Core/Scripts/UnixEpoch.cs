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
    public class UnixEpoch
    {
        static readonly DateTime epochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            return epochTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static long FromDateTime(DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();
            return (long) dateTime.Subtract(epochTime).TotalSeconds;
        }
    }
}

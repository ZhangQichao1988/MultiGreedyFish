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
#if UNITY_ANDROID
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Jackpot
{
    partial class AndroidPlatform
    {
        public class AndroidPlatformListener : AndroidJavaProxy
        {
            public event Action<string> OnLoadResemaraDetectionIdentifierComplete;

            public AndroidPlatformListener() : base("com.klab.jackpot.Listener")
            {
            }

            public void onReceived(string id, int kind)
            {
                switch (kind)
                {
                    case (int) ResemaraDetectionIdResultKind.Succeeded:
                        MainThreadDispatcher.Post(() =>
                        {
                            OnLoadResemaraDetectionIdentifierComplete(id);
                        });
                        break;
                    case (int) ResemaraDetectionIdResultKind.GooglePlayServicesNotAvailable:
                    case (int) ResemaraDetectionIdResultKind.GooglePlayServicesDisconnect:
                    case (int) ResemaraDetectionIdResultKind.GooglePlayServicesRepairable:
                    case (int) ResemaraDetectionIdResultKind.UnknownError:
                    case (int) ResemaraDetectionIdResultKind.Failed:
                        // 固有IDの取得に失敗した場合は空文字を返す
                        MainThreadDispatcher.Post(() =>
                        {
                            OnLoadResemaraDetectionIdentifierComplete("");
                        });
                        break;
                }

                if (resemaraObject != null)
                {
                    resemaraObject = null;
                }
            }

        }
    }
}
#endif

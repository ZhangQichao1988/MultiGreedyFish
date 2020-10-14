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

#if UNITY_ANDROID
namespace Jackpot
{
    public partial class AndroidRequestPermission
    {
        /// <summary>
        /// Android permission listener.
        /// </summary>
        class AndroidRequestPermissionListener : AndroidJavaProxy
        {
            public AndroidRequestPermissionListener()
                : base("com.klab.jackpot.PermissionListener")
            {
            }


            /// <summary>
            /// Requesets the permission success.
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void requestPermissionGranted()
            {
                MainThreadDispatcher.Post(() =>
                {
                    if (GrantedRequestPermission != null)
                    {
                        GrantedRequestPermission();
                    }
                });
            }

            /// <summary>
            /// Requests the permission failed.
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void requestPermissionDenied()
            {
                MainThreadDispatcher.Post(() =>
                {
                    if (DeniedRequestPermission != null)
                    {
                        DeniedRequestPermission();
                    }
                });
            }

            /// <summary>
            /// Requests the permission need explain.
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void requestPermissionNeedExplain(int kind, string permission)
            {
                MainThreadDispatcher.Post(() =>
                {
                    if (NeedExplainRequestPermission != null)
                    {
                        NeedExplainRequestPermission(kind, permission);
                    }
                });
            }
        }
    }
}
#endif

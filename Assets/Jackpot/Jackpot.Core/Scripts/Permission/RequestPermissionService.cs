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
using System.Diagnostics;
using UnityEngine.SocialPlatforms;

namespace Jackpot
{
    /// <summary>
    /// Request permission service.
    /// </summary>
    public class RequestPermissionService
    {

#region Fields

#if UNITY_ANDROID
        public static RequestPermissionAndroid requestPermission;
#endif
        
#endregion

#region Public Methods

        /// <summary>
        /// Requests the permission.
        /// </summary>
        /// <returns><c>true</c>, if permission was requested, <c>false</c> otherwise.</returns>
        /// <param name="grantedPermission">Granted permission.</param>
        /// <param name="needExplainPermission">Need explain permission.</param>
        /// <param name="deniedPermission">Denied permission.</param>
        /// <param name="permission">Permission.</param>
        public static void LaunchGrantFlow(GrantedPermissionDelegate grantedPermission,
            NeedExplainPermissionDelegate needExplainPermission,
            DeniedPermissionDelegate deniedPermission,
            string permission)
        {

#if UNITY_ANDROID
            if (requestPermission != null)
            {
                throw new PermissionException("StartRequestPermission() started.");
            }

            requestPermission = new RequestPermissionAndroid(
                grantedPermission,
                needExplainPermission,
                deniedPermission,
                permission
            );
            requestPermission.StartRequestPermission();
#else
            if(grantedPermission != null)
            {
                grantedPermission();
            }
#endif
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Jackpot.RequestPermissionService"/> object.
        /// </summary>
        public static void Dispose()
        {
#if UNITY_ANDROID
            if (requestPermission != null)
            {
                requestPermission.Dispose();
            }
            requestPermission = null;
#endif
        }

#endregion
    }
}

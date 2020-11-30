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
using UnityEngine;

#if UNITY_ANDROID
namespace Jackpot
{
    public class RequestPermissionAndroid : IDisposable
    {
#region Fields

        bool disposed = false;
        string permission;
        GrantedPermissionDelegate defaultGrantedPermission;
        NeedExplainPermissionDelegate defaultNeedExplainPermission;
        DeniedPermissionDelegate defaultDeniedPermission;

#endregion

        public RequestPermissionAndroid(GrantedPermissionDelegate defaultGrantedPermission,
            NeedExplainPermissionDelegate defaultNeedExplainPermission,
            DeniedPermissionDelegate defaultDeniedPermission,
            string permission)
        {
            this.disposed = false;
            this.permission = permission;
            this.defaultGrantedPermission = defaultGrantedPermission;
            this.defaultNeedExplainPermission = defaultNeedExplainPermission;
            this.defaultDeniedPermission = defaultDeniedPermission;
            AndroidRequestPermission.GrantedRequestPermission += HandleRequestPermissionGranted;
            AndroidRequestPermission.DeniedRequestPermission += HandleRequestPermissionDenied;
            AndroidRequestPermission.NeedExplainRequestPermission += HandleRequestPermissionNeedExplain;
        }

        ~RequestPermissionAndroid()
        {
            if (disposed)
            {
                return;
            }
            Dispose();
        }

        public void StartRequestPermission()
        {
            AndroidRequestPermission.StartRequestPermission(this.permission);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            AndroidRequestPermission.GrantedRequestPermission -= HandleRequestPermissionGranted;
            AndroidRequestPermission.DeniedRequestPermission -= HandleRequestPermissionDenied;
            AndroidRequestPermission.NeedExplainRequestPermission -= HandleRequestPermissionNeedExplain;
            AndroidRequestPermission.Finish();
            disposed = true;
        }

        void HandleRequestPermissionGranted()
        {
            deneidFinishAction();
            defaultGrantedPermission();
        }

        void HandleRequestPermissionDenied()
        {
            defaultDeniedPermission(() =>
            {
                defaultNeedExplainPermission(
                    PermissionExplainType.AppFinish,
                    NeedExplainAppFinishOkAction,
                    NeedExplainCancelAction
                );
            }, deneidFinishAction);

        }

        void HandleRequestPermissionNeedExplain(int kind, string permission)
        {
            switch (kind)
            {
                case (int) PermissionExplainType.Normal:
                    defaultNeedExplainPermission(
                        PermissionExplainType.Normal,
                        NeedExplainNormalOkAction,
                        NeedExplainCancelAction
                    );
                    break;
                case (int) PermissionExplainType.AppFinish:
                    defaultNeedExplainPermission(
                        PermissionExplainType.AppFinish,
                        NeedExplainAppFinishOkAction,
                        NeedExplainCancelAction
                    );
                    break;
                case (int) PermissionExplainType.OpenSettings:
                    defaultNeedExplainPermission(
                        PermissionExplainType.OpenSettings,
                        NeedExplainOpenSettingsOkAction,
                        NeedExplainCancelAction
                    );
                    break;
            }
        }

        void NeedExplainNormalOkAction()
        {
            AndroidRequestPermission.RequestPermission(this.permission);
        }

        void NeedExplainAppFinishOkAction()
        {
            AndroidRequestPermission.RequestPermission(this.permission);
        }

        void NeedExplainOpenSettingsOkAction()
        {
            AndroidRequestPermission.OpenSettings();
        }

        void NeedExplainCancelAction()
        {
            deneidFinishAction();
        }

        void deneidFinishAction()
        {
            RequestPermissionService.Dispose();
        }
    }
}
#endif

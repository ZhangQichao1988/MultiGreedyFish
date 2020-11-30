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
//
// KJPAppController.mm
// UnityAppControllerのオーバーライド
//

#import "KJPAppController.h"

@implementation KJPAppController

#ifdef JACKPOT_NOTIFICATION
- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    // Jackpot.Notification：アプリ起動時の処理
    [KJPNotification application:application didFinishLaunchingWithOptions:launchOptions];
    return YES;
}

- (void)application:(UIApplication*)application
didReceiveLocalNotification:(UILocalNotification*)notification
{
    // Jackpot.Notification：ローカル通知を受信した時の処理
    [KJPNotification application:application didReceiveLocalNotification:notification];
}

#endif

@end

#ifdef JACKPOT_APP_CONTROLLER
IMPL_APP_CONTROLLER_SUBCLASS(KJPAppController)
#endif

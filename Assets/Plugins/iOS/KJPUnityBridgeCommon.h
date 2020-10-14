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
//  KJPUnityBridgeCommon.h
//  KJPUnityPlugins
//

#import <Foundation/Foundation.h>

#ifndef KJPUnityPlugins_KJPUnityBridgeCommon_h
#define KJPUnityPlugins_KJPUnityBridgeCommon_h

#pragma mark Bridge Interface

#ifdef __cplusplus
extern "C" {
#endif

/**
 * Unityにて定義済み。補完用
 */
UIViewController* UnityGetGLViewController();

/**
 * Unityにて定義済み。補完用
 */
void UnitySendMessage(const char*, const char*, const char*);

/**
 * Unityにて定義済み。
 */
extern bool _unityAppReady;

/*!
 @function  makeStringCopy
 @abstract  Unity側に文字列を返却する為のchar*を返却
 @return
 */
static char* KJPMakeStringCopy(const char* string);

/*!
 @function  stringWithCString
 @abstract  Unity側から受け取った文字列をNSString*に変換
 @return
 */
static NSString* KJPStringWithCString(const char* string);

/*!
 @function KJPIsIPad
 @abstract IPadか否かを返却
 @return
 */
static BOOL KJPIsIPad();

/*!
 @function KJPUseLegacyLayout
 @abstract iOS8以前の古いレイアウト方式を使用するかを判断します
 @return
 */
static BOOL KJPUseLegacyLayout();

/*!
 @function KJPOsVersion
 @abstract iOSのバージョンを返却します
 @return
 */
static float KJPOsVersion();

#ifdef __cplusplus
}
#endif

@protocol AppDelegateListener;

/**
 * Unityにて定義済み。
 */
void UnityRegisterAppDelegateListener(id<AppDelegateListener> obj);

static char* KJPMakeStringCopy(const char* string)
{
    if (string == NULL) {
        return NULL;
    }
    char* res = (char*) malloc(sizeof(char) * (strlen(string) + 1));
    strcpy(res, string);
    return res;
}

static NSString* KJPStringWithCString(const char* string)
{
    return string == NULL ? [NSString stringWithUTF8String:""] : [NSString stringWithUTF8String:string];
}

static BOOL KJPIsIPad()
{
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 30200
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
        return YES;
    }
#endif
    return NO;
}

static BOOL KJPUseLegacyLayout()
{
    // 本当は使用しているiOSSDK(UIKit)が7.0かまで見ないといけない。。。
    float osVersion = KJPOsVersion();
    return osVersion < 8.0;
}

static float KJPOsVersion()
{
    static dispatch_once_t pred = 0;
    static float osVersion;
    dispatch_once(&pred, ^{
        osVersion = [[[UIDevice currentDevice] systemVersion] floatValue];
    });
    return osVersion;
}

#endif

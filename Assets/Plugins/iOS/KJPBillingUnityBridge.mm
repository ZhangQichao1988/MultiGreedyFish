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
//  KJPBillingUnityBridge.mm
//  Jackpot::Billing向けUnityBridge
//

#import "KJPBillingConfig.h"
#import "KJPBillingService.h"
#import "KJPUnityBridgeCommon.h"

#pragma mark Bridge Interface

#ifdef __cplusplus
extern "C" {
#endif
    
    // BillingConfig
    void _KJPBillingConfig_EnableUsingDeprecatedReceipt();
    
    // BillingService
    void _KJPBilling_EnableDebugging();
    bool _KJPBilling_canMakePayments();
    void _KJPBilling_startProductsRequest(const char* productIds);
    void _KJPBilling_startPayment(const char* productId);
    void _KJPBilling_finishTransaction(const char* transactionId);
    char* _KJPBilling_getPendingTransactions();
    char* _KJPBilling_getSavedIAPProductId();
    void _KJPBilling_refreshReceipt();
    void _KJPBilling_resume();
    void _KJPBilling_restore();
    
#ifdef __cplusplus
}
#endif

#pragma mark KJPBillingConfig

void _KJPBillingConfig_EnableUsingDeprecatedReceipt()
{
    [[KJPBillingConfig sharedConfig] enableUsingDeprecatedReceipt];
}

#pragma mark KJPBillingService

void _KJPBilling_EnableDebugging()
{
    [[KJBillingService sharedInstance] enableDebugging];
}

bool _KJPBilling_canMakePayments()
{
    return [[KJBillingService sharedInstance] canMakePayments];
}

void _KJPBilling_startProductsRequest(const char* productIds)
{
    [[KJBillingService sharedInstance]
     startProductsRequest:[KJPStringWithCString(productIds) componentsSeparatedByString:@","]];
}

void _KJPBilling_startPayment(const char* productId)
{
    [[KJBillingService sharedInstance] startPayment:KJPStringWithCString(productId)];
}

void _KJPBilling_finishTransaction(const char* transactionId)
{
    [[KJBillingService sharedInstance] finishTransaction:KJPStringWithCString(transactionId)];
}

char* _KJPBilling_getPendingTransactions()
{
    NSString* jsonString = [[KJBillingService sharedInstance] jsonStringByPendingTransactions];
    return KJPMakeStringCopy([jsonString UTF8String]);
}

char* _KJPBilling_getSavedIAPProductId()
{
    NSString* str = [[KJBillingService sharedInstance] getSavedIAPProductId];
    return KJPMakeStringCopy([str UTF8String]);
}

void _KJPBilling_refreshReceipt()
{
    [[KJBillingService sharedInstance] refreshReceipt];
}

void _KJPBilling_resume()
{
    [[KJBillingService sharedInstance] resumeCompletedTransactions];
}

void _KJPBilling_restore()
{
    [[KJBillingService sharedInstance] restoreCompletedTransactions];
}

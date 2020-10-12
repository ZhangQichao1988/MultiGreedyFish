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
#import <StoreKit/StoreKit.h>

extern NSString* const KJPBillingErrorDomain;
// error codes for the KJPBillingErrorDomain
enum {
    KJPBillingErrorUnknown,
    KJPBillingErrorPurchaseFailed,
};

@interface KJBillingService : NSObject <SKPaymentTransactionObserver, SKRequestDelegate, SKProductsRequestDelegate>

@property (nonatomic, readonly) BOOL isDebug;

+ (KJBillingService*)sharedInstance;
- (void)enableDebugging;
- (BOOL)canMakePayments;
- (void)startProductsRequest:(NSArray*)productIds;
- (void)startPayment:(NSString*)identifier;
- (void)resumeCompletedTransactions;
- (void)restoreCompletedTransactions;
- (void)finishTransaction:(NSString*)identifier;
- (void)startDownloads:(SKPaymentTransaction*)transaction;
- (void)didFailedToMakePayment:(NSError*)error;
- (void)refreshReceipt;
- (NSString*)jsonStringByPendingTransactions;
- (NSString*)getSavedIAPProductId;
@end

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
#if !__has_feature(objc_arc)
#warning This file must be compiled with ARC. Use -fobjc-arc flag (or convert project to ARC).
#endif

#import "KJPBillingProduct.h"
#import "KJPBillingService.h"
#import <StoreKit/StoreKit.h>

@interface KJBillingProduct () <SKRequestDelegate, SKProductsRequestDelegate>

@end

@implementation KJBillingProduct

- (void)startPayment:(NSString*)identifier
{
    // Create a product request object and initialize it with our product identifiers
    SKProductsRequest* request =
        [[SKProductsRequest alloc] initWithProductIdentifiers:[NSSet setWithArray:@[ identifier ]]];
    request.delegate = self;

    // Send the request to the App Store
    [request start];
}

- (void)addPaymentQueue:(SKPayment*)payment
{
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}

#pragma mark -
#pragma mark - SKProductsRequestDelegate methods

// Used to get the App Store's response to your request and notifies your observer
- (void)productsRequest:(SKProductsRequest*)request didReceiveResponse:(SKProductsResponse*)response
{
    if (response.products.count > 0) {
        SKMutablePayment* payment = [SKMutablePayment paymentWithProduct:response.products[0]];
        [[SKPaymentQueue defaultQueue] addPayment:payment];
        return;
    }

    NSMutableDictionary* details = [NSMutableDictionary dictionary];
    [details setValue:@"Invalid product identifier." forKey:NSLocalizedDescriptionKey];
    NSError* error =
        [NSError errorWithDomain:KJPBillingErrorDomain code:KJPBillingErrorPurchaseFailed userInfo:details];
    [[KJBillingService sharedInstance] didFailedToMakePayment:error];
}

#pragma mark -
#pragma mark SKRequestDelegate methods

// Called when the product request failed.
- (void)request:(SKRequest*)request didFailWithError:(NSError*)error
{
    [[KJBillingService sharedInstance] didFailedToMakePayment:error];
}

@end

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

#import <StoreKit/StoreKit.h>
#import "KJPBillingConfig.h"
#import "KJPBillingProduct.h"
#import "KJPBillingService.h"
#import "KJPUnityBridgeCommon.h"
#import "NSString+Base64Encode.h"

#define kMessageObject @"AppleStoreKitListener"
#define kMessageShouldAddStorePayment @"HandleShouldAddStorePayment"
#define kMessageSucceededProductsRequest @"HandleSucceededProductsRequest"
#define kMessageFailedProductsRequest @"HandleFailedProductsRequest"
#define kMessageUpdatedTransaction @"HandleUpdatedTransaction"
#define kMessageRemovedTransaction @"HandleRemovedTransaction"
#define kMessageSucceededResumeCompletedTransaction @"HandleSucceededResumeCompletedTransaction"
#define kMessageSucceededRestoreCompletedTransaction @"HandleSucceededRestoreCompletedTransaction"
#define kMessageFailedRestoreCompletedTransaction @"HandleFailedRestoreCompletedTransaction"
#define kMessageFailedToMakePayment @"HandleFailedToMakePayment"
#define kMessageSucceededRefreshReceipt @"HandleSucceededRefreshReceipt"
#define kMessageFailedRefreshReceipt @"HandleFailedRefreshReceipt"
NSString* const KJPBillingErrorDomain = @"KJPBillingErrorDomain";

@interface KJBillingService () <SKPaymentTransactionObserver, SKRequestDelegate, SKProductsRequestDelegate>
@property (nonatomic, readwrite) BOOL isDebug;
@property (nonatomic, readwrite) SKPayment* savedIAPPayment;
@property (nonatomic) NSMutableArray* restoreTransactions;
@end

@implementation KJBillingService {
    KJBillingProduct* _billingProduct;
}

+ (KJBillingService*)sharedInstance {
    static dispatch_once_t once;
    static KJBillingService* sharedInstance;

    dispatch_once(&once, ^{
        sharedInstance = [[super allocWithZone:nil] init];
    });

    return sharedInstance;
}

+ (id)allocWithZone:(NSZone*)zone {
    return [self sharedInstance];
}

- (id)init {
    if (self == [super init]) {
        [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
        self.isDebug = NO;
    }
    return self;
}

- (void)dealloc {
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
}

- (void)enableDebugging {
    self.isDebug = YES;
}

- (void)debugLog:(NSString*)message {
    if (self.isDebug) {
        NSLog(@"%@", message);
    }
}

#pragma mark -
#pragma mark Billing process

- (BOOL)canMakePayments {
    return [SKPaymentQueue canMakePayments];
}

- (void)startProductsRequest:(NSArray*)productIds {
    if ([SKPaymentQueue canMakePayments]) {
        // Create a product request object and initialize it with our product identifiers
        SKProductsRequest* request =
            [[SKProductsRequest alloc] initWithProductIdentifiers:[NSSet setWithArray:productIds]];
        request.delegate = self;

        // Send the request to the App Store
        [request start];
    }
}

// そのアイテムのSKPaymentがIAPプロモによって既に作られていた場合は、
// 既にあるIAPプロモのSKPaymentで購入を開始する
// それ以外は、今まで通り通常購入を開始する
- (void)startPayment:(NSString*)identifier {
    _billingProduct = [KJBillingProduct new];
    
    if(self.savedIAPPayment != nil && [self.savedIAPPayment.productIdentifier isEqualToString:identifier]){
        [_billingProduct addPaymentQueue:self.savedIAPPayment];
        self.savedIAPPayment = nil;
    }
    else{
        [_billingProduct startPayment:identifier];
    }
}

- (void)resumeCompletedTransactions {
    NSMutableArray* transactions = [NSMutableArray array];
    for (SKPaymentTransaction* t in [SKPaymentQueue defaultQueue].transactions) {
        if(t.transactionState == SKPaymentTransactionStatePurchased) {
            [transactions addObject:[self dictionaryByTransaction:t]];
        }
    }
    NSDictionary* dic = @{@"transactions": transactions};
    [self sendMessage:kMessageSucceededResumeCompletedTransaction andJSONParameters:dic];
}

- (void)restoreCompletedTransactions {
    if(self.restoreTransactions == nil) {
        self.restoreTransactions = @[].mutableCopy;
    }
    [self.restoreTransactions removeAllObjects];
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

- (void)finishTransaction:(NSString*)identifier {
    SKPaymentTransaction* transaction = [self transactionMatchingIdentifier:identifier];
    if (transaction != nil) {
        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    }
}

- (void)startDownloads:(SKPaymentTransaction*)transaction {
    [[SKPaymentQueue defaultQueue] startDownloads:transaction.downloads];
}

- (void)refreshReceipt {
    SKReceiptRefreshRequest* receiptRequest = [[SKReceiptRefreshRequest alloc] init];
    receiptRequest.delegate = self;
    [receiptRequest start];
}

#pragma mark -
#pragma mark Handle billing event

- (BOOL)paymentQueue:(SKPaymentQueue *)queue shouldAddStorePayment:(SKPayment *)payment forProduct:(SKProduct *)product{
    self.savedIAPPayment = payment;
    NSNumberFormatter* numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [numberFormatter setLocale:product.priceLocale];
    
    NSDictionary* productDic = @{
                                 @"localizedDescription" : product.localizedDescription,
                                 @"localizedTitle" : product.localizedTitle,
                                 @"price" : product.price,
                                 @"formattedPrice" : [numberFormatter stringFromNumber:product.price],
                                 @"productIdentifier" : product.productIdentifier
                                 };
    
    [self sendMessage:kMessageShouldAddStorePayment andJSONParameters:productDic];
    
    return NO;
}

- (void)handleSucceededProductsRequest:(NSArray*)products invalidProductIdentifiers:(NSArray*)identifiers {
    NSMutableArray* productArr = [@[] mutableCopy];
    NSMutableArray* invalidProductIdArr = [@[] mutableCopy];

    for (SKProduct* product in products) {
        NSNumberFormatter* numberFormatter = [[NSNumberFormatter alloc] init];
        [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
        [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
        [numberFormatter setLocale:product.priceLocale];

        NSDictionary* productDic = @{
            @"localizedDescription" : product.localizedDescription,
            @"localizedTitle" : product.localizedTitle,
            @"price" : product.price,
            @"formattedPrice" : [numberFormatter stringFromNumber:product.price],
            @"currencyCode" : [product.priceLocale objectForKey:NSLocaleCurrencyCode],
            @"currencySymbol" : [product.priceLocale objectForKey:NSLocaleCurrencySymbol],
            @"countryCode" : [product.priceLocale objectForKey:NSLocaleCountryCode],
            @"productIdentifier" : product.productIdentifier
        };
        [productArr addObject:productDic];
    }

    for (NSString* identifier in identifiers) {
        [invalidProductIdArr addObject:identifier];
    }

    NSDictionary* dic = @{
        @"products" : [[NSArray alloc] initWithArray:productArr],
        @"invalidProductIdentifiers" : [[NSArray alloc] initWithArray:invalidProductIdArr]
    };

    [self sendMessage:kMessageSucceededProductsRequest andJSONParameters:dic];
}

- (void)handleFailedProductsRequest:(NSError*)error {
    [self sendMessage:kMessageFailedProductsRequest andErrorParameters:error];
}

- (void)handleUpdatedTransaction:(SKPaymentTransaction*)transaction {
    [self sendMessage:kMessageUpdatedTransaction andTransactionParameters:transaction];
}

- (void)handleUpdatedDownload:(SKDownload*)download {
    // not implemented yet
}

- (void)handleRemovedTransaction:(SKPaymentTransaction*)transaction {
    [self sendMessage:kMessageRemovedTransaction andTransactionParameters:transaction];
}

- (void)handleFailedRestoreCompletedTransaction:(NSError*)error {
    [self sendMessage:kMessageFailedRestoreCompletedTransaction andErrorParameters:error];
}

- (void)handleFinishedRestoreCompletedTransactions {
    NSDictionary* dic = @{@"transactions": self.restoreTransactions};
    [self sendMessage:kMessageSucceededRestoreCompletedTransaction andJSONParameters:dic];
}

- (void)didFailedToMakePayment:(NSError*)error {
    [self sendMessage:kMessageFailedToMakePayment andErrorParameters:error];
}

- (void)handleFinishedRefreshReceipt {
    NSURL* receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
    NSData* receiptData = [NSData dataWithContentsOfURL:receiptURL];

    NSString* base64EncodedReceipt = @"";
    if (receiptData != NULL) {
        base64EncodedReceipt = [receiptData base64EncodedStringWithOptions:0];
    }
    [self sendMessage:kMessageSucceededRefreshReceipt andParameters:base64EncodedReceipt];
}

- (void)handleFailedRefreshReceipt:(NSError*)error {
    [self sendMessage:kMessageFailedRefreshReceipt andErrorParameters:error];
}

#pragma mark -
#pragma mark Helper methods

// Return the transaction matching a given identifier
- (SKPaymentTransaction*)transactionMatchingIdentifier:(NSString*)identifier {
    NSArray* transactions = [SKPaymentQueue defaultQueue].transactions;
    for (SKPaymentTransaction* transaction in transactions) {
        if ([identifier isEqualToString:transaction.transactionIdentifier]) {
            return transaction;
        }
    }
    return nil;
}

- (void)sendMessage:(NSString*)message andErrorParameters:(NSError*)error {
    NSDictionary* dic = @{
        @"code" : [NSNumber numberWithInteger:error.code],
        @"domain" : error.domain,
        @"description" : error.description
    };
    [self sendMessage:message andJSONParameters:dic];
}

- (void)sendMessage:(NSString*)message andTransactionParameters:(SKPaymentTransaction*)transaction {
    NSDictionary* dic = [self dictionaryByTransaction:transaction];
    [self sendMessage:message andJSONParameters:dic];
}

- (void)sendMessage:(NSString*)message andJSONParameters:(NSDictionary*)parameters {
    NSError* error = nil;
    NSData* data = nil;
    if ([NSJSONSerialization isValidJSONObject:parameters]) {
        data = [NSJSONSerialization dataWithJSONObject:parameters options:NSJSONWritingPrettyPrinted error:&error];
        [self sendMessage:message andParameters:[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding]];
    } else {
        [self debugLog:[NSString stringWithFormat:@"%@: invalid JSON Object", message]];
    }
    if (error != nil) {
        [self debugLog:[NSString stringWithFormat:@"handleReceivedProducts: error: %@", [error localizedDescription]]];
    }
}

- (void)sendMessage:(NSString*)message andParameters:(NSString*)parameters {
    [self debugLog:[NSString stringWithFormat:@"sendMessage: %@", parameters]];
    UnitySendMessage([kMessageObject UTF8String], [message UTF8String], [parameters UTF8String]);
}

- (NSDictionary*)dictionaryByTransaction:(SKPaymentTransaction*)transaction {
    NSMutableDictionary* dic = @{
        @"productIdentifier" : transaction.payment.productIdentifier,
        @"quantity" : [NSNumber numberWithInteger:transaction.payment.quantity],
        @"transactionState" : [NSNumber numberWithInt:transaction.transactionState]
    }.mutableCopy;
    if (KJPOsVersion() >= 7.0) {
        if (transaction.payment.applicationUsername != NULL) {
            dic[@"applicationUsername"] = transaction.payment.applicationUsername;
        }
    }
    NSURL* receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
    NSData* receiptData = [NSData dataWithContentsOfURL:receiptURL];
    if (receiptData != NULL) {
        dic[@"base64EncodedReceipt"] = [receiptData base64EncodedStringWithOptions:0];
    } else {
        [self debugLog:@"dictionaryByTransaction: receiptData is null"];
    }
    if (transaction.transactionDate != NULL) {
        dic[@"transactionDate"] = [NSNumber numberWithDouble:[transaction.transactionDate timeIntervalSince1970]];
    }
    if (transaction.transactionIdentifier != NULL) {
        dic[@"transactionIdentifier"] = transaction.transactionIdentifier;
    }
    if (transaction.originalTransaction != NULL && transaction.originalTransaction.transactionIdentifier != NULL) {
        dic[@"originalTransactionIdentifier"] = transaction.originalTransaction.transactionIdentifier;
    }
    if (transaction.error != NULL) {
        dic[@"errorCode"] = [NSNumber numberWithInteger:transaction.error.code];
        dic[@"errorDomain"] = transaction.error.domain;
        dic[@"errorDescription"] = transaction.error.description;
    }
    return dic;
}

- (NSString*)jsonStringByPendingTransactions {
    NSMutableArray* transactionArray = [NSMutableArray array];
    NSArray* transactions = [SKPaymentQueue defaultQueue].transactions;
    for (SKPaymentTransaction* transaction in transactions) {
        NSDictionary* dic = [self dictionaryByTransaction:transaction];
        [transactionArray addObject:dic];
    }
    if (![NSJSONSerialization isValidJSONObject:transactionArray]) {
        [self debugLog:[NSString stringWithFormat:@"jsonStringByPendingTransactions"
                                                  @": invalid JSON Object"]];
        return @"";
    }
    NSError* error = nil;
    NSData* data =
        [NSJSONSerialization dataWithJSONObject:transactionArray options:NSJSONWritingPrettyPrinted error:&error];
    if (error != nil) {
        [self debugLog:[NSString stringWithFormat:@"NSJSONSerialization dataWithJSONObject: error: %@",
                                                  [error localizedDescription]]];
        return @"";
    }
    if (data == nil) {
        return @"";
    }
    return [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
}

- (NSString*)getSavedIAPProductId {
    if([self.savedIAPPayment.productIdentifier length] != 0){
        return self.savedIAPPayment.productIdentifier;
    }
    else {
        return @"";
    }
}

#pragma mark -
#pragma mark - SKProductsRequestDelegate methods

// Used to get the App Store's response to your request and notifies your observer
- (void)productsRequest:(SKProductsRequest*)request didReceiveResponse:(SKProductsResponse*)response {
    [self handleSucceededProductsRequest:response.products
               invalidProductIdentifiers:response.invalidProductIdentifiers];
}

#pragma mark -
#pragma mark SKRequestDelegate methods
- (void)requestDidFinish:(SKRequest*)request {
    if ([request isKindOfClass:[SKReceiptRefreshRequest class]]) {
        [self handleFinishedRefreshReceipt];
    }
}

// Called when the product request failed.
- (void)request:(SKRequest*)request didFailWithError:(NSError*)error {
    if ([request isKindOfClass:[SKReceiptRefreshRequest class]]) {
        [self handleFailedRefreshReceipt:error];
    } else {
        [self handleFailedProductsRequest:error];
    }
}

#pragma mark -
#pragma mark SKPaymentTransactionObserver methods

// Called when there are trasactions in the payment queue
- (void)paymentQueue:(SKPaymentQueue*)queue updatedTransactions:(NSArray*)transactions {
    for (SKPaymentTransaction* transaction in transactions) {
        switch (transaction.transactionState) {
            case SKPaymentTransactionStateFailed:
                [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                [self handleUpdatedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                if(self.restoreTransactions == nil) {
                    [self debugLog:[NSString stringWithFormat:@"restore transaction finished. transactionId:%@", transaction.transactionIdentifier]];
                    // 起動時であれば即時finish
                    // Restore中にクラッシュなどで残ったゾンビトランザクションを消す
                    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                } else {
                    [self debugLog:[NSString stringWithFormat:@"add restore transaction. transactionId:%@", transaction.transactionIdentifier]];
                    NSDictionary *dict = [self dictionaryByTransaction:transaction];
                    NSUInteger index = NSNotFound;
                    if (self.restoreTransactions.count > 0) {
                        index = [[self.restoreTransactions valueForKey:@"transactionIdentifier"] indexOfObject:transaction.transactionIdentifier];
                    }
                    if (index != NSNotFound) {
                        // 同じproductIdentifierの場合は置き換える
                        [self.restoreTransactions replaceObjectAtIndex:index withObject:dict];
                    } else {
                        [self.restoreTransactions addObject:dict];
                    }
                }
                break;
            default:
                [self handleUpdatedTransaction:transaction];
                break;
        }
    }
}

// Called when the payment queue has downloaded content
- (void)paymentQueue:(SKPaymentQueue*)queue updatedDownloads:(NSArray*)downloads {
    for (SKDownload* download in downloads) {
        [self handleUpdatedDownload:download];
    }
}

// Logs all transactions that have been removed from the payment queue
- (void)paymentQueue:(SKPaymentQueue*)queue removedTransactions:(NSArray*)transactions {
    for (SKPaymentTransaction* transaction in transactions) {
        [self handleRemovedTransaction:transaction];
    }
}

// Called when an error occur while restoring purchases. Notify the user about the error.
- (void)paymentQueue:(SKPaymentQueue*)queue restoreCompletedTransactionsFailedWithError:(NSError*)error {
    [self handleFailedRestoreCompletedTransaction:error];
}

// Called when all restorable transactions have been processed by the payment queue
- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue*)queue {
    [self handleFinishedRestoreCompletedTransactions];
}

@end


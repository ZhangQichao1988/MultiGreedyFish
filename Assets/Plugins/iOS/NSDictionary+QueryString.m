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
//  NSDictionary+QueryString.m
//  KJPUnityPlugins
//

#if !__has_feature(objc_arc)
#warning This file must be compiled with ARC. Use -fobjc-arc flag (or convert project to ARC).
#endif

#import "NSDictionary+QueryString.h"
#import "NSString+URIEncoding.h"

//
// key[]=valueなformatまでしか対応はしてないのでRailsみたいにkey[hoge]=valueとかしたい場合は修正する必要があります
//

@implementation NSDictionary (QueryString)

///////////////////////////////////////
#pragma mark NSDictionary+QueryString
///////////////////////////////////////

+ (instancetype)dictionaryWithQueryString:(NSString*)queryString
{
    if ([queryString length] <= 0) {
        return @{};
    }
    NSArray* pairs = [queryString componentsSeparatedByString:@"&"];
    if ([pairs count] <= 0) {
        return @{};
    }
    NSMutableDictionary* results = [NSMutableDictionary dictionary];
    for (NSString* pair in pairs) {
        NSRange range = [pair rangeOfString:@"="];
        if (range.location == NSNotFound) {
            continue;
        }
        NSString* escapedKey = [pair substringToIndex:(range.location + 0)];
        NSString* escapedValue = [pair substringFromIndex:(range.location + 1)];
        if ([escapedKey length] <= 0) {
            continue;
        }
        NSString* key = [escapedKey stringByDecodingURIComponent];
        NSString* value = [escapedValue stringByDecodingURIComponent];
        [self normalizeParameters:results withValue:value forKey:key];
    }
    return [NSDictionary dictionaryWithDictionary:results];
}

- (NSString*)queryString
{
    return [self queryStringWithPrefix:NO];
}

- (NSString*)queryStringWithPrefix:(BOOL)prefix
{
    NSArray* keys = [self allKeys];
    if ([keys count] == 0) {
        return @"";
    }
    NSMutableString* result = [NSMutableString stringWithString:@""];
    for (NSString* k in keys) {
        // verify key
        if (k == nil) {
            continue;
        }
        // enumerations k cannot modify, so set local variables
        NSString* key = k;

        // verify value
        id value = [self objectForKey:key];
        if (!value) {
            value = @"";
        }

        // append key list-value
        if ([value isKindOfClass:[NSArray class]]) {
            if (![k hasSuffix:@"[]"]) {
                key = [NSString stringWithFormat:@"%@%@", key, @"[]"];
            }
            for (NSString* v in value) {
                [self appendQuerySeparatorToString:result prefix:prefix];
                [self appendQueryValue:v forKey:key toString:result];
            }
            continue;
        }

        // append key value
        [self appendQuerySeparatorToString:result prefix:prefix];
        [self appendQueryValue:value forKey:key toString:result];
    }
    return result;
}

//////////////////////////////
#pragma mark Private Methods
//////////////////////////////

+ (void)normalizeParameters:(NSMutableDictionary*)parameters withValue:(NSString*)value forKey:(NSString*)key
{
    value = [value length] > 0 ? value : @"";
    if (![key hasSuffix:@"[]"]) {
        [parameters setObject:value forKey:key];
        return;
    }
    id currentValue = [parameters objectForKey:key];
    if (!currentValue) {
        NSMutableArray* newArray = [NSMutableArray array];
        [newArray addObject:value];
        [parameters setObject:value forKey:key];
        return;
    }
    if (![currentValue isKindOfClass:[NSMutableArray class]]) {
        [parameters removeObjectForKey:key];
        NSMutableArray* newArray = [NSMutableArray array];
        [newArray addObject:currentValue];
        [newArray addObject:value];
        [parameters setObject:newArray forKey:key];
        return;
    }
    [currentValue addObject:value];
}

- (void)appendQuerySeparatorToString:(NSMutableString*)string prefix:(BOOL)prefix
{
    // append separator
    if ([string length] <= 0) {
        if (prefix) {
            [string appendString:@"?"];
        }
        return;
    }
    [string appendFormat:@"&"];
}

- (void)appendQueryValue:(NSString*)value forKey:(NSString*)key toString:(NSMutableString*)string
{
    [string appendFormat:@"%@=%@", [key stringByEncodingURIComponent:@"[]"], [value stringByEncodingURIComponent]];
}

@end

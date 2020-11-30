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
//  NSString+URIEncoding.m
//  KJPUnityPlugins
//

#if !__has_feature(objc_arc)
#warning This file must be compiled with ARC. Use -fobjc-arc flag (or convert project to ARC).
#endif

#import "NSString+URIEncoding.h"

@implementation NSString (URIEncoding)

- (NSString*)stringByEncodingURIComponent
{
    return [self stringByEncodingURIComponent:nil];
}

- (NSString*)stringByEncodingURIComponent:(NSString*)unescapedString
{
    NSMutableCharacterSet *allowdCharSet = [NSMutableCharacterSet alphanumericCharacterSet];
    [allowdCharSet addCharactersInString:@"-._~"];
    [allowdCharSet addCharactersInString:unescapedString];
    return [self stringByAddingPercentEncodingWithAllowedCharacters:allowdCharSet];
}

- (NSString*)stringByDecodingURIComponent
{
    CFStringRef cSelf = (__bridge CFStringRef) self;
    CFStringRef cResult = CFURLCreateStringByReplacingPercentEscapes(NULL, cSelf, CFSTR(""));
    NSString* result = [NSString stringWithString:(__bridge NSString*) cResult];
    CFRelease(cResult);
    return result;
}

@end

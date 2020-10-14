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
//  NSString+Base64Encode.m
//  KJPUnityPlugins
//

#import "NSString+Base64Encode.h"

@implementation NSString (Base64Encode)

+ (NSString*)stringEncodedWithBase64:(NSString*)str
{
    static const char* tbl = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    const char* s = [str UTF8String];
    int length = (int)[str length];
    char* tmp = malloc(length * 4 / 3 + 4);

    int i = 0;
    int n = 0;
    char* p = tmp;

    while (i < length) {
        n = s[i++];
        n *= 256;
        if (i < length) n += s[i];
        i++;
        n *= 256;
        if (i < length) n += s[i];
        i++;
        p[0] = tbl[((n & 0x00fc0000) >> 18)];
        p[1] = tbl[((n & 0x0003f000) >> 12)];
        p[2] = tbl[((n & 0x00000fc0) >> 6)];
        p[3] = tbl[((n & 0x0000003f) >> 0)];

        if (i > length) p[3] = '=';
        if (i > length + 1) p[2] = '=';

        p += 4;
    }

    *p = '\0';

    NSString* ret = [NSString stringWithCString:tmp encoding:NSUTF8StringEncoding];

    free(tmp);

    return ret;
}

@end

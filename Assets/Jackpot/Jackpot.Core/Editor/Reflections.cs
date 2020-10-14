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
using System.Reflection;

namespace Jackpot
{
    public class Reflections
    {
#region Field

        public static FieldInfo StaticField(Type type, string name)
        {
            return type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
        }

        public static FieldInfo InstanceField(object target, string name)
        {
            return target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static FieldInfo BaseInstanceField(object target, string name)
        {
            return target.GetType().BaseType.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

#endregion

#region Generics Arguments

        public static Type[] GenericsArguments(object target)
        {
            return target.GetType().GetGenericArguments();
        }

        public static Type[] BaseGenericsArguments(object target)
        {
            return target.GetType().BaseType.GetGenericArguments();
        }

#endregion

    }
}

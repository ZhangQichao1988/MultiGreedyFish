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
using UnityEditor.iOS.Xcode;

namespace XcodeTools
{
    public class PBXBuildFileDataExt
    {
        /// <summary>
        /// PBXBuildFileDataを更新する
        /// </summary>
        /// <param name="pbxBuildFileDataObj">PBXBuildFileData(Internal)のインスタンス</param>
        /// <param name="weak"></param>
        /// <param name="compileFlags"></param>
        public static void Update(object pbxBuildFileDataObj, bool weak, string compileFlags)
        {
            var pbxBuildFileDataRfl = new ReflectionClass(pbxBuildFileDataObj, typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXBuildFileData");
            pbxBuildFileDataRfl.SetField("weak", weak);
            pbxBuildFileDataRfl.SetField("compileFlags", compileFlags);
        }
    }

    public class PBXVariantGroupDataExt
    {
        /// <summary>
        /// PBXVariantGroupDataを作成する
        /// </summary>
        /// <param name="name"></param>
        /// <returns>PBXVariantGroupData(internal)のobject</returns>
        public static object CreateVariantGroup(string name)
        {
            if (name.Contains("/"))
                throw new Exception("Group name must not contain '/'");

            // new PBXVariantGroupData()
            var pbxVariantGroupDataRfl = new ReflectionClass(typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXVariantGroupData", new Type[] { }, new object[] { });

            // PBXGUID.Generate()
            var pbxGuid = ReflectionClass.ExecuteStaticMethod<string>(typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXGUID", "Generate", new object[] { });

            pbxVariantGroupDataRfl.SetField("guid", pbxGuid);
            pbxVariantGroupDataRfl.ExecuteMethod("SetPropertyString", new object[] { "isa", "PBXVariantGroup" });
            pbxVariantGroupDataRfl.SetField("name", name);
            pbxVariantGroupDataRfl.SetField("tree", PBXSourceTree.Group);

            // new GUIDList()
            var guidListRfl = new ReflectionClass(typeof(PBXProject), ReflectionNameSpaces.Pbx, "GUIDList", new Type[] { }, new object[] { });

            pbxVariantGroupDataRfl.SetField("children", guidListRfl.Instance);

            return pbxVariantGroupDataRfl.Instance;
        }
    }
}

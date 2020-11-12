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
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.iOS.Xcode;

namespace XcodeTools
{
    /// <summary>
    /// Project Capability Manager Extention Methods
    /// </summary>
    public static class ProjectCapabilityManagerExtensions
    {
        static ProjectCapabilityManagerExtensions()
        {
        }

        #region public methods

        /// <summary>
        /// pbxProject内のSystemCapabilityにある指定capabilityのenableを設定した上で
        /// Info.Plistに記載されている必要なFrameWorkを削除します
        /// 設定されればtrue、設定する値が無い場合はfalseが返ってきます
        /// </summary>
        /// <param name="self">self</param>
        /// <param name="pbxProject">pbxProject</param>
        /// <param name="projectPath">projectPath</param>
        /// <param name="capability">対象のcapability名</param>
        /// <param name="targetGuid">targetGuid</param>
        /// <param name="isEnable">isEnable</param>
        /// <param name="infoPlistKey">infoPlistKey</param>
        /// <param name="infoPlistValue">infoPlistValue</param>
        /// <returns>成否</returns>
        public static bool SetSystemCapabilityActive(
            this ProjectCapabilityManager self, 
            PBXProject pbxProject, 
            string projectPath,
            string capability,
            string targetGuid,
            bool isEnable,
            string infoPlistKey,
            string infoPlistValue)
        {
            var result = self.SetSystemCapabilityActive(pbxProject, capability, targetGuid, isEnable);
            
            if(isEnable == false)
            {
                result = self.RemoveInfoPlistValue(projectPath, infoPlistKey, infoPlistValue);
            }
            
            return result;
        }
        
        /// <summary>
        /// pbxProject内のSystemCapabilityにある指定capabilityのenableを設定します
        /// 設定されればtrue、設定する値が無い場合はfalseが返ってきます
        /// </summary>
        /// <param name="self">self</param>
        /// <param name="pbxProject">pbxProject</param>        
        /// <param name="capability">対象のcapability名</param>
        /// <param name="targetGuid">targetGuid</param>
        /// <param name="isEnable">isEnable</param>
        /// <returns>成否</returns>
        public static bool SetSystemCapabilityActive(
            this ProjectCapabilityManager self, 
            PBXProject pbxProject, 
            string capability,
            string targetGuid,
            bool isEnable)
        {
            var privateBinding = BindingFlags.Instance | BindingFlags.NonPublic;
            var publicBinding = BindingFlags.Instance | BindingFlags.Public;
            var pBXProjectType = typeof(PBXProject);
            
            // アセンブリを読み込む
            var pBXElementDictType = Assembly.Load("UnityEditor.iOS.Extensions.Xcode").GetTypes().First(t => t.Name == "PBXElementDict");

            // PBXProject.m_Data(PBXProjectData)を取得
            var m_DataFieldInfo = pBXProjectType.GetField("m_Data", privateBinding);
            var m_Data = m_DataFieldInfo.GetValue(pbxProject);

            // 上記で取得したm_Data変数である(PBXProjectData.projectPBXProjectSection)を取得
            var projectFieldInfo = m_Data.GetType().GetField("project", publicBinding);
            var project = projectFieldInfo.GetValue(m_Data);

            // 上記で取得したprojectプロパティ(PBXProjectSection.projectPBXProjectObjectData)を取得
            var projectProjectPropertyInfo = project.GetType().GetProperty("project", publicBinding);
            var projectProject = projectProjectPropertyInfo.GetValue(project, null);

            // 上記で取得したprojectプロパティ(PBXProjectObjectData.m_PropertiesPBXElementDict)を取得
            var m_PropertiesFieldInfo = projectProject.GetType().GetField("m_Properties", privateBinding);
            var m_Properties = m_PropertiesFieldInfo.GetValue(projectProject);

            // 上記で取得したm_Propertiesプロパティ(IDictionary<string, PBXElement>)を取得
            var valuesPropertyInfo = m_Properties.GetType().GetProperty("values", publicBinding);

            // 以下、XCodeProject内のpbxproj内の Begin PBXProject sectionにあるCapability部分を書き換えにいく処理
            // 階層になっているattributes > TargetAttributes > TargetGUID > SystemCapabilities > 指定したCapability を探しに行く
            
            var m_PropertiesValues = valuesPropertyInfo.GetValue(m_Properties, null) as IDictionary;
            if(!m_PropertiesValues.Contains("attributes"))
            {
                UnityEngine.Debug.Log("attributes Not Found");
                return false;
            }
            
            var attributes = valuesPropertyInfo.GetValue(m_PropertiesValues["attributes"], null) as IDictionary;
            if (attributes == null || !attributes.Contains("TargetAttributes"))
            {
                UnityEngine.Debug.Log("TargetAttributes Not Found");
                return false;
            }
            var targetAttributesAll = valuesPropertyInfo.GetValue(attributes["TargetAttributes"], null) as IDictionary;
            if (targetAttributesAll == null || !targetAttributesAll.Contains(targetGuid))
            {
                UnityEngine.Debug.Log("TargetAttribute in targetGuid Not Found targetGuid" + targetGuid);
                return false;
            }
            var targetGuidAttributes = valuesPropertyInfo.GetValue(targetAttributesAll[targetGuid], null) as IDictionary;
            if (targetGuidAttributes == null || !targetGuidAttributes.Contains("SystemCapabilities"))
            {
                UnityEngine.Debug.Log("SystemCapabilities Not Found");
                return false;
            }
            var systemCapabilities = valuesPropertyInfo.GetValue(targetGuidAttributes["SystemCapabilities"], null) as IDictionary;
            if (systemCapabilities == null)
            {
                UnityEngine.Debug.Log("Target Capability Not Found");
                return false;
            }
            
            // SystemCapabilities内に指定Capabilityが存在した場合、新しい値に上書きする
            var valueDict = pBXElementDictType.GetConstructor(Type.EmptyTypes).Invoke(null);
            pBXElementDictType.GetMethod("SetString", new Type[] { typeof(String), typeof(String) }).
                Invoke(valueDict, new object[] { "enabled", isEnable ? "1" : "0" });
            systemCapabilities[capability] = valueDict;
            return true;
        }
        
        #endregion

        static bool RemoveInfoPlistValue(this ProjectCapabilityManager self, string projectPath, string infoPlistKey, string infoPlistValue)
        {
            var plistPath = Path.Combine(projectPath, "Info.plist");
            var plist = new InfoPlist(plistPath);
            var result = plist.RemoveStringArrayValue(infoPlistKey, infoPlistValue);
            plist.WriteToFile();
            return result;
        }

    }
}

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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEditor.iOS.Xcode;

namespace XcodeTools
{
    /// <summary>
    /// PBX project Extention Methods
    /// </summary>
    public static class PBXProjectExtension
    {
        #region constant

        const string LocalizableStrings = "Localizable.strings";
        const string SupportingFiles = "Supporting Files";
        const string InfoPlistStrings = "InfoPlist.strings";

        #endregion

        static PBXProjectExtension()
        {
        }

        #region public methods

        /// <summary>
        /// Determines whether this instance has library the specified library.
        /// </summary>
        /// <returns><c>true</c> if this instance has library the specified library; otherwise, <c>false</c>.</returns>
        /// <param name="library">Library.</param>
        public static bool HasLibrary(this PBXProject self, string library)
        {
            return self.ContainsFileByRealPath("usr/lib/" + library);
        }

        /// <summary>
        /// Adds the library to project.
        /// </summary>
        /// <param name="targetGuid">Target GUID.</param>
        /// <param name="library">Library.</param>
        /// <param name="weak">If set to <c>true</c> weak.</param>
        public static void AddLibraryToProject(this PBXProject self, string targetGuid, string library, bool weak)
        {
            var fileGuid = self.AddFile("usr/lib/" + library, "Libraries/" + library, PBXSourceTree.Absolute);
            self.AddFileToBuild(targetGuid, fileGuid);
        }

        /// <summary>
        /// Removes the library from project.
        /// </summary>
        /// <param name="targetGuid">Target GUID.</param>
        /// <param name="library">Library.</param>
        public static void RemoveLibraryFromProject(this PBXProject self, string targetGuid, string library)
        {
            var fileGuid = self.FindFileGuidByRealPath("usr/lib/" + library);
            if (fileGuid != null)
            {
                self.RemoveFile(fileGuid);
            }
        }

        /// <summary>
        /// Adds the file to project.
        /// </summary>
        /// <param name="targetGuid">Target GUID.</param>
        /// <param name="absolutePath">Absolute path.</param>
        /// <param name="sourceGroup">Source group.</param>
        /// <param name="compileFlags">Compile flags.</param>
        public static void AddFileToProject(
            this PBXProject self,
            string targetGuid,
            string absolutePath,
            string sourceGroup,
            bool weak = false,
            string compileFlags = null)
        {
            var fileInfo = new FileInfo(absolutePath);
            if (fileInfo.Name == LocalizableStrings)
            {
                self.AddLocalizableStringsFile(targetGuid, absolutePath);
                return;
            }

            var projectPath = Path.Combine(sourceGroup, Path.GetFileName(absolutePath));
            var fileGuid = self.AddFile(absolutePath, projectPath, PBXSourceTree.Absolute);

            self.AddFileToBuildWithFlags(targetGuid, fileGuid, compileFlags);
        }

        public static void ModifyFileToProject(
            this PBXProject self,
            string targetGuid,
            string absolutePath,
            string sourceGroup,
            bool weak = false,
            string compileFlags = null)
        {
            var isUpdate = false;
            var path = "";
            var projectPath = sourceGroup + "/" + Path.GetFileName(absolutePath);

            // update for group
            if (self.ContainsFileByProjectPath(projectPath))
            {
                isUpdate = true;
                path = projectPath;
            }

            // update for absolute
            if (self.ContainsFileByRealPath(absolutePath))
            {
                isUpdate = true;
                path = absolutePath;
            }

            if (isUpdate)
            {
                var fileGuid = self.FindFileGuidByProjectPath(path);

                // PbxProject.BuildFilesGetForSourceFile()
                var pbxProjectRfl = GetPbxProjectReflectionClass(self);
                var buildFileData = pbxProjectRfl.ExecuteMethod<object>("BuildFilesGetForSourceFile", new object[] { targetGuid, fileGuid });

                PBXBuildFileDataExt.Update(buildFileData, weak, compileFlags);
                return;
            }

            // add new
            self.AddFileToProject(targetGuid, absolutePath, sourceGroup, weak, compileFlags);
        }

        /// <summary>
        /// Adds the localizable strings file.
        /// </summary>
        /// <param name="targetGuid">Target GUID.</param>
        /// <param name="absolutePath">Absolute path.</param>
        public static void AddLocalizableStringsFile(this PBXProject self, string targetGuid, string absolutePath)
        {
            var path = ReflectionClass.ExecuteStaticMethod<string>(typeof(PBXProject), ReflectionNameSpaces.Xcode, "PBXPath", "FixSlashes", new object[] { absolutePath });
            var fileGuid = self.FindFileGuidByRealPath(path);
            if (fileGuid != null)
            {
                return;
            }

            var lprojRegex = new Regex(@"([a-z]{2}).lproj/" + LocalizableStrings + @"$");
            var lprojMatch = lprojRegex.Match(path);
            var langCode = lprojMatch.Groups[1].ToString();

            // PBXProjectData.fileRefs
            var pbxProjectDataRfl = GetPbxProjectDataReflectionClass(self);
            var fileRefSection = pbxProjectDataRfl.GetField<object>("fileRefs");
            var fileRefSectionRfl = new ReflectionClass(fileRefSection);

            // PBXFileReferenceData.CreateFromFile()
            var fileRefData = ReflectionClass.ExecuteStaticMethod<object>(typeof(PBXProject),
                ReflectionNameSpaces.Pbx, "PBXFileReferenceData", "CreateFromFile", new object[] { path, langCode, PBXSourceTree.Group });

            // KnownSectionBase<PBXFileReferenceData>.AddEntry()
            fileRefSectionRfl.ExecuteMethod("AddEntry", new object[] { fileRefData });

            // PBXFileReferenceData:PBXObjectData.guid
            var fileRefDataRfl = new ReflectionClass(fileRefData);
            var fileRefDataGuid = fileRefDataRfl.GetField<string>("guid");

            // PBXVariantGroupData:PBXGroupData.children
            var localizableGroup = self.CreateLocalizableGroup(targetGuid);
            var localizableGroupRfl = new ReflectionClass(localizableGroup, typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXVariantGroupData");
            var children = localizableGroupRfl.GetField<object>("children");
            var childrenRfl = new ReflectionClass(children, typeof(PBXProject), ReflectionNameSpaces.Pbx, "GUIDList");

            childrenRfl.ExecuteMethod("AddGUID", new object[] { fileRefDataGuid });
        }

        /// <summary>
        /// Adds the folder to project.
        /// </summary>
        /// <param name="targetGuid">Target GUID.</param>
        /// <param name="folderPath">Folder path.</param>
        /// <param name="sourceGroup">Source group.</param>
        /// <param name="exclude">Exclude.</param>
        /// <param name="recursive">If set to <c>true</c> recursive.</param>
        public static void AddFolderToProject(
            this PBXProject self,
            string targetGuid,
            string folderPath,
            string sourceGroup,
            string[] exclude = null,
            bool recursive = true)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            var sourceDirectoryInfo = new DirectoryInfo(folderPath);
            var parentGroup = Path.Combine(sourceGroup, sourceDirectoryInfo.Name);

            var regexExclude = string.Empty;
            if (exclude != null)
            {
                regexExclude = string.Format(@"{0}", string.Join("|", exclude));
            }


            foreach (string directory in Directory.GetDirectories(folderPath))
            {
                if (!string.IsNullOrEmpty(regexExclude) && Regex.IsMatch(directory, regexExclude))
                {
                    continue;
                }

                if (directory.EndsWith(".bundle"))
                {
                    // Treath it like a file and copy even if not recursive
                    self.AddFileToProject(targetGuid, directory, parentGroup);
                    continue;
                }

                if (recursive)
                {
                    self.AddFolderToProject(targetGuid, directory, parentGroup, exclude, recursive);
                }
            }

            // Adding files.
            foreach (string file in Directory.GetFiles(folderPath))
            {
                if (!string.IsNullOrEmpty(regexExclude) && Regex.IsMatch(file, regexExclude))
                {
                    continue;
                }
                self.AddFileToProject(targetGuid, file, parentGroup);
            }
        }

        /// <summary>
        /// Containses the folder by real path.
        /// </summary>
        /// <returns><c>true</c>, if folder by real path was containsed, <c>false</c> otherwise.</returns>
        /// <param name="folderPath">Folder path.</param>
        /// <param name="exclude">Exclude.</param>
        /// <param name="recursive">If set to <c>true</c> recursive.</param>
        public static bool ContainsFolderByRealPath(
            this PBXProject self,
            string folderPath,
            string[] exclude = null,
            bool recursive = true)
        {
            if (!Directory.Exists(folderPath))
            {
                return false;
            }

            var regexExclude = string.Empty;
            if (exclude != null)
            {
                regexExclude = string.Format(@"{0}", string.Join("|", exclude));
            }

            foreach (string directory in Directory.GetDirectories(folderPath))
            {
                if (!string.IsNullOrEmpty(regexExclude) && Regex.IsMatch(directory, regexExclude))
                {
                    continue;
                }

                if (directory.EndsWith(".bundle"))
                {
                    if (!self.ContainsFileByRealPath(directory))
                    {
                        return false;
                    }
                    continue;
                }

                if (recursive)
                {
                    if (!self.ContainsFolderByRealPath(directory, exclude, recursive))
                    {
                        return false;
                    }
                }
            }

            // Adding files.
            foreach (string file in Directory.GetFiles(folderPath))
            {
                if (!string.IsNullOrEmpty(regexExclude) && Regex.IsMatch(file, regexExclude))
                {
                    continue;
                }
                if (!self.ContainsByRealPath(file))
                {
                    return false;
                }
            }
            return true;
        }

#if UNITY_2018_2_OR_NEWER
        /// <summary>
        /// Containses the build property.
        /// </summary>
        /// <returns><c>true</c>, if build property was containsed, <c>false</c> otherwise.</returns>
        /// <param name="targetGuid">Target GUID.</param>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static bool ContainsBuildProperty(this PBXProject self, string targetGuid, string name, string value)
        {
            var isContain = false;
            var propValues = self.GetBuildPropertyForAnyConfig(targetGuid, name);
            if(propValues != null)
            {
                var propList = new List<string>();
                propList.AddRange(propValues.Split(' '));
                isContain = propList.Contains(value);
            }
            return isContain;
        }
#endif

        public static bool ContainsByRealPath(this PBXProject self, string path)
        {
            return FindGuidByRealPath(self, path) != null;
        }

        public static string FindGuidByRealPath(this PBXProject self, string path)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Name == LocalizableStrings)
            {
                return FindGuidInGroup(self, path, LocalizableStrings);
            }
            return self.FindFileGuidByRealPath(path);
        }

        #endregion

        /// <summary>
        /// LocalizableGroupを作成する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="targetGuid"></param>
        /// <returns>PBXVariantGroupData(internal)のobject</returns>
        static object CreateLocalizableGroup(this PBXProject self, string targetGuid)
        {
            var variantGroup = LocalizableStrings;

            // PBXProject.variantGroups
            var pbxProjectRfl = GetPbxProjectReflectionClass(self);
            var variantGroupsSection = pbxProjectRfl.GetProperty<object>("variantGroups");

            // KnownSectionBase<PBXVariantGroupData>.m_Entries
            var variantGroupsSectionRfl = new ReflectionClass(variantGroupsSection);
            var entries = variantGroupsSectionRfl.GetField<IDictionary>("m_Entries");

            foreach (var v in entries.Values)
            {
                // PBXGroupData.name
                var pbxGroupDataRfl = new ReflectionClass(v, typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXGroupData");
                var name = pbxGroupDataRfl.GetField<string>("name");

                if (name == variantGroup)
                {
                    return pbxGroupDataRfl.Instance;
                }
            }

            var pbxVariantGroupData = PBXVariantGroupDataExt.CreateVariantGroup(variantGroup);

            // KnownSectionBase<PBXVariantGroupData>.AddEntry()
            variantGroupsSectionRfl.ExecuteMethod("AddEntry", new object[] { pbxVariantGroupData });

            // PBXVariantGroupData:PBXObjectData.guid
            var pbxVariantGroupDataRfl = new ReflectionClass(pbxVariantGroupData, typeof(PBXProject),
                ReflectionNameSpaces.Pbx, "PBXVariantGroupData");
            var groupGuid = pbxVariantGroupDataRfl.GetField<string>("guid");

            // PBXProjectData.groups
            var pbxProjectDataRfl = GetPbxProjectDataReflectionClass(self);
            var groupsSection = pbxProjectDataRfl.GetField<object>("groups");
            var groupsSectionRfl = new ReflectionClass(groupsSection);

            // KnownSectionBase<PBXGroupData>.m_Entries
            var groupsEntries = groupsSectionRfl.GetField<IDictionary>("m_Entries");

            foreach (var v in groupsEntries.Values)
            {
                // PBXGroupData.name
                var pbxGroupDataRfl = new ReflectionClass(v, typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXGroupData");
                var name = pbxGroupDataRfl.GetField<string>("name");

                if (name == "CustomTemplate")
                {
                    var children = pbxGroupDataRfl.GetField<object>("children");
                    var childrenRfl = new ReflectionClass(children, typeof(PBXProject), ReflectionNameSpaces.Pbx, "GUIDList");
                    childrenRfl.ExecuteMethod("AddGUID", new object[] { groupGuid });
                }
            }

            // PBXBuildFileData.CreateFromFile()
            var buildFileData = ReflectionClass.ExecuteStaticMethod<object>(typeof(PBXProject),
                ReflectionNameSpaces.Pbx, "PBXBuildFileData", "CreateFromFile", new object[] { groupGuid, false, null });

            var buildFileDataRfl = new ReflectionClass(buildFileData, typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXBuildFileData");

            // PBXProjectData.buildFiles
            var buildFiles = pbxProjectDataRfl.GetField<object>("buildFiles");
            var buildFilesRfl = new ReflectionClass(buildFiles);

            // KnownSectionBase<PBXGroupData>.AddEntry()
            buildFilesRfl.ExecuteMethod("AddEntry", new object[] { buildFileData });

            // PBXProject.nativeTargets
            var nativeTargets = pbxProjectRfl.GetProperty<object>("nativeTargets");
            var nativeTargetsRfl = new ReflectionClass(nativeTargets);

            // KnownSectionBase<PBXNativeTargetData>.m_Entries[targetGuid]
            var nativeTargetsEntry = nativeTargetsRfl.GetField<IDictionary>("m_Entries");
            var nativeTarget = nativeTargetsEntry[targetGuid];

            var fileGuids = pbxProjectDataRfl.ExecuteMethod<object>("BuildSectionAny", new object[] { nativeTarget, variantGroup, false });

            // FileGUIDListBase.files
            var fileGuidsRfl = new ReflectionClass(fileGuids, typeof(PBXProject), ReflectionNameSpaces.Pbx, "FileGUIDListBase");
            var files = fileGuidsRfl.GetField<object>("files");
            var filesRfl = new ReflectionClass(files, typeof(PBXProject), ReflectionNameSpaces.Pbx, "GUIDList");

            // PBXBuildFileData.guid
            var buildFileGuid = buildFileDataRfl.GetField<string>("guid");

            filesRfl.ExecuteMethod("AddGUID", new object[] { buildFileGuid });

            return pbxVariantGroupData;
        }

        static string FindGuidInGroup(this PBXProject self, string path, string group)
        {
            string guid = null;

            // PBXProjectData.fileRefs
            var pbxProjectDataRfl = GetPbxProjectDataReflectionClass(self);
            var fileRefsSection = pbxProjectDataRfl.GetField<object>("fileRefs");
            var fileRefsSectionRfl = new ReflectionClass(fileRefsSection);

            // KnownSectionBase<PBXFileReferenceData>.m_Entries
            var fileRefsEntries = fileRefsSectionRfl.GetField<IDictionary>("m_Entries");

            foreach (var item in fileRefsEntries.Values)
            {
                var fileRefRfl = new ReflectionClass(item, typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXFileReferenceData");

                if (fileRefRfl.GetProperty<string>("path") == path)
                {
                    guid = fileRefRfl.GetField<string>("guid");
                }
            }
            if (guid != null)
            {
                // PBXProjectData.variantGroups
                var variantGroupsSection = pbxProjectDataRfl.GetField<object>("variantGroups");
                var variantGroupsSectionRfl = new ReflectionClass(variantGroupsSection);

                // KnownSectionBase<PBXVariantGroupData>.m_Entries
                var variantGroupsEntries = variantGroupsSectionRfl.GetField<IDictionary>("m_Entries");

                foreach (var item in variantGroupsEntries.Values)
                {
                    var variantGroupsRfl = new ReflectionClass(item, typeof(PBXProject), ReflectionNameSpaces.Pbx, "PBXVariantGroupData");
                    if (variantGroupsRfl.GetField<string>("name") == group)
                    {
                        // PBXVariantGroupData.children.m_List
                        var guids = variantGroupsRfl.GetField<object>("children");
                        var guidsRfl = new ReflectionClass(guids, typeof(PBXProject), ReflectionNameSpaces.Pbx, "GUIDList");
                        var guidsList = guidsRfl.GetField<IList>("m_List");

                        if (guidsList.Contains(guid))
                        {
                            return guid;
                        }
                    }
                }
            }

            return null;
        }

        static ReflectionClass GetPbxProjectReflectionClass(PBXProject instance)
        {
            return new ReflectionClass(instance);
        }

        static ReflectionClass GetPbxProjectDataReflectionClass(PBXProject instance)
        {
            var pbxProjectRfl = GetPbxProjectReflectionClass(instance);
            var pbxProjectData = pbxProjectRfl.GetField<object>("m_Data");
            return new ReflectionClass(pbxProjectData, typeof(PBXProject), ReflectionNameSpaces.Xcode, "PBXProjectData");
        }
    }
}

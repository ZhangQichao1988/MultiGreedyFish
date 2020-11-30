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
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor.iOS.Xcode;
using XcodeTools.XcodeUtility;

namespace XcodeTools
{
    using Debug = UnityEngine.Debug;
    using Json = XMiniJSON;

    public class XCMod
    {
        private Hashtable _datastore;
        private ArrayList _libs;
        private ArrayList _userLibs;
        private ArrayList _userFrameworks;

        public string name { get; private set; }

        public string path { get; private set; }

        public string group
        {
            get
            {
                return (string) _datastore["group"];
            }
        }

        public ArrayList patches
        {
            get
            {
                return (ArrayList) _datastore["patches"];
            }
        }

        public ArrayList libs
        {
            get
            {
                if (_libs == null)
                {
                    _libs = new ArrayList(((ArrayList) _datastore["libs"]).Count);
                    foreach (string fileRef in (ArrayList) _datastore["libs"])
                    {
                        _libs.Add(new XCModFile(fileRef));
                    }
                }
                return _libs;
            }
        }

        public ArrayList userLibs
        {
            get
            {
                if (_userLibs != null)
                {
                    return _userLibs;
                }

                var userLibsArrays = (ArrayList) _datastore["user_libs"];
                _userLibs = new ArrayList(userLibsArrays.Count);
                foreach (string configStr in userLibsArrays)
                {
                    _userLibs.Add(new XCModFile(configStr));
                }

                return _userLibs;
            }
        }

        public ArrayList frameworks
        {
            get
            {
                return (ArrayList) _datastore["frameworks"];
            }
        }

        public ArrayList userFrameworks
        {
            get
            {
                if (_userFrameworks != null)
                {
                    return _userFrameworks;
                }

                var userFrameworksArrays = (ArrayList) _datastore["user_frameworks"];
                _userFrameworks = new ArrayList(userFrameworksArrays.Count);
                foreach (string configStr in userFrameworksArrays)
                {
                    _userFrameworks.Add(new XCModFile(configStr));
                }

                return _userFrameworks;
            }
        }

        public ArrayList headerpaths
        {
            get
            {
                return (ArrayList) _datastore["headerpaths"];
            }
        }

        public Hashtable buildSettings
        {
            get
            {
                return (Hashtable) _datastore["buildSettings"];
            }
        }

        public ArrayList files
        {
            get
            {
                return (ArrayList) _datastore["files"];
            }
        }

        public ArrayList arcSources
        {
            get
            {
                return (ArrayList) _datastore["arc_sources"];
            }
        }

        public ArrayList noArcSources
        {
            get
            {
                var list = _datastore["no_arc_sources"] as ArrayList;
                if (list == null)
                {
                    list = new ArrayList();
                }
                return list;
            }
        }

        public ArrayList folders
        {
            get
            {
                return (ArrayList) _datastore["folders"];
            }
        }

        public ArrayList excludes
        {
            get
            {
                return (ArrayList) _datastore["excludes"];
            }
        }

        public bool? automaticallyManageSigning
        {
            get
            {
                if (_datastore.Contains("automatically_manage_signing"))
                {
                    return (bool) _datastore["automatically_manage_signing"];
                }
                return null;
            }
        }

        public XCMod(string filename)
        {
            FileInfo projectFileInfo = new FileInfo(filename);
            if (!projectFileInfo.Exists)
            {
                Debug.LogWarning("File does not exist.");
            }
            name = System.IO.Path.GetFileNameWithoutExtension(filename);
            path = System.IO.Path.GetDirectoryName(filename);
            string contents = projectFileInfo.OpenText().ReadToEnd();
            _datastore = (Hashtable) XMiniJSON.jsonDecode(contents);
        }
        //	"group": "GameCenter",
        //	"patches": [],
        //	"libs": [],
        //	"frameworks": ["GameKit.framework"],
        //	"headerpaths": ["Editor/iOS/GameCenter/**"],
        //	"files":   ["Editor/iOS/GameCenter/GameCenterBinding.m",
        //				"Editor/iOS/GameCenter/GameCenterController.h",
        //				"Editor/iOS/GameCenter/GameCenterController.mm",
        //				"Editor/iOS/GameCenter/GameCenterManager.h",
        //				"Editor/iOS/GameCenter/GameCenterManager.m"],
        //	"folders": [],
        //	"excludes": ["^.*\\.meta$", "^.*\\.mdown^", "^.*\\.pdf$"]

        public void ApplyTo(PBXProject project)
        {
            
            var projectGuid = project.GetUnityMainTargetGuid();
            //project.TargetGuidByName(PBXProject.GetUnityTargetName());

            // libraries
            foreach (XCModFile lib in libs)
            {
                if (project.HasLibrary(lib.filePath))
                {
                    continue;
                }
                project.AddLibraryToProject(projectGuid, lib.filePath, lib.isWeak);
            }

            // frameworks
            foreach (string frameworkPath in frameworks)
            {
                var framework = new XCModFile(frameworkPath);
                if (project.ContainsFramework(projectGuid, framework.filePath))
                {
                    continue;
                }
                project.AddFrameworkToProject(projectGuid, framework.filePath, framework.isWeak);
            }

            // files
            foreach (string filePath in files)
            {
                var file = new XCModFile(filePath);
                var absoluteFilePath = Path.Combine(Application.dataPath, file.filePath);
                project.AddFileToProject(projectGuid, absoluteFilePath, group, file.isWeak, null);
            }

            // ARC source
            foreach (string arcSourcePath in arcSources)
            {
                var absoluteFilePath = Path.Combine(Application.dataPath, arcSourcePath);
                project.ModifyFileToProject(projectGuid, absoluteFilePath, group, false, "-fobjc-arc");
            }

            // Non ARC source
            foreach (string noArcSourcePath in noArcSources)
            {
                var absoluteFilePath = Path.Combine(Application.dataPath, noArcSourcePath);
                project.ModifyFileToProject(projectGuid, absoluteFilePath, group, false, "-fno-objc-arc");
            }

            // folder with excludes
            string[] excludePaths = null;
            if (excludes.Count > 0)
            {
                excludePaths = new string[excludes.Count];
                for (var i = 0; i < excludes.Count; i++)
                {
                    excludePaths[i] = (string) excludes[i];
                }
            }
            foreach (string folderPath in folders)
            {
                var absoluteFolderPath = Path.Combine(Application.dataPath, folderPath);
                project.AddFolderToProject(projectGuid, absoluteFolderPath, group, excludePaths);
            }

            // build settings
            foreach (string key in buildSettings.Keys)
            {
                if (buildSettings[key] == null)
                {
                    continue;
                }
                if (buildSettings[key] is string)
                {
                    project.SetBuildProperty(projectGuid, key, (string) buildSettings[key]);
                    continue;
                }
                if (buildSettings[key] is ArrayList)
                {
                    var values = buildSettings[key] as ArrayList;
                    foreach (string value in values)
                    {
                        project.AddBuildProperty(projectGuid, key, value);
                    }
                    continue;
                }
            }

            // ProvisioningStyle
            if (automaticallyManageSigning != null)
            {
                XcodeDefaultSettingsExtensions.SetAutomaticallySign((bool) automaticallyManageSigning);
            }
        }
    }

    public class XCModFile
    {
        public string filePath { get; private set; }

        public bool isWeak { get; private set; }

        public XCModFile(string inputString)
        {
            isWeak = false;
            if (inputString.Contains(":"))
            {
                string[] parts = inputString.Split(':');
                filePath = parts[0];
                isWeak = (parts[1].CompareTo("weak") == 0);
            }
            else
            {
                filePath = inputString;
            }
        }
    }
}

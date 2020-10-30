##!/usr/bin/bash
set -euo pipefail

USE_SERVER="false"
if [[ $# -eq 1 ]]; then
  USE_SERVER=${1}
fi

SHELL_PATH=$(cd "$(dirname "$0")";pwd)
WORKDIR=$(dirname $(dirname "$SHELL_PATH"))
PRODUCT_NAME="FISH-FIGHT"
BUNDLE_VERSION="0.0.1"
BUNDLE_VERSION_CODE_SHOW="1"
ENABLE_DEBUG="true"
BUNDLE_IDENTIFIER="com.klab.fishfight"
DEV_BUILD="true"

#ios build
DEV_TEAM=${DEV_TEAM:-E84VPJ2AUN}
MOBILE_PROVISION_UUID=${MOBILE_PROVISION_UUID:-88d86ceb-326f-4bde-91c1-e2726f80daf8}
CODE_SIGN_IDENTITY=${CODE_SIGN_IDENTITY:-"iPhone Distribution: KLab INC. (ENT)"}
BUNDLE_METHOD=${BUNDLE_METHOD:-enterprise}


echo ${WORKDIR}
UNITY_PATH=/Applications/Unity/Hub/Editor/2019.4.11f1/Unity.app/Contents/MacOS/Unity

BUILD_DATA=$(date "+%Y%m%d%H%M%S")

echo "Start build unity ios"
echo ${UNITY_PATH} -batchmode -quit -projectPath ${WORKDIR} -executeMethod MultiGreedyFish.Pipline.ProjectBuild.Build \
-devBuild=${DEV_BUILD} \
-productName=${PRODUCT_NAME} -iosBuild=true -bundleVersion=${BUNDLE_VERSION} -buildNumber=${BUNDLE_VERSION_CODE_SHOW} -enabledDebugMenu=${ENABLE_DEBUG} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} -provisionPID=${MOBILE_PROVISION_UUID} -teamID=${DEV_TEAM} -isDevelop=false -logFile /tmp/fish/build/ios/build_log-${BUILD_DATA}.log 


${UNITY_PATH} -batchmode -quit -projectPath ${WORKDIR} -executeMethod MultiGreedyFish.Pipline.ProjectBuild.Build \
-devBuild=${DEV_BUILD} \
-productName=${PRODUCT_NAME} -iosBuild=true -useSever=${USE_SERVER} -bundleVersion=${BUNDLE_VERSION} -buildNumber=${BUNDLE_VERSION_CODE_SHOW} -enabledDebugMenu=${ENABLE_DEBUG} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} -provisionPID=${MOBILE_PROVISION_UUID} -teamID=${DEV_TEAM} -isDevelop=false -logFile /tmp/fish/build/ios/build_log-${BUILD_DATA}.log 


echo "start build  ios xcode project"
# export ios app by xcode build
sudo xcode-select -s /Applications/Xcode11.3.app/Contents/Developer
XCODE_PROJ_PATH=${WORKDIR}/Achieve/xcodeProj
ARCHIVE_PATH=${WORKDIR}/Achieve/output/fishgame.xcarchive
EXPORT_PATH=${WORKDIR}/Achieve/output/
IPA_PATH=${WORKDIR}/Achieve/output/Unity-iPhone.ipa

pushd ${XCODE_PROJ_PATH}

#if [[ $IS_DEVELOPMENT = "false" ]] && [[ $IS_APPSTORE = "false" ]]; then
  # remove StoreKit.framework
  sed -e '/StoreKit/d' Unity-iPhone.xcodeproj/project.pbxproj > /tmp/tmp.xcodeproj
  mv /tmp/tmp.xcodeproj Unity-iPhone.xcodeproj/project.pbxproj
#fi

# 清理
xcodebuild  -workspace "Unity-iPhone.xcworkspace" -scheme "Unity-iPhone" clean


# Use XcodeBuild to Make Package
xcodebuild archive -workspace "Unity-iPhone.xcworkspace" -configuration Release -scheme Unity-iPhone -archivePath ${ARCHIVE_PATH}  ENABLE_BITCODE=NO
#CODE_SIGN_IDENTITY="${CODE_SIGN_IDENTITY}" PROVISIONING_PROFILE="${MOBILE_PROVISION_UUID}" PRODUCT_BUNDLE_IDENTIFIER="${BUNDLE_IDENTIFIER}" DEVELOPMENT_TEAM="${DEV_TEAM}" ENABLE_BITCODE=NO
echo '<?xml version="1.0" encoding="UTF-8"?>
    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
    <plist version="1.0">
    <dict>
        <key>teamID</key>
        <string>'${DEV_TEAM}'</string>
        <key>method</key>
        <string>'${BUNDLE_METHOD}'</string>
        <key>uploadSymbols</key>
        <false/>
        <key>compileBitcode</key>
        <false/>
        <key>provisioningProfiles</key>
        <dict>
            <key>'${BUNDLE_IDENTIFIER}'</key>
            <string>'${MOBILE_PROVISION_UUID}'</string>
        </dict>
    </dict>
    </plist>' > Enterprise.plist

xcodebuild -exportArchive -archivePath ${ARCHIVE_PATH} -exportPath ${EXPORT_PATH} -exportOptionsPlist Enterprise.plist


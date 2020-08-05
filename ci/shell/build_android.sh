##!/usr/bin/bash
set -euo pipefail
SHELL_PATH=$(cd "$(dirname "$0")";pwd)
WORKDIR=$(dirname $(dirname "$SHELL_PATH"))
PRODUCT_NAME="FISH-FIGHT"
BUNDLE_VERSION="0.0.1"
BUNDLE_VERSION_CODE_SHOW="1"
ENABLE_DEBUG="true"
BUNDLE_IDENTIFIER="com.klab.fishfight"

#android build
KEY_STORE_PATH="${WORKDIR}/ci/config/fish.keystore"
KEY_STORE_PASS="fish007"
KEY_ALIAS_NAME="com.klab.fishfight"
KEY_ALIAS_PASS="fish007"


echo ${WORKDIR}
UNITY_PATH=/Applications/Unity/Hub/Editor/2019.4.0f1/Unity.app/Contents/MacOS/Unity

BUILD_DATA=$(date "+%Y%m%d%H%M%S")

echo "Start build unity android"
echo ${UNITY_PATH} -batchmode -quit -projectPath ${WORKDIR} -executeMethod MultiGreedyFish.Pipline.ProjectBuild.Build \
-productName=${PRODUCT_NAME} -iosBuild=false -bundleVersion=${BUNDLE_VERSION} -buildNumber=${BUNDLE_VERSION_CODE_SHOW} -enabledDebugMenu=${ENABLE_DEBUG} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} -keyStorePath="${KEY_STORE_PATH}" -keyStorePass="${KEY_STORE_PASS}" -keyAliasName="${KEY_ALIAS_NAME}" -apk_name="fish-${BUILD_DATA}" -keyAliasPass="${KEY_ALIAS_PASS}" -logFile /tmp/fish/build/android/build_log-${BUILD_DATA}.log 


${UNITY_PATH} -batchmode -quit -projectPath ${WORKDIR} -executeMethod MultiGreedyFish.Pipline.ProjectBuild.Build \
-productName=${PRODUCT_NAME} -iosBuild=false -bundleVersion=${BUNDLE_VERSION} -buildNumber=${BUNDLE_VERSION_CODE_SHOW} -enabledDebugMenu=${ENABLE_DEBUG} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} -keyStorePath="${KEY_STORE_PATH}" -keyStorePass="${KEY_STORE_PASS}" -keyAliasName="${KEY_ALIAS_NAME}" -apk_name="fish-${BUILD_DATA}" -keyAliasPass="${KEY_ALIAS_PASS}" -logFile /tmp/fish/build/android/build_log-${BUILD_DATA}.log 
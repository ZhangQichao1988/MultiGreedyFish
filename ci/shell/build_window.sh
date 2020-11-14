##!/usr/bin/bash
set -euo pipefail

USE_SERVER="ESeverType.TENCENT_DEV"
IS_RELEASE="false"
if [[ $# -eq 1 ]]; then
  USE_SERVER=${1}
fi
if [[ $# -eq 2 ]]; then
  IS_RELEASE=${2}
fi


SHELL_PATH=$(cd "$(dirname "$0")";pwd)
WORKDIR=$(dirname $(dirname "$SHELL_PATH"))
PRODUCT_NAME="FISH-FIGHT"
BUNDLE_VERSION="0.0.1"
BUNDLE_VERSION_CODE_SHOW="5"
ENABLE_DEBUG="true"
BUNDLE_IDENTIFIER="jp.co.cad.crazyfish"
DEV_BUILD="true"

if [[ ${IS_RELEASE} == "true" ]]; then
  DEV_BUILD="false"
  ENABLE_DEBUG="false"
fi


#android build
KEY_STORE_PATH="${WORKDIR}/ci/config/fish.keystore"
KEY_STORE_PASS="hulaoshi007"
KEY_ALIAS_NAME="crazyfish"
KEY_ALIAS_PASS="hulaoshi007"


echo ${WORKDIR}
UNITY_PATH="/c/Program Files/Unity/Hub/Editor/2019.4.14f1/Editor/Unity.exe"

BUILD_DATA=$(date "+%Y%m%d%H%M%S")

echo "Start build unity android"
echo ${UNITY_PATH} -batchmode -quit -projectPath ${WORKDIR} -executeMethod MultiGreedyFish.Pipline.ProjectBuild.Build \
-devBuild=${DEV_BUILD} \
-productName=${PRODUCT_NAME} -iosBuild=false -bundleVersion=${BUNDLE_VERSION} -buildNumber=${BUNDLE_VERSION_CODE_SHOW} -enabledDebugMenu=${ENABLE_DEBUG} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} -keyStorePath="${KEY_STORE_PATH}" -keyStorePass="${KEY_STORE_PASS}" -keyAliasName="${KEY_ALIAS_NAME}" -apk_name="fish-${BUILD_DATA}" -keyAliasPass="${KEY_ALIAS_PASS}" -logFile /tmp/fish/build/android/build_log-${BUILD_DATA}.log 


"${UNITY_PATH}" -batchmode -quit -projectPath ${WORKDIR} -executeMethod MultiGreedyFish.Pipline.ProjectBuild.Build \
-devBuild=${DEV_BUILD} \
-productName=${PRODUCT_NAME} -iosBuild=false -useSever=${USE_SERVER} -bundleVersion=${BUNDLE_VERSION} -buildNumber=${BUNDLE_VERSION_CODE_SHOW} -enabledDebugMenu=${ENABLE_DEBUG} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} -keyStorePath="${KEY_STORE_PATH}" -keyStorePass="${KEY_STORE_PASS}" -keyAliasName="${KEY_ALIAS_NAME}" -apk_name="fish-${BUILD_DATA}" -keyAliasPass="${KEY_ALIAS_PASS}" -logFile /tmp/fish/build/android/build_log-${BUILD_DATA}.log 

# MultiGreedyFish
woshi xues


### PB version 3.12.2
./protoc -I/Users/li-j/project/unity-project/MultiGreedyFishServer/fishgame/other/proto/ /Users/li-j/project/unity-project/MultiGreedyFishServer/fishgame/other/proto/protocol.proto --csharp_out=/Users/li-j/project/unity-project/MultiGreedyFish/Assets/Scripts/GameModule/Network/


### 子模块更新
git submodule update --remote


### 打包
./ci/shell/build_android.sh ${SERVER} ${IS_RELEASE}
${SERVER}
ESeverType.OFFLINE          离线包
ESeverType.LOCAL_SERVER     本地服务器
ESeverType.TENCENT_DEV      腾讯云开发服
ESeverType.TENCENT_STABLE   腾讯云稳定服
ESeverType.TENCENT_PROD     腾讯云prod

${IS_RELEASE}
true 正式包 会删除 resouce下 json资源 关闭console等

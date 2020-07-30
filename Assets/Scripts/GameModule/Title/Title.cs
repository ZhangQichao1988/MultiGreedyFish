using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;

public class Title : UIBase
{
    void SampleBaseData()
    {
        //基表数据获取
        List<FishDataInfo> fishInfos = FishDataTableProxy.Instance.GetAll();
        foreach (var item in fishInfos)
        {
            Debug.Log(JsonUtility.ToJson(item));
        }

        //通过id获取 api需要自己实现 
        FishDataInfo fishInfo = FishDataTableProxy.Instance.GetDataById(1);
        Debug.Log(JsonUtility.ToJson(fishInfo));

        //读取基表(缓存) 默认会自动缓存 如果有性能问题的话 可以在加载场景读条时 把基表cache 放在 scene cache中
        FishDataTableProxy.Instance.Cached();

        //销毁单个基表
        BaseDataTableProxyMgr.Destory(FishDataTableProxy.Instance.TableName);

        //销毁所有基表
        BaseDataTableProxyMgr.Destory();
    }

    public void OnClickLogin()
    {
        SampleBaseData();
        return;
        GameServiceController.GetPlatformToken((token)=>{
            token = "tesadfasfa";
            UserLoginFlowController.ProcessLoginLogic(token);
        });
    }

    public void OnClickBattle()
    {
        Close();
        BlSceneManager.LoadSceneByClass(SceneId.BATTLE_SCENE, typeof(BattleScene));
    }

}
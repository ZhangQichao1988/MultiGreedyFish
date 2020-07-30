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
        FishDataInfo fishInfo = FishDataTableProxy.Instance.GetDataById(1);
        Debug.Log(JsonUtility.ToJson(fishInfo));

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
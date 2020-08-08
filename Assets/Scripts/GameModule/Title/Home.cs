using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;

public class Home : UIBase
{
    public HomeFishControl fishControl;
    public void Awake()
    {
        UIBase.Close("BattleResult");
        OnClickLogin();
    }
    public void OnClickLogin()
    {
        if (UserLoginFlowController.OnGetPlayer == null)
        {
            UserLoginFlowController.OnGetPlayer = OnGetPlayer;
        }

        GameServiceController.GetPlatformToken((token)=>{
            token = token == null ? "" : token ;
            UserLoginFlowController.ProcessLoginLogic(token);
            
        });
    }

    private void OnGetPlayer(PBPlayer pBPlayer)
    {
        var fishBaseData = FishDataTableProxy.Instance.GetDataById(pBPlayer.FightFish);
        var asset = ResourceManager.LoadSync(Path.Combine(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath), typeof(GameObject));
        GameObject go =  GameObjectUtil.InstantiatePrefab(asset.Asset as GameObject, fishControl.gameObject);
        fishControl.SetFishModel(go);
    }
    public void OnClickBattle()
    {
        NetWorkHandler.GetDispatch().AddListener<P4_Response>(GameEvent.RECIEVE_P4_RESPONSE, OnRecvBattle);
        NetWorkHandler.RequestBattle();

        
    }

    void OnRecvBattle<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P4_RESPONSE);
        var realResponse = response as P4_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            Close();
            DataBank.stageInfo = realResponse.StageInfo;
            BlSceneManager.LoadSceneByClass(SceneId.BATTLE_SCENE, typeof(BattleScene));
        }
        else
        {
            Debug.Log("战斗通信错误！");
        }

    }
}
using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class UIFishStatus : UIBase
{
    public FishStatusFishControl fishControl;

    private PBPlayerFishLevelInfo playerFishLevelInfo;

    public void Setup(PBPlayerFishLevelInfo playerFishLevelInfo)
    {
        if (this.playerFishLevelInfo == null || this.playerFishLevelInfo.FishId != playerFishLevelInfo.FishId)
        { CreateFishModel(playerFishLevelInfo.FishId); }

        this.playerFishLevelInfo = playerFishLevelInfo;
        

    }
    private void CreateFishModel(int fishId)
    {
        var fishBaseData = FishDataTableProxy.Instance.GetDataById(fishId);
        var asset = ResourceManager.LoadSync(Path.Combine(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath), typeof(GameObject));
        GameObject go =  GameObjectUtil.InstantiatePrefab(asset.Asset as GameObject, fishControl.gameObject);
        fishControl.SetFishModel(go);
    }

    public void OnClickFishSelect()
    {
        NetWorkHandler.GetDispatch().AddListener<P6_Response>(GameEvent.RECIEVE_P6_RESPONSE, OnRecvFishSelect);
        NetWorkHandler.RequestFightFishSet(playerFishLevelInfo.FishId);
    }

    void OnRecvFishSelect<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P6_RESPONSE);
        var realResponse = response as P6_Response;
        PlayerModel.Instance.player.FightFish = playerFishLevelInfo.FishId;
        ((HomeScene)BlSceneManager.GetCurrentScene()).GotoSceneUI("Home");
    }

}
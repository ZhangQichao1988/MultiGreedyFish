using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerModel : BaseModel<PlayerModel>
{
    public long playerId;
    public PBPlayer player;
    public int goldPoolLevel;
    public List<PBMission> pBMissions;
    public Dictionary<int, int> dicBattleMissionActionAddTrigger = new Dictionary<int, int>();
    public long fetchMissionTime;

    public PlayerModel() : base()
    {

    }
    public void BattleStart()
    {
        dicBattleMissionActionAddTrigger.Clear();
    }
    public int GetMissionActionTrigger(int actionId)
    {
        foreach (var note in pBMissions)
        {
            if (note.ActionId == actionId)
            {
                return note.CurrTrigger;
            }
        }
        return 0;
    }
    public void MissionActionTrigger(int actionId, int value)
    {
        int maxWin = PlayerModel.Instance.GetMissionActionTrigger(actionId);
        if (value > maxWin)
        {
            PlayerModel.Instance.MissionActionTriggerAdd(actionId, value - maxWin);
        }
    }
    public void MissionActionTriggerAdd(int actionId, int addValue)
    {
        bool isReach = false;
        foreach (var note in pBMissions)
        {
            isReach = note.CurrTrigger >= note.Trigger;
            if (note.ActionId == actionId)
            {
                if (!dicBattleMissionActionAddTrigger.ContainsKey(actionId))
                {
                    dicBattleMissionActionAddTrigger.Add(actionId, addValue);
                }
                else
                {
                    dicBattleMissionActionAddTrigger[actionId] += addValue;
                }
                note.CurrTrigger += addValue;
                if (note.CurrTrigger >= note.Trigger && !isReach)
                {
                    UIPopupMissionComplete.Instance.AddCompleteMission(note);
                }
            }
        }
    }
    public int gainGold { set; get; }
    public PBPlayerFishLevelInfo GetCurrentPlayerFishLevelInfo()
    {
        return GetPlayerFishLevelInfo(player.FightFish);
    }
    public PBPlayerFishLevelInfo GetPlayerFishLevelInfo(int fishId)
    {
        foreach (var note in player.AryPlayerFishInfo)
        {
            if (note.FishId == fishId) { return note; }
        }
        Debug.LogError("DataBank.GetCurrentPlayerFishLevelInfo()_1");
        return null;
    }
    public void SetPlayerFishLevelInfo(int fishId, PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        for(int i = 0; i < player.AryPlayerFishInfo.Count; ++i)
        {
            if (player.AryPlayerFishInfo[i].FishId == fishId) 
            {
                player.AryPlayerFishInfo[i] = pBPlayerFishLevelInfo;
                return;
            }
        }
        Debug.LogError("DataBank.SetPlayerFishLevelInfo()_1, not found set fish id");
    }
    public int GetTotalRankLevel()
    {
        int totalRank = 0;
        foreach (var note in player.AryPlayerFishInfo)
        {
            totalRank += note.RankLevel;
        }
        return totalRank;
    }

    /// <summary>
    /// 资产更新
    /// </summary>
    public void UpdateAssets(ShopItemVo itemVo, RewardMapVo rewardVo)
    {
        if (itemVo.Paytype == PayType.Diamond)
        {
            player.Diamond -= itemVo.Price;
        }
        else if (itemVo.Paytype == PayType.Gold)
        {
            player.Gold -= itemVo.Price;
            MissionActionTriggerAdd(5, itemVo.Price);
        }
        UpdateAssets(rewardVo);

        
    }
    public void UpdateAssets(RewardMapVo rewardVo)
    {
        if (rewardVo.Content != null && rewardVo.Content.Count > 0)
        {
            ProcessReward(rewardVo.Content);
        }

        if (rewardVo.IsTreasure)
        {
            ProcessReward(rewardVo.TreasureContent);
        }
    }
    public void ProcessReward(List<RewardItemVo> rewards)
    {
        foreach (RewardItemVo vo in rewards)
        {
            if (vo.masterDataItem.type == FishItemType.Diamond)
            {
                player.Diamond += vo.Amount;
            }
            else if (vo.masterDataItem.type == FishItemType.Gold)
            {
                player.Gold += vo.Amount;
                PlayerModel.Instance.MissionActionTriggerAdd(14, vo.Amount);
            }
            else if (vo.masterDataItem.type == FishItemType.Piece)
            {
                PBPlayerFishLevelInfo fishItem = GetPlayerFishLevelInfo(vo.masterDataItem.extra);
                if (fishItem == null)
                {
                    fishItem = new PBPlayerFishLevelInfo(){
                        FishId = vo.masterDataItem.extra,
                        FishChip = 0,
                        FishLevel = 0,
                        RankLevel = 0
                    };
                    player.AryPlayerFishInfo.Add(fishItem);
                }
                fishItem.FishChip += vo.Amount;
            }
        }
    }
    
    public override void Dispose()
    {
        base.Dispose();
        player = null;
    }
}
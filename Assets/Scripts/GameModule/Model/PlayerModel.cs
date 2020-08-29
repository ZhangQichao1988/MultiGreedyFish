
using UnityEngine;

public class PlayerModel : BaseModel<PlayerModel>
{
    public PBPlayer player;
    public PlayerModel() : base()
    {

    }

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
}
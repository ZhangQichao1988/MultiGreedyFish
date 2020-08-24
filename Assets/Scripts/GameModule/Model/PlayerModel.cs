
using UnityEngine;

public class PlayerModel : BaseModel<PlayerModel>
{
    public PBPlayer player;
    public PlayerModel() : base()
    {

    }

    public PBPlayerFishLevelInfo GetCurrentPlayerFishLevelInfo()
    {
        return GetPlayerFishLevelInfo(player);
    }
    public PBPlayerFishLevelInfo GetPlayerFishLevelInfo(PBPlayer player)
    {
        foreach (var note in player.AryPlayerFishInfo)
        {
            if (note.FishId == player.FightFish) { return note; }
        }
        Debug.LogError("DataBank.GetCurrentPlayerFishLevelInfo()_1");
        return null;
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
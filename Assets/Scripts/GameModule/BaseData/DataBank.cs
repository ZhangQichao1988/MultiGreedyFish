using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class DataBank
{
    static public  PBPlayer player;
    static public PBStageInfo stageInfo;

    static public PBPlayerFishLevelInfo GetCurrentPlayerFishLevelInfo()
    {
        return GetPlayerFishLevelInfo(player);
    }
    static public PBPlayerFishLevelInfo GetPlayerFishLevelInfo(PBPlayer player)
    {
        foreach (var note in player.AryPlayerFishInfo)
        {
            if (note.FishId == player.FightFish) { return note; }
        }
        Debug.LogError("DataBank.GetCurrentPlayerFishLevelInfo()_1");
        return null;
    }
}

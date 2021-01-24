using System.Collections.Generic;

public class RankingRewardDataTableProxy : BaseDataTableProxy<RankingRewardDataTable, RankingRewardData, RankingRewardDataTableProxy>
{

    public RankingRewardDataTableProxy() : base("JsonData/RankingRewardData") {}
    public List<RankingRewardData> GetRewardList(int groupId)
    {
        List<RankingRewardData> rankingRewardDatas = new List<RankingRewardData>();
        var allData = GetAll();
        foreach (var note in allData)
        {
            if (note.groupId == groupId)
            {
                rankingRewardDatas.Add(note);
            }
        }
        return rankingRewardDatas;
    }
    public string GetRankingStr(int groupId, long rank, float rankRate)
    {
        var allData = GetRewardList(groupId);
        int rankMax = 0;
        foreach (var note in allData)
        {
            if (note.ranking > 0 && note.ranking > rankMax)
            {
                rankMax = note.ranking;
            }
        }
        if (rank <= rankMax)
        {
            return rank.ToString();
        }
        else
        {
            return rankRate + "%";
        }
    }
}
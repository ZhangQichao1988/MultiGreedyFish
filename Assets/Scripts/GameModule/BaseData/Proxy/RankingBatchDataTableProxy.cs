using System.Collections.Generic;

public class RankingBatchDataTableProxy : BaseDataTableProxy<RankingBatchDataTable, RankingBatchData, RankingBatchDataTableProxy>
{

    public RankingBatchDataTableProxy() : base("JsonData/RankingBatchData") {}

    public int GetGroupId(int batch)
    {
        var allData = GetAll();
        foreach (var note in allData)
        {
            if (note.batch == batch)
            {
                return note.groupId;
            }
        }
        return -1;
    }
}

public class NetWorkResponseCode
{
    public static int SUCEED = 0;
    public static int FAILED = 1;

    public static int ADS_NEED_RETRY = 5; 
    public static int NO_ENOUGH_DIAMOND = 6;
    public static int NO_ENOUGH_GOLD = 7;
    public static int GOOD_POOL_MAX_LEVEL = 8;

    //排行相关
    
    //请求的排行榜不存在
    public static int RANK_BOARD_NO_EXIST = 9;

    //没有当前玩家的排行
    public static int USER_NO_RANK = 10;

    //排行榜数据为空
    public static int NO_RANK_DATA = 11;
}
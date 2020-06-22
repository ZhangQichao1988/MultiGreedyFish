
public enum eResBundleFlags
{
    local = 0,
    remote = 1 << 0,//1
    preload = 1 << 1,//2
    optional = 1 << 2,//4
}

public enum eResBundleType : int
{
    local = eResBundleFlags.local,//0
    remote = eResBundleFlags.remote,//1
    localpreload = eResBundleFlags.local | eResBundleFlags.preload,//2
    remotepreload = eResBundleFlags.remote | eResBundleFlags.preload,//3
    //localoptional = eResBundleFlags.local | eResBundleFlags.optional,//4 不存在的类型
    remoteoptional = eResBundleFlags.remote | eResBundleFlags.optional,//5
}

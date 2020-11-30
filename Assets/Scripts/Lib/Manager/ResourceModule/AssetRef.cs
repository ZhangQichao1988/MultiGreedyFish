
using UnityEngine;


public class AssetRef : ObjectRef
{
    public Object Asset => m_Object;

    public AssetRef(RequestInfo info, Object asset) : base(asset)
    {
        Info = info;
    }

    public RequestInfo Info { get; }
}

public class AssetRef<T> : AssetRef where T : Object
{
    public new T Asset => m_Object as T;

    public AssetRef(RequestInfo info, Object asset) : base(info, asset)
    {
        
    }

}

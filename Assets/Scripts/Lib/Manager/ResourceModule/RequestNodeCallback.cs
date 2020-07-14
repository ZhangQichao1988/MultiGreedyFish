using UnityEngine;
using UnityEngine.Events;



/// <summary>
/// 用于通过回调把请求的对象返回
/// </summary>
public class RequestCallbackNode : RequestNode
{
    public UnityAction<int, int, AssetRef> Callback
    {
        get;
        private set;
    }

    public RequestCallbackNode(RequestGroup group, RequestInfo resInfo, UnityAction<int, int, AssetRef> callback)
        : base(group, resInfo)
    {
        Callback = callback;
    }

    public override void AssignLoaded(int groupHandle, int nodeHandle, AssetRef loaded)
    {
        Callback?.Invoke(groupHandle, nodeHandle, loaded);
    }
}

public class RequestCallbackNode<T> : RequestNode where T : Object
{
    public UnityAction<int, int, AssetRef<T>> Callback
    {
        get;
        private set;
    }

    public RequestCallbackNode(RequestGroup group, RequestInfo resInfo, UnityAction<int, int, AssetRef<T>> callback)
        : base(group, resInfo)
    {
        Callback = callback;
    }

    public override void AssignLoaded(int groupHandle, int nodeHandle, AssetRef loaded)
    {
        Callback?.Invoke(groupHandle, nodeHandle, loaded as AssetRef<T>);
    }
}


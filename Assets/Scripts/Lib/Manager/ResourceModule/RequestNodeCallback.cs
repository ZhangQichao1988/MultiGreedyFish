using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于通过回调把请求的对象返回
/// </summary>
public class RequestCallbackNode : RequestNode
{
    public UnityAction<int, int, Object> Callback
    {
        get;
        private set;
    }

    public RequestCallbackNode(RequestGroup group, RequestInfo resInfo, UnityAction<int, int, Object> callback)
        : base(group, resInfo)
    {
        Callback = callback;
    }

    public override void AssignLoaded(int groupHandle, int nodeHandle, Object loaded)
    {
        Callback?.Invoke(groupHandle, nodeHandle, loaded);
    }
}
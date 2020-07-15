using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RequestGroup
{
    static int handleCount;

    /// <summary>
    /// 组的唯一ID
    /// </summary>
    public int Handle
    {
        get;
        private set;
    }

    /// <summary>
    /// 请求完成进度
    /// </summary>
    public float Progress
    {
        get
        {
            if(_totalRequest > 0)
            {
                return 1.0f - (float)_requestNodes.Count / (float)_totalRequest;
            }
            return 0.0f;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return _requestNodes.Count <= 0;
        }
    }

    public bool IsComplete
    {
        get;
        set;
    }

    /// <summary>
    /// 总请求数
    /// </summary>
    private int _totalRequest;

    /// <summary>
    /// 请求节点
    /// </summary>
    private Dictionary<int, RequestNode> _requestNodes;

    /// <summary>
    /// 整个请求组完成的回调
    /// </summary>
    private UnityAction<int> _onComplete;

    /// <summary>
    /// 完成进度变化的回调
    /// </summary>
    private UnityAction<int, float> _onProgress;

    public RequestGroup(UnityAction<int, float> onProgress, UnityAction<int> onComplete)
    {
        _onProgress = onProgress;
        _onComplete = onComplete;
        Handle = ++handleCount;
        IsComplete = false;
        _requestNodes = new Dictionary<int, RequestNode>();
    }

    public void Prepare(Dictionary<RequestInfo, Request> requestsToLoad)
    {
        foreach(RequestNode requestNode in _requestNodes.Values)
        {
            Request request;

            if (!requestsToLoad.TryGetValue(requestNode.ResInfo, out request))
            {
                request = new Request(requestNode.ResInfo);

                requestsToLoad.Add(requestNode.ResInfo, request);
            }
            request.AddNode(requestNode);
        }
        _totalRequest = _requestNodes.Count;
    }

    public int AddRequestNode(RequestInfo resInfo, UnityAction<int, int, AssetRef> callback)
    {
        RequestNode requestNode = new RequestCallbackNode(this, resInfo, callback);

        _requestNodes.Add(requestNode.Handle, requestNode);

        return requestNode.Handle;
    }

    public int AddRequestNode<T>(RequestInfo resInfo, UnityAction<int, int, AssetRef<T>> callback) where T : Object
    {
        RequestNode requestNode = new RequestCallbackNode<T>(this, resInfo, callback);

        _requestNodes.Add(requestNode.Handle, requestNode);

        return requestNode.Handle;
    }

    public int AddRequestNode(RequestInfo resInfo, object propertyOwner, string propertyName, object propertyIndex)
    {
        RequestNode requestNode = new RequestNodeReflection(this, resInfo, propertyOwner, propertyName, propertyIndex);

        _requestNodes.Add(requestNode.Handle, requestNode);

        return requestNode.Handle;
    }

    public void OnRequestNodeComplete(int requestNodeHandle)
    {
        _requestNodes.Remove(requestNodeHandle);

        _onProgress?.Invoke(Handle, Progress);
    }

    public void OnComplete()
    {
        _onComplete?.Invoke(Handle);
    }
}

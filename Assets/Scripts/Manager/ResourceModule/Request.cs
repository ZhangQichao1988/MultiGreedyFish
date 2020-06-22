using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Request
{
    static int handleCount;

    public int Handle
    {
        get;
        private set;
    }

    public RequestInfo ResInfo
    {
        get;
        private set;
    }

    public bool IsEmpty
    {
        get
        {
            return _nodes.Count <= 0;
        }
    }

    public Coroutine coroutine;

    private List<RequestNode> _nodes;

    public Request(RequestInfo info)
    {
        Handle = ++handleCount;
        ResInfo = info;

        _nodes = new List<RequestNode>();
    }

    public void AddNode(RequestNode node)
    {
        _nodes.Add(node);
    }

    public void RemoveNode(int groupHandle, RequestNode node)
    {
        _nodes.Remove(node);
    }

    public void RemoveNodes(int groupHandle)
    {
        for(int i = _nodes.Count - 1; i >= 0; i--)
        {
            RequestNode requestNode = _nodes[i];

            if (requestNode.Group.Handle == groupHandle)
            {
                _nodes.RemoveAt(i);
            }
        }
    }

    public void OnComplete(Object loaded)
    {
        if (loaded == null)
        {
            Debug.LogError("Load resource failed, please check: " + ResInfo.path + " with type: " + ResInfo.type);
        }
        foreach (RequestNode node in _nodes)
        {
            node.OnComplete(loaded);
        }
        _nodes.Clear();
    }
}
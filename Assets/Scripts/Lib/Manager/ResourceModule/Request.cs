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
            return Nodes.Count <= 0;
        }
    }

    public Coroutine coroutine;

    public List<RequestNode> Nodes;

    public Request(RequestInfo info)
    {
        Handle = ++handleCount;
        ResInfo = info;

        Nodes = new List<RequestNode>();
    }

    public void AddNode(RequestNode node)
    {
        Nodes.Add(node);
    }

    public void RemoveNode(int groupHandle, RequestNode node)
    {
        Nodes.Remove(node);
    }

    public void RemoveNodes(int groupHandle)
    {
        for(int i = Nodes.Count - 1; i >= 0; i--)
        {
            RequestNode requestNode = Nodes[i];

            if (requestNode.Group.Handle == groupHandle)
            {
                Nodes.RemoveAt(i);
            }
        }
    }

    public void OnComplete(AssetRef loaded)
    {
        if (loaded == null)
        {
            Debug.LogError("Load resource failed, please check: " + ResInfo.path + " with type: " + ResInfo.type);
        }
        foreach (RequestNode node in Nodes)
        {
            node.OnComplete(loaded);
        }
        Nodes.Clear();
    }
}
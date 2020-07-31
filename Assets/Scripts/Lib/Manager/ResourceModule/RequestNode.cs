using UnityEngine;



public class RequestNode
{
    static int handleCount;

    public int Handle
    {
        get;
        private set;
    }

    public RequestGroup Group
    {
        get;
        private set;
    }

    public RequestInfo ResInfo
    {
        get;
        private set;
    }

    public RequestNode(RequestGroup group, RequestInfo resInfo)
    {
        Handle = ++handleCount;
        Group = group;
        ResInfo = resInfo;
    }

    public void OnComplete(AssetRef loaded)
    {
        AssignLoaded(Group.Handle, Handle, loaded);

        Group.OnRequestNodeComplete(Handle);
    }

    public virtual void AssignLoaded(int groupHandle, int nodeHandle, AssetRef loaded)
    {

    }
}

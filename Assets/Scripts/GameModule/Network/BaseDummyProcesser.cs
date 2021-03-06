using Google.Protobuf;
using NetWorkModule.Dummy;
public class BaseDummyProcesser<T, U>: IDummyResponseProcesser where T : class, IMessage where U : class, IMessage 
{
    System.Object cached;

    public IMessage ProcessRequest(int msgId, IMessage pbData)
    {
        return ProcessRequest(msgId, pbData as T);
    }
    public void DispatchRes(int msgId, IMessage request, IMessage response)
    {
        DispatchRes(msgId, request as T, response as U);
    }

    public System.Object GetCachedData()
    {
        return cached;
    }
    public void SetCachedData(System.Object obj)
    {
        cached = obj;
    }

    public virtual U ProcessRequest(int msgId, T pbData)
    {
        return default(U);
    }
    public virtual void DispatchRes(int msgId, T request, U response)
    {
        NetWorkHandler.GetDispatch().Dispatch<U>(NetWorkHandler.GetDispatchKey(msgId), response);
    }
    
}
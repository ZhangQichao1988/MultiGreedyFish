using Google.Protobuf;

namespace NetWorkModule.Dummy
{
    /// <summary>
    /// 假数据处理器
    /// </summary>
    public interface IDummyResponseProcesser
    {
         IMessage ProcessRequest(int msgId, IMessage pbData);
         void DispatchRes(int msgId, IMessage request, IMessage response);

         System.Object GetCachedData();
         void SetCachedData(System.Object obj);
    }
}
using Google.Protobuf;

namespace NetWorkModule.Dummy
{
    /// <summary>
    /// 假数据处理器
    /// </summary>
    public interface IDummyResponseProcesser
    {
         IMessage ProcessRequest(int resId, IMessage pbData);
         void DispatchRes(int resId, IMessage request, IMessage response);
    }
}
using Google.Protobuf;

namespace NetWorkModule.Dummy
{
    /// <summary>
    /// 假数据处理器
    /// </summary>
    public interface IDummyResponseProcesser
    {
         void ProcessRequest(int resId, IMessage pbData);
    }
}
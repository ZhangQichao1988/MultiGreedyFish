using Google.Protobuf;
using Google.Protobuf.Reflection;

public class NullMessage : IMessage
{
    public MessageDescriptor Descriptor { get; }

    public int CalculateSize()
    {
        return 0;
    }
    public void MergeFrom(CodedInputStream input)
    {

    }
    public void WriteTo(CodedOutputStream output)
    {

    }
}

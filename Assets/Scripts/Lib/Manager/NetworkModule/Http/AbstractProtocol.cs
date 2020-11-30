using System.Text;



namespace NetWorkModule
{
    public enum EncryptType
    {
        None,
        DES3,
    }

    public class PackData
    {
        public byte[] pbData;
        public int msgId;
        public long pid;
    }

    public abstract class AbstractProtocol
    {
        public EncryptType Encrypt
        {
            get;
            private set;
        }
        public string EncryptKey
        {
            get; set;
        }
        public void SetEncryptType(int type)
        {
            Encrypt = (EncryptType)type;
        }

        public abstract byte[] Pack(string msg, long pid, byte[] body);
        public abstract bool Parser(byte[] data, ref int offset, int length);
        
        public abstract PackData ParserOutput(byte[] data, int length);

        protected void EncryptBody(ref byte[] body)
        {
            if (Encrypt == EncryptType.DES3)
            {
                body = CryptographyUtil.GetDES3Bytes(body, UTF8Encoding.UTF8.GetBytes(EncryptKey));
            }
        }
        protected void DecryptBody(ref byte[] body)
        {
            if (Encrypt == EncryptType.DES3)
            {
                body = CryptographyUtil.DecryptDES3(body, UTF8Encoding.UTF8.GetBytes(EncryptKey));
            }
        }
    }
}
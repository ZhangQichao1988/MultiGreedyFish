using System;
using System.Text;
using System.IO;
using System.Net;
using NetWorkModule;


public class FishProtocol : AbstractProtocol
{
		static readonly int RecvHeaderSize = 16;

		MemoryStream m_ms = new MemoryStream(1024);

		public FishProtocol()
		{

		}

		//Length uint32 4
		//Cmd    uint32 4
		//Pid    uint64 8

		public override byte[] Pack(string msg, long pid, byte[] body)
		{
			//byte[] byte_body = body;// UTF8Encoding.UTF8.GetBytes(body);

			m_ms.Position = 0;

			if (body == null)
				Write(RecvHeaderSize);
			else
			{
				Write(body.Length + RecvHeaderSize);
			}

			string str_id = msg.Substring(1, msg.IndexOf("_") - 1);

			//Write(MxDispatcher.Instance.GetMsgID(msg));
			Write(int.Parse(str_id));
			Write(pid);

			if (body != null && body.Length > 0)
			{
				Write(body);
			}

			byte[] datas = new byte[m_ms.Position];
			m_ms.Position = 0;
			m_ms.Read(datas, 0, datas.Length);



			return datas;
		}

		public override PackData ParserOutput(byte[] data, int length)
		{
			_restLen = length;
			// The head isn't enough
			if (_restLen - RecvHeaderSize < 0)
				return null;

			// Decode packet header
			//int flag = data[0];
			//int id = BitConverter.ToInt32(data, offset + 1);
			//int group = BitConverter.ToInt32(data, offset + 5);
			//int len = BitConverter.ToInt32(data, offset + 9);

			// converts a value from network byte order to host byte order
			int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 0));
			int id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 4));
			long pid = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(data, 8));

			// The body isn't enough
			if (_restLen - len < 0)
			{
				return null;
			}

			byte[] body = null;

			int body_sz = len - RecvHeaderSize;

			if (body_sz > 0)
			{
				body = new byte[body_sz];
				Array.ConstrainedCopy(data, RecvHeaderSize, body, 0, len - RecvHeaderSize);
			}

			return new PackData(){
				pbData = body,
				msgId = id
			};
		}

		int _restLen;
		public override bool Parser(byte[] data, ref int offset, int length)
		{
			_restLen = length - offset;
			// The head isn't enough
			if (_restLen - RecvHeaderSize < 0)
				return false;

			// Decode packet header
			//int flag = data[0];
			//int id = BitConverter.ToInt32(data, offset + 1);
			//int group = BitConverter.ToInt32(data, offset + 5);
			//int len = BitConverter.ToInt32(data, offset + 9);

			// converts a value from network byte order to host byte order
			int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, offset + 0));
			int id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, offset + 4));
			long group = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(data, offset + 8));
			long pid = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(data, offset + 16));

			// The body isn't enough
			if (_restLen - len < 0)
			{
				return false;
			}
			byte[] body = null;

			int body_sz = len - RecvHeaderSize;

			if (body_sz > 0)
			{
				body = new byte[body_sz];
				Array.ConstrainedCopy(data, offset + RecvHeaderSize, body, 0, len - RecvHeaderSize);
                
			}

			// Seek to this message of tail
			offset += (len);

			return true;
		}



		public void Write(byte o)
		{
			m_ms.WriteByte(o);
		}

		public void Write(int o)
		{
			byte[] datas = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(o));
			m_ms.Write(datas, 0, datas.Length);
		}

		public void Write(long o)
		{
			byte[] datas = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(o));
			m_ms.Write(datas, 0, datas.Length);
		}

		public void Write(byte[] o)
		{
			m_ms.Write(o, 0, o.Length);
		}

		public void Write(string o)
		{
			byte[] datas = UTF8Encoding.UTF8.GetBytes(o);
			m_ms.Write(datas, 0, datas.Length);
		}
}

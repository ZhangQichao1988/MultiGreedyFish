using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptographyUtil
{
	public static string GetFileHash(string path)
	{
		using (FileStream fs = File.OpenRead(path))
		{
			SHA1 sha1 = new SHA1CryptoServiceProvider();

			byte[] bytes = sha1.ComputeHash(fs);

			return GetSHA1BytesString(bytes);
		}
	}

	//SHA1

	public static byte[] GetSHA1Bytes(byte[] datas, int offset = 0)
	{
		SHA1 sha1 = new SHA1CryptoServiceProvider();
		byte[] ret = sha1.ComputeHash(datas, offset, datas.Length - offset);
		return ret;
	}

	public static string GetSHA1BytesString(byte[] datas, int offset = 0)
	{
		byte[] ret = GetSHA1Bytes(datas, offset);
		StringBuilder sb = new StringBuilder();
		foreach (byte b in ret)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();
	}

	public static string GetFileSHA1(string fn, int offset = 0)
	{
		byte[] datas = File.ReadAllBytes(fn);
		return GetSHA1BytesString(datas, offset);
	}

	public static string GetSHA1(string datas, int offset = 0)
	{
		return GetSHA1BytesString(Encoding.UTF8.GetBytes(datas), offset);
	}

	public static byte[] GetMd5Bytes(byte[] datas, int offset = 0)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] ret = md5.ComputeHash(datas, offset, datas.Length - offset);

		return ret;
	}

	//MD5

	public static string GetMd5BytesString(byte[] datas, int offset = 0)
	{
		byte[] ret = GetMd5Bytes(datas, offset);
		StringBuilder sb = new StringBuilder();
		foreach (byte b in ret)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();
	}

	public static string GetFileMd5(string fn, int offset = 0)
	{
		byte[] datas = File.ReadAllBytes(fn);
		return GetMd5BytesString(datas, offset);
	}

	public static string GetMd5(string datas, int offset = 0)
	{
		return GetMd5BytesString(Encoding.UTF8.GetBytes(datas), offset);
	}

	public static string GetDES3(string text, string str_key)
	{
		byte[] data = UTF8Encoding.UTF8.GetBytes(text);
		byte[] key = UTF8Encoding.UTF8.GetBytes(str_key);
		byte[] ret = GetDES3Bytes(data, key);

		return Convert.ToBase64String(ret);
	}

	//DES3

	public static byte[] GetDES3Bytes(byte[] data, byte[] key)
	{
		try
		{
			// Create a MemoryStream.
			MemoryStream mStream = new MemoryStream();
			TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
			tdsp.Mode = CipherMode.ECB;             //默认值
			tdsp.Padding = PaddingMode.PKCS7;       //默认值
													// Create a CryptoStream using the MemoryStream 
													// and the passed key and initialization vector (IV).
			CryptoStream cStream = new CryptoStream(mStream,
													tdsp.CreateEncryptor(key, null),
													CryptoStreamMode.Write);
			// Write the byte array to the crypto stream and flush it.
			cStream.Write(data, 0, data.Length);
			cStream.FlushFinalBlock();
			// Get an array of bytes from the 
			// MemoryStream that holds the 
			// encrypted data.
			byte[] ret = mStream.ToArray();
			// Close the streams.
			cStream.Close();
			mStream.Close();

			// Return the encrypted buffer.
			return ret;
		}
		catch (CryptographicException e)
		{
			string err = e.Message;
			Console.WriteLine("A Cryptographic error occurred: {0}", err);
			return null;
		}
	}

	public static byte[] DecryptDES3(byte[] data, byte[] key)
	{
		try
		{
			// Create a MemoryStream.
			MemoryStream mStream = new MemoryStream();
			TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
			tdsp.Mode = CipherMode.ECB;             //默认值
			tdsp.Padding = PaddingMode.PKCS7;       //默认值
													// Create a CryptoStream using the MemoryStream 
													// and the passed key and initialization vector (IV).
			CryptoStream cStream = new CryptoStream(mStream,
													tdsp.CreateDecryptor(key, null),
													CryptoStreamMode.Write);
			// Write the byte array to the crypto stream and flush it.
			cStream.Write(data, 0, data.Length);
			cStream.FlushFinalBlock();
			// Get an array of bytes from the 
			// MemoryStream that holds the 
			// encrypted data.
			byte[] ret = mStream.ToArray();
			// Close the streams.
			cStream.Close();
			mStream.Close();

			// Return the encrypted buffer.
			return ret;
		}
		catch (CryptographicException e)
		{
			string err = e.Message;
			Console.WriteLine("A Cryptographic error occurred: {0}", err);
			return null;
		}
	}
	static RSACryptoServiceProvider rsa = CreateRSAProvider();
	static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

	static RSACryptoServiceProvider CreateRSAProvider()
	{
		var p = new RSACryptoServiceProvider(1024);
		p.FromXmlString(AppConst.RsaPublicKey);
		return p;
	}

	public static byte[] PublicEncrypt(byte[] data)
	{
		return rsa.Encrypt(data, false);
	}

	public static byte[] RandomBytes(int len)
	{
		byte[] res = new byte[len];
		rng.GetBytes(res);
		return res;
	}

	//request mask data
	public static string GetMakData(byte[] randomBytes)
	{
		return Convert.ToBase64String(PublicEncrypt(randomBytes));
	}
	
	public static byte[] HmacSha1(byte[] data, byte[] key)
	{
		return new HMACSHA1(key).ComputeHash(data);
	}
	
	public static string Hexlify(byte[] data)
	{
		if (data.Length == 0)
		{
			return string.Empty;
		}

		var s = new StringBuilder(data.Length * 2);
		for (int i = 0; i < data.Length; i++)
		{
			s.Append(data[i].ToString("x2"));
		}

		return s.ToString();
	}
	
	public static byte[] XorBytes(byte[] l, byte[] r)
	{
		if (l.Length != r.Length)
		{
			throw new ArgumentException();
		}

		var res = new byte[l.Length];
		for (int i = 0; i < l.Length; i++)
		{
			res[i] = (byte)(l[i] ^ r[i]);
		}

		return res;
	}
	
}
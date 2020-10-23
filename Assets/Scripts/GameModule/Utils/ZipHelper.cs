using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class ZipHelper
{
    /// <summary>
    /// 解压缩一个 zip 文件。
    /// </summary>
    /// <param name="zipedFile">The ziped file.</param>
    /// <param name="strDirectory">The STR directory.</param>
    /// <param name="password">zip 文件的密码。</param>
    /// <param name="overWrite">是否覆盖已存在的文件。</param>
    public static void UnZip(byte[] zipStream, string strDirectory, string password = "", bool overWrite = true)
    {
 
        if (string.IsNullOrEmpty(strDirectory))
        {
            throw new DirectoryNotFoundException("Direction is not found!!");
        }
        var fileStream = new MemoryStream(zipStream);
 
        using (ZipInputStream s = new ZipInputStream(fileStream))
        {
            s.Password = password;
            ZipEntry theEntry;
 
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = "";
                string pathToZip = "";
                pathToZip = theEntry.Name;
 
                if (pathToZip != "")
                {
                    directoryName = Path.GetDirectoryName(pathToZip);
                }
 
                string fileName = Path.GetFileName(pathToZip);
                string savedDir = Path.Combine(strDirectory, directoryName);
                if (!Directory.Exists(savedDir))
                {
                    Directory.CreateDirectory(savedDir);
                }
 
                if (!string.IsNullOrEmpty(fileName))
                {
                    using (FileStream streamWriter = File.Create( Path.Combine(savedDir, fileName)))
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);

                            if (size > 0)
                                streamWriter.Write(data, 0, size);
                            else
                                break;
                        }
                        streamWriter.Close();
                    }
                }
            }
 
            s.Close();
        }
    }

    const uint LCG_A = 214013;
    const uint LCG_C = 2531011;
    public static void DesMasterFile(byte[] b)
    {
        uint key1 = 100;
        uint key2 = 200;
        uint key3 = 300;

        for (int i = 0 ; i < b.Length; i++)
        {   
            b[i] ^= (byte)((key1 >> 24) ^ (key2 >> 24) ^ (key3 >> 24));

            key1 = key1 * LCG_A + LCG_C;
            key2 = key2 * LCG_A + LCG_C;
            key3 = key3 * LCG_A + LCG_C;
        }   
    }
}
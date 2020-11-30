using System.IO;
using System.Runtime.InteropServices;

public class DesStream : FileStream
{
    private int _key;
    public DesStream(string path, FileMode mode, int key):base(path, mode, FileAccess.Read)
    {
        _key = key;
    }

    public override int Read(byte[] array, int offset, int count)
    {
        int startPos = ((int)Position % 20 == 0) ? 0 : (20 - ((int)Position % 20));
        var index =  base.Read(array, offset, count);
#if UNITY_EDITOR || AIRTEST_ENABLE
        for (int i = startPos ; i < array.Length; i += 20)
        {   
            array[i] ^= (byte)(_key >> 12);
        }
#else
        LapisExtern._Lapis_Decrypt(array, _key, startPos, array.Length);
#endif
        return index;
    }

	public class LapisExtern
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
#else
        [DllImport("Lapis")]
#endif
		/// <summary>
		/// 解压
		/// </summary>
		/// <param name="[MarshalAs(UnmanagedType.LPArray"></param>
		/// <param name="2"></param>
		/// <returns></returns>
        public static extern void _Lapis_Decrypt(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] array,
            int key,
            int start,
            int size
        );
	}
}
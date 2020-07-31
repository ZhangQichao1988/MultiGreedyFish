using System;

public class BlEnumUtil
{
    //for lua 字符串 转 int
    public static int ToInt(string type, string eString)
    {
        return (int)Enum.Parse(Type.GetType(type), eString);
    }

    //for lua int 转 字符串
    public static string ToString(string type, int eInteger)
    {
        return Enum.GetName(Type.GetType(type), eInteger);
    }

    //for lua 字符串 转 枚举对象
    public static object ToEnum(string type, string eString)
    {
        return Enum.Parse(Type.GetType(type), eString);
    }

    //for lua 获取枚举类型数
    public static int GetEnumCount(string type)
    {
        return Enum.GetValues(Type.GetType(type)).Length;
    }

    public static T ToEnum<T>(string eString)
    {
        return (T)Enum.Parse(typeof(T), eString);
    }

    public static T ToEnum<T>(int iValue)
    {
        return (T)Enum.ToObject(typeof(T), iValue);
    }
    /// <summary>
    /// 指定された文字列を列挙型に変換します
    /// </summary>
    /// <typeparam name="T">列挙型</typeparam>
    /// <param name="value">変換する文字列</param>
    /// <param name="ignoreCase">大文字と小文字を区別しない場合は true</param>
    /// <returns>列挙型のオブジェクト</returns>
    public static T Parse<T>(string value, bool ignoreCase)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    /// <summary>
    /// 指定された文字列を列挙型に変換して成功したかどうかを返します
    /// </summary>
    /// <typeparam name="T">列挙型</typeparam>
    /// <param name="value">変換する文字列</param>
    /// <param name="result">列挙型のオブジェクト</param>
    /// <returns>正常に変換された場合は true</returns>
    public static bool TryParse<T>(string value, out T result)
    {
        return TryParse<T>(value, true, out result);
    }

    /// <summary>
    /// 指定された文字列を列挙型に変換して成功したかどうかを返します
    /// </summary>
    /// <typeparam name="T">列挙型</typeparam>
    /// <param name="value">変換する文字列</param>
    /// <param name="ignoreCase">大文字と小文字を区別しない場合は true</param>
    /// <param name="result">列挙型のオブジェクト</param>
    /// <returns>正常に変換された場合は true</returns>
    public static bool TryParse<T>(string value, bool ignoreCase, out T result)
    {
        try
        {
            result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            return true;
        }
        catch
        {
            result = default(T);
            return false;
        }
    }
}
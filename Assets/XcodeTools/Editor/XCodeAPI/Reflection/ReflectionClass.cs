using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// リフレクションクラス
/// </summary>
public class ReflectionClass
{
    public object Instance { get; private set; }
    private Type instanceType;
    private BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// インスタンスをリフレクションクラス化したい場合のコンストラクター
    /// インスタンスの型がPublicの場合
    /// </summary>
    /// <param name="instance">リフレクションクラス化したいインスタンス</param>
    /// <exception cref="ArgumentException"></exception>
    public ReflectionClass(object instance)
    {
        if (instance == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instance"));
        }
        Instance = instance;

        instanceType = instance.GetType();
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }
    }

    /// <summary>
    /// インスタンスをリフレクションクラス化したい場合のコンストラクター
    /// インスタンスの型がInternalの場合
    /// 
    /// Internalクラスと同じアセンブリにあるPublicクラスの型情報から、アセンブリを特定し、ネームスペースと型名からクラスの型を特定します
    /// </summary>
    /// <param name="instance">リフレクションクラス化したいインスタンス</param>
    /// <param name="sameAssemblyClassType">Internalクラスと同じアセンブリにあるPublicクラス</param>
    /// <param name="nameSpace">Internalクラスのネームスペース</param>
    /// <param name="classTypeStr">クラスの型名</param>
    /// <exception cref="ArgumentException"></exception>
    public ReflectionClass(object instance, Type sameAssemblyClassType, string nameSpace, string classTypeStr)
    {
        if (instance == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instance"));
        }
        Instance = instance;

        var assembly = Assembly.GetAssembly(sameAssemblyClassType);
        if (assembly == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "assembly"));
        }

        instanceType = assembly.GetType(nameSpace + "." + classTypeStr);
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }
    }

    /// <summary>
    /// Internalクラスをnewしてリフレクションクラス化したい場合のコンストラクター
    /// 
    /// Internalクラスと同じアセンブリにあるPublicクラスの型情報から、アセンブリを特定し、ネームスペースと型名からクラスの型を特定します
    /// </summary>
    /// <param name="sameAssemblyClassType">Internalクラスと同じアセンブリにあるPublicクラス</param>
    /// <param name="nameSpace">Internalクラスのネームスペース</param>
    /// <param name="classTypeStr">クラスの型名</param>
    /// <param name="types">Internalクラスのコンストラクターの引数の型</param>
    /// <param name="objects">Internalクラスのコンストラクターの引数</param>
    /// <exception cref="ArgumentException"></exception>
    public ReflectionClass(Type sameAssemblyClassType, string nameSpace, string classTypeStr, Type[] types, object[] objects)
    {
        var assembly = Assembly.GetAssembly(sameAssemblyClassType);
        if (assembly == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "assembly"));
        }

        instanceType = assembly.GetType(nameSpace + "." + classTypeStr);
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }

        Instance = instanceType.GetConstructor(types).Invoke(objects);
        if (Instance == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "Instance"));
        }
    }

    /// <summary>
    /// フィールドを取得する
    /// </summary>
    /// <param name="name">フィールド名</param>
    /// <typeparam name="T">フィールドの型</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public T GetField<T>(string name)
    {
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }

        var fieldInfo = instanceType.GetField(name, bindingFlags);

        return (T) fieldInfo.GetValue(Instance);
    }

    /// <summary>
    /// フィールドへ値を設定する
    /// </summary>
    /// <param name="name">フィールド名</param>
    /// <param name="value">値</param>
    /// <exception cref="ArgumentException"></exception>
    public void SetField(string name, object value)
    {
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }

        var fieldInfo = instanceType.GetField(name, bindingFlags);

        fieldInfo.SetValue(Instance, value);
    }

    /// <summary>
    /// プロパティーを取得する
    /// </summary>
    /// <param name="name">プロパティー名</param>
    /// <typeparam name="T">プロパティーの型<</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public T GetProperty<T>(string name)
    {
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }

        var propertyInfo = instanceType.GetProperty(name, bindingFlags);

        return (T) propertyInfo.GetValue(Instance, null);
    }

    /// <summary>
    /// メソッドを実行する
    /// 戻り値がある場合
    /// </summary>
    /// <param name="name">メソッド名</param>
    /// <param name="parameters">メソッドの引数</param>
    /// <typeparam name="T">戻り値の型</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public T ExecuteMethod<T>(string name, object[] parameters)
    {
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }
        var invokeBinding = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;
        return (T)Instance.GetType().InvokeMember(name, invokeBinding, null, Instance, parameters);
    }

    /// <summary>
    /// メソッドを実行する
    /// 戻り値がない場合
    /// </summary>
    /// <param name="name">メソッド名</param>
    /// <param name="parameters">メソッドの引数</param>
    /// <exception cref="ArgumentException"></exception>
    public void ExecuteMethod(string name, object[] parameters)
    {
        if (instanceType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "instanceType"));
        }
        var invokeBinding = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;
        Instance.GetType().InvokeMember(name, invokeBinding, null, Instance, parameters);
    }

    /// <summary>
    /// 静的メソッドを実行する
    /// 戻り値がある場合
    /// </summary>
    /// <param name="sameAssemblyClassType">静的メソッドの含まれているInternalクラスと同じアセンブリにあるPublicクラス</param>
    /// <param name="nameSpace">静的メソッドの含まれているInternalクラスのネームスペース</param>
    /// <param name="classTypeStr">クラスの型名</param>
    /// <param name="methodNameStr">メソッド名</param>
    /// <param name="parameters">メソッドの引数</param>
    /// <typeparam name="T">戻り値の型</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T ExecuteStaticMethod<T>(Type sameAssemblyClassType, string nameSpace, string classTypeStr,
        string methodNameStr,
        object[] parameters)
    {
        var assembly = Assembly.GetAssembly(sameAssemblyClassType);
        if (assembly == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "assembly"));
        }

        var classType = assembly.GetType(nameSpace + "." + classTypeStr);
        if (classType == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "classType"));
        }
        var methodInfo = classType.GetMethod(methodNameStr);
        if (methodInfo == null)
        {
            throw new ArgumentException(string.Format("Illegal argument. {0} is null.", "methodInfo"));
        }

        return (T) methodInfo.Invoke(null, parameters);
    }
}

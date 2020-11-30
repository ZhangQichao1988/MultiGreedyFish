using System;



public class RequestInfo
{
    public string path
    {
        get;
        private set;
    }
    public Type type
    {
        get;
        private set;
    }

    public RequestInfo(string path, Type type)
    {
        this.path = path;
        this.type = type;
    }

    public override int GetHashCode()
    {
        return (path + type.Name).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        RequestInfo key = obj as RequestInfo;
        if (key != null)
        {
            return key.path.Equals(path) && key.type.Equals(type);
        }
        else
        {
            return false;
        }
    }
}

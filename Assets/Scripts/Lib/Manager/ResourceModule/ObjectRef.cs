
using UnityEngine;


public class ObjectRef
{
    protected Object m_Object;

    public int RefCount { get; private set; }

    public ObjectRef(Object obj)
    {
        m_Object = obj;
        RefCount = 0;
    }

    public int IncRef()
    {
        return ++RefCount;
    }

    public int DecRef()
    {
        return --RefCount;
    }

    public void Dispose()
    {
        RefCount = 0;
        m_Object = null;
    }
}

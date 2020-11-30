using System.Reflection;

    /// <summary>
    /// 用于通过反射机制把请求到的对象返回
    /// </summary>
public class RequestNodeReflection : RequestNode
{
    private object _propertyOwner;
    private PropertyInfo _propertyInfo;
    private object _propertyIndex;

    public RequestNodeReflection(RequestGroup group, RequestInfo resInfo, object propertyOwner, string propertyName, object index)
        : base(group, resInfo)
    {
        _propertyOwner = propertyOwner;
        _propertyIndex = index;

        if (resInfo.type != null)
        {
            _propertyInfo = _propertyOwner.GetType().GetProperty(propertyName, typeof(AssetRef<>).MakeGenericType(resInfo.type));
        }
        else
        {
            _propertyInfo = _propertyOwner.GetType().GetProperty(propertyName);
        }
    }

    public override void AssignLoaded(int groupHandle, int nodeHandle, AssetRef loaded)
    {
        if (_propertyOwner != null && _propertyInfo != null)
        {
            if (_propertyIndex == null)
            {
                _propertyInfo.SetValue(_propertyOwner, loaded, null);
            }
            else
            {
                _propertyInfo.SetValue(_propertyOwner, loaded, new object[] { _propertyIndex });
            }
        }
    }
}
using UnityEngine;

public class AssetBundleRef
{
	public AssetBundle Bundle
	{
		get
		{
			return _assetBundle;
		}
	}
	private AssetBundle _assetBundle;

	private DesStream _desStream;

	private int _referencedCount;

	public AssetBundleRef(AssetBundle assetBundle)
	{
		_assetBundle = assetBundle;

		_referencedCount = 1;
	}

	public AssetBundleRef(AssetBundle assetBundle, DesStream desStream)
	{
		_assetBundle = assetBundle;
		_desStream = desStream;

		_referencedCount = 1;
	}

	public int IncRef()
	{
		return ++_referencedCount;
	}

	public int DecRef()
	{
		return --_referencedCount;
	}

	public void Unload(bool unloadAllLoadedObjects)
	{
		if(_assetBundle != null)
		{
			_assetBundle.Unload(unloadAllLoadedObjects);
		}
		if (_desStream != null)
		{
			_desStream.Dispose();
		}
	}
}
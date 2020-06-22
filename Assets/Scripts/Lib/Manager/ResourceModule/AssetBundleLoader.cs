using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AssetBundleLoader
{
	/// <summary>
	/// Key: AssetBundleFileName
	/// Value: MxAssetBundle
	/// </summary>
	private Dictionary<string, AssetBundleRef> _loadedAssetBundles;

	/// <summary>
	/// manifest
	/// </summary>
	private BundlesManifest _manifest;

	public AssetBundleLoader()
	{
		_loadedAssetBundles = new Dictionary<string, AssetBundleRef>();
	}

	public IEnumerator Initialize(ResourceBundleManifest manifest)
	{
		_manifest = new BundlesManifest(manifest);

		string[] preloadAssetBundles = _manifest.GetAllPreloadAssetBundles();

		for (int i = 0; i < preloadAssetBundles.Length; i++)
		{
			string assetBundleName = preloadAssetBundles[i];

			LoadAssetBundle(assetBundleName);

			yield return null;
		}
	}

	public void Destroy()
	{
		foreach (AssetBundleRef assetBundle in _loadedAssetBundles.Values)
		{
			assetBundle.Bundle.Unload(false);
		}
		_loadedAssetBundles.Clear();
	}
	
	public AssetBundleRef GetLoadedAssetBundle(string assetBundleName)
	{
		AssetBundleRef assetBundle;

		if (_loadedAssetBundles.TryGetValue(assetBundleName, out assetBundle))
		{
			return assetBundle;
		}
		return null;
	}

	public UnityEngine.Object LoadAssetSync(string assetBundleName, string assetName)
	{
		AssetBundleRef assetBundle = GetLoadedAssetBundle(assetBundleName);

		if (assetBundle != null && assetBundle.Bundle != null)
		{
			return assetBundle.Bundle.LoadAsset(assetName);
		}
		return null;
	}

	public UnityEngine.Object LoadAssetSync(string assetBundleName, string assetName, Type type)
	{
		AssetBundleRef assetBundle = GetLoadedAssetBundle(assetBundleName);

		if (assetBundle != null && assetBundle.Bundle != null)
		{
			return assetBundle.Bundle.LoadAsset(assetName, type);
		}
		return null;
	}

	public void UnloadAssetBundle(string assetBundleName)
	{
		UnloadAllDependencies(assetBundleName);

		UnloadAssetBundleInternal(assetBundleName);
	}

	public AssetBundleRef LoadAssetBundle(string assetBundleName)
	{
		LoadAllDependencies(assetBundleName);

		AssetBundleRef assetBundle = LoadAssetBundleInternal(assetBundleName);

		return assetBundle;
	}

	public AssetBundleRequest LoadAssetAsync(string assetBundleName, string assetName, Type type)
	{
		AssetBundleRef assetBundle = GetLoadedAssetBundle(assetBundleName);

		if (assetBundle != null && assetBundle.Bundle != null)
		{
			return assetBundle.Bundle.LoadAssetAsync(assetName, type);
		}
		return null;
	}

	public IEnumerator LoadAssetBundleAysc(string assetBundleName)
	{
		yield return LoadAllDependenciesAysc(assetBundleName);

		yield return LoadAssetBundleInternalAysc(assetBundleName);
	}

	private IEnumerator LoadAllDependenciesAysc(string assetBundleName)
	{
		string[] dependencies = _manifest.GetAllDependencies(assetBundleName);

		if (dependencies != null)
		{
			for (int i = 0; i < dependencies.Length; i++)
			{
				yield return LoadAssetBundleInternalAysc(dependencies[i]);
			}
		}
	}

	private IEnumerator LoadAssetBundleInternalAysc(string assetBundleName)
	{
		AssetBundleRef assetBundle = GetLoadedAssetBundle(assetBundleName);

		if (assetBundle != null)
		{
			assetBundle.IncRef();
			yield break;
		}

		string assetBundleFileName;
		if (_manifest.GetAssetBundleFileName(assetBundleName, out assetBundleFileName))
		{
			string assetBundleFilePath = _manifest.GetAssetBundleLoadPath(assetBundleName) + assetBundleFileName;

			int encryptKey = _manifest.GetAssetBundleEncKey(assetBundleName);

			if (encryptKey != 0)
			{
				DesStream stream = new DesStream(assetBundleFilePath, System.IO.FileMode.Open, encryptKey);
				AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromStreamAsync(stream);

				yield return bundleLoadRequest;

				if (bundleLoadRequest != null)
				{
					assetBundle = new AssetBundleRef(bundleLoadRequest.assetBundle, stream);
					_loadedAssetBundles.Add(assetBundleName, assetBundle);
				}
				else
				{
					stream.Dispose();
				}
			}
			else
			{
				AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(assetBundleFilePath);

				yield return bundleLoadRequest;

				if (bundleLoadRequest != null)
				{
					assetBundle = new AssetBundleRef(bundleLoadRequest.assetBundle);
					_loadedAssetBundles.Add(assetBundleName, assetBundle);
				}
			}				
		}
	}

	private AssetBundleRef LoadAssetBundleInternal(string assetBundleName)
	{
		AssetBundleRef assetBundle = GetLoadedAssetBundle(assetBundleName);

		if (assetBundle != null)
		{
			assetBundle.IncRef();

			return assetBundle;
		}
		string assetBundleFileName;

		if (_manifest.GetAssetBundleFileName(assetBundleName, out assetBundleFileName))
		{
			string assetBundleFilePath = _manifest.GetAssetBundleLoadPath(assetBundleName) + assetBundleFileName;
#if UNITY_IOS || UNITY_EDITOR
			// ios full pack support
			assetBundleFilePath = System.IO.File.Exists(assetBundleFilePath) ? assetBundleFilePath : _manifest.GetAssetBundleLoadPath(assetBundleName, true) + assetBundleFileName;
#endif

			int encryptKey = _manifest.GetAssetBundleEncKey(assetBundleName);

			if(encryptKey != 0)
			{
				DesStream stream = new DesStream(assetBundleFilePath, System.IO.FileMode.Open, encryptKey);
				AssetBundle ab = AssetBundle.LoadFromStream(stream);

				if (ab != null)
				{
					assetBundle = new AssetBundleRef(ab, stream);

					_loadedAssetBundles.Add(assetBundleName, assetBundle);

					return assetBundle;
				}
				else
				{
					stream.Dispose();
				}
			}
			else
			{
				AssetBundle ab = AssetBundle.LoadFromFile(assetBundleFilePath);

				if (ab != null)
				{
					assetBundle = new AssetBundleRef(ab);

					_loadedAssetBundles.Add(assetBundleName, assetBundle);

					return assetBundle;
				}
			}	
		}
		return null;
	}

	private void UnloadAssetBundleInternal(string assetBundleName)
	{
		AssetBundleRef bundle = GetLoadedAssetBundle(assetBundleName);
		if (bundle == null)
			return;

		if (bundle.DecRef() == 0)
		{
			bundle.Unload(false);


			_loadedAssetBundles.Remove(assetBundleName);
		}
	}

	private void LoadAllDependencies(string assetBundleName)
	{
		string[] dependencies = _manifest.GetAllDependencies(assetBundleName);

		if (dependencies != null)
		{
			for (int i = 0; i < dependencies.Length; i++)
			{
				LoadAssetBundleInternal(dependencies[i]);
			}
		}
	}

	private void UnloadAllDependencies(string assetBundleName)
	{
		string[] dependencies = _manifest.GetAllDependencies(assetBundleName);

		if (dependencies != null)
		{
			for (int i = 0; i < dependencies.Length; i++)
			{
				UnloadAssetBundleInternal(dependencies[i]);
			}
		}
	}

	public bool ExistAssetBundleName(string assetBundleName)
	{
		return _manifest.Exist(assetBundleName);
	}	
}

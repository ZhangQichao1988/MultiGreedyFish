using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssetBundleLoader
{
#if UNITY_EDITOR
	/// <summary>
	/// 用于编辑器模式下，显示AssetBundle加载情况
	/// </summary>
	public static List<AssetBundleRef> AssetBundleRefs
	{
		get;
		private set;
	}
#endif
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

#if UNITY_EDITOR
		AssetBundleRefs = new List<AssetBundleRef>();
#endif
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

#if UNITY_EDITOR
		AssetBundleRefs.Clear();
#endif
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

	public void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjectsbool = false)
	{
		UnloadAllDependencies(assetBundleName, unloadAllLoadedObjectsbool);

		UnloadAssetBundleInternal(assetBundleName, unloadAllLoadedObjectsbool);
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
#if UNITY_IOS || UNITY_EDITOR
			// ios full pack support
			assetBundleFilePath = PathUtility.GetExistAssetPath(assetBundleFilePath, ref assetBundleFileName);
#endif

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
#if UNITY_EDITOR
					AssetBundleRefs.Add(assetBundle);
#endif
				}
				else
				{
					Debug.LogError("Load assetbundle: " + assetBundleFilePath + " failed.");

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

#if UNITY_EDITOR
					AssetBundleRefs.Add(assetBundle);
#endif
				}
				else
				{
					Debug.LogError("Load assetbundle: " + assetBundleFilePath + " failed.");
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
			assetBundleFilePath = PathUtility.GetExistAssetPath(assetBundleFilePath, ref assetBundleFileName);
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
#if UNITY_EDITOR
					AssetBundleRefs.Add(assetBundle);
#endif
					return assetBundle;
				}
				else
				{
					Debug.LogError("Load assetbundle: " + assetBundleFilePath + " failed.");

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
#if UNITY_EDITOR
					AssetBundleRefs.Add(assetBundle);
#endif
					return assetBundle;
				}
				else
				{
					Debug.LogError("Load assetbundle: " + assetBundleFilePath + " failed.");
				}
			}	
		}
		return null;
	}

	private void UnloadAssetBundleInternal(string assetBundleName, bool unloadAllLoadedObjectsbool = false)
	{
		AssetBundleRef bundle = GetLoadedAssetBundle(assetBundleName);

		if (bundle == null)
			return;

		if (bundle.DecRef() == 0)
		{
			bundle.Unload(unloadAllLoadedObjectsbool);


			_loadedAssetBundles.Remove(assetBundleName);

#if UNITY_EDITOR
			AssetBundleRefs.Remove(bundle);
#endif
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

	private void UnloadAllDependencies(string assetBundleName, bool unloadAllLoadedObjectsbool = false)
	{
		string[] dependencies = _manifest.GetAllDependencies(assetBundleName);

		if (dependencies != null)
		{
			for (int i = 0; i < dependencies.Length; i++)
			{
				UnloadAssetBundleInternal(dependencies[i], unloadAllLoadedObjectsbool);
			}
		}
	}

	public bool ExistAssetBundleName(string assetBundleName)
	{
		return _manifest.Exist(assetBundleName);
	}	
}

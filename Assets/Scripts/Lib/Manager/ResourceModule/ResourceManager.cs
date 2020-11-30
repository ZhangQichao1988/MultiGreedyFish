using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;


[ExecuteInEditMode]
public class ResourceManager : MonoBehaviour
{
    private static ResourceManager Instance { get; set; }

    /// <summary>
    /// 最大同时执行的加载请求数量
    /// </summary>
    private static uint maxLoadingRequest = 4;

    /// <summary>
    /// 正在接受注册请求的组
    /// </summary>
    private static Stack<RequestGroup> _acceptingRequestGroups;

    /// <summary>
    /// 完成注册请求的组
    /// </summary>
    private static List<RequestGroup> _groups;

    /// <summary>
    /// 还未开始加载的请求
    /// </summary>
    private static Dictionary<RequestInfo, Request> _requestsToLoad;

    /// <summary>
    /// 正在加载的请求
    /// </summary>
    private static Dictionary<int, Request> _loadingRequests;

    /// <summary>
    /// AssetBundle加载器
    /// </summary>
    private static AssetBundleLoader m_AssetBundleLoader;

    /// <summary>
    /// 资源缓存器
    /// </summary>
    private static AssetsCacher m_AssetCacher;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _acceptingRequestGroups = new Stack<RequestGroup>();
        _groups = new List<RequestGroup>();
        _requestsToLoad = new Dictionary<RequestInfo, Request>();
        _loadingRequests = new Dictionary<int, Request>();

        m_AssetBundleLoader = new AssetBundleLoader();
        m_AssetCacher = new AssetsCacher();
    }

    private void OnDestroy()
    {
        if (m_AssetCacher != null)
        {
            m_AssetCacher.Clear();
        }

        if (m_AssetBundleLoader != null)
        {
            m_AssetBundleLoader.Destroy();
        }

        _loadingRequests.Clear();
        _requestsToLoad.Clear();
        _groups.Clear();
        _acceptingRequestGroups.Clear();

        if (Instance != null)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (m_AssetCacher != null)
        {
            m_AssetCacher.Update();
        }

        //当存在还未开始加载的请求，并且正在加载的请求数小于最大允许数量
        while (_requestsToLoad.Count > 0 && _loadingRequests.Count < maxLoadingRequest)
        {
            //取出一个还未开始加载的请求
            Request request = _requestsToLoad.First().Value;

            _requestsToLoad.Remove(request.ResInfo);

            //放入正在加载的请求列队
            _loadingRequests.Add(request.Handle, request);

            //启动一个新的加载协程
            request.coroutine = StartCoroutine(AsyncLoad(request));
        }

        if(_groups.Count > 0)
        {
            for(int i = _groups.Count - 1; i >= 0; i--)
            {
                RequestGroup requestGroup = _groups[i];

                if (requestGroup.IsComplete)
                {
                    requestGroup.OnComplete();

                    _groups.RemoveAt(i);
                }
                else if(requestGroup.IsEmpty)
                {
                    requestGroup.IsComplete = true;
                }
            }
        }
    }

    /// <summary>
    /// 使用资源清单文件初始化资源管理器
    /// </summary>
    /// <param name="manifest"></param>
    /// <returns></returns>
    public static IEnumerator Initialize(ResourceBundleManifest manifest)
    {
        yield return m_AssetBundleLoader.Initialize(manifest);
    }

    public static void SetMaxCoroutine(uint n)
    {
        maxLoadingRequest = System.Math.Max(n, 1);
    }

    ////========== - SyncLoad - 同步加载============
    private static Object SyncLoad(string resourcesPath, System.Type type)
    {
        Object loaded = null;

#if LUA_DEBUG_HOT_REFRESH
        //load preload folder first
        loaded = RealProResourceLoader.LoadSync(resourcesPath, type);
        
#endif

        if (AppConst.BundleMode && loaded == null)
        {
            string assetBundleName;
            string assetName;

            if (PathUtility.ResourcePathToBundlePath(resourcesPath, out assetBundleName, out assetName))
            {
                if (m_AssetBundleLoader.ExistAssetBundleName(assetBundleName))
                {
                    AssetBundleRef assetBundleRef = m_AssetBundleLoader.LoadAssetBundle(assetBundleName);

                    if (assetBundleRef != null && assetBundleRef.Bundle != null)
                    {
                        if (type != null)
                        {
                            loaded = m_AssetBundleLoader.LoadAssetSync(assetBundleName, assetName, type);
                        }
                        else
                        {
                            loaded = m_AssetBundleLoader.LoadAssetSync(assetBundleName, assetName, typeof(GameObject));
                        }

                        if (loaded == null)
                        {
                            m_AssetBundleLoader.UnloadAssetBundle(assetBundleName);
                        }
                    }
                }
            }
        }

        if (loaded == null)
        {
            if (type != null)
            {
                if (!string.IsNullOrEmpty(resourcesPath))
                {
                    loaded = Resources.Load(resourcesPath, type);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(resourcesPath))
                {
                    loaded = Resources.Load(resourcesPath);
                }
            }
        }
        return loaded;
    }

    /// <summary>
    /// 同步加载指定路径的资源
    /// </summary>
    /// <param name="resourcesPath">Resources目录相对路径</param>
    /// <param name="type">资源类型</param>
    /// <returns>AssetRef资源引用器</returns>
    public static AssetRef LoadSync(string resourcesPath, System.Type type)
    {
        RequestInfo info = new RequestInfo(resourcesPath, type);
        if (m_AssetCacher == null) return null;
        //先尝试从缓存中取资源
        AssetRef cached = m_AssetCacher.GetFromCache(info);

        if (cached != null)
        {
            return cached;
        }

        //缓存中取不到正常加载
        Object loaded = SyncLoad(info.path, info.type);

        //加载成功放入缓存
        if (loaded != null)
        {
            m_AssetCacher.PutIntoCache(info, loaded);

            return m_AssetCacher.GetFromCache(info);
        }
        return null;
    }

    /// <summary>
    /// 同步加载指定路径的资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="resourcesPath">Resources目录相对路径</param>
    /// <returns>AssetRef<T>资源引用器</returns>
    public static AssetRef<T> LoadSync<T>(string resourcesPath) where T : Object
    {
        return LoadSync(resourcesPath, typeof(T)) as AssetRef<T>;
    }


    ////========== - AsyncLoad - 异步加载============

    IEnumerator AsyncLoad(Request request)
    {
        RequestInfo assetInfo = request.ResInfo;

        //先尝试从缓存中取资源

        if(m_AssetCacher.IsCached(assetInfo))
        {
            foreach (RequestNode node in request.Nodes)
            {
                AssetRef cached = m_AssetCacher.GetFromCache(assetInfo);

                node.OnComplete(cached);
            }
            request.Nodes.Clear();

            _loadingRequests.Remove(request.Handle);

            yield break;
        }

        //缓存中取不到正常加载

        Object loaded = null;

#if LUA_DEBUG_HOT_REFRESH
        loaded = RealProResourceLoader.LoadSync(assetInfo.path, assetInfo.type);
#endif

        if (AppConst.BundleMode && loaded == null)
        {
            if (loaded == null)
            {
                string assetBundleName;
                string assetName;

                if (PathUtility.ResourcePathToBundlePath(assetInfo.path, out assetBundleName, out assetName))
                {
                    if (m_AssetBundleLoader.ExistAssetBundleName(assetBundleName))
                    {
                        AssetBundleRef assetBundleRef = m_AssetBundleLoader.LoadAssetBundle(assetBundleName);

                        if (assetBundleRef != null && assetBundleRef.Bundle != null)
                        {
                            AssetBundleRequest abr = m_AssetBundleLoader.LoadAssetAsync(assetBundleName, assetName, assetInfo.type);

                            if (abr != null)
                            {
                                yield return abr;

                                loaded = abr.asset;
                            }

                            if (loaded == null)
                            {
                                m_AssetBundleLoader.UnloadAssetBundle(assetBundleName);
                            }
                        }                           
                    }
                }
                else
                {
                    Debug.LogError("Invaild resource path: " + assetInfo.path);
                }
            }
        }
        if (loaded == null)
        {
            if (!string.IsNullOrEmpty(assetInfo.path))
            {
                ResourceRequest rr = Resources.LoadAsync(assetInfo.path, assetInfo.type);

                if (rr != null)
                {
                    yield return rr;

                    loaded = rr.asset;
                }
            }
        }

        //加载成功放入缓存

        if (loaded != null)
        {
            m_AssetCacher.PutIntoCache(assetInfo, loaded);
        }
        else
        {
            Debug.LogError("Load resource failed, please check: " + assetInfo.path + " with type: " + assetInfo.type);
        }

        foreach (RequestNode node in request.Nodes)
        {
            AssetRef cached = m_AssetCacher.GetFromCache(assetInfo);

            node.OnComplete(cached);
        }
        request.Nodes.Clear();
        
        _loadingRequests.Remove(request.Handle);
    }

    ////========== - PreCache - 预加载模式==============

    /// <summary>
    /// 开启一个新的资源加载请求组用于接受加载请求
    /// </summary>
    /// <param name="onGroupComplete">资源加载请求组加载完成回调</param>
    /// <param name="onProgress">资源加载进度回调</param>
    /// <returns>资源加载请求组句柄</returns>
    /// <summary>
    public static int Begin(UnityAction<int> onGroupComplete, UnityAction<int, float> onProgress = null)
    {
        RequestGroup group = new RequestGroup(onProgress, onGroupComplete);

        _acceptingRequestGroups.Push(group);

        return group.Handle;
    }

    /// <summary>
    /// 结束接受加载请求
    /// </summary>
    public static void End()
    {
        if(_acceptingRequestGroups.Count > 0)
        {
            RequestGroup group = _acceptingRequestGroups.Pop();

            group.Prepare(_requestsToLoad);

            _groups.Add(group);
        }
        else
        {
            Debug.LogError("unexpect end() mismatch the begin()!");
        }
    }

    /// <summary>
    /// 取消一个请求组
    /// </summary>
    /// <param name="groupHandle">资源加载请求组句柄</param>
    public static void Cancel(int groupHandle)
    {
        if(_requestsToLoad != null)
        {
            foreach (Request request in _requestsToLoad.Values.ToArray())
            {
                request.RemoveNodes(groupHandle);

                if (request.IsEmpty)
                {
                    _requestsToLoad.Remove(request.ResInfo);
                }
            }
        }

        if(_loadingRequests != null)
        {
            foreach (Request request in _loadingRequests.Values.ToArray())
            {
                request.RemoveNodes(groupHandle);

                if (request.IsEmpty)
                {
                    Instance.StopCoroutine(request.coroutine);

                    _loadingRequests.Remove(request.Handle);
                }
            }
        }

        if(_groups != null)
        {
            _groups.RemoveAll((RequestGroup group) => group.Handle == groupHandle);
        }
    }

    /// <summary>
    /// 请求加载
    /// </summary>
    /// <param name="resourcesPath">Resources目录相对路径</param>
    /// <param name="type">资源类型</param>
    /// <param name="onComplete">资源加载完成回调</param>
    public static int Request(string resourcesPath, System.Type type, UnityAction<int, int, AssetRef> onComplete)
    {
        if (_acceptingRequestGroups.Count > 0)
        {
            if(!string.IsNullOrEmpty(resourcesPath))
            {
                RequestGroup group = _acceptingRequestGroups.Peek();

                RequestInfo info = new RequestInfo(resourcesPath, type);

                return group.AddRequestNode(info, onComplete);
            }
            else
            {
                Debug.LogError("Null or empty resource path is not valid!");
            }
        }
        return -1;
    }

    /// <summary>
    /// 请求加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="resourcesPath">Resources目录相对路径</param>
    /// <param name="onComplete">资源加载完成回调</param>
    /// <returns>资源加载请求句柄</returns>
    public static int Request<T>(string resourcesPath, UnityAction<int, int, AssetRef<T>> onComplete) where T : Object
    {
        if (_acceptingRequestGroups.Count > 0)
        {
            if (!string.IsNullOrEmpty(resourcesPath))
            {
                RequestGroup group = _acceptingRequestGroups.Peek();

                RequestInfo info = new RequestInfo(resourcesPath, typeof(T));

                return group.AddRequestNode(info, onComplete);
            }
            else
            {
                Debug.LogError("Null or empty resource path is not valid!");
            }
        }
        return -1;
    }

    /// <summary>
    /// 请求加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="resourcesPath">Resources目录相对路径</param>
    /// <param name="propertyOwner">资源反射对象</param>
    /// <param name="propertyName">反射对象属性名</param>
    /// <param name="propertyIndex">反射对象属性索引</param>
    /// <returns>资源加载请求句柄</returns>
    public static int Request<T>(string resourcesPath, object propertyOwner, string propertyName, object propertyIndex = null)
    {
        if (_acceptingRequestGroups.Count > 0)
        {
            if (!string.IsNullOrEmpty(resourcesPath))
            {
                RequestGroup group = _acceptingRequestGroups.Peek();

                RequestInfo info = new RequestInfo(resourcesPath, typeof(T));

                return group.AddRequestNode(info, propertyOwner, propertyName, propertyIndex);
            }
            else
            {
                Debug.LogError("Null or empty resource path is not valid!");
            }
        }
        return -1;
    }

    /// <summary>
    /// 卸载一个资源
    /// 如果是AssetBundle模式，则会为不再需要的AssetBundle包调用AssetBundle.Unload(true)
    /// </summary>
    /// <param name="assetRef">资源引用器</param>
    public static void Unload(AssetRef assetRef)
    {
        if (m_AssetCacher.RemoveFromCache(assetRef))
        {
            if (AppConst.BundleMode)
            {
                if (PathUtility.ResourcePathToBundlePath(assetRef.Info.path, out string assetBundleName, out string assetName))
                {
                    m_AssetBundleLoader.UnloadAssetBundle(assetBundleName, true);
                }
            }
        }
    }

    /// <summary>
    /// 卸载所有资源
    /// 如果是AssetBundle模式,则会为所有已经加载的AssetBundle包调用AssetBundle.Unload(false)
    /// </summary>
    public static void UnloadAll()
    {
        if (AppConst.BundleMode)
        {
            Dictionary<RequestInfo, AssetRef> cachedAssets = m_AssetCacher.GetAllCachedAssets();

            foreach (RequestInfo info in cachedAssets.Keys)
            {
                if (PathUtility.ResourcePathToBundlePath(info.path, out string assetBundleName, out string assetName))
                {
                    m_AssetBundleLoader.UnloadAssetBundle(assetBundleName, false);
                }
            }
        }
        m_AssetCacher.Clear();
    }
}
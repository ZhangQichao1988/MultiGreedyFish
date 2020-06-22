using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager Instance { get; set; }

    /// <summary>
    /// 最大同时执行的加载请求数量
    /// </summary>
    private static int maxLoadingRequest = 4;

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
    private static AssetBundleLoader _assetBundleLoader;

    private static List<string> _assetBundleToUnload;

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
        _assetBundleToUnload = new List<string>();

        _assetBundleLoader = new AssetBundleLoader();
    }

    private void OnDestroy()
    {
        if(_assetBundleLoader != null)
        {
            _assetBundleLoader.Destroy();
        }
        _loadingRequests.Clear();
        _requestsToLoad.Clear();
        _groups.Clear();
        _acceptingRequestGroups.Clear();
        _assetBundleToUnload.Clear();

        if (Instance != null)
        {
            Instance = null;
        }
    }

    private void Update()
    {
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

                if (requestGroup.IsEmpty)
                {
                    ClockRecorder.GetRecord();
                    requestGroup.OnComplete();

                    _groups.RemoveAt(i);
                }
            }
        }
    }

    public static IEnumerator Initialize(ResourceBundleManifest manifest)
    {
        yield return _assetBundleLoader.Initialize(manifest);
    }

    public static void UnloadAssetBundles()
    {
        for(int i = 0; i < _assetBundleToUnload.Count; i++)
        {
            _assetBundleLoader.UnloadAssetBundle(_assetBundleToUnload[i]);                
        }
        _assetBundleToUnload.Clear();
    }

    ////========== - SyncLoad - 同步加载============
    ///
    public static Object LoadSync(string resourcesPath, System.Type type)
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
                if (_assetBundleLoader.ExistAssetBundleName(assetBundleName))
                {
                    AssetBundleRef assetBundle = _assetBundleLoader.LoadAssetBundle(assetBundleName);

                    if(assetBundle != null && assetBundle.Bundle != null)
                    {
                        _assetBundleToUnload.Add(assetBundleName);

                        if (type != null)
                        {
                            loaded = _assetBundleLoader.LoadAssetSync(assetBundleName, assetName, type);
                        }
                        else
                        {
                            loaded = _assetBundleLoader.LoadAssetSync(assetBundleName, assetName, typeof(GameObject));
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

    public static T LoadSync<T>(string resourcesPath) where T : Object
    {
        return LoadSync(resourcesPath, typeof(T)) as T;
    }

    ////========== - AsyncLoad - 异步加载============

    IEnumerator AsyncLoad(Request request)
    {
        RequestInfo assetInfo = request.ResInfo;

        Object loaded = null;

#if LUA_DEBUG_HOT_REFRESH
        loaded = RealProResourceLoader.LoadSync(assetInfo.path, assetInfo.type);
#endif

        if (AppConst.BundleMode && loaded == null)
        {
            string assetBundleName;
            string assetName;

            if (PathUtility.ResourcePathToBundlePath(assetInfo.path, out assetBundleName, out assetName))
            {
                if (_assetBundleLoader.ExistAssetBundleName(assetBundleName))
                {
                    AssetBundleRef assetBundleRef = _assetBundleLoader.LoadAssetBundle(assetBundleName);

                    if(assetBundleRef != null && assetBundleRef.Bundle != null)
                    {
                        _assetBundleToUnload.Add(assetBundleName);

                        AssetBundleRequest abr = _assetBundleLoader.LoadAssetAsync(assetBundleName, assetName, assetInfo.type);

                        if (abr != null)
                        {
                            yield return abr;

                            loaded = abr.asset;
                        }
                    }                     
                }
            }
            else
            {
                Debug.LogError("Invaild resource path: " + assetInfo.path);
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
        request.OnComplete(loaded);

        _loadingRequests.Remove(request.Handle);
    }

    ////========== - PreCache - 预加载模式==============

    /// <summary>
    /// 开启一个新的资源加载请求组用于接受加载请求
    /// </summary>
    /// <param name="onGroupComplete"></param>
    /// <returns></returns>
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

            ClockRecorder.StartRecord();
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
    /// <param name="groupHandle"></param>
    public static void Cancel(int groupHandle)
    {
        foreach (Request request in _requestsToLoad.Values.ToArray())
        {
            request.RemoveNodes(groupHandle);

            if (request.IsEmpty)
            {
                _requestsToLoad.Remove(request.ResInfo);
            }
        }

        foreach (Request request in _loadingRequests.Values.ToArray())
        {
            request.RemoveNodes(groupHandle);

            if (request.IsEmpty)
            {
                Instance.StopCoroutine(request.coroutine);

                _loadingRequests.Remove(request.Handle);
            }
        }

        _groups.RemoveAll((RequestGroup group) => group.Handle == groupHandle);
    }

    /// <summary>
    /// 请求加载
    /// </summary>
    /// <param name="resourcesPath"></param>
    /// <param name="type"></param>
    /// <param name="onComplete"></param>
    public static int Request(string resourcesPath, System.Type type, UnityAction<int, int, Object> onComplete)
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
    /// <typeparam name="T"></typeparam>
    /// <param name="resourcesPath"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public static int Request<T>(string resourcesPath, UnityAction<int, int, Object> onComplete)
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
    /// <typeparam name="T"></typeparam>
    /// <param name="resourcesPath"></param>
    /// <param name="propertyOwner"></param>
    /// <param name="propertyName"></param>
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
    /// 是否处于接受加载请求状态
    /// </summary>
    /// <returns></returns>
    public static bool IsAcceptRequest()
    {
        return _acceptingRequestGroups.Count > 0;
    }
}

using UnityEngine;
using System;
using System.Collections;
using NetWorkModule.Dummy;

namespace NetWorkModule
{
    public class NetWorkManager : MonoBehaviour
    {
        public static NetWorkManager Instance { get; set; }
        
        private static IDummyData dummyData;
        static SimpleHttpClient httpClient;
        public static SimpleHttpClient HttpClient
        {
            get
            {
                return httpClient;
            }
        }

        public void InitWithServerCallBack(AbstractProtocol protocol, int loginMsgId, HttpDispatcher.DgtServerEvent serverCb, IDummyData dummy = null)
        {
            dummyData = dummy;
            httpClient = new SimpleHttpClient(protocol, Application.version, Application.platform == RuntimePlatform.Android ? "a" : "i", loginMsgId);
            HttpDispatcher.CreateInstance();
            HttpDispatcher.Instance.OnServeEvent += serverCb;
        }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            if (HttpDispatcher.Instance != null)
            {
                HttpDispatcher.Instance.Dispatch();
            }
        }

        /// <summary>
        /// 游戏热重启调用
        /// </summary>
        public void Reset()
        {
            httpClient.Reset();
            HttpDispatcher.Clean();
        }

        private void OnDestroy()
        {
            HttpDispatcher.DestroyInstance();
            httpClient = null;
            StopAllCoroutines();

            if (Instance != null)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// http1 sevices
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        public static void Request(string msg, byte[] data, System.Object cachedData = null, bool needAuth = true, string requestId = null)
        {
            HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpRequestSend, msg, data);
            Instance.StartCoroutine(RequestHttpOneInternal(msg, data, cachedData, needAuth, requestId));
        }
        static IEnumerator RequestHttpOneInternal(string msg, byte[] data, System.Object cachedData, bool needAuth, string requestId)
        {
            if (AppConst.ServerType == ESeverType.OFFLINE)
            {
                dummyData.Recieve(msg, data, cachedData);
                yield break;
            }
            else
            {
                yield return httpClient.RequestHttp(msg, data, cachedData, needAuth, requestId);
            }
        }

        public static void SimpleGet<T>(string url, Action<T> callback, Action retry) where T : class
        {
            Instance.StartCoroutine(RequestHttpGet<T>(url, callback, retry));
        }

        static IEnumerator RequestHttpGet<T>(string url, Action<T> callback, Action retry) where T : class
        {
            yield return httpClient.RequestHttpGet<T>(url, callback, retry);
        }
    }
}
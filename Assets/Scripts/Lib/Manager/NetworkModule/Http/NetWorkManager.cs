using UnityEngine;
using System;
using System.Collections;
using NetWorkModule.Dummy;

namespace NetWorkModule
{
    public class NetWorkManager : MonoBehaviour
    {
        public static NetWorkManager Instance { get; set; }
        
        private IDummyData dummyData;
        static SimpleHttpClient httpClient;
        public static SimpleHttpClient HttpClient
        {
            get
            {
                return httpClient;
            }
        }

        public void InitWithServerCallBack(AbstractProtocol protocol, int loginMsgId, HttpDispatcher.DgtServerEvent serverCb, IDummyData dummy)
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
        public static void Request(string msg, byte[] data, System.Object cachedData = null, bool needAuth = true)
        {
            HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpRequestSend, msg, data);
            Instance.StartCoroutine(RequestHttpOneInternal(msg, data, cachedData, needAuth));
        }
        static IEnumerator RequestHttpOneInternal(string msg, byte[] data, System.Object cachedData, bool needAuth)
        {
#if DUMMY_DATA
            yield return dummyData.RequestHttp(msg, data);
#else
            yield return httpClient.RequestHttp(msg, data, cachedData, needAuth);
#endif
        }
        
    }
}
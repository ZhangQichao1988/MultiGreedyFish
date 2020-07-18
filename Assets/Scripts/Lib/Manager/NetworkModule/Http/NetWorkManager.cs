using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NetWorkModule
{
    public class NetWorkManager : MonoBehaviour
    {
        public static NetWorkManager Instance { get; set; }
        static SimpleHttpClient httpClient;
        private static SimpleHttpClient HttpClient
        {
            get
            {
                return httpClient;
            }
        }

        public void InitWithServerCallBack(AbstractProtocol protocol, int loginMsgId, HttpDispatcher.DgtServerEvent serverCb)
        {
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
        public static void Request(string msg, byte[] data, bool needAuth = true)
        {
            HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpRequestSend, msg, data);
            Instance.StartCoroutine(RequestHttpOneInternal(msg, data, needAuth));
        }
        static IEnumerator RequestHttpOneInternal(string msg, byte[] data, bool needAuth)
        {
            yield return httpClient.RequestHttp(msg, data, needAuth);
        }
        
    }
}
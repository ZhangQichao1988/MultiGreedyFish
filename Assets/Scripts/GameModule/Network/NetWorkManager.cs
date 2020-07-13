using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NetWorkModule;

public class NetWorkManager : MonoBehaviour
{
    private static NetWorkManager Instance { get; set; }
    static SimpleHttpClient httpClient;
    private static SimpleHttpClient HttpClient
    {
        get
        {
            return httpClient;
        }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        httpClient = new SimpleHttpClient(new FishProtocol(), Application.version, Application.platform == RuntimePlatform.Android ? "a" : "i", (int)MessageId.MidLogin);

        HttpDispatcher.CreateInstance();
        HttpDispatcher.Instance.OnServeEvent = OnServerEvent;
    }

    private void Update()
    {
        HttpDispatcher.Instance.Dispatch();
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
        Instance.StartCoroutine(RequestHttpOneInternal(msg, data, needAuth));
    }
    static IEnumerator RequestHttpOneInternal(string msg, byte[] data, bool needAuth)
    {
        yield return httpClient.RequestHttp(msg, data, needAuth);
    }

    void OnServerEvent(HttpDispatcher.EventType type, string msg, System.Object obj)
    {
        switch (type)
        {
            case HttpDispatcher.EventType.HTTP_ERROR:
            case HttpDispatcher.EventType.SignatureError:
            case HttpDispatcher.EventType.Failed:
            case HttpDispatcher.EventType.KickOutLoginUser:
                Debug.LogWarning("got server error " + type);
                break;
            case HttpDispatcher.EventType.Caution:
                Debug.LogWarning("got a warning");
                break;
        }
    }
    
}

using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TimerModule;


namespace NetWorkModule
{
    public class SimpleHttpClient
    {
        public static float TIMEOUT_SEC = 5.0f;
        public static string X_APP_VERSION = "x-app-version";
        public static string X_APP_PLATFORM= "x-app-platform";
        public static string X_PLAYER_ID = "x-player-id";
        public static string X_SIGNATURE = "x-signature";
        public static string X_STATUS_CODE = "x-status-code";

        public static string X_APP_LANGUAGE = "x-app-language";

        Dictionary<string, int> timeoutDict;

        HashSet<int> noAuthMsg = new HashSet<int>(); 
        HashSet<string> useLongTimeout = new HashSet<string>(){"P13_Request"};
        //screct key
        byte[] gSessionKey = null;
        byte[] cachedSession = null;

        string m_version;
        string m_platform;
        Int64 m_playerId;
        public void SetPlayerId(Int64 playerId)
        {
            m_playerId = playerId;
        }
        public long PID = 1;

        AbstractProtocol m_protocol;

        int cachedProtocolId;

        public SimpleHttpClient(AbstractProtocol baseProtocol, string version, string platform, int protocolID)
        {
            timeoutDict = new Dictionary<string, int>();
            m_protocol = baseProtocol;
            cachedProtocolId = protocolID;
            m_version = version;
            m_platform = platform;
            string sessionStr = PlayerPrefs.GetString(NetworkConst.SESSION_KEY_FOR_LOGIN, null);
            Debug.LogWarning("get session key :" + sessionStr);
            if (sessionStr != null)
            {
                cachedSession = Convert.FromBase64String(sessionStr);
            }
        }


        public void Reset()
        {
            gSessionKey = null;
            cachedSession = null;
        }

        public System.Collections.IEnumerator RequestHttpGet<T>(string url, Action<T> finish, Action retry) where T : class
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 5;
                yield return request.SendWebRequest();
                if (request.isNetworkError)
                {
                    retry();
                }
                else
                {
                    if (typeof(T).Equals(typeof(string)))
                    {
                        finish(request.downloadHandler.text as T);
                    }
                    else
                    {
                        finish(request.downloadHandler.data as T);
                    }
                }
            }
        }

        public System.Collections.IEnumerator RequestHttp(string msg, byte[] body, System.Object cachedData, bool needAuth)
        {
            byte[] data = m_protocol.Pack(msg, PID++, body);
            int msgId = int.Parse(msg.Substring(1, msg.IndexOf("_") - 1));
            string err = null;
            byte[] combinedData = GetCombineData(msgId, body, m_playerId, m_platform);
            string signStr = GetRequestSign(combinedData, needAuth, msgId, ref err);

            Debug.LogWarning("Send Signuare:" + signStr);
            
            using (UnityWebRequest request = new UnityWebRequest(AppConst.HttpEndPoint, UnityWebRequest.kHttpVerbPOST))
            {
                // request.SetRequestHeader("Content-Type", "application/x-protobuf");
                request.SetRequestHeader(X_APP_VERSION, m_version);
                request.SetRequestHeader(X_APP_PLATFORM, m_platform);
                request.SetRequestHeader(X_PLAYER_ID, m_playerId.ToString());
                request.SetRequestHeader(X_SIGNATURE, signStr);
                request.SetRequestHeader(X_APP_LANGUAGE, AppConst.languageMode.ToString());
                
                var upLoaderHandler = new UploadHandlerRaw(data);
                upLoaderHandler.contentType = "application/proto";
                request.uploadHandler = upLoaderHandler;
                request.downloadHandler = new DownloadHandlerBuffer();

                var timeoutKey = signStr + PID;
                var reqInfo = new ReqData(){
                    TimeoutKey = timeoutKey,
                    Msg = msg,
                    Body = body,
                    CachedData = cachedData,
                    NeedAuth = needAuth,
                    RequestTimeing = UnityEngine.Time.realtimeSinceStartup
                };
                timeoutDict.Add(timeoutKey, TimerManager.AddTimer((int)eTimerType.RealTime, GetTimeoutSec(msg), TimeoutHandler, reqInfo));

                yield return request.SendWebRequest();

                //timeout check
                if (!timeoutDict.ContainsKey(reqInfo.TimeoutKey))
                {
                    //超时
                    yield break;
                }
                else
                {
                    TimerManager.RemoveTimer(timeoutDict[reqInfo.TimeoutKey]);
                }

                if (!request.isNetworkError && !request.isHttpError && err == null)
                {
                    ProcessCommonResponse(request.GetResponseHeaders(), request.downloadHandler.data, cachedData, body);
                }
                else
                {
                    // error
                    HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpError, err != null ? err : string.Format("Error Http Response, Got Error {0}" ,request.responseCode));
                }
            }
        }

        float GetTimeoutSec(string msg)
        {
            return useLongTimeout.Contains(msg) ? 20 : TIMEOUT_SEC;
        }

        void TimeoutHandler(System.Object obj)
        {
            ReqData rd = obj as ReqData;
            HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.TimeOut, rd.Msg, rd);
            if (timeoutDict.ContainsKey(rd.TimeoutKey))
            {
                timeoutDict.Remove(rd.TimeoutKey);
            }
        }

        public void SaveSessionKey(string resKey, byte[] randomBytes, bool isCached)
		{
            byte[] byteKey = CryptographyUtil.XorBytes(randomBytes, Convert.FromBase64String(resKey));
			if (isCached)
            {
                PlayerPrefs.SetString(NetworkConst.SESSION_KEY_FOR_LOGIN, Convert.ToBase64String(byteKey));
                cachedSession = byteKey;
            }
            gSessionKey = byteKey;

            Debug.LogWarning("Saved Session key : " + Convert.ToBase64String(gSessionKey));
		}

        byte[] GetCombineData(int msgId, byte[] data)
        {
            byte[] numByte = BitConverter.GetBytes(msgId);
            byte[] result;
            Array.Reverse(numByte);
            if (data != null)
            {
                result = new byte[numByte.Length + data.Length];
                Array.Copy(numByte, 0, result, 0, numByte.Length);
                Array.Copy(data, 0, result, numByte.Length, data.Length);
            }
            else
            {
                result = new byte[numByte.Length];
                Array.Copy(numByte, 0, result, 0, numByte.Length);
            }
            
            return result;
        }

        byte[] GetCombineData(int msgId, byte[] data, long playerId, string platform)
        {
            byte[] numByte = BitConverter.GetBytes(msgId);
            byte[] result;
            byte[] playerBytes = BitConverter.GetBytes(playerId);
            byte[] platformBytes = Encoding.UTF8.GetBytes(platform);
            Array.Reverse(numByte);
            Array.Reverse(playerBytes);
            int currLen = 0;
            if (data != null)
            {
                result = new byte[numByte.Length + data.Length + playerBytes.Length + platformBytes.Length];
                Array.Copy(numByte, 0, result, 0, numByte.Length);
                Array.Copy(data, 0, result, numByte.Length, data.Length);
                currLen = numByte.Length + data.Length;
            }
            else
            {
                result = new byte[numByte.Length + playerBytes.Length + platformBytes.Length];
                Array.Copy(numByte, 0, result, 0, numByte.Length);
                currLen = numByte.Length;
            }
            Array.Copy(playerBytes, 0, result, currLen, playerBytes.Length);
            Array.Copy(platformBytes, 0, result, currLen + playerBytes.Length, platformBytes.Length);
            
            return result;
        }

        void ProcessCommonResponse(Dictionary<string, string> headers, byte[] res, System.Object cachedData, byte[] req)
        {
            string sign = headers.ContainsKey(X_SIGNATURE) ? headers[X_SIGNATURE] : null;
            int stateCode = (int)StatusCode.Failed;

            if (headers.ContainsKey(X_STATUS_CODE))
            {
                int.TryParse(headers[X_STATUS_CODE], out stateCode);
            }
            PackData output = res == null ? null : m_protocol.ParserOutput(res, res.Length);

            if (output != null)
            {
                HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpRecieve, string.Format("P{0}_Response", output.msgId), output.pbData);
            }
            
            //status code 处理
            bool statuOk = sign == null ? true : ProcessStatues((StatusCode)stateCode, output, sign);
            if (statuOk)
            {
                HttpDispatcher.Instance.PushMsg(output.msgId, output.pbData, output.pid, cachedData, req);
            }
        }

        bool ProcessStatues(StatusCode status, PackData data, string serverSign)
        {
            bool result = true;
            switch (status)
            {
                case StatusCode.SignatureError:
                    HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.SignatureError);
                    result = false;
                    break;
                case StatusCode.KickOutLoginUser:
                    HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.KickOutLoginUser);
                    result = false;
                    break;
                case StatusCode.Caution:
                    HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.Caution);
                    result = false;
                    break;
                case StatusCode.Failed:
                    HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.Failed);
                    result = false;
                    break;
                default:
                    if (AppConst.EnableProtocolEncrypt && data.msgId != (int)MessageId.MidLogin)
                    {
                        string err = null;
                        byte[] combinedData = GetCombineData(data.msgId, data.pbData);
                        string signatureData = GetRequestSign(combinedData, !noAuthMsg.Contains(data.msgId), data.msgId, ref err);
                        if (signatureData != serverSign || err != null)
                        {
                            HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.SignatureError, "client signature not equal with server");
                            result = false;
                        }
                    }
                    break;
            }
            return result;
        }

        string GetRequestSign(byte[] data, bool needAuth, int msgId, ref string err)
        {
            if (!needAuth && !noAuthMsg.Contains(msgId))
            {
                noAuthMsg.Add(msgId);
            }
            byte[] tmpSession = needAuth ? gSessionKey : AppConst.StartUpKey;
                            
            if (!AppConst.EnableProtocolEncrypt)
            {
                tmpSession = AppConst.StartUpKey;
            }
            else
            {
                if (msgId == cachedProtocolId)
                {
                    //login protocol
                    tmpSession = cachedSession;
                }
            }

            if (tmpSession == null)
            {
                err = "Session Key is Empty!!";
            }
            string signatureData = CryptographyUtil.Hexlify(CryptographyUtil.HmacSha1(data, tmpSession));

            return signatureData;
        }
    }

    public class ReqData
    {
        public string TimeoutKey;
        public string Msg;
        public byte[] Body;
        public System.Object CachedData;
        public bool NeedAuth;
        public float RequestTimeing;
    }
}
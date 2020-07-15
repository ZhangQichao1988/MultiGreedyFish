using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace NetWorkModule
{
    public class SimpleHttpClient
    {
        public static string X_APP_VERSION = "x-app-version";
        public static string X_APP_PLATFORM= "x-app-platform";
        public static string X_PLAYER_ID = "x-player-id";
        public static string X_SIGNATURE = "x-signature";
        public static string X_STATUS_CODE = "x-status-code";



        HashSet<int> noAuthMsg = new HashSet<int>(); 
        //screct key
        byte[] gSessionKey = null;
        byte[] cachedSession = null;

        string m_version;
        string m_platform;
        Int64 m_playerId;
        public long PID = 1;

        AbstractProtocol m_protocol;

        int cachedProtocolId;

        public SimpleHttpClient(AbstractProtocol baseProtocol, string version, string platform, int protocolID)
        {
            m_protocol = baseProtocol;
            cachedProtocolId = protocolID;
            m_version = version;
            m_platform = platform;
        }

        public System.Collections.IEnumerator RequestHttp(string msg, byte[] body, bool needAuth)
        {
            byte[] data = m_protocol.Pack(msg, PID++, body);
            int msgId = int.Parse(msg.Substring(1, msg.IndexOf("_") - 1));
            string err = null;
            byte[] combinedData = GetCombineData(msgId, data, m_playerId, m_platform);
            string signStr = GetRequestSign(combinedData, needAuth, msgId, ref err);
            
            using (UnityWebRequest request = new UnityWebRequest(AppConst.HttpEndPoint, UnityWebRequest.kHttpVerbPOST))
            {
                // request.SetRequestHeader("Content-Type", "application/x-protobuf");
                request.SetRequestHeader(X_APP_VERSION, m_version);
                request.SetRequestHeader(X_APP_PLATFORM, m_platform);
                request.SetRequestHeader(X_PLAYER_ID, m_playerId.ToString());
                request.SetRequestHeader(X_SIGNATURE, signStr);
                
                var upLoaderHandler = new UploadHandlerRaw(data);
                upLoaderHandler.contentType = "application/proto";
                request.uploadHandler = upLoaderHandler;
                yield return request.SendWebRequest();


                HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpRecieve, msg, body);
                if (!request.isNetworkError && !request.isHttpError && err == null)
                {
                    ProcessCommonResponse(request.GetResponseHeaders(), request.downloadHandler.data);
                }
                else
                {
                    // error
                    HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpError, err != null ? err : string.Format("Error Http Response, Got Error {0}" ,request.responseCode));
                }
            }
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

        void ProcessCommonResponse(Dictionary<string, string> headers, byte[] res)
        {
            string sign = headers[X_SIGNATURE];
            int stateCode = (int)StatusCode.Failed;
            int.TryParse(headers[X_STATUS_CODE], out stateCode);
            var output = m_protocol.ParserOutput(res, res.Length);

            //status code 处理
            bool statuOk = ProcessStatues((StatusCode)stateCode, output, sign);
            if (statuOk)
            {
                HttpDispatcher.Instance.PushMsg(output.msgId, output.pbData, output.pid);
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
                    if (AppConst.EnableProtocolEncrypt)
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
}
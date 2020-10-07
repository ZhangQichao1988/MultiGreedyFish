using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;
using System;
using System.IO;

namespace NetWorkModule.Dummy
{
    /// <summary>
    /// 模拟数据
    /// </summary>
    public class BaseDummyData : IDummyData 
    {
        Dictionary<string, IDummyResponseProcesser> pbResProcesssInst = new Dictionary<string, IDummyResponseProcesser>();
        protected Dictionary<string, System.Type> pbResProcesss = new Dictionary<string, Type>();
        protected Dictionary<string, MessageParser> pbParserDic;
        public BaseDummyData(Dictionary<string, MessageParser> parser)
        {
            pbParserDic = parser;
        }

        public virtual void Recieve(string msg, byte[] data, System.Object cachedData = null)
        {
            IMessage pbMsg = null;
            if (pbParserDic.ContainsKey(msg))
            {
                pbMsg = pbParserDic[msg].ParseFrom(data);
            }
            
            var resMsgId = msg.Split('_')[0] + "_Response";
            ProcessResponse(resMsgId, pbMsg, cachedData);
        }

        void ProcessResponse(string resMsg, IMessage pbMsg, System.Object cachedData = null)
        {
            if (!pbResProcesssInst.ContainsKey(resMsg))
            {
                if (pbResProcesss.ContainsKey(resMsg))
                {
                    pbResProcesssInst[resMsg] = Activator.CreateInstance(pbResProcesss[resMsg]) as IDummyResponseProcesser;
                }
                else
                {
                    Debug.LogWarning("No Register Response Msg " + resMsg);
                    return;
                }
            }

            int msgId = int.Parse(resMsg.Substring(1, resMsg.IndexOf("_") - 1));

            IMessage resData = pbResProcesssInst[resMsg].ProcessRequest(msgId, pbMsg);

            HttpDispatcher.Instance.PushEvent(HttpDispatcher.EventType.HttpRecieve, string.Format("P{0}_Response", msgId), GetStreamBytes(resData));
            
            pbResProcesssInst[resMsg].SetCachedData(cachedData);
            pbResProcesssInst[resMsg].DispatchRes(msgId, pbMsg, resData);
        }

        byte[] GetStreamBytes(IMessage pbMsg)
        {
            byte[] bytesDatas;
            using (MemoryStream stream = new MemoryStream())
            {
                pbMsg.WriteTo(stream);
                bytesDatas = stream.ToArray();
            }
            return bytesDatas;
        }
    }
}
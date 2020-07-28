using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;
using System;

namespace NetWorkModule.Dummy
{
    /// <summary>
    /// 模拟数据
    /// </summary>
    public class BaseDummyData : IDummyData 
    {
        Dictionary<string, IDummyResponseProcesser> pbResProcesssInst;
        protected Dictionary<string, System.Type> pbResProcesss;
        protected Dictionary<string, MessageParser> pbParserDic;
        public BaseDummyData(Dictionary<string, MessageParser> parser)
        {
            pbParserDic = parser;
        }

        public virtual void Recieve(string msg, byte[] data)
        {
            IMessage pbMsg = null;
            if (pbParserDic.ContainsKey(msg))
            {
                pbMsg = pbParserDic[msg].ParseFrom(data);
            }
            
            var resMsgId = msg.Split('_')[0] + "Response";
            ProcessResponse(resMsgId, pbMsg);
        }

        void ProcessResponse(string resMsg, IMessage pbMsg)
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

            int msgId = int.Parse(resMsg.Substring(1, resMsg.IndexOf("_")));
            pbResProcesssInst[resMsg].ProcessRequest(msgId, pbMsg);
        }
    }
}
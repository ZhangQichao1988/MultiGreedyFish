using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace NetWorkModule
{
    public class HttpDispatcher
    {
        internal static HttpDispatcher m_inst;

        public static HttpDispatcher Instance
        {
            get { return m_inst; }
        }
        public static void CreateInstance()
        {
            m_inst = new HttpDispatcher();
        }
        public static void DestroyInstance()
        {
            m_inst = null;
        }

        public delegate void DgtServerEvent(EventType type, string msg, System.Object obj);
        public event DgtServerEvent OnServeEvent;


        public delegate void DgtMessage(NodeMsg msg);
        //public DgtMessage OnAllMsg;
        //public delegate void DgtMsgError(OzChannel chl, string text);
        //public DgtMsgError OnMsgError;

        public class ObsNode
        {
            public DgtMessage DgtMsg;
        }
        public Dictionary<int, ObsNode> m_msg_obs = new Dictionary<int, ObsNode>();
        public DgtMessage AddObserver(int k, DgtMessage dgt)
        {
            if (!m_msg_obs.ContainsKey(k))
            {
                m_msg_obs[k] = new ObsNode();
            }
            m_msg_obs[k].DgtMsg += dgt;
            return dgt;
        }
        public bool RemoveObserver(int k, DgtMessage dgt)
        {
            if (m_msg_obs.ContainsKey(k))
            {
                m_msg_obs[k].DgtMsg -= dgt;
                return true;
            }
            return false;
        }


        public enum EventType
        {
            SignatureError = 1,
            KickOutLoginUser = 2,
            Caution = 3,
            Failed = 4,
            HttpError = 5,
            HttpRequestSend = 16,
            HttpRecieve = 32
        }
        public class NodeEvent
        {
            public EventType Event;
            public string Msg;
            public System.Object Obj;

            public NodeEvent(EventType et, string msg, System.Object obj)
            {
                Event = et;
                Msg = string.Copy(msg);
                Obj = obj;
            }
        }
        List<NodeEvent> m_node_evt = new List<NodeEvent>();
        List<NodeEvent> m_node_evt_post = new List<NodeEvent>();
        System.Object m_lock_node_evt = new System.Object();


        public class NodeMsg
        {

            public int Key;
            public byte[] Body;
            public long Pid;
            public int state;

            public System.Object CachedData;
            
            public byte[] ReqMsg;
            
            public NodeMsg(int _key, int _state)
            {
                Key = _key;
                state = _state;
            }

            public NodeMsg(int key, byte[] body, long pid, System.Object cachedData, byte[] reqMsg)
            {
                Key = key;
                Body = body;
                Pid = pid;
                state = 0;
                CachedData = cachedData;
                ReqMsg = reqMsg;
            }
        }
        List<NodeMsg> m_node_msg = new List<NodeMsg>();
        List<NodeMsg> m_node_msg_post = new List<NodeMsg>();
        System.Object m_lock_node_msg = new System.Object();

        Dictionary<string, Type> m_cmd_types = new Dictionary<string, Type>();
        public Dictionary<string, Type> CmdTypes
        {
            get { return m_cmd_types; }
        }


        public void PushEvent(EventType et, string msg = "", System.Object obj = null)
        {
            lock (m_lock_node_evt)
            {
                m_node_evt.Add(new NodeEvent(et, msg, obj));
            }
        }

        public void PushMsg(int k, byte[] param, long pid, System.Object cachedData, byte[] req)
        {
            lock (m_lock_node_msg)
            {
                m_node_msg.Add(new NodeMsg(k, param, pid, cachedData, req));
            }
        }

        public void Dispatch()
        {
            lock (m_lock_node_msg)
            {
                m_node_msg_post.AddRange(m_node_msg);
                m_node_msg.Clear();
            }
            lock (m_lock_node_evt)
            {
                m_node_evt_post.AddRange(m_node_evt);
                m_node_evt.Clear();
            }


            for (int i = 0; i < m_node_evt_post.Count; ++i)
            {
                NodeEvent n = m_node_evt_post[i];
                if (OnServeEvent != null)
                {
                    OnServeEvent(n.Event, n.Msg, n.Obj);
                }
            }
            m_node_evt_post.Clear();


            for (int i = 0; i < m_node_msg_post.Count; ++i)
            {
                NodeMsg n = m_node_msg_post[i];
                NotifySpecMsg(n);
            }
            m_node_msg_post.Clear();
        }
        
        void NotifySpecMsg(NodeMsg msg)
        {
            ObsNode obs;
            if (m_msg_obs.TryGetValue(msg.Key, out obs))
            {
                if (obs.DgtMsg != null)
                {
                    obs.DgtMsg(msg);
                }
            }
        }
    }
}

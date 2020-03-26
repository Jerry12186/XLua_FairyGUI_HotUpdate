using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;
using Timer = System.Timers.Timer;

namespace NetModel
{
    /// <summary>
    /// 这个类是连接C#和Lua的接口
    /// </summary>
    [LuaCallCSharp]
    public class NetHelper
    {
        [CSharpCallLua]
        public delegate void LuaCallback(bool isOk);

        [CSharpCallLua]
        public delegate void LuaMsgHandler(Msg msg);

        public delegate void LuaNetExEvent();

        [CSharpCallLua]
        public delegate void HeartBeat();

        private static LuaMsgHandler toLuaMsg;

        public static LuaMsgHandler ToLuaMsg
        {
            get { return toLuaMsg; }

            private set { toLuaMsg = value; }
        }

        /// <summary>
        /// 注册网络异常事件
        /// </summary>
        /// <param name="action"></param>
        public static void RegisterNetExEvent(Action action)
        {
            if (action != null)
                ConnectionMgr.RegisterNetEx(action);
        }

        /// <summary>
        /// 同步建立网络连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static bool CreateConnectBySync(string ip, int port)
        {
            bool isOk = ConnectionMgr.ConnectBySync(ip, port);
            if (isOk)
                Debug.Log("同步连接网络成功");
            else
                Debug.LogError("同步连接网络失败");
            return isOk;
        }

        /// <summary>
        /// 异步建立网络连接
        /// 不知道为啥原来的人加了其他东西，明天研究一下为啥
        /// 已经搞明白了，2020.3.24
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="callback"></param>
        public static void CreateConnectByAsync(string ip, int port, LuaCallback callback)
        {
            bool isOk = false;
            bool isComplate = false;
            ConnectionMgr.ConnentByAsync(ip, port, (cb)
                =>
            {
                isOk = cb.IsCompleted && (cb.AsyncState as MySocket).IsConnect();
                isComplate = true;
            });
            Debug.Log("创建新物体");
            GameObject gameObject = new GameObject();
            gameObject.name = "昙花一现";
            var myMonoBehaviour = gameObject.AddComponent<MyMonoBehaviour>();
            myMonoBehaviour.UpdateAction = () =>
            {
                if (isComplate)
                {
                    callback(isOk);
                    GameObject.Destroy(myMonoBehaviour);
                }
            };

            #region 测试没不能用的，但不是没用，是不能用

            //这里开一个线程吧，安全点
            //线程调用不了"注册心跳函数"，因为子线程不能访问Unity的主线程，艹
//            Thread thread = new Thread(()=>{
//                while (true)
//                {
//                    if (isComplate)
//                    {
//                        callback(isOk);
//                        break;
//                    }
//                }
//            });
//            thread.Start();

            //这个如果不是立马连上服务器，客户端会有卡顿的情况，哎，头疼
//            while (true)
//            {
//                if (isComplate)
//                {
//                    callback((bool) isOk);
//                    break;
//                }
//            }

            #endregion

        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public static void CloseConnect()
        {
            ConnectionMgr.CloseConnect();
        }

        /// <summary>
        /// 修改发送消息格式
        /// 修改为msgId + msgBody(pb)
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="msgId"></param>
        /// <param name="msgMainType"></param>
        /// <param name="msgSunType"></param>
        public static void SendMessage(int msgId, byte[] pb = null)
        {
            byte[] msg = ConnectionMgr.EncodeMsg(msgId, pb);
            ConnectionMgr.SendMsg(msg);
        }

        public static void RegisterLuaMsgHandler(LuaMsgHandler handler)
        {
            if (ToLuaMsg != null)
            {
                toLuaMsg = null;
            }

            toLuaMsg = handler;
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        public static int GetSystemTime()
        {
           return System.Environment.TickCount;
        }
        /// <summary>
        /// 注册心跳
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="spacing"></param>
        public static void RegisterHeartBeat(HeartBeat fun, float spacing)
        {
            Debug.Log("注册心跳");
            CoroutineTool.Instance.StartCoroutine(SendHeartBeat(fun, spacing));
        }

        private static IEnumerator SendHeartBeat(HeartBeat heartBeat, float spacing)
        {
            while (true)
            {
                if (heartBeat != null)
                {
                    heartBeat();
                    yield return new WaitForSecondsRealtime(spacing);
                }
            }
        }
    }
}
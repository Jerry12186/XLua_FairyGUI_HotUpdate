using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetModel
{
    /// <summary>
    /// 封装一层，为了让使用起来比较明朗
    /// 这里处理网络连接，消息的打包，原生消息的获取（收到的消息解析放到Lua中去）
    /// </summary>
    public static class ConnectionMgr
    {
        private static Queue<Msg> msgQueue = new Queue<Msg>();
        private static MySocket _socket = new MySocket(msgQueue);
        private static Action NetExEvent;
        /// <summary>
        /// 同步建立网络连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool ConnectBySync(string ip, int port)
        {
            if (_socket.IsConnect())
            {
                _socket.CloseSocket();
            }

            try
            {
                _socket.ConnectBySyn(ip, port);
            }
            catch (Exception e)
            {
                Debug.LogError("同步建立网络连接失败--->" + e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 异步建立网络连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static bool ConnentByAsync(string ip, int port, AsyncCallback callback)
        {
            if (_socket.IsConnect())
            {
                _socket.CloseSocket();
            }
            try
            {
                _socket.ConnectByAsync(ip, port, callback);
            }
            catch (Exception e)
            {
                Debug.LogError("异步建立网络连接失败---->" + e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 注册网络异常事件
        /// </summary>
        public static void RegisterNetEx(Action exEvent)
        {
            NetExEvent = exEvent;
        }

        /// <summary>
        /// 关闭网络连接
        /// </summary>
        public static void CloseConnect()
        {
            if (_socket != null)
            {
                _socket.CloseSocket();
                Debug.Log("执行lua传过来的异常函数");
                NetExEvent();
                Debug.Log("暂停所有协程");
                CoroutineTool.Instance.StopAllCoroutines();
            }
        }

        /// <summary>
        /// 清空网络异常事件
        /// </summary>
        private static void ClearNetExEvent()
        {
            if (_socket != null)
                _socket.ClearNetEx();
        }

        /// <summary>
        /// 清空消息队列
        /// </summary>
        private static void ClearMsgQueue()
        {
            msgQueue.Clear();
        }

        /// <summary>
        /// 检测网络是否连接
        /// </summary>
        /// <returns></returns>
        private static bool IsConnect()
        {
            return _socket.IsConnect();
        }

        /// <summary>
        /// 重置网络
        /// </summary>
        public static void ResetNet()
        {
            if (IsConnect())
            {
                CloseConnect();
                ClearNetExEvent();
                ClearMsgQueue();
            }
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <returns></returns>
        public static Msg GetMsg()
        {
            if (msgQueue.Count > 0)
            {
                return msgQueue.Dequeue();
            }

            return null;
        }

        /// <summary>
        /// 获取消息数量
        /// </summary>
        /// <returns></returns>
        public static int GetMsgCount()
        {
            return msgQueue.Count;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buff"></param>
        public static void SendMsg(byte[] buff)
        {
            try
            {
                if (_socket != null)
                    _socket.SendMsg(buff);
            }
            catch (Exception e)
            {
                Debug.LogError("连接异常----->" + e);
                CloseConnect();
//                CoroutineTool.Instance.StopAllCoroutines();
            }
        }

        /// <summary>
        /// 打包消息
        /// </summary>
        /// <param name="msgId">消息id</param>
        /// <param name="data">消息数据，protocol的byte[]</param>
        public static byte[] EncodeMsg(int msgId, byte[] data = null)
        {
            List<byte> msgBuff = new List<byte>();
            int len = NetConfig.HEAD_LENGTH;
            if (data != null)
                len += data.Length;
            //长度
            byte[] lenBytes = BitConverter.GetBytes(len);
//            Array.Reverse(lenBytes);
            msgBuff.AddRange(lenBytes);
            //消息id
            byte[] msgIdBytes = BitConverter.GetBytes(msgId);
//            Array.Reverse(msgIdBytes);
            msgBuff.AddRange(msgIdBytes);
            //消息主体
            //这里不确定protocol那边是大字节在前，还是小字节在前
            //现按照小字节在前面处理
            if (data != null)
            {
                if (BitConverter.IsLittleEndian)
                {
//                    Array.Reverse(data);
                }
                msgBuff.AddRange(data);
            }
            return msgBuff.ToArray();
        }
        /// <summary>
        /// 解码消息
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static Msg DecodeMsgBuff(byte[] buff)
        {
            List<byte> msgBytes = new List<byte>();
            
            //长度
            byte[] lenBytes = new byte[4];
            for (int i = 0; i < lenBytes.Length; i++)
            {
                lenBytes[i] = buff[i];
            }
            msgBytes.AddRange(lenBytes);
            //消息Id
            byte[] msgIdBtyes = new byte[4];
            for (int i = 0; i < msgIdBtyes.Length; i++)
            {
                msgIdBtyes[i] = buff[i + 4];
            }
            msgBytes.AddRange(msgIdBtyes);
            //消息body
            if (buff.Length - 8 > 0)
            {
                byte[] body = new byte[buff.Length - 4];
                for (int i = 0; i < body.Length; i++)
                {
                    body[i] = buff[i + 8];
                }
//                Array.Reverse(body);
                msgBytes.AddRange(body);
            }

            return new Msg(msgBytes.ToArray());
        }
    }
}
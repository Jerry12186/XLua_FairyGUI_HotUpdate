using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace NetModel
{
    public class NetConfig
    {
        /// <summary>
        /// 消息格式
        /// 长度+msgId(int)+pb(byte[])
        /// 消息id(int)
        /// </summary>
        public static readonly int HEAD_LENGTH = 4; //消息头的长度,即msgId+pb的长度，不包括长度本身
    }

    /// <summary>
    /// 处理Socket相关的东西
    /// 网络连接，网络异常，收发消息等功能
    /// </summary>
    public class MySocket
    {
        public delegate void OnConnectUnexpected();

        /// <summary>
        /// 网络异常事件
        /// </summary>
        public event OnConnectUnexpected OnConnectionExEvent = null;

        /// <summary>
        /// 消息缓冲区
        /// </summary>
        private int MSG_BUF_MAX_SIZE = 20480;

        /// <summary>
        /// 消息偏移，
        /// </summary>
        private int msgOffset = 0;

        private object synLock = new object();
        private Socket _socket;
        private Thread thread = null;
        private bool bStartThread = false;
        private List<byte> msgBuff;
        public Queue<Msg> msgQueue = new Queue<Msg>();

        public MySocket(Queue<Msg> msgQueue)
        {
            this.msgQueue = msgQueue;
            msgBuff = new List<byte>();
        }

        /// <summary>
        /// 同步建立网络连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void ConnectBySyn(string ip, int port)
        {
            CloseSocket();
            ClearNetEx();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ip, port);
            //开一个线程，处理接收的消息
            if (thread == null)
            {
                bStartThread = true;
                thread = new Thread(new ThreadStart(MsgDisposeThread));
                thread.Start();
            }
        }

        /// <summary>
        /// 异步建立网络连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="callback"></param>
        public void ConnectByAsync(string ip, int port, AsyncCallback callback)
        {
            CloseSocket();
            ClearNetEx();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress tempIp;
            if (IPAddress.TryParse(ip, out tempIp))
            {
                _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), callback, this);
            }
            else
            {
                Debug.Log("域名服务器地址");
                _socket.BeginConnect(ip, port, callback, this);
            }

            //开一个线程，处理接收的消息
            if (thread == null)
            {
                bStartThread = true;
                thread = new Thread(new ThreadStart(MsgDisposeThread));
                thread.Start();
            }
        }

        /// <summary>
        /// 消息处理线程
        /// </summary>
        private void MsgDisposeThread()
        {
            while (true)
            {
                if (!bStartThread)
                    break;
                try
                {
                    lock (_socket)
                    {
                        if (_socket != null)
                        {
                            byte[] data = null;
                            int count = 0;
                            if (_socket.Available > 0 && msgOffset < MSG_BUF_MAX_SIZE)
                            {
                                if (_socket.Available > MSG_BUF_MAX_SIZE - msgOffset)
                                {
                                    data = new byte[MSG_BUF_MAX_SIZE - msgOffset];
                                    count = _socket.Receive(data, data.Length, SocketFlags.None);
                                }
                                else
                                {
                                    data = new byte[_socket.Available];
                                    count = _socket.Receive(data, data.Length, SocketFlags.None);
                                }

                                for (int i = 0; i < count; i++)
                                {
                                    msgBuff.Add(data[i]);
                                    ++msgOffset;
                                }

                                Msg msg = GetLogicMsg();
                                if (msg != null)
                                {
                                    msgQueue.Enqueue(msg);
                                }

                                //if (BHasAvailableMsg(msgBuff))
                                //{
                                //    Msg msg = new Msg(msgBuff);
                                //    msgQueue.Enqueue(msg);
                                //}
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("获取消息失败--->" + ex.ToString());
                    CloseSocket();
                    continue;
                }

                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// 检测网络是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnect()
        {
            if (_socket != null && _socket.Connected)
                return true;
            return false;
        }

        /// <summary>
        /// 关闭Socket连接
        /// </summary>
        public void CloseSocket()
        {
            lock (synLock)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                    CloseMsgThread();
                    Debug.Log("断开网络连接");
                }
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg(byte[] msg)
        {
            try
            {
                _socket.Send(msg);
            }
            catch (Exception ex)
            {
                OnConnectionExEvent();
                Debug.LogError("消息发送失败---->" + ex);
            }
        }

        /// <summary>
        /// 清除网络异常事件
        /// </summary>
        public void ClearNetEx()
        {
            OnConnectionExEvent = null;
        }

        public void CloseMsgThread()
        {
            try
            {
                lock (synLock)
                {
                    bStartThread = false;
                    thread.Abort();
                    Debug.Log("关闭接收消息线程");
                }
            }
            catch (Exception e)
            {
                Debug.Log("关闭消息线程出现异常");
            }
        }

        /// <summary>
        /// 判断是否有可用的Msgbuff
        /// 好像没啥用
        /// </summary>
        /// <returns></returns>
        private bool BHasAvailableMsg(byte[] msg)
        {
            if (msgOffset < NetConfig.HEAD_LENGTH)
                return false;
            Msg m = new Msg(msg);
            if (m.BAvailableMsg)
                return true;
            return false;
        }

        /// <summary>
        /// 获取一个逻辑消息,包括消息长度
        /// </summary>
        /// <returns></returns>
        private Msg GetLogicMsg()
        {
            if (msgOffset < NetConfig.HEAD_LENGTH)
                return null;

            //获取消息长度
            byte[] lenBytes = new byte[4];
            for (int i = 0; i < lenBytes.Length; i++)
            {
                lenBytes[i] = msgBuff[i];
            }

            int len = BitConverter.ToInt32(lenBytes, 0);
            if (len == msgBuff.Count - 4)
            {
                byte[] logicMsg = new byte[4 + NetConfig.HEAD_LENGTH];
                for (int i = 0; i < logicMsg.Length; i++)
                {
                    logicMsg[i] = msgBuff[i];
                }

                msgBuff.RemoveRange(0, logicMsg.Length);
                msgOffset -= logicMsg.Length;
                
                Msg msg = ConnectionMgr.DecodeMsgBuff(logicMsg);
                return msg;
            }

            return null;
        }
    }
}
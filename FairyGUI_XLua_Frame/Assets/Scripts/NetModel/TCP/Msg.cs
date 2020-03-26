using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetModel
{
   
    /// <summary>
    /// 此类定义消息的基本格式
    /// 消息格式：消息头+消息内容
    /// </summary>
    [XLua.LuaCallCSharp]
    public class Msg
    {
        //消息长度
        private int _len;
        //消息id
        private int _msgId;
        //消息体
        private byte[] _body;
        //消息内容
        private List<byte> msgBuff;
        //消息拷贝，备用
        private List<byte> msgBuffCopy;

        /// <summary>
        /// 消息长度
        /// </summary>
        public int Len
        {
            get
            {
                byte[] len = new byte[4];
                for (int i = 0; i < len.Length; i++)
                {
                    len[i] = msgBuff[i];
                }
                return BitConverter.ToInt32(len, 0);
            }
        }
        
        /// <summary>
        /// 消息id
        /// </summary>
        public int MsgId
        {
            get
            {
                byte[] msgIdBytes = new byte[4];
                for (int i = 0; i < msgIdBytes.Length; i++)
                {
                    msgIdBytes[i] = msgBuff[i + 4];
                }
                return BitConverter.ToInt32(msgIdBytes, 0);
            }
        }
        /// <summary>
        /// 消息体
        /// </summary>
        public byte[] Body
        {
            get
            {
                _body = new byte[msgBuff.Count - 8];
                for (int i = 0; i < _body.Length; i++)
                {
                    _body[i] = msgBuff[i + 8];
                }
                return _body;
            }
        }

        /// <summary>
        /// 判断消息是否可用
        /// </summary>
        public bool BAvailableMsg
        {
            get { return Len == GetMsgLengthByMsg(); }
        }
        public Msg() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">这是一个完整的消息，包括消息头的</param>
        public Msg(byte[] msg)
        {
            msgBuff = new List<byte>();
            msgBuff.AddRange(msg);
            msgBuffCopy = new List<byte>(msgBuff.ToArray());
        }

        /// <summary>
        /// 根据消息内容获取消息长度
        /// </summary>
        /// <returns></returns>
        private int GetMsgLengthByMsg()
        {
            int msgLength = 0;
            msgLength = msgBuffCopy.Count - 5;
            return msgLength;
        }

        /// <summary>
        /// 判断是否是有效消息
        /// 即消息里的长度，和消息本身的长度是否相同
        /// </summary>
        /// <returns></returns>
        public bool BCompleteMsg()
        {
            return Len == GetMsgLengthByMsg();
        }
    }
}
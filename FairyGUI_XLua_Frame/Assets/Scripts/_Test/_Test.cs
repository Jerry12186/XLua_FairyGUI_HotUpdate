using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using EventMgr;
using UnityEngine;
using NetModel;
public class _Test : MonoBehaviour
{
    Queue<Msg> msgQ = new Queue<Msg>();
    MySocket socket;

    // Use this for initialization
    void Start()
    {
        int a = 1;
        int b = 2;
//        Debug.Log(a=b);

        #region 消息测试

        //byte split = 0x11;
        //int len = 58;
        //byte version = 1;
        //long session = 10005;
        //int msgId = 1001;
        //int mainType = 1000;
        //short subType = 10;
        //string body = "啦啦啦，心烦啊啊啊啊啊啊啊 啊啊啊阿萨。";

        //List<byte> msg = new List<byte>();
        //msg.Add(split);
        //msg.AddRange(BitConverter.GetBytes(len));
        //msg.Add(version);
        //msg.AddRange(BitConverter.GetBytes(session));
        //msg.AddRange(BitConverter.GetBytes(msgId));
        //msg.AddRange(BitConverter.GetBytes(mainType));
        //msg.AddRange(BitConverter.GetBytes(subType));
        //msg.AddRange(System.Text.Encoding.Default.GetBytes(body));

        //Debug.Log("消息长度----->" + m.Len);
        //Debug.Log("消息版本----->" + m.Version);
        //Debug.Log("回应id------>" + m.Sessionid);
        //Debug.Log("消息id---->" + m.MsgId);
        //Debug.Log("消息主类型---->" + m.MainType);
        //Debug.Log("消息子类型---->" + m.SubType);
        //Debug.Log("消息内容---->" + System.Text.Encoding.Default.GetString(m.Body));

        #endregion

        #region 网络连接测试

        //socket = new MySocket(msgQ);
        //socket.ConnectBySyn("192.168.1.233", 2334);
        //socket.SendMsg(msg.ToArray());

        #endregion

        //测试最新2020.3.24
//        NetHelper.CreateConnectBySync("127.0.0.1", 8090);
//        NetHelper.SendMessage(0, 2, 1, 0);


//        NetHelper.CreateConnectByAsync("127.0.0.1", 8888, (isOk) =>
//        {
//            Debug.Log("啦啦啦啦");
//            NetHelper.RegisterHeartBeat(() => { Debug.Log(1111); }, 2);
//        });

    }


    private void Update()
    {
//        if (ConnectionMgr.GetMsgCount() > 0)
//        {
//            Msg msg = ConnectionMgr.GetMsg();
//            Debug.Log("消息Id---->"+msg.MsgId);
//        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using EventMgr;
using UnityEngine;

public class Test_326 : MonoBehaviour
{
    MyEvent e = new MyEvent();

    private void Start()
    {
        e.AddListener("Test1", Handler);
        e.AddListener("Test1", (p) => { Debug.Log("测试1----->2222"); });
        e.AddListener("Test2", (p) => { Debug.Log("测试2----->参数：" + p); });
        e.AddListener("Test2", (p) => { Debug.Log("测试2----->参数：" + p); });
    }

    void Handler(params object[] p)
    {
        Debug.Log("handler");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            e.Broadcast("Test1");
        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            e.Broadcast("Test2","assdsd");
            e.RemoveListener("Test1",Handler);
        }
    }
}
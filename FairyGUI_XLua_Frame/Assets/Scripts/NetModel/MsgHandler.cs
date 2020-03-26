using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace NetModel
{
    public class MsgHandler : MonoBehaviour
    {
        public static MsgHandler Instance()
        {
                if (null == _instance)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(MsgHandler).Name;
                    _instance = go.AddComponent<MsgHandler>();
                }
                return _instance;
        }
        private static MsgHandler _instance;
        private void Awake()
        {
            if (null != _instance)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        private void Update()
        {
            while (ConnectionMgr.GetMsgCount() > 0)
            {
                Msg m = ConnectionMgr.GetMsg();
                if (NetHelper.ToLuaMsg != null && m != null)
                    NetHelper.ToLuaMsg(m);
                else
                    Debug.LogError("没有找到Lua中的处理方法");
            }
        }

        private void OnDestroy()
        {
            
        }

    }
}
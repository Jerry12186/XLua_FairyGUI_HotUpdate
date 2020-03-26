using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace EventMgr
{
    [LuaCallCSharp]
    public class MyEvent
    {
        [CSharpCallLua]
        public delegate void Handler(params object[] p);//可以带参数的
        private Dictionary<string,EventClass> eventNamesDictionary = new Dictionary<string, EventClass>();

        /// <summary>
        /// 添加监控事件
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="luaFunction"></param>
        
        [LuaCallCSharp]
        public void AddListener(string eventName, Handler luaFunction)
        {
            if (!eventNamesDictionary.ContainsKey(eventName))
            {
                EventClass c = new EventClass();
                c.EventHandler += luaFunction;
                eventNamesDictionary.Add(eventName,c);
            }
            else
            {
                eventNamesDictionary[eventName].EventHandler += luaFunction;
            }
        }

        /// <summary>
        /// 需要通知各个订阅的函数，进行调用
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="p"></param>
        [LuaCallCSharp]
        public void Broadcast(string eventName, params object[] p)
        {
            if (!eventNamesDictionary.ContainsKey(eventName))
            {
                Debug.LogError("未注册事件----->" + eventName);
            }
            else
            {
                eventNamesDictionary[eventName].ExecuteHandler(p);
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="luaFunction"></param>
        [LuaCallCSharp]
        public void RemoveListener(string eventName, Handler luaFunction)
        {
            if (!eventNamesDictionary.ContainsKey(eventName))
            {
                Debug.LogError("未注册事件----->" + eventName); 
            }
            else
            {
                eventNamesDictionary[eventName].EventHandler -= luaFunction;
            }
        }
    }
    
    public class EventClass
    {
        public event MyEvent.Handler EventHandler;

        public void ExecuteHandler(params object[] p)
        {
            EventHandler(p);
        }
    }
}

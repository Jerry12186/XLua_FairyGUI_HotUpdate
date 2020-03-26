using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine;
using XLua;

namespace NetModel
{
    [LuaCallCSharp]
    public class HttpMgr
    {
        public static void HttpGet(string url, UnityAction<string> action)
        {
            CoroutineTool.Instance.StartCoroutine(Get(url, action));
        }

        public static void HttpPost(string url, WWWForm form, UnityAction<string> action)
        {
            CoroutineTool.Instance.StartCoroutine(Post(url, form, action));
        }

        private static IEnumerator Get(string url, UnityAction<string> action)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (string.IsNullOrEmpty(request.error))
            {
                action(request.downloadHandler.text);
            }
            else
            {
                action(null);
            }
        }

        private static IEnumerator Post(string url, WWWForm form, UnityAction<string> action)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (string.IsNullOrEmpty(request.error))
            {
                action(request.downloadHandler.text);
            }
            else
            {
                action(null);
            }
        }
    }
}
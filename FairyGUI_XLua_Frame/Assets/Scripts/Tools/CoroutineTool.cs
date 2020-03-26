using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 开启协程的工具
/// 我是一个木得感情的协程工具类
/// </summary>
public class CoroutineTool : MonoBehaviour
{
    private static CoroutineTool _instance;

    public static CoroutineTool Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject gameObject = new GameObject
                {
                    name = "CoroutineTool"
                };
                _instance = gameObject.AddComponent<CoroutineTool>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }
    private void OnDestroy()
    {
        _instance = null;
    }
}

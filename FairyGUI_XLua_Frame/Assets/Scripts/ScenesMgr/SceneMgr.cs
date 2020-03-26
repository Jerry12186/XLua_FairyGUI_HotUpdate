using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;
using Object = System.Object;

namespace MyScene
{
    [LuaCallCSharp]
    public class SceneMgr
    {
        [CSharpCallLua]
        public delegate void OnUpdateProgress(int progress);
        [CSharpCallLua]
        public delegate void OnLoaderEnd(string sceneName);

        /// <summary>
        /// 同步加载游戏场景
        /// 这个一般用来加载Loading场景
        /// </summary>
        /// <param name="sceneName"></param>
        public static void LoadSceneSyc(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadSceneAsync(string sceneName, OnUpdateProgress updateProgress, OnLoaderEnd endLoad)
        {
            CoroutineTool.Instance.StartCoroutine(LoadAsync(sceneName, updateProgress, endLoad));
        }

        /// <summary>
        /// 异步加载场景
        /// 多用于
        /// </summary>
        /// <param name="sceneName"></param>
        private static IEnumerator LoadAsync(string sceneName, OnUpdateProgress updateProgress, OnLoaderEnd onEndLoad = null)
        {
            int showProgress = 0;
            int toProgress = 0;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            if (operation.progress < 0.9)
            {
                toProgress = (int)operation.progress * 100;
                while (showProgress < toProgress)
                {
                    ++showProgress;
                    updateProgress(showProgress);
                    yield return new WaitForEndOfFrame();
                }
            }

            toProgress = 100;
            while (showProgress < toProgress)
            {
                ++showProgress;
                updateProgress(showProgress);
                if (showProgress == 99)
                     break;
                yield return new WaitForEndOfFrame();
            }
            onEndLoad(sceneName);
            operation.allowSceneActivation = true;
            //_asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            //if (_asyncOperation != null)
            //{
            //    onBeginLoad(_asyncOperation);
            //}

            //yield return _asyncOperation;
            //onEndLoad(sceneName);
        }
    }
}
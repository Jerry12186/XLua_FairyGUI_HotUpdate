using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using XLua;

namespace MyABSys
{
    /// <summary>
    /// 该类分平台处理资源的加载
    /// 在Editor下，直接加载Resources下的文件
    /// 移动平台通过AssetBundle加载
    /// </summary>
    [LuaCallCSharp]
    public class ResHelper : Singleton<ResHelper>
    {
        private Dictionary<string, int> UIDict = new Dictionary<string, int>();
        private int index = 0;
        private const string Config_UIPath = "UI/";

        public delegate void OnLoaderOver(Object obj);

        /// <summary>
        /// 加载FairyGUI
        /// UI加载异步加载有和现有的Lua有点冲突
        /// 所以UI加载只能采用同步
        /// 所以要求UI的资源的大小不能太大
        /// 所以缓存机制就很有必要了
        /// </summary>
        /// <param name="uiPackage">UI所在的Package</param>
        public void LoadUI(string uiPackage)
        {
#if UNITY_EDITOR
            UIPackage.AddPackage(uiPackage);
#else
            uiPackage = uiPackage.Replace(Config_UIPath, "");
            LoadUIFromABSync(uiPackage);
#endif
        }

        /// <summary>
        /// 同步加载资源其他
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>
        public Object LoadResFromABSync(string resName, bool bPublic = false)
        {
#if UNITY_EDITOR
            Object obj = null;
            if (!bPublic)
            {
                obj = Resources.Load(LocalizeTool.Instance.GetLocalResPath() + "/" + resName);
            }
            else
            {
                obj = Resources.Load(resName);
            }
            return obj;
#else
            return AssetBundleLoadMgr.Instance.LoadSync(resName, bPublic);
#endif
        }

        /// <summary>
        /// 异步加载其他资源
        /// </summary>
        /// <param name="resName"></param>
        public void LoadResFromABAsync(string resName, AssetBundleLoadMgr.AssetBundleLoadCallback callback,
            bool bPublic = false)
        {
#if UNITY_EDITOR
            CoroutineTool.Instance.StartCoroutine(LoadFromResAsync(resName, callback, bPublic));
#else
            AssetBundleLoadMgr.Instance.LoadAsync(resName, callback, bPublic);
#endif
        }

        public void Update()
        {
            AssetBundleLoadMgr.Instance.Update();
        }

        private IEnumerator LoadFromResAsync(string resName, AssetBundleLoadMgr.AssetBundleLoadCallback callback, bool bPublic = false)
        {
            if (bPublic)
            { }
            else
                resName = LocalizeTool.Instance.GetLocalResPath() + "/" + resName;
            ResourceRequest request = Resources.LoadAsync(resName);
            yield return request;

            if (request.asset == null)
            {
                Debug.LogError("加载的资源" + resName + "不存在");
            }
            else
            {
                callback(request.asset);
            }
        }

        #region 这个没有用了，但是不想删除

        private IEnumerator LoadUIFromAB(string UIPackage)
        {
            AssetBundle desc_bundle = null;
            AssetBundle res_bundle = null;

            if (!UIDict.ContainsKey(UIPackage))
            {
                UIDict.Add(UIPackage, index++);
                List<string> names = new List<string>
                {
                    AssetBundlePathConfig.Instance.UIPath + UIPackage + "_fui.cao",
                    AssetBundlePathConfig.Instance.UIPath + UIPackage + "_atlas0.cao"
                };

                for (int i = 0; i < names.Count; i++)
                {
                    UnityWebRequest abRequest = UnityWebRequestAssetBundle.GetAssetBundle(names[i]);
                    yield return abRequest.SendWebRequest();
                    if (!string.IsNullOrEmpty(abRequest.error))
                    {
                        Debug.LogError("UI资源" + names[i] + "加载错误，请检查路径是否正确");
                        abRequest.Dispose();
                    }
                    else
                    {
                        if (i == 0)
                        {
                            desc_bundle = DownloadHandlerAssetBundle.GetContent(abRequest);
                        }
                        else if (1 == i)
                        {
                            res_bundle = DownloadHandlerAssetBundle.GetContent(abRequest);
                        }
                        else
                        {
                            Debug.LogError("这个错误应该不会出现");
                        }
                    }
                }

                FairyGUI.UIPackage.AddPackage(desc_bundle, res_bundle);
                names.Clear();
            }
        }

        #endregion

        /// <summary>
        /// 同步加载UI AB
        /// 
        /// </summary>
        /// <param name="uiPackage"></param>
        private void LoadUIFromABSync(string uiPackage)
        {
            AssetBundle desc_bundle = null;
            AssetBundle res_bundle = null;

            if (!UIDict.ContainsKey(uiPackage))
            {
                UIDict.Add(uiPackage, index++);
                string desc_bundle_path = AssetBundlePathConfig.Instance.UIPath + uiPackage + "_fui.cao";
                string res_bundle_path = AssetBundlePathConfig.Instance.UIPath + uiPackage + "_atlas0.cao";
                int start = System.Environment.TickCount;
                desc_bundle = AssetBundle.LoadFromFile(desc_bundle_path);
                res_bundle = AssetBundle.LoadFromFile(res_bundle_path);
                Debug.Log("资源加载时间为===>" + (System.Environment.TickCount - start) + "ms");
                if (desc_bundle == null || res_bundle == null)
                {
                    Debug.LogError("要加载的UIPackage  " + uiPackage + "  不存在");
                }
                else
                {
                    UIPackage.AddPackage(desc_bundle, res_bundle);
                    desc_bundle = null;
                    res_bundle = null;
                }
            }
        }
    }
}
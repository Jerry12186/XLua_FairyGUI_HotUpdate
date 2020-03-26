using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using MyABSys;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 自制FairyGUI装载器
/// 方便加载外部资源
/// </summary>
public class MyLoader : GLoader
{
    protected override void LoadExternal()
    {
//        Debug.Log("-----" + url);
        if (url.Contains("http://") || url.Contains("https://"))
        {
            CoroutineTool.Instance.StartCoroutine(DownloadResFromWeb());
        }
        else
        {
            ///暂时用基类的
            ///后续需要对Editor和移动平台进行区分
            ///Editor直接加载Resources下的
            ///移动平台需要加载AssetBundle的
            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                Texture2D tex = (Texture2D) Resources.Load(LocalizeTool.Instance.GetLocalResPath() + "/" + this.url,
                    typeof(Texture2D));
                if (tex != null)
                    onExternalLoadSuccess(new NTexture(tex));
                else
                    onExternalLoadFailed();
            }
            else
            {
                ///这里涉及到公共资源了，需要修改
                /// 10.14 TODD
                /// 不用做了，仔细想想没什么用10.15
                ResHelper.Instance.LoadResFromABAsync(url, (obj)
                    =>
                {
                    string[] strs = url.Split('/');
                    AssetBundle assetBundle = obj as AssetBundle;
                    Debug.Log(strs[strs.Length - 1]);
                    var asset = assetBundle.LoadAsset(strs[strs.Length - 1]);
                    if (asset == null)
                    {
                        Debug.LogError("不存在资源" + url);
                        onExternalLoadFailed();
                    }
                    else
                    {
                        onExternalLoadSuccess(new NTexture(asset as Texture));
                    }
                });
            }
        }
    }

    /// <summary>
    /// 从网络上下载资源
    /// </summary>
    /// <returns></returns>
    IEnumerator DownloadResFromWeb()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        webRequest.downloadHandler = texDl;
        yield return webRequest.SendWebRequest();
        if (string.IsNullOrEmpty(webRequest.error))
        {
            Texture2D tex = null;
            tex = texDl.texture;
            if (tex != null)
                onExternalLoadSuccess(new NTexture(tex));
        }
        else
        {
            onExternalLoadFailed();
        }
    }
}
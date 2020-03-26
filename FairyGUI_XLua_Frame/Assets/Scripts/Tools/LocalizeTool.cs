using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

/// <summary>
///本地化工具
/// 对应系统语言的资源路径的根目录
/// </summary>
public class LocalizeTool : Singleton<LocalizeTool>
{
    string path = "";
    public string GetLocalResPath()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
                path = "CHS";  
                break;
            case SystemLanguage.ChineseSimplified:
                path = "CHS";  
                break;
            case SystemLanguage.ChineseTraditional:
                path = "CHT";
                break;
            case SystemLanguage.English:
                path = "EN";
                break;
            case SystemLanguage.Japanese:
                path = "JPN";
                break;
            default:
                //path = "Unknow";
                path = "EN";//语言默认为英语
                break;
        }

        return path;
    }

    /// <summary>
    /// 获取本地化的文本
    /// </summary>
    /// <returns></returns>
    public void InitLanguage()
    {
        TextAsset lanXML = null;
#if UNITY_EDITOR
        lanXML = Resources.Load<TextAsset>("Lan/" + this.path);
#else
        string path = AssetBundlePathConfig.Instance.Path + "/Lan/" + this.path+".cao";
        AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
        lanXML = assetBundle.LoadAsset(this.path) as TextAsset;
#endif
        if (lanXML != null)
        {
            FairyGUI.Utils.XML xml = new FairyGUI.Utils.XML(lanXML.ToString());
            UIPackage.SetStringsSource(xml);
        }
    }
}

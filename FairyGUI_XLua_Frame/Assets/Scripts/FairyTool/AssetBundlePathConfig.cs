using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundlePathConfig : Singleton<AssetBundlePathConfig>
{
    public readonly string Path;
    public readonly string UIPath;

    public AssetBundlePathConfig()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                Path = FilePathTool.Instance.Normalization(Application.persistentDataPath + "/Android", RuntimePlatform.Android);
                break;
            case RuntimePlatform.IPhonePlayer:
                Path = FilePathTool.Instance.Normalization(Application.persistentDataPath + "/iOS", RuntimePlatform.IPhonePlayer); ;
                break;
            case RuntimePlatform.OSXEditor:
                Path = "/Users/jerry/Documents/MyHotFixDir/iOS";
                break;
            default:
                Path = "D:/Android";//这个自己设置吧，暂定D盘Android文件夹下
                break;
        }

        UIPath = Path + "/ui/";
    }
}

/*
/*code is far away from bug with the animal protecting
    *  ┏┓　　　┏┓
    *┏┛┻━━━┛┻┓
    *┃　　　　　　　┃
    *┃　　　━　　　┃
    *┃　┳┛　┗┳　┃
    *┃　　　　　　　┃
    *┃      ┻　  　┃
    *┃　　　　　　　┃
    *┗━┓　　　┏━┛
    *　　┃　　　┃神兽保佑
    *　　┃　　　┃代码无BUG！
    *　　┃　　　┗━━━┓
    *　　┃　　　　　　　┣┓
    *　　┃　　　　　　　┏┛
    *　　┗┓┓┏━┳┓┏┛
    *　　　┃┫┫　┃┫┫
    *　　　┗┻┛　┗┻┛ 
    *　　　
    *              
*/

using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using MyABSys;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    private static Initialization _instance;
    public static void CreateInitialization()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject();
            go.name = "Initialization";
            _instance = go.AddComponent<Initialization>();
        }
    }
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        MyABSys.ResHelper.Instance.Update();
    }

    private void Init()
    {
        Main.CreateMain();
        NetModel.MsgHandler.Instance();
        //自定义的loader方便url加载外部资源
        FairyGUI.UIObjectFactory.SetLoaderExtension(typeof(MyLoader));
        InitLanguage();
    }

    /// <summary>
    /// 初始化UI的语言
    /// 这个就不需要用ResMgr加载了，语言文本需要一直都在的
    /// </summary>
    private void InitLanguage()
    {
        string localLan = LocalizeTool.Instance.GetLocalResPath();
        if ("Unknow" == localLan)
        {
            ///如果没有当前系统语言支持，提示玩家选择一种语言进行默认
            Debug.Log("抱歉，不支持您当前的系统语言，请选择要设置的语言");
            ///具体UI层面上怎么表示，暂时不管
        }
        else
        {
            LocalizeTool.Instance.InitLanguage();
        }
    }

    private void OnDestroy()
    {
        if (_instance != null)
            _instance = null;
    }
}

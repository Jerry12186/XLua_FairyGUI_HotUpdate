using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace UpdateSys
{
    public class UpdateMgr : MonoBehaviour
    {
        public bool bDebug = true;
        private UnityWebRequest webRequest = null;
        
        enum NetState
        {
            No,
            WiFi,
            Data//流量
        }

        NetState state = NetState.No;

        private void Awake()
        {
            //检查一下当前手机的网络状态
            //对玩家进行提示
            state = CheckNet();
        }
        
        private void Start()
        {
            BeginUpdate();
            Debug.unityLogger.logEnabled = bDebug;
        }

        private void Update()
        {
            if (FixFile.Instance.Request != null)
            {
//                DebugTool.Log(FixFile.Instance.DownloadingFileIndex);
//                DebugTool.Log(FixFile.Instance.Request.downloadProgress);
//                DebugTool.Log(FixFile.Instance.TotalProgress);
//                DebugTool.Log(FixFile.Instance.DownloadSpeed);
                UpdateUi.Instance.SetDownloadInfo(FixFile.Instance.TotalProgress, FixFile.Instance.DownloadSpeed);
            }

            if (webRequest != null)
            {
//                DebugTool.Log("要下载文件的长度(bytes):" + webRequest.downloadHandler.data.Length);
//                DebugTool.Log("下载文件当前长度(bytes)：" + webRequest.downloadedBytes);
            }
        }

        private void BeginUpdate()
        {
            if (state == NetState.No)
            {
                Debug.LogError("当前设备无网络连接，请稍后重试");
            }
            else
            {
                //连接WiFi，直接更新
                CheckState.Instance.Check(this, (code) =>
                {
                    FixFile.Instance.FindUpdataFile();
                    Debug.Log(code);
                    switch (code)
                    {
                        case CheckState.State.NoUpdate:
                            //直接进入游戏，加载游戏入口方法
                            StartGame();
                            break;
                        case CheckState.State.NeedUpdate:
                            //调用下载的方法
                            //需要先检测玩家手机的网络情况
                            switch (state)
                            {
                                case NetState.WiFi:
                                    Debug.Log("WiFi环境啦啦啦啦");
//                                    FixFile.Instance.DownloadFile(() =>
//                                    {
//                                        Debug.Log("更新完成了，可以开始主程序了");
//                                        StartGame();
//                                    });
                                    UpdateUi.Instance.SetDownloadFileInfo(
                                        FixFile.Instance.NeedDownloadFileAndBytes.Count,
                                        FixFile.Instance.TotalToDownload);
                                    break;
                                case NetState.Data:
//                                    Debug.Log("当前网络状况非WiFi，是否要下载?");
//                                    Debug.Log("需要下载" + FixFile.Instance.NeedDownloadFileAndBytes.Count + "个文件");
//                                    Debug.Log("总共大小为：" + FixFile.Instance.TotalToDownload + "MB");
                                    UpdateUi.Instance.SetDownloadFileInfo(
                                        FixFile.Instance.NeedDownloadFileAndBytes.Count,
                                        FixFile.Instance.TotalToDownload,false);
                                    break;
                            }
                            break;
                        case CheckState.State.Redownload:
                            if (Application.platform == RuntimePlatform.Android)
                            {
                                switch (state)
                                {
                                    case NetState.Data:
                                        break;
                                    case  NetState.WiFi:
                                        StartCoroutine(DownloadApk(() =>
                                        {
                                            Debug.Log("下载完成了");
                                        }));
                                        break;
                                }
                                FixFile.Instance.DownloadApk();
                            }
                            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                            {
                                //这里让玩家直接跳转到App Store更新软件
                                //提供的是一个跳转到App Store的URL
                            }
                            else if (Application.platform == RuntimePlatform.OSXEditor)
                            {
                                Debug.Log("需要重新下载安装包");
//                                FixFile.Instance.DownloadApk();
                            }
                            else if(Application.platform == RuntimePlatform.WindowsEditor)
                            {
                                Debug.Log("需要重新下载安装包");
                            }
                            break;
                    }

                });
            }
        }

        private NetState CheckNet()
        {
            NetState state = NetState.No;

            if (Application.internetReachability == NetworkReachability.NotReachable)
                state = NetState.No;
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                state = NetState.Data;
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                state = NetState.WiFi;

            return state;
        }

        private void StartGame()
        {
            Initialization.CreateInitialization();
            //MyABSys.ResMgr.Instance.LoadUI("lrspackage",
            //    () =>
            //    {
            //        Window win = new Window();
            //        win.contentPane = UIPackage.CreateObject("LrsPackage", "LoginWin").asCom;
            //        win.Show();
            //    });
            //MyABSys.ResMgr.Instance.LoadUI("lrspackage");
            //Window win = new Window();
            //win.contentPane = UIPackage.CreateObject("LrsPackage", "LoginWin").asCom;
            //win.Show();
        }

        /// <summary>
        /// 下载APK是一个很简单的事情
        /// 就在这个管理工具里完成吧
        /// </summary>
        private IEnumerator DownloadApk(Action callback)
        {
            string downloadURL = UpdateConfig.Instance.ApkUrl;
            webRequest = UnityWebRequest.Get(downloadURL);
            yield return webRequest.SendWebRequest();

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.LogError("文件下载错误。");
            }
            else
            {
                string savePath = "";
                try
                {
                    savePath = UpdateConfig.Instance.LocalPath + "/update.apk";
                    using (FileStream fs = File.Create(savePath))
                    {
                        fs.Write(webRequest.downloadHandler.data, 0, webRequest.downloadHandler.data.Length);
                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                webRequest = null;
                ///自动安装需要调用Android底层的东西，这个暂时不弄
                /// TODO
                Application.OpenURL(savePath);
            }
        }
    }
}
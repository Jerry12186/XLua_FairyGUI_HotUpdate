using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace UpdateSys
{
    public class Download : MonoBehaviour
    {
        private bool bDownloading = false;
        private int downloadingIndex;
        private Dictionary<string, double> toDownFileInof;
        private float currentFileProgress = 0;
        UnityWebRequest webRequest;

        public delegate void OnDownloadEvent();

        /// <summary>
        /// 下载失败，弹窗提示玩家网络问题
        /// </summary>
        public event OnDownloadEvent OnDownloadFail;

        public event OnDownloadEvent OnDownlaodSucess;
        public event OnDownloadEvent OnAllSuceess;

        public static Download CreateDownloadObj()
        {
            GameObject go = new GameObject();
            go.name = "DontMoveMe";
            Download download = go.AddComponent<Download>();
            return download;
        }


        public void StartDown(Dictionary<string, double> downloadList)
        {
            StartCoroutine(StartDownload(downloadList));
        }

        public void StartDownloadApk()
        {
            StartCoroutine(DownloadAPk());
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="serverPath">服务器端文件的地址</param>
        /// <param name="localPath">保存的地址</param>
        /// <returns></returns>
        private IEnumerator StartDownload(Dictionary<string, double> downloadList)
        {
            bDownloading = true;
            FixFile.Instance.DownloadingFileIndex = 1;
            toDownFileInof = new Dictionary<string, double>();
            toDownFileInof = downloadList;

            foreach (var item in downloadList)
            {
                Debug.Log("正在下载第" + FixFile.Instance.DownloadingFileIndex + "个文件" + "------" + item.Key);
                string serverPath = UpdateConfig.Instance.ResServerRoot + item.Key;
                webRequest = UnityWebRequest.Get(serverPath);
                FixFile.Instance.Request = webRequest;
                yield return webRequest.SendWebRequest();

                if (!string.IsNullOrEmpty(webRequest.error))
                {
                    Debug.LogError("文件" + serverPath + "下载失败,请检查下载路径");
                    if (OnDownloadFail != null)
                        OnDownloadFail();
                    bDownloading = false;
                    webRequest.Dispose();
                    yield break;
                }
                else
                {
                    string local = UpdateConfig.Instance.LocalPath + item.Key;
                    try
                    {
                        string saveDir = Path.GetDirectoryName(local);
                        if (!Directory.Exists(saveDir))
                        {
                            Directory.CreateDirectory(saveDir);
                        }

                        using (FileStream fs = File.Create(local))
                        {
                            fs.Write(webRequest.downloadHandler.data, 0, webRequest.downloadHandler.data.Length);
                            fs.Close();
                        }
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError(local + "文件存储本地异常");
                        webRequest.Dispose();
                    }

                    if (OnDownlaodSucess != null)
                        OnDownlaodSucess();
                    FixFile.Instance.DownloadingFileIndex++;
                    webRequest.Dispose();
                }
            }

            ///全部下载完成
            if (OnAllSuceess != null)
                OnAllSuceess();
            bDownloading = false;
        }

        /// <summary>
        /// 下载apk文件
        /// 使Android玩家能更新到最新的apk
        /// </summary>
        private IEnumerator DownloadAPk()
        {
            string downloadURL = UpdateConfig.Instance.ApkUrl;
            Debug.Log(downloadURL);
            webRequest = UnityWebRequest.Get(downloadURL);
            FixFile.Instance.Request = webRequest;
            yield return webRequest.SendWebRequest();

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.LogError("游戏更新失败，请检查你的网络");
                if (OnDownloadFail != null)
                {
                    OnDownloadFail();
                }
            }
            else
            {
                //apk保存的位置
                try
                {
                    string savePath = UpdateConfig.Instance.LocalPath + "/update.apk";
                    using (FileStream fs = File.Create(savePath))
                    {
                        fs.Write(webRequest.downloadHandler.data, 0, webRequest.downloadHandler.data.Length);
                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                if (OnDownlaodSucess != null)
                {
                    OnDownlaodSucess();
                }
                webRequest.Dispose();
            }
        }
    }
}
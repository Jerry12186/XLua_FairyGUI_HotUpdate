using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PackageSys;
using UnityEngine.Networking;

namespace UpdateSys
{
    public class FixFile : Singleton<FixFile>
    {
        private string localPath = UpdateConfig.Instance.LocalPath;
        private double downloadedBytes = 0;//已经下载的字节数
        private List<double> toDownloadList;//需要下载的字节list
        //public List<string> NeedDownloadFile { get; private set; }
        /// <summary>
        /// 需要下载的总大小
        /// </summary>
        public double TotalToDownload { get; private set; }
        /// <summary>
        /// 当前文件的进度
        /// </summary>
        public UnityWebRequest Request { get; set; }
        /// <summary>
        /// 正在下载的第几个文件
        /// </summary>
        public int DownloadingFileIndex { get; set; }

        /// <summary>
        /// 总进度
        /// </summary>
        public float TotalProgress
        {
            get
            {
                //当前进度 = （已经下载的文件的字节数+当前下载文件的字节数 x 当前文件进度）/总字节数
                double currentBytes = downloadedBytes + Request.downloadedBytes;

                return (float)(currentBytes / TotalToDownload);
            }
        }

        private float oldTime = 0;
        private double oldBytes = 0;
        private bool bFirst = true;
        private float speed = -1;
        /// <summary>
        /// 下载速度
        /// </summary>
        public float DownloadSpeed
        {
            get
            {
                double current = downloadedBytes + Request.downloadedBytes;
                if (System.Environment.TickCount - oldTime > 1000)
                {
                    double bytesSpan = current - oldBytes;
                    if (bytesSpan > 0)
                    {
                        speed = (float)bytesSpan / (System.Environment.TickCount - oldTime);

                        oldBytes = downloadedBytes + Request.downloadedBytes;
                        oldTime = System.Environment.TickCount;
                    }
                }

                return speed;
            }
        }
        public Dictionary<string, double> NeedDownloadFileAndBytes { get; private set; }
        /// <summary>
        /// 找出要更新的文件
        /// </summary>
        public void FindUpdataFile()
        {
            try
            {
                localPath = FilePathTool.Instance.Normalization(localPath, Application.platform);

                if (!Directory.Exists(localPath))//如果不存在本地储存路径，则创建
                {
                    Directory.CreateDirectory(localPath);
                }

                Dictionary<string, string> localFiles = new Dictionary<string, string>();
                DirectoryInfo dirInfo = new DirectoryInfo(localPath);
                foreach (FileInfo file in dirInfo.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (file.Name.EndsWith(".manifest")) continue;
                    if (file.Name.Contains("Md5.txt")) continue;
                    string key = MD5Tool.Instance.GetMd5(file.FullName);
                    localFiles.Add(key,file.FullName);//本地文件的md5，去与服务对比，如果与服务器的一样，则不需要下载
                }

                ///与服务器的MD5进行对比，默认服务器的需要全部下载
                ///把本地的相同的除去，则是确实需要下载的
                Dictionary<string, string> serverFile = new Dictionary<string, string>();
                toDownloadList = new List<double>();
                serverFile = CheckState.Instance.Md5Dir;
                if (serverFile != null)
                {
                    foreach (var item in localFiles)
                    {
                        if(!serverFile.ContainsKey(item.Key))
                        {
                            ///删除服务器中没有的本地文件，防止玩家手机内存越占越多
                            Debug.Log("正在删除文件" + item.Value);
                            File.Delete(item.Value);
                        }
                        if (serverFile.ContainsKey(item.Key))
                        {
                            serverFile.Remove(item.Key);
                        }
                    }
                    NeedDownloadFileAndBytes = new Dictionary<string, double>();
                    foreach (var item in serverFile)
                    {
                        string[] strs = item.Value.Split('|');
                        NeedDownloadFileAndBytes.Add(strs[0], double.Parse(strs[1]));
                        toDownloadList.Add(double.Parse(strs[1]));
                        TotalToDownload += double.Parse(strs[1]);
                    }
                    //这个地方不处理了，留个UI层去处理，直接返回字节数就行
                    //UI层处理示例如下，返回MB
                    //TotalToDownload = System.Math.Round(TotalToDownload / 1024 / 1024, 2);
                }
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// 下载更新文件
        /// </summary>
        /// <param name="callback">下载完成之后的回调</param>
        public void DownloadFile(System.Action callback)
        {
            Download download = Download.CreateDownloadObj();
            download.StartDown(NeedDownloadFileAndBytes);
            download.OnDownloadFail += () =>
            {
                Debug.Log("下载失败，请检查你的网络");
                ///进行UI提示
                callback();
            };
            download.OnDownlaodSucess += () =>
              {
                  downloadedBytes += toDownloadList[DownloadingFileIndex - 1];
                  Debug.Log("下载完成");
              };
            download.OnAllSuceess += () =>
            {
                DownloadingFileIndex = -1;
                Request = null;
                ///开始游戏主程序
                callback();
            };
        }

        public void DownloadApk()
        {
            Download download = Download.CreateDownloadObj();
            download.StartDownloadApk();
            download.OnDownloadFail += () =>
            {
                Debug.LogError("下载失败");
            };
            download.OnDownlaodSucess += () =>
            {
                Debug.Log("下载成功");
            };
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace UpdateSys
{
    /// <summary>
    /// 流程说明
    /// 1.先下载MD5文件，统计一下要下载的文件个数以及下载的大小
    /// 2.再获取版本号，判断需不需要更新
    /// </summary>
    public class CheckState : Singleton<CheckState>
    {
        public delegate void GetServerText(string code);
        public delegate void CheckCallBack(State state);

        public Dictionary<string, string> Md5Dir { get; private set; }

        public enum State
        {
            Fail,//检查更新失败
            NoUpdate,
            NeedUpdate,
            Redownload//重新下载
        }


        public void Check(MonoBehaviour mono, CheckCallBack callback)
        {
            mono.StartCoroutine(GetMd5List(mono, UpdateConfig.Instance.VersionUrl,(version)
                =>
            {
                if (string.IsNullOrEmpty(version))
                {
                    callback(State.Fail);
                    return;
                }
                string localVersion = LocalVersion();
                if (string.IsNullOrEmpty(localVersion))
                {
                    callback(State.NeedUpdate);
                    return;
                }
                if (localVersion == version)
                {
                    callback(State.NoUpdate);
                }
                else
                {
                    string[] _local = localVersion.Split('.');
                    string[] _server = version.Split('.');
                    Debug.Log(localVersion + "--------" + version);
                    if (_local[0] != _server[0] || _local[1] != _server[1])
                    {
                        callback(State.Redownload);
                    }
                    else
                    {
                        callback(State.NeedUpdate);
                    }
                }
                ///这个地方已经把MD5文件下载完成了
            }));

            #region 暂时不用
            //mono.StartCoroutine(GetVersion(serverUrl + "/Android/version.txt", (version) =>
            //   {
            //       if (string.IsNullOrEmpty(version))
            //       {
            //           callback(State.Fail);
            //           return;
            //       }
            //       string localVersion = LocalVersion();
            //       if (string.IsNullOrEmpty(localVersion))
            //       {
            //           callback(State.NeedUpdate);
            //           return;
            //       }
            //       if (localVersion == version)
            //       {
            //           callback(State.NoUpdate);
            //       }
            //       else
            //       {
            //           string[] _local = localVersion.Split('.');
            //           string[] _server = version.Split('.');
            //           if (_local[0] == _server[0])
            //           {
            //               callback(State.NeedUpdate);
            //           }
            //           else
            //           {
            //               callback(State.Redownload);
            //           }
            //       }

            //   }));
            #endregion
        }


        /// <summary>
        /// 本地版本号获取
        /// </summary>
        /// <returns></returns>
        private string LocalVersion()
        { 
            //获取当的版本号，打包的时候注意版本号对应
            //这里说的对应，包括Unity中的版本号，以及version.txt版本
            //Unity的版本控制下包更新，所以，version中前两位必须和Unity的保持一致
            //version的后一位是资源更新控制
            string realVersion = Application.version;
            string version = null;
            if(File.Exists(UpdateConfig.Instance.LocalVersionPath))
            {
                using (StreamReader sr = File.OpenText(UpdateConfig.Instance.LocalVersionPath))
                {
                    version = sr.ReadToEnd();
                    string[] strs = version.Split('.');
                    version = strs[strs.Length - 1];
                }
                return realVersion + "." + version;
            }

            return realVersion + ".-1";//-1是为了防止隔版本的是直接下载资源，浪费玩家的流量
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>

        private IEnumerator GetVersion(string url, GetServerText callback)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.LogError(url + "-----获取版本号异常");
                callback(null);
                yield break;
            }
            callback(webRequest.downloadHandler.text);
        }

        /// <summary>
        /// 获取md5文件
        /// </summary>
        /// <param name="url">version的路径</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator GetMd5List(MonoBehaviour mono, string url,GetServerText callback)
        {
            string md5Url = UpdateConfig.Instance.Md5URL;
            UnityWebRequest webRequest = UnityWebRequest.Get(md5Url);
            webRequest.timeout = 15;//设置下timeout，可以不设置，不过默认的时间有点长
            yield return webRequest.SendWebRequest();

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.LogError(md5Url + "-----获取md5文件异常");
                callback(null);
                yield break;
            }
            ParseMd5String(webRequest.downloadHandler.text);
          
            mono.StartCoroutine(GetVersion(url, callback));
        }

        private void ParseMd5String(string str)
        {
            Md5Dir = new Dictionary<string, string>();
            using (StringReader sr = new StringReader(str))
            {
                string line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    string[] strs = line.Split('\t');
                    //MD5码做为key，确保唯一性
                    Md5Dir.Add(strs[1], strs[0]);
                    line = sr.ReadLine();
                }
            }
            //foreach (var item in Md5Dir)
            //{
            //    DebugTool.Log(item.Key + "------" + item.Value);
            //}
        }
    }
}
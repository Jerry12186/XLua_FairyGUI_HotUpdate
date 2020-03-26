using UnityEngine;

namespace UpdateSys
{
    public class UpdateConfig : Singleton<UpdateConfig>
    {
        public readonly string ResServerRoot;
        public readonly string VersionUrl;
        public readonly string Md5URL;

        ///需要重新下载包的时候，自动下载的地址
        /// 好像iOS不支持这种更新模式，需要玩家跳转到App Store商店
        public readonly string ApkUrl;

        public readonly string LocalPath;
        public readonly string LocalVersionPath;

#if UNITY_EDITOR_OSX
        private string serverUrl = "http://127.0.0.1/"; //家里电脑的ip
#elif UNITY_EDITOR_WIN
        private string serverUrl = "http://192.168.1.233/";//公司电脑的IP
#else
        private string serverUrl = "http://192.168.3.8/"; //家里电脑的ip
#endif
        public UpdateConfig()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ResServerRoot = serverUrl + "Android";
                    ApkUrl = serverUrl + "update.apk";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ResServerRoot = serverUrl + "iOS";
                    break;
                case RuntimePlatform.OSXEditor:
                    ResServerRoot = serverUrl + "iOS";
//                    ApkUrl = serverUrl + "update.apk";
                    break;
                default:
                    ResServerRoot = serverUrl + "Android";
                    ;
                    break;
            }

            VersionUrl = ResServerRoot + "/version.txt";
            Md5URL = ResServerRoot + "/Md5.txt";

#if UNITY_EDITOR_OSX
            LocalPath = "/Users/jerry/Documents/MyHotFixDir/iOS"; //Mac的下载文件保存的位置
#elif UNITY_EDITOR_WIN
             LocalPath = "D:/Android";//PC测试
#else
            LocalPath = Application.persistentDataPath + "/"+Application.platform.ToString();//移动平台
#endif
            LocalVersionPath = LocalPath + "/version.txt";
        }
    }
}
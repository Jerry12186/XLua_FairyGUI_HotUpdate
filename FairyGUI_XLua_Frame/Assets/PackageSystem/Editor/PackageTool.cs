using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PackageSys
{
    public class PackageTool
    {
        public const string version = "1.0.0";
        public const string finalLuaPath = "FinalLua";//处理后lua文件夹
#if (UNITY_EDITOR_OSX)
        static string subPath = "iOS";
#else
        static string subPath = "Android";
#endif
        // private string bundlePath = Application.persistentDataPath+"/"+subP;
        [MenuItem("工具/当前版本" + version)]
        public static void CurrentVersion()
        {
            //没什么卵用       
        }
        #region 就是想关了它
        //[MenuItem("工具/处理Lua文件/去掉后缀")]
        //public static void DivideVariant()
        //{
        //    HeadleLuaFile();
        //}
        //[MenuItem("工具/处理Lua文件/添加后缀")]
        //public static void AddVarian()
        //{
        //    ResetLuaFile();
        //}
        [MenuItem("工具/打开AB输出目录")]
        public static void OpenAssetBundlePath()
        {
            string bundlePath = subPath;
            if (!Directory.Exists(bundlePath))
            {
                Directory.CreateDirectory(bundlePath);
            }
            Application.OpenURL(bundlePath);
        }


        //[MenuItem("工具/Tag相关/Tag资源")]
        //public static void Tag()
        //{
        //    TagRes(BuildTarget.Android);
        //}
        [MenuItem("工具/清除资源Tag")]
        public static void ClearTag()
        {
            string[] allFiles = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < allFiles.Length; i++)
            {
                if (allFiles[i].EndsWith(".cs")) continue;
                AssetImporter importer = AssetImporter.GetAtPath(allFiles[i]);
                importer.assetBundleName = "";
            }

            Debug.Log("ABTag清除完成");
        }
#if (UNITY_EDITOR_OSX)
        [MenuItem("工具/打包AB for iOS")]
        public static void BuildForiOS()
        {
            BuildAB(BuildTarget.iOS);
        }
#else
        [MenuItem("工具/打包AB for Android")]
        public static void BuildForAndroid()
        {
            BuildAB(BuildTarget.Android);
        }
#endif
        #endregion

        private static void TagRes(BuildTarget target)
        {
            HeadleLuaFile(target);
            //需要打包的文件夹
            string[] paths =
            {
                "/"+finalLuaPath,
                "/Resources",
            };
            for (int i = 0; i < paths.Length; i++)
            {
                string path = Application.dataPath + paths[i];
                DirectoryInfo directory = new DirectoryInfo(path);
                FileInfo[] fileInfo = directory.GetFiles("*.*", SearchOption.AllDirectories);
                for (int j = 0; j < fileInfo.Length; j++)
                {
                    FileInfo curFile = fileInfo[j];
                    if (curFile.Name.EndsWith(".meta")) continue;
                    string fullPath = FilePathTool.Instance.Normalization(curFile.FullName, target);
                    string fileName = Path.GetFileName(fullPath);
                    string basePath = fullPath.Replace(Application.dataPath, "Assets");

                    AssetImporter importer = AssetImporter.GetAtPath(basePath);
                    if (null == importer) continue;
                    if (fileName.Contains(" "))
                    {
                        Debug.LogError("文件" + fileName + "命名非法");
                    }

                    if (fileName.EndsWith(".lua"))
                    {
                        EditorUtility.DisplayDialog("警告", "有后缀为.lua的文件，无法打包，请先处理.lua文件", "ok");
                        return;
                    }
                    else
                    {
                        string assetName = "";
                        //assetName = fullPath.Replace(Application.dataPath + "/", "");
                        //if (i != 0)//把Lua代码文件夹保留
                        assetName = fullPath.Replace(Application.dataPath + paths[i] + "/", "");
                        string[] names = assetName.Split('.');
                        importer.assetBundleName = names[0];
                        importer.assetBundleVariant = "cao";
                        importer.SaveAndReimport();
                    }
                }
                AssetDatabase.Refresh();
            }
        }

        private static void BuildAB(BuildTarget target)
        {
            string outputBundlePath = subPath;
            //打包之前，先把原来的文件给删除了
            DeletFianlLua(outputBundlePath);
            if (!Directory.Exists(outputBundlePath))
            {
                Directory.CreateDirectory(outputBundlePath);
            }
            //string mainfestName = Path.GetFileName(outputBundlePath);
            TagRes(target);
            /*AssetBundleManifest mainfest = */
            BuildPipeline.BuildAssetBundles(outputBundlePath, BuildAssetBundleOptions.None, target);
            DeletFianlLua(Application.dataPath + "/" + finalLuaPath);
            EncryptLua(target);
            CreateMD5File(target);
            Application.OpenURL(outputBundlePath);
            Debug.Log("打包完成");
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Unity 不支持Lua打成AssetBundle包，
        /// 这里把Lua文件后缀修改成txt，方便打包
        /// 先把lua文件改成txt，然后复制到Asset下的某个文件夹下，确保打包的时候不影响原来的文件
        /// </summary>
        private static void HeadleLuaFile(BuildTarget target)
        {
            string luaFilePath = Application.dataPath + "/LuaScripts";
            string _newPath = Application.dataPath + "/" + finalLuaPath;
            DirectoryInfo directory = new DirectoryInfo(luaFilePath);
            FileInfo[] fileInfo = directory.GetFiles("*.*", SearchOption.AllDirectories);

            for (int i = 0; i < fileInfo.Length; i++)
            {
                string _old = fileInfo[i].FullName;
                if (_old.EndsWith(".meta")) continue;
                _old = FilePathTool.Instance.Normalization(_old, target);
                //string _new = _old.Split('.')[0] + ".txt";
                string _new = _newPath + "/" + _old.Replace(Application.dataPath, "") + ".txt";
                _new = _new.Replace(".lua", "");
                string fileDir = Path.GetDirectoryName(_new);
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                File.Copy(_old, _new);
            }
            AssetDatabase.Refresh();
        }
        #region 这个没有用了，但是舍不得删除
        /// <summary>
        /// 这里还原Lua后缀，方面编写
        /// 在这里给lua脚本解密
        /// </summary>
        private static void ResetLuaFile()
        {
            //HeadleLuaFile();//先去掉，防止重复添加后缀
            string luaFilePath = Application.dataPath + "/LuaScripts";
            DirectoryInfo directory = new DirectoryInfo(luaFilePath);
            FileInfo[] fileInfo = directory.GetFiles("*.*", SearchOption.AllDirectories);

            for (int i = 0; i < fileInfo.Length; i++)
            {
                string _old = fileInfo[i].FullName;
                if (_old.EndsWith(".meta")) continue;
                string _new = _old.Split('.')[0] + ".lua";
                File.Move(_old, _new);
            }

            AssetDatabase.Refresh();
        }
        #endregion
        /// <summary>
        /// 加密lua AssetBundle
        /// </summary>
        private static void EncryptLua(BuildTarget target)
        {
            //string path = subPath + "/LuaScripts";
            string assetPath = subPath + "/LuaScripts";
            DirectoryInfo directory = new DirectoryInfo(assetPath);
            FileInfo[] fileInfo = directory.GetFiles("*.cao", SearchOption.AllDirectories);
            foreach (FileInfo file in fileInfo)
            {
                string _var = file.FullName;
                _var = FilePathTool.Instance.Normalization(_var, target);
                byte[] oldBytes = File.ReadAllBytes(_var);
                byte[] newBytes = LuaEncrypt.Instance.Encrypt(oldBytes);

                string newPath = _var;
                string fileDir = Path.GetDirectoryName(newPath);
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                File.WriteAllBytes(newPath, newBytes);
                string _new = newPath.Split('.')[0] + ".kannimane";
                File.Move(newPath, _new);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 打包完成后删除文件夹，防止二次打包出现问题
        /// </summary>
        private static void DeletFianlLua(string path)
        {
            if (!Directory.Exists(path)) return;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        //如果 使用了 streamreader 在删除前 必须先关闭流 ，否则无法删除 sr.close();
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
                Directory.Delete(path);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 创建MD5文件，用于更新检查
        /// </summary>
        private static void CreateMD5File(BuildTarget target)
        {
            string path = subPath;
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] fileInfo = directory.GetFiles("*.*", SearchOption.AllDirectories);
            using (StreamWriter vsw = File.CreateText(path + "/version.txt"))
            {
                vsw.WriteLine(version);
                vsw.Flush();
            }

            using (StreamWriter sw = File.CreateText(path + "/Md5.txt"))
            {
                foreach (FileInfo file in fileInfo)
                {
                    string fullPath = file.FullName;
                    if (fullPath.EndsWith(".manifest")) continue;
                    fullPath = FilePathTool.Instance.Normalization(fullPath, target);
                    //DebugTool.Log(fullPath + "----" + MD5Tool.Instance.GetMd5(fullPath));
                    string parentPath = FilePathTool.Instance.Normalization(Path.GetFullPath(path), target);
                    string subPath1 = fullPath.Replace(parentPath, "");
                    Int64 fileLength = file.Length;//获取文件的字节数

                    string line = subPath1 + "|" + fileLength + "\t" + MD5Tool.Instance.GetMd5(fullPath);
                    sw.WriteLine(line);
                }
                ///因为md5.txt每次更新必须要下载
                ///而且还必须要倒数第二个下载（倒数第一个是版本号），
                ///所以以时间戳作为md5文件的特征码写入md5.txt
                ///反正是固定的，干脆写死算了
                sw.WriteLine("/Md5.txt|6\t" + DateTime.Now.ToFileTimeUtc());
                //写入版本号
                string tempPath = FilePathTool.Instance.Normalization(path + "/version.txt", target);
                string tempSubPath = tempPath.Replace(subPath, "");
                sw.WriteLine(tempSubPath + "|9\t" + MD5Tool.Instance.GetMd5(tempPath));
                sw.Flush();
            }
        }
    }
}
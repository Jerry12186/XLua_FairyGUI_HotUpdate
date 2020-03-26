using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyABSys
{
    public class AssetBundleLoadMgr : Singleton<AssetBundleLoadMgr>
    {
        public delegate void AssetBundleLoadCallback(Object asset);

        public class AssetBundleObject
        {
            public string name;
            public int refCount;
            public List<AssetBundleLoadCallback> _callbacks = new List<AssetBundleLoadCallback>();

            public AssetBundleCreateRequest request;
            public AssetBundle assetbundle;

            public bool bPublic = false; //是否是公共资源

            /// <summary>
            ///依赖个数
            /// </summary>
            public int _dependLoadingCount;

            public List<AssetBundleObject> depends = new List<AssetBundleObject>(); //该AB的所有依赖
        }

        private const int MAX_LOADING_NUM = 50; //最多同时加载的数量
        public Dictionary<string, string[]> _dependsDataList;
        private List<AssetBundleObject> _tempABList;
        private Dictionary<string, AssetBundleObject> _readyABList; //预备加载的列表
        private Dictionary<string, AssetBundleObject> _loadingABList; //正在加载的列表
        private Dictionary<string, AssetBundleObject> _loadedABList; //加载完成的列表
        private Dictionary<string, AssetBundleObject> _unloadABList; //准备卸载的列表

        private bool bLoadMainfest = false;
        public AssetBundleLoadMgr()
        {
            _dependsDataList = new Dictionary<string, string[]>();
            _readyABList = new Dictionary<string, AssetBundleObject>();
            _loadingABList = new Dictionary<string, AssetBundleObject>();
            _loadedABList = new Dictionary<string, AssetBundleObject>();
            _unloadABList = new Dictionary<string, AssetBundleObject>();
            _tempABList = new List<AssetBundleObject>();
        }

        /// <summary>
        /// 不管怎么说，先加载Mainfest总不会错的
        /// </summary>
        public void LoadMainfest()
        {
            ///PC暂定为Android
            /// MacOS为iOS
            string subPath = "";
#if UNITY_EDITOR_OSX || UNITY_IPHONE
            subPath = "/iOS";
#elif UNITY_EDITOR_WIN || UNITY_ANDROID
            subPath = "/Android";
#endif
            string path = AssetBundlePathConfig.Instance.Path + subPath;
            _dependsDataList.Clear();

            AssetBundle ab = AssetBundle.LoadFromFile(path);
            if (ab == null)
            {
                Debug.LogError("LoadMainfest ab NULL error !");
            }
            else
            {
                AssetBundleManifest mainfest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (mainfest == null)
                {
                    Debug.LogError("LoadMainfest NULL error !");
                }
                else
                {
                    foreach (var assetName in mainfest.GetAllAssetBundles())
                    {
                        //DebugTool.Log(assetName);
                        string hashName = assetName.Replace(".cao", "");
                        string[] dps = mainfest.GetAllDependencies(assetName);
                        for (int i = 0; i < dps.Length; i++)
                            dps[i] = dps[i].Replace(".cao", "");
                        _dependsDataList.Add(hashName, dps);
                    }

                    ab.Unload(true);
                    ab = null;

                    Debug.Log("AssetBundleLoadMgr dependsCount=" + _dependsDataList.Count);
                    bLoadMainfest = true;
                }
            }
        }

        /// <summary>
        /// 获取一个文件的Hash值
        /// 这个值是自定义的
        /// 在此方法里自定义
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private string GetHashName(string assetName)
        {
            return assetName.ToLower();
        }

        private string GetFileName(string assetName)
        {
            return assetName + ".cao";
        }

        /// <summary>
        /// 获取一个AB的路径
        /// </summary>
        /// <returns></returns>
        private string GetAssetBundlePath(string hasName, bool bPublic = false)
        {
            ///暂时不做本地化处理
            ///本地化处理，需要在这里对加载路径进行处理
            ///下面是例子

            //这个地方需要修改一下
            //作为公共的资源，比如模型，则不需要，BGM等没必要本地化的资源放到本地文件夹中
            //以节省玩家的存储空间
            if (bPublic)
            {
                hasName = AssetBundlePathConfig.Instance.Path + "/" + hasName;
            }
            else
            {
                hasName = AssetBundlePathConfig.Instance.Path + "/" +
                          GetHashName(LocalizeTool.Instance.GetLocalResPath()) + "/" +
                          hasName;
            }

            //DebugTool.Log(hasName);
            //hasName = AssetBundlePathConfig.Instance.Path + "/" + hasName;
            return GetFileName(hasName);
        }

        /// <summary>
        /// 同步加载
        /// 每次调用refCount  +1
        /// </summary>
        /// <param name="_hasName"></param>
        /// <returns></returns>
        public AssetBundle LoadSync(string _hasName,bool bPublic = false)
        {
            string hasName = GetHashName(_hasName);
            var abObj = LoadAssetBundleSync(hasName);
            return abObj.assetbundle;
        }

        /// <summary>
        /// 异步加载
        /// 每次调用refCount +1
        /// </summary>
        /// <param name="_hasName"></param>
        /// <param name="callback"></param>
        public void LoadAsync(string _hasName, AssetBundleLoadCallback callback, bool bPublic = false)
        {
            _hasName = GetHashName(_hasName);
            LoadAssetBundleAsync(_hasName, callback, bPublic);
        }

        /// <summary>
        /// 卸载
        /// 引用计数-1
        /// </summary>
        /// <param name="assetName"></param>
        public void Unload(string assetName)
        {
            string hasName = GetHashName(assetName);
            UnloadAssetBundleAsync(hasName);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="_hasName"></param>
        /// <returns></returns>
        private AssetBundleObject LoadAssetBundleSync(string _hasName, bool bPublic = false)
        {
            AssetBundleObject abObj = null;
            if (_loadedABList.ContainsKey(_hasName)) //已经加载了
            {
                abObj = _loadedABList[_hasName];
                abObj.refCount++;

                foreach (var dpObj in abObj.depends)
                {
                    LoadAssetBundleSync(dpObj.name);
                }

                return abObj;
            }
            else if (_loadingABList.ContainsKey(_hasName)) //正在加载中
            {
                abObj = _loadingABList[_hasName];
                abObj.refCount++;
                foreach (var dpObj in abObj.depends)
                {
                    LoadAssetBundleSync(dpObj.name);
                }

                DoLoadedCallFun(abObj, false);
                return abObj;
            }
            else if (_readyABList.ContainsKey(_hasName)) //在准备加载中
            {
                abObj = _readyABList[_hasName];
                abObj.refCount++;
                foreach (var dpObj in abObj.depends)
                {
                    LoadAssetBundleSync(dpObj.name);
                }

                string abPath = GetAssetBundlePath(_hasName, bPublic);
                abObj.assetbundle = AssetBundle.LoadFromFile(abPath);
                _readyABList.Remove(abObj.name);
                _loadedABList.Add(abObj.name, abObj);

                DoLoadedCallFun(abObj, false);

                return abObj;
            }

            abObj = new AssetBundleObject();
            abObj.name = _hasName;
            abObj.bPublic = bPublic;
            abObj.refCount = 1;
            string path = GetAssetBundlePath(_hasName, bPublic);
            abObj.assetbundle = AssetBundle.LoadFromFile(path);

            if (abObj.assetbundle == null)
            {
                Debug.LogError(path + "不存在");
                return null;
            }

            string[] dependData = null;
            if (_dependsDataList.ContainsKey(_hasName))
            {
                dependData = _dependsDataList[_hasName];
            }

            if (dependData != null && dependData.Length > 0)
            {
                abObj._dependLoadingCount = 0;
                foreach (var dpAssetName in dependData)
                {
                    var dpObj = LoadAssetBundleSync(dpAssetName);
                    abObj.depends.Add(dpObj);
                }
            }

            _loadedABList.Add(abObj.name, abObj);

            return abObj;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="_hasName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private AssetBundleObject LoadAssetBundleAsync(string _hasName, AssetBundleLoadCallback callback,
            bool bPublic = false)
        {
            AssetBundleObject abObj = null;
            if (_loadedABList.ContainsKey(_hasName)) //已经加载
            {
                abObj = _loadedABList[_hasName];
                DoDependsRef(abObj);
                callback(abObj.assetbundle);

                return abObj;
            }
            else if (_loadingABList.ContainsKey(_hasName)) //正在加载
            {
                abObj = _loadingABList[_hasName];
                DoDependsRef(abObj);
                abObj._callbacks.Add(callback);
                return abObj;
            }
            else if (_readyABList.ContainsKey(_hasName))
            {
                abObj = _readyABList[_hasName];
                DoDependsRef(abObj);
                abObj._callbacks.Add(callback);
                return abObj;
            }

            abObj = new AssetBundleObject();
            abObj.name = _hasName;
            abObj.bPublic = bPublic;
            abObj.refCount = 1;
            abObj._callbacks.Add(callback);
            ///先加载依赖项
            ///这里设置一个约定
            ///公共资源的依赖项必须放在公共资源文件夹中
            ///即不能放在CNS,EN等文件夹中
            string[] dependsData = null;

            if (_dependsDataList.ContainsKey(_hasName))
            {
                Debug.Log("加载物体的名字" + _hasName);
                dependsData = _dependsDataList[_hasName];
                foreach (string name in dependsData)
                {
                    Debug.Log(name);
                }
            }

            if (dependsData != null && dependsData.Length > 0)
            {
                abObj._dependLoadingCount = dependsData.Length;
                //依赖依次加载
                foreach (var dpName in dependsData)
                {
                    AssetBundleObject dpObj = LoadAssetBundleAsync(dpName,
                        (ab) =>
                        {
                            if (abObj._dependLoadingCount <= 0)
                            {
                                Debug.LogError("LoadAssetBundle: " + _hasName + " depends error");
                            }
                            else
                            {
                                abObj._dependLoadingCount--;
                                //依赖加载完成，回调
                                if (abObj._dependLoadingCount == 0 && abObj.request != null && abObj.request.isDone)
                                {
                                    DoLoadedCallFun(abObj);
                                }
                            }
                        }, bPublic);
                    abObj.depends.Add(dpObj);
                }
            }

            if (_loadingABList.Count < MAX_LOADING_NUM)
            {
                DoLoad(abObj);
                _loadingABList.Add(_hasName, abObj);
            }
            else
            {
                _readyABList.Add(_hasName, abObj);
            }

            return abObj;
        }

        private void DoLoadedCallFun(AssetBundleObject abObj, bool bAsync = true)
        {
            if (abObj.request != null)
            {
                abObj.assetbundle = abObj.request.assetBundle;
                abObj.request = null;
                _loadingABList.Remove(abObj.name);
                _loadedABList.Add(abObj.name, abObj);
            }

            if (abObj.assetbundle == null)
            {
                ///这里可以添加下载
                ///如果游戏太大，可以等玩家先下载游戏之后，剩余的资源等用到再下载
            }

            foreach (var callback in abObj._callbacks)
            {
                callback(abObj.assetbundle);
            }

            abObj._callbacks.Clear();
        }

        /// <summary>
        /// 异步加载添加引用计数
        /// </summary>
        /// <param name="abObj"></param>
        private void DoDependsRef(AssetBundleObject abObj)
        {
            abObj.refCount++;
            if (abObj.depends.Count == 0) return;
            foreach (var dps in abObj.depends)
            {
                DoDependsRef(dps);
            }
        }

        private void DoLoad(AssetBundleObject abObj)
        {
            string path = GetAssetBundlePath(abObj.name,abObj.bPublic);
            abObj.request = AssetBundle.LoadFromFileAsync(path);
            if (abObj.request == null)
            {
                string errormsg = string.Format("LoadAssetbundle path error ! assetName:{0}", abObj.name);
                Debug.LogError(errormsg);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="_hasName"></param>
        private void UnloadAssetBundleAsync(string _hasName)
        {
            AssetBundleObject abObj = null;
            if (_loadedABList.ContainsKey(_hasName))
                abObj = _loadedABList[_hasName];
            else if (_loadingABList.ContainsKey(_hasName))
                abObj = _loadingABList[_hasName];
            else if (_readyABList.ContainsKey(_hasName))
                abObj = _readyABList[_hasName];

            if (abObj == null)
            {
                Debug.LogError("Unload Asset" + _hasName + "Fail");
            }
            else
            {
                abObj.refCount--;
                foreach (var dpObj in abObj.depends)
                {
                    UnloadAssetBundleAsync(dpObj.name);
                }

                if (abObj.refCount == 0)
                {
                    _unloadABList.Add(_hasName, abObj);
                }
            }
        }

        private void UpdateLoad()
        {
            if (_loadingABList.Count == 0) return;
            _tempABList.Clear();

            foreach (var abObj in _loadingABList.Values)
            {
                if (abObj._dependLoadingCount == 0 && abObj.request != null && abObj.request.isDone)
                {
                    _tempABList.Add(abObj);
                }
            }

            foreach (var abObj in _tempABList)
            {
                DoLoadedCallFun(abObj);
            }
        }

        /// <summary>
        /// 卸载
        /// </summary>
        private void UpdateUnload()
        {
            if (_unloadABList.Count == 0) return;
            _tempABList.Clear();

            foreach (var abObj in _unloadABList.Values)
            {
                if (abObj.refCount == 0 && abObj.assetbundle != null)
                {
                    DoUnload(abObj);
                    _loadedABList.Remove(abObj.name);
                    _tempABList.Add(abObj);
                }

                if (abObj.refCount > 0) //这段存疑
                {
                    //引用计数加回来（销毁又瞬间重新加载，不销毁，从销毁列表移除）
                    _tempABList.Add(abObj);
                }
            }

            foreach (var abObj in _tempABList)
            {
                _unloadABList.Remove(abObj.name);
            }
        }

        private void DoUnload(AssetBundleObject abObj)
        {
            if (abObj.assetbundle != null)
            {
                abObj.assetbundle.Unload(true);
                abObj.assetbundle = null;
            }
            else
            {
                Debug.LogError("LoadAssetbundle DoUnload error !" + abObj.name);
            }
        }

        private void UpdateReadyLoad()
        {
            if (_readyABList.Count == 0) return;
            if (_loadingABList.Count >= MAX_LOADING_NUM) return;
            _tempABList.Clear();
            foreach (var abObj in _readyABList.Values)
            {
                DoLoad(abObj);
                _tempABList.Add(abObj);
                _loadingABList.Add(abObj.name, abObj);
                if (_loadingABList.Count >= MAX_LOADING_NUM) break;
            }

            foreach (var abObj in _tempABList)
            {
                _readyABList.Remove(abObj.name);
            }
        }

        public void Update()
        {
            if (!bLoadMainfest)
                LoadMainfest();
            UpdateLoad();
            UpdateReadyLoad();
            UpdateUnload();
        }
    }
}
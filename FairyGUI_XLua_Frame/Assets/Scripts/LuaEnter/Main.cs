using UnityEngine;
using XLua;
using System.IO;
using PackageSys;
using System;

[Hotfix]
public class Main : MonoBehaviour
{

    private static Main _instance;
    public static void CreateMain()
    {
        if(_instance==null)
        {
            GameObject go = new GameObject();
            go.name = "ImEnter";
            _instance = go.AddComponent<Main>();
        }
    }

    LuaEnv env;
   
#if UNITY_ANDROID
    string subPath = "/Android";
# elif UNITY_IPHONE
    string subPath = "/iOS";
#endif

    // Use this for initialization
    void Start()
    {
        env = new LuaEnv();
        //添加json解析工具
        env.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
        env.AddBuildin("pb",XLua.LuaDLL.Lua.LoadLuaProfobuf);
        env.AddLoader(LuaLoader);
        env.DoString("require 'Main'");
    }

    // Update is called once per frame
    void Update()
    {
        if (env != null)
        {
            env.Tick();
        }
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    SceneManager.LoadScene(0);
        //}
    }

    void OnDestroy()
    {
        //env.Dispose();
        if (_instance != null)
            _instance = null;
    }
    [Hotfix]
    byte[] LuaLoader(ref string filename)
    {
        string path = "";
#if UNITY_EDITOR
        path = Application.dataPath + "/LuaScripts/" + filename + ".lua";

        if (File.Exists(path))
        {
            byte[] content = File.ReadAllBytes(path);
            return content;
        }
        return null;
#else
        #region 这部分是给移动端准备的
        path = Application.persistentDataPath + subPath +"/LuaScripts/" + filename + ".kannimane";
        byte[] ciphertext = new byte[1];
        try
        {
            ciphertext = File.ReadAllBytes(path);
        }
        catch (Exception e)
        {
            Debug.Log("这个错误不会改");
        }
        if (ciphertext.Length != 1)
        {
            byte[] plaintext = LuaEncrypt.Instance.Deencrypt(ciphertext);

            AssetBundle asset = AssetBundle.LoadFromMemory(plaintext);
            //AssetBundle asset = AssetBundle.LoadFromFile(path);

            if (asset != null)
            {
                if (filename.Contains("/"))
                {
                    string[] str = filename.Split('/');
                    filename = str[str.Length - 1];
                }
                object obj = asset.LoadAsset(filename);
                byte[] result = System.Text.Encoding.UTF8.GetBytes(obj.ToString());
                return result;
            }
        }
        return null;
        #endregion
#endif
    }

    private void OnDisable()
    {
        //env.DoString(@"xlua.hotfix(CS.Main,'OnButtonClick',nil)");
    }
}

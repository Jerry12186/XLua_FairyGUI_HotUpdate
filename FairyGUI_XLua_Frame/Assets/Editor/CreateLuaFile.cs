using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CreateLuaFile
{
    public const string defaultName = "NewLuaBehaviourScript";

    [MenuItem("Assets/Create/LuaScript", false, 0)]
    public static void CreateLua()
    {
        var selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var path = Application.dataPath.Replace("Assets", "") + "/";
        var newFileName = "new_" + "lua" + "." + "lua";
        var newFilePath = selectPath + "/" + newFileName;
        var fullPath = path + newFilePath;

        //简单的重名处理
        if (File.Exists(fullPath))
        {
            var newName = "new_" + "lua" + "-" + UnityEngine.Random.Range(0, 100) + "." + "lua";
            newFilePath = selectPath + "/" + newName;
            fullPath = fullPath.Replace(newFileName, newName);
        }

        //如果是空白文件，编码并没有设成UTF-8
//        File.WriteAllText(fullPath, "--开始编写你的lua脚本", Encoding.UTF8);
        StreamWriter witer = File.CreateText(fullPath);
        witer.Dispose();
        AssetDatabase.Refresh();

        //选中新创建的文件
        var asset = AssetDatabase.LoadAssetAtPath(newFilePath, typeof(Object));
        Selection.activeObject = asset;
    }
}

using UnityEngine;
#if(UNITY_EDITOR)
using UnityEditor;
#endif

/// <summary>
/// 文件路径工具
/// </summary>
public class FilePathTool : Singleton<FilePathTool>
{
    /// <summary>
    /// 规范化至对应平台
    /// </summary>
    /// <param name="path"></param>
    /// <param name="platfrom"></param>
    /// <returns></returns>
    public string Normalization(string path, RuntimePlatform platfrom)
    {
        string Separator = "/";
        if (platfrom == RuntimePlatform.IPhonePlayer)
        {
            Separator = @"/";
        }

        path = Process(path, Separator);

        return path;

    }

#if (UNITY_EDITOR)
    /// <summary>
    /// 规范化至对应平台
    /// </summary>
    /// <param name="path"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public string Normalization(string path, BuildTarget target)
    {
        string Separator = "/";
        if (target == BuildTarget.iOS)
        {
            Separator = @"/";
        }
        path = Process(path, Separator);

        return path;
    }
#endif


    private string Process(string path, string Separator)
    {
        path = path.Replace(@"\\", Separator);
        path = path.Replace(@"\", Separator);
        path = path.Replace(@"/", Separator);
        path = path.Replace(@"//", Separator);
        path = path.Replace(@"/\", Separator);
        path = path.Replace(@"\/", Separator);
        return path;
    }
}
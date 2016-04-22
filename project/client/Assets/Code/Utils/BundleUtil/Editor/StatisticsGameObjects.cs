using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class StatisticsGameObjects
{
    //选中的资源后过滤掉不必要的资源只剩下预设
    public static Object[] GetFilterGameObject()
    {
        Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable);//过滤只剩下预设
        return objs;
    }
    //获取文件夹下面的预设 过滤掉文件夹 只剩下预设
    public static Object[] GetFolderGameObject()
    {
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Editable);//过滤掉只剩下预设和文件夹
        int i, count = objs.Length;
        GameObject go;
        List<Object> list = new List<Object>();
        for (i = 0; i < count; i++)
        {
            go = objs[i] as GameObject;
            if (go)
            {
                list.Add(go);
            }
        }
        return list.ToArray();
    }
    //一致统计GameObjects alban
    //获取先关依赖路径 过滤掉不必要的cs文件
    public static List<string> GetRelevancesFilterPath(string[] relevances)
    {
        List<string> filterPaths = new List<string>();
        int i = 0, count = relevances.Length;
        string tempPath, suffix;
        for (i = 0; i < count; i ++)
        {
            tempPath = relevances[i];
            suffix = Path.GetExtension(tempPath);//获取依赖Path的后缀名
            if (!suffix.Contains(".cs"))
            {
                filterPaths.Add(tempPath);
            }
        }
        return filterPaths;
    }
    //获取相关go上面的依赖资源的路径
    public static List<UnityEngine.Object> GetRelevancesRes(string[] relevances)
    {
        int i, count = relevances.Length;
        string dep, suffix;
        List<UnityEngine.Object> list = new List<UnityEngine.Object>();
        //遍历相关的依赖资源
        for (i = 0; i < count; i ++ )
        {
            dep = relevances[i];
            suffix = Path.GetExtension(dep);//获取后缀名
            if(!suffix.Contains(".cs"))//非脚本则执行
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(suffix, typeof(UnityEngine.Object));
                if (obj is Material || obj is Shader || obj is Texture)
                {
                    list.Add(obj);
                }
            }
        }
        return list;
    }
    //获取材质球的贴图路径
    public static List<string> GetMaterialTexturePathByMaterial(Material material, int type = 0)
    {
        MaterialProperty[] mpList = MaterialEditor.GetMaterialProperties(new UnityEngine.Object[] {material});
        int i, count = mpList.Length;
        List<string> list = new List<string>();
        string path;
        if(type == 1)//shader
        {
            if (material.shader)
            {
                path = AssetDatabase.GetAssetPath(material.shader);
                if (!string.IsNullOrEmpty(path)) list.Add(path);
            }
            return list;
        }
        MaterialProperty mp;
        for (i = 0; i < count; i ++)
        {
            mp = mpList[i];
            if (mp.textureValue != null)//贴图
            {
                path = AssetDatabase.GetAssetPath(mp.textureValue);
                if (!string.IsNullOrEmpty(path)) list.Add(path);
            }
        }
        return list;
    }
    //设置绑定bundle
    public static string GetBundleAtPathByObject(Object go)
    {
        if (go == null)
        {
            Debug.LogError("set bundle name :: Obejct is null");
            return null;
        }
        string url = GetResourcePathByGameObejct(go);
        AssetImporter aImporter = AssetImporter.GetAtPath(url);
        aImporter.assetBundleName = go.name + ".bundle";
        return aImporter.assetBundleName;
    }
    //获取资源查看通过路径
    public static UnityEngine.Object GetLookIntoObjectByPath(string path)
    {
        return AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
    }
    //获取相关资源路径
    public static string GetResourcePathByGameObejct(Object obj)
    {
        return AssetDatabase.GetAssetPath(obj);
    }
    //获取依赖GameObejct上的相关资源
    public static string[] GetDependencieListByPath(string path)
    {
        return AssetDatabase.GetDependencies(new string[] { path });
    }
    //获取后缀类型名通过路径
    public static GameObjectsInfo.Type GetTypeByPath(string path)
    {
        GameObjectsInfo.Type result = GameObjectsInfo.Type.empty;
        UnityEngine.Object obj = GetLookIntoObjectByPath(path);
        if (obj != null)
        {
            if (obj is Texture) result = GameObjectsInfo.Type.texture;
            else if (obj is Material) result = GameObjectsInfo.Type.material;
            else if (obj is Shader) result = GameObjectsInfo.Type.shader;
            else if (path.Contains(".fbx") || path.Contains(".FBX")) result = GameObjectsInfo.Type.fbx;
        }
        return result;
    }
    //获取查看object类型是否属于 texture、matrail、fbx、shader
    public static bool GetTypeByObject(UnityEngine.Object into, string suffix = null, int intoTye = 1, List<string> list = null)
    {
        if (intoTye == 1)
        {
            if (into is Texture || into is Material || into is Shader || (suffix.Contains("fbx") || suffix.Contains("FBX")))
            {
                return true;
            }
        }
        else 
        {
            if (!string.IsNullOrEmpty(suffix))
            {
                suffix = Path.GetExtension(suffix).ToLower();
            }
            if(into is Texture || into is Material || list.Contains(suffix))
            {
                return true;
            }
        }
        return false;
    }
}

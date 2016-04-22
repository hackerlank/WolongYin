using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GameObjectsInfo
{
    //GameObejcts信息存储 author alban
    
    //保存AssetEelement信息字典
    public Dictionary<string, AssetElement> elementHasMap = new Dictionary<string, AssetElement>();
    /// <summary>
    /// 引用的数据集合
    /// </summary>
    public struct AssetElement
    {
        public int refCount;//引用次数
        public Type refType;//引用类型
        public string pathName;//引用路径名字
        public string goName;//选中的GameObject名字
        public AssetElement(int count, Type type, string pathName, string goName)
        {
            refCount = count;
            refType = type;
            this.pathName = pathName;
            this.goName = goName;
        }
    }
    //结果写入文本中
    public void WriteAssetElementToTxt()
    {
        StringBuilder result = new StringBuilder();
        List<AssetElement> list;
        Dictionary<string, List<AssetElement>> planHasMap = new Dictionary<string, List<AssetElement>>();//规划分类
        Dictionary<string, List<AssetElement>> textureHasMap = new Dictionary<string, List<AssetElement>>();//贴图分类
        string pathName;
        foreach(AssetElement element in elementHasMap.Values)
        {
            pathName = element.pathName;
            string[] split = pathName.Split('.');
            if (split.Length < 1) continue;
            string lower = split[1].ToLower();
            if (element.refType == Type.texture)
            {
                if (textureHasMap.ContainsKey(lower))
                {
                    list = textureHasMap[lower];
                    list.Add(element);
                }
                else 
                {
                    list = new List<AssetElement>();
                    list.Add(element);
                    textureHasMap[lower] = list;
                }
            }
            else 
            {
                if (planHasMap.ContainsKey(lower))
                {
                    list = planHasMap[lower];
                    list.Add(element);
                }
                else
                {
                    list = new List<AssetElement>();
                    list.Add(element);
                    planHasMap[lower] = list;
                }
            }
        }
        AssetElement meRef;
        int i, count;
        UnityEngine.Object go;
        foreach (List<AssetElement> value in planHasMap.Values)
        {
            count = value.Count;
            for (i = 0; i < count; i ++)
            {
                meRef = value[i];
                if (meRef.refCount > 1)
                {
                    go = StatisticsGameObjects.GetLookIntoObjectByPath(meRef.pathName);
                    meRef.goName = StatisticsGameObjects.GetBundleAtPathByObject(go);//设置bundle
                    result.AppendLine(meRef.pathName + "  RefCount:" + meRef.refCount.ToString());
                }
            }
        }
        //遍历贴图
        foreach (List<AssetElement> value in textureHasMap.Values)
        {
            count = value.Count;
            for (i = 0; i < count; i ++ )
            {
                meRef = value[i];
                if (meRef.refCount > 1)
                {
                    go = StatisticsGameObjects.GetLookIntoObjectByPath(meRef.pathName);
                    meRef.goName = StatisticsGameObjects.GetBundleAtPathByObject(go);//设置bundle
                    result.AppendLine(meRef.pathName + "  RefCount:" + meRef.refCount.ToString());
                }
            }
        }
        SaveToFileStream(result.ToString());
    }
    //刷新引用的数量 在写入之前刷新
    public void RefurbishRefCount()
    {
        foreach (AssetElement element in elementHasMap.Values)
        {
            //只刷新贴图
            if (element.refType == Type.texture && element.refCount >= 1)
            {
            }
        }
    }
    //写入文件
    void SaveToFileStream(string result)
    {
        FileStream fs = new FileStream("E://FindRefResultText.txt", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(result);//写入流
        sw.Flush();//清理缓存重新写入基础流
        sw.Close();
    }
    /// <summary>
    /// 引用的类型
    /// </summary>
    public enum Type
    {
        empty,
        texture,
        shader,
        fbx,
        material
    }
}

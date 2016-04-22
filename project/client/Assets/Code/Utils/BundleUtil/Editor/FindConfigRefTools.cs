using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using GameObject = UnityEngine.GameObject;
using Object = UnityEngine.Object;
public class FindConfigRefTools
{
    /// <summary>
    /// 配置查找类型
    /// </summary>
    //[MenuItem("FindRefGameObjectTools/依赖配置&选中资源")]
    public static void FindSettingResTools()
    {
        FindRefGameObjects.FindRefPitchOnGameObejcts(StatisticsGameObjects.GetFilterGameObject(), OnLoadSettingXML(), 2);
    }
    [MenuItem("FindRefGameObjectTools/依赖配置&选中文件夹 | 同时选中文件夹和资源")]
    public static void FindSettingEndFolderRes()
    {
        FindRefGameObjects.FindRefPitchOnGameObejcts(StatisticsGameObjects.GetFolderGameObject(), OnLoadSettingXML(), 2);
    }
    /// <summary>
    ///  选中查找
    /// </summary>
   // [MenuItem("FindRefGameObjectTools/选中资源")]
    static void FindSelectionRef()
    {
        FindRefGameObjects.FindRefPitchOnGameObejcts(StatisticsGameObjects.GetFilterGameObject());
    }
    /// <summary>
    ///  查找文件夹的预设
    /// </summary>
   // [MenuItem("FindRefGameObjectTools/选中文件夹 | 同时选中文件夹和资源")]
    static void FindFolderGameObject()
    {
        FindRefGameObjects.FindRefPitchOnGameObejcts(StatisticsGameObjects.GetFolderGameObject());
    }
    //加载xml配置解析
    private static string OnLoadSettingXML()
    {
        string result = string.Empty;
        string path = "setting/setting.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlNode node = xmlDoc.SelectSingleNode("setting").SelectSingleNode("suffix");
        result = node.InnerText;
        return result;
    }
}

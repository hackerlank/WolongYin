using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class FindRefGameObjects {

    //开始查找
    //[MenuItem("FindRefTools/FindRefPitchOnGameObejcts")]
    public static void FindRefPitchOnGameObejcts(Object[] goList = null, string config =  null, int findType = 1)
    {
        AssetDatabase.Refresh();
        Object[] objPitchOns = goList;
        if (objPitchOns.Length <= 0) return;
        List<string> configList = new List<string>();
        if (!string.IsNullOrEmpty(config)) configList.AddRange(config.Split('|'));
        GameObjectsInfo goListInfo = new GameObjectsInfo();
        int i = 0, j, l, count = objPitchOns.Length;
        string result = string.Empty;
        do
        {
            GameObject go = objPitchOns[i] as GameObject;//获取单个选中的预设
            go.name = StatisticsGameObjects.GetBundleAtPathByObject(go);
            string path = StatisticsGameObjects.GetResourcePathByGameObejct(go);
            string[] relevances = StatisticsGameObjects.GetDependencieListByPath(path);
            List<string> filterList = StatisticsGameObjects.GetRelevancesFilterPath(relevances);//获取相关的依赖列表
            List<string> objPathList = new List<string>();//过滤好的列表
            string filterPath, tempPath;
            for (j = 0; j < filterList.Count; j++)
            {
                filterPath = filterList[j];
                if (filterPath != null)
                {
                    Object lookInto = StatisticsGameObjects.GetLookIntoObjectByPath(filterPath);
                    if (lookInto != null)
                    {
                        bool objType = StatisticsGameObjects.GetTypeByObject(lookInto, filterPath, findType, configList);
                        if (objType)
                        {
                            objPathList.Add(filterPath);
                        }
                    }
                }
            }
            for (j = 0; j < objPathList.Count; j++)
            {
                tempPath = objPathList[j];
                GameObjectsInfo.AssetElement element;
                GameObjectsInfo.Type type;
                Material material;
                List<string> listPath;
                if (!goListInfo.elementHasMap.ContainsKey(tempPath))
                {
                    type = StatisticsGameObjects.GetTypeByPath(tempPath);
                    element = new GameObjectsInfo.AssetElement(1, type, tempPath, "www");
                    goListInfo.elementHasMap[tempPath] = element;
                }
                else
                {
                    if (tempPath.Contains(".mat"))
                    {
                        material = StatisticsGameObjects.GetLookIntoObjectByPath(tempPath) as Material;
                        if (material == null)
                        {
                            Debug.Log("material is null");
                            continue;
                        }
                        //texture与shader在material里面计算统计
                        for (int n = 0; n < 2; n++)
                        {
                            listPath = StatisticsGameObjects.GetMaterialTexturePathByMaterial(material, n);
                            string relyOnPath;
                            for (l = 0; l < listPath.Count; l++)
                            {
                                relyOnPath = listPath[l];
                                if (!string.IsNullOrEmpty(relyOnPath))
                                {
                                    if (goListInfo.elementHasMap.ContainsKey(relyOnPath))
                                    {
                                        element = goListInfo.elementHasMap[relyOnPath];
                                        element.refCount--;
                                        //使用struct字段在字典里面不重新保存则字典里面的元素不会改变。
                                        goListInfo.elementHasMap[relyOnPath] = element;
                                    }
                                }
                            }
                        }
                    }
                    element = goListInfo.elementHasMap[tempPath];
                    element.refCount++;
                    goListInfo.elementHasMap[tempPath] = element;
                }
            }
            i++;
        } while (i < count);
        goListInfo.WriteAssetElementToTxt();
    }
}

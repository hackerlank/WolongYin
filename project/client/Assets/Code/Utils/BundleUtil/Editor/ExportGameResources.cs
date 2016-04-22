using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using Object = UnityEngine.Object;
using GameObject = UnityEngine.GameObject;
using Debug = UnityEngine.Debug;
public class ExportGameResources
{
    static BuildAssetBundleOptions m_option = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle;
    [MenuItem("Asset/导出/资源")]
    static public void ExporyNgui()
    {
        //Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Assets);
        //Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable);//过滤只剩下预设
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Editable);
         int count = objs.Length;
//         for(int i = 0; i < count; i ++)
//         {
//             Object obj = objs[i];
//             Debug.Log(obj.name);
//         }
        Debug.Log("::选中预有::" + count);
    }
}
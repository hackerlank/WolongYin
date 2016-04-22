using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class BundleTools : Editor {

    const string ExportUrl = "AssetBundle/";

    [MenuItem("Tools/删除所有bundle名字")]
    static void DelAllBundleName()
    {
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Editable);
        foreach (Object obj in objs)
        {
            string url = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(url))
                continue;

            string ext = Path.GetExtension(url).ToLower();
            if (ext == "cs")
                continue;

            AssetImporter aimp = AssetImporter.GetAtPath(url);
            if (aimp == null)
                continue;

            aimp.assetBundleName = string.Empty;
        }
    }

    [MenuItem("Tools/删除阴影接受")]
    static void DelReceiveShadow()
    {
        //GameObject[] gos = Object.FindObjectsOfType<GameObject>();
        //foreach (GameObject go in gos)
        //{
        //    Renderer r = go.GetComponent<Renderer>();
        //    if (r != null)
        //    {
        //        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //        r.receiveShadows = false;
        //        r.useLightProbes = false;
        //    }
        //}

        Transform[] gos = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);//Object.FindObjectsOfType<GameObject>();
        foreach (Transform go in gos)
        {
            Renderer[] rs = go.gameObject.GetComponentsInChildren<Renderer>();
            if (rs != null)
            {
                foreach (Renderer r in rs)
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    r.receiveShadows = false;
                    r.useLightProbes = false;
                }
            }
        }
    }

    [MenuItem("Tools/添加阴影接受")]
    static void AddReceiveShadow()
    {
        Transform[] gos = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);//Object.FindObjectsOfType<GameObject>();
        foreach (Transform go in gos)
        {
            Renderer[] rs = go.gameObject.GetComponentsInChildren<Renderer>();
            if (rs != null)
            {
                foreach (Renderer r in rs)
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    r.receiveShadows = true;
                    r.useLightProbes = true;
                }
            }
        }
    }

    /*[MenuItem("Tools/导出bundle %e")]
    static void ExportBundles()
    {
        _ExportCurrentPlatformBundles();
    }*/

    static bool ContainsFolder( string path, string folderName )
    {
        path += "/" + folderName + "/";
        DirectoryInfo di = new DirectoryInfo(path);
        return di.Exists;
    }

    static string GetExportUrl()
    {
        string path = Application.dataPath;

        int breakOut = 10;
        while (!ContainsFolder(path, /*"AssetBundle"*/"resource"))
        {
            path = Path.GetDirectoryName(path);
            breakOut--;

            if (breakOut < 0)
            {
                Debug.LogError("Can't found Resources folder!\n");
                break;
            }
        }

        string folder = "windows";
#if UNITY_IOS
        folder = "ios";
#endif
#if UNITY_ANDROID
        folder = "android";
#endif
#if UNITY_WP8
        folder = "wp";
#endif

        path += "/resource/data/" + folder + "/";
        return path;
    }

    [MenuItem("Assets/Set Bundle Name")]
    static void SetBundleName()
    {
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object obj in selection)
        {
            string url = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(url))
                continue;

            AssetImporter aimp = AssetImporter.GetAtPath(url);
            if (aimp == null)
                continue;

            if (obj is Shader)
            {
                aimp.assetBundleName = "shader.bundle";
            }
            else if (obj is AnimationClip
                || obj is UnityEditor.Animations.AnimatorController)
            {
                aimp.assetBundleName = "animation.bundle";
            }
            else
            {
                aimp.assetBundleName = obj.name + ".bundle";
            }
        }
    }

    [MenuItem("Assets/Clear Bundle Name")]
    static void ClearBundleName()
    {
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object obj in selection)
        {
            string url = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(url))
                continue;

            AssetImporter aimp = AssetImporter.GetAtPath(url);
            if (aimp == null)
                continue;

            aimp.assetBundleName = string.Empty;
        }
    }


    static void _ExportCurrentPlatformBundles()
    {
        string url = GetExportUrl();//Application.dataPath + "/" + ExportUrl;
        BuildPipeline.BuildAssetBundles( url,
            BuildAssetBundleOptions.DeterministicAssetBundle
            /*| BuildAssetBundleOptions.DisableWriteTypeTree*/,
            GetPlatformTarget() );
    }

    static BuildTarget GetPlatformTarget()
    {
        BuildTarget t = BuildTarget.StandaloneWindows;
#if UNITY_IPHONE
        t = BuildTarget.iOS;
#endif
#if UNITY_ANDROID
        t = BuildTarget.Android;
#endif
#if UNITY_WP8
        t = BuildTarget.WP8Player;
#endif
        return t;
    }

}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class AutoBuildCommonUtil
{
    public static string GetDefaultBankProject()
    {
        string url = Application.dataPath + "/../../../tools/FMODWorkspace/projectv/Build/";
        return url;
    }

    public static string GetResourcesPath(int iIdx)
    {
        string sPath = GetRootResourcesPath();
        sPath += "resources_";
        sPath += iIdx;
        sPath += "/";
        return sPath;
    }
    public static string GetRootResourcesPath()
    {
        string sPath = Application.dataPath;
        Star.Foundation.CPath.AddRightSlash(ref sPath);
        sPath += "../../../resource/";
        return sPath;
    }
    public static string GetPatchableResourcesPath(int iIdx, UnityEditor.BuildTarget eBuildTarget)
    {
        string sPath = GetRootResourcesPath();
        Star.Foundation.CPath.AddRightSlash(ref sPath);
        sPath += "patchable_resources_";
        sPath += iIdx;

        if (eBuildTarget == UnityEditor.BuildTarget.iOS) sPath += "/ios/";
        else if (eBuildTarget == UnityEditor.BuildTarget.Android) sPath += "/android/";
        else sPath += "/windows/";

        return sPath;
    }
    public static UnityEditor.BuildTarget GetCurrentBuildTarget()
    {
#if UNITY_IOS
            return UnityEditor.BuildTarget.iOS;
#elif UNITY_ANDROID
        return UnityEditor.BuildTarget.Android;
#else
        return UnityEditor.BuildTarget.StandaloneWindows;
#endif
    }

    public static string GetFMODBankPath()
    {
        string url = GetResourcesPath(1) + "banks/";
        if (!System.IO.Directory.Exists(url))
        {
            System.IO.Directory.CreateDirectory(url);
        }
        return url;
    }

}

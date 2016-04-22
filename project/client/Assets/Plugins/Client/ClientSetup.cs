using UnityEngine;
using System.Collections.Generic;


public class ClientSetup
{
    protected static readonly ClientSetup ms_instance = new ClientSetup();

    public static ClientSetup Get()
    {
        return ms_instance;
    }

    public List<string> listWorkingPaths = new List<string>();
    static bool mSetupedWorkinggList = false;

    ClientSetup()
    {
        SetupWorkinggPathList();
    }

    public string GetBankTag()
    {
        return "banks/";
    }

    public void SetupWorkinggPathList()
    {
        if (mSetupedWorkinggList)
            return;

        listWorkingPaths.Clear();
        if (Application.platform == RuntimePlatform.Android)
        {
            string sPath = Application.persistentDataPath;
            Star.Foundation.CPath.AddRightSlash(ref sPath);
            sPath += "patchable_resources/";
            listWorkingPaths.Add(sPath);
            //Star.Foundation.Log.LogError( "sPath: " + sPath );

            sPath = Application.streamingAssetsPath; // with 'jar:file://' inside
            Star.Foundation.CPath.AddRightSlash(ref sPath);
            sPath += "patchable_resources/";
            listWorkingPaths.Add(sPath);
            //Star.Foundation.Log.LogError( "sPath: " + sPath );
            //return "jar:file://" + Application.dataPath + "!/assets/patchable_resources/" + folder + name + ext;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string sPath = Application.persistentDataPath;
            Star.Foundation.CPath.AddRightSlash(ref sPath);
            sPath += "patchable_resources/";
            listWorkingPaths.Add(sPath);
            //Star.Foundation.Log.LogError( "sPath: " + sPath );

            sPath = Application.streamingAssetsPath;
            Star.Foundation.CPath.AddRightSlash(ref sPath);
            sPath += "patchable_resources/";
            listWorkingPaths.Add(sPath);
            //Star.Foundation.Log.LogError( "sPath: " + sPath );
            //return "file://" + Application.dataPath + "/Raw/patchable_resources/" + folder + name + ext;
        }
        else
        {
            string sPath = Application.dataPath;
            Star.Foundation.CPath.AddRightSlash(ref sPath);
            sPath += "../../../resource/patchable_resources_1/" + _GetPatchableResourcesPlatformFolder();
            listWorkingPaths.Add(sPath);

            sPath = Application.dataPath;
            Star.Foundation.CPath.AddRightSlash(ref sPath);
            sPath += "../../../resource/resources_1/";
            listWorkingPaths.Add(sPath);
        }

        mSetupedWorkinggList = true;
    }

    string _GetPatchableResourcesPlatformFolder()
    {
        string sPath = string.Empty;
#if UNITY_ANDROID
        sPath = "android/";
#elif UNITY_IPHONE
            sPath = "ios/";
#elif UNITY_WP8
            sPath = "wp/";
#else
        sPath = "windows/";
#endif
        return sPath;
    }
}

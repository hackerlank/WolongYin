using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class ResourceCenter : SingletonMonoBehavior<ResourceCenter>
{
    public delegate void Handle(Object obj);

    public static bool USE_CLEAR_SCENE = false;

    const string ResFolder = "assetbundles/";
    const string IconFolder = "icons/";
    const string ManifestFile = "assetbundles";
    const string BundleExtension = ".bundle";

    static List<string> m_listWorkingPaths = new List<string>();

    public enum AssetType
    {
        normal, // prefab
        scene,
        additive_scene,
        audio,
        image, 
        texture,
    }

    #region class & struct

    [System.Serializable]
    public class U3DAsset
    {
        [SerializeField]
        string mResName = string.Empty;
        public string ResName
        {
            get { return mResName; }
            set { mResName = value; }
        }

        [SerializeField]        
        string mUrl = string.Empty;
        public string Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

        [SerializeField]        
        Object mAsset = null;
        public Object Asset
        {
            get { return mAsset; }
            set { mAsset = value; }
        }
    }

    [System.Serializable]
    public struct AssetLoadTask
    {
        public string url;
        public string resName;
        public AssetType assetType;
        public bool staticAsset;
        private List<Handle> callbackList;
        //public event Handle callback;

        public List<Handle> CallBacks
        {
            get
            {
                if (callbackList == null)
                    callbackList = new List<Handle>();

                return callbackList;
            }
        }

        public System.Type GetUnityType()
        {
            switch (assetType)
            {
                case AssetType.normal:
                    {
                        return typeof(GameObject);
                    }
                case AssetType.audio:
                    {
                        return typeof(AudioClip);
                    }
                case AssetType.texture:
                    {
                        return typeof(Texture);
                    }
                default:
                    {
                        return typeof(GameObject);
                    }
            }
        }

        public void OnLoaded(Object obj)
        {
            if (obj == null 
                && assetType != AssetType.scene
                && assetType != AssetType.additive_scene)
                return;

            for (int i = 0; i < CallBacks.Count; ++i)
            {
                if (CallBacks[i] != null)
                    CallBacks[i](obj);
            }
        }
    }

    #endregion

    #region field

    [SerializeField]
    AssetBundleManifest mManifest = null;

    //[SerializeField]
    //string mUrlPrefix = string.Empty;

    [SerializeField]
    List<U3DAsset> mAssets = new List<U3DAsset>();

    [SerializeField]
    List<AssetLoadTask> mTasks = new List<AssetLoadTask>();

    [SerializeField]
    List<AssetLoadTask> mLoadingTasks = new List<AssetLoadTask>();

    [SerializeField]
    List<AssetLoadTask> mToLoadList = new List<AssetLoadTask>();

    [SerializeField]
    int mPerRequestCount = 3;

    //Dictionary<string, AssetBundle> mTempbundleDic = new Dictionary<string, AssetBundle>();
    //Dictionary<string, string> mTempDepDic = new Dictionary<string, string>();
    #endregion


    /*public string UrlPrefix
    {
        get { return mUrlPrefix; }
    }*/

    public static void Build( List<string> listWorkingPaths )
    {
        ResourceCenter.SetupWorkinggPathList( listWorkingPaths );

        if( ResourceCenter.instance != null )
        {
            Object.Destroy(ResourceCenter.instance.gameObject);
        }

        GameObject go = new GameObject("_ResourceMgr");
        go.AddComponent<ResourceCenter>();
        DontDestroyOnLoad(go);

        Caching.CleanCache();
    }

    public static void SetupWorkinggPathList( List<string> listWorkingPaths )
    {
        m_listWorkingPaths = listWorkingPaths;
        if( listWorkingPaths == null ) return;

        int iCount = m_listWorkingPaths.Count;
        for( int i = 0; i < iCount; i++ )
        {
            string sPath = m_listWorkingPaths[ i ];

            if( sPath.IndexOf( "file://" ) < 0 ) sPath = "file://" + sPath;
            //Star.Foundation.CPath.AddRightSlash( ref sPath );

            m_listWorkingPaths[ i ] = sPath;
        }
    }

    #region mono method
    protected override void Awake()
    {
        base.Awake();

        mManifest = null;
        StartCoroutine(RequestManifest());
        //StartCoroutine(ClearSync());
    }

    public void Update()
    {
        if (mManifest == null)
            return;

        mToLoadList.Clear();

        int cnt = 0;
        for (int i = 0; i < mTasks.Count; ++i)
        {
            AssetLoadTask task = mTasks[i];

            mLoadingTasks.Add(task);
            mToLoadList.Add(task);

            ++cnt;
            if (cnt >= mPerRequestCount)
            {
                break;
            }
        }

        for (int i = 0; i < mToLoadList.Count; ++i)
        {
            mTasks.Remove(mToLoadList[i]);
        }

        if (mToLoadList.Count > 0)
        {
            for (int i = 0; i < mToLoadList.Count; ++i)
            {
                StartCoroutine(Request(mToLoadList[i]));
            }
            //StartCoroutine(MulRequest(mToLoadList.ToArray()));
        }

//         if (mTasks.Count ==0 
//             && mLoadingTasks.Count == 0
//             && DreamTown.GameClient.instance.ActiveStage is DreamTown.GameStage)
//         {
//             ClearDependencies();
//         }
    }

    public void Clear()
    {
        mAssets.Clear();
        mTasks.Clear();

        for (int i = 0; i < mAssetDependencies.Count; ++i)
        {
            AssetDependencies dpd = mAssetDependencies[i];
            if (dpd.Counter <= 0 && dpd.Bundle != null)
            {
                dpd.Bundle.Unload(true);
                mAssetDependencies.RemoveAt(i);
                --i;
            }
        }
    }

    public void Destroy()
    {
        Clear();

        //GameObject.Destroy(ResourceCenter.instance.gameObject);

        Resources.UnloadUnusedAssets();
    }
    #endregion

    #region public loader method

    // resource/icon/资源读取
    public Handle LoadIcon(string name, Handle callback)
    {
        if (string.IsNullOrEmpty(name))
        {
            Logger.instance.Warning("empty icon name !\n");
            return callback; 
        }

        return Load( GetFileUrl( name, IconFolder, string.Empty ), AssetType.image, callback );
    }

    /*public byte[] LoadFile(string sFilename, string folder)
    {
        return ReadFile(GetFileUrl(name, folder, string.Empty, false, false));
    }*/

    public Handle LoadTexture(string name, Handle callback)
    {
        return Load(name, AssetType.texture, callback);
    }

    public void BreakLoadTexture(string name, Handle callback)
    {
        BreakLoad(name, AssetType.texture, callback);
    }

    public Handle LoadObject(string name, Handle callback)
    {
        return Load(name, AssetType.normal, callback);
    }

    public void BreakLoadObject(string name, Handle callback)
    {
        BreakLoad(name, AssetType.normal, callback);
    }

    public Handle LoadScene(string name, Handle callback)
    {
        return Load(name, AssetType.scene, callback);
    }

    public Handle LoadAdditiveScene(string name, Handle callback)
    {
        return Load(name, AssetType.additive_scene, callback);
    }

    public void BreakLoadScene(string name, Handle callback)
    {
        BreakLoad(name, AssetType.scene, callback);
    }
    public void BreakLoadAddtiveScene(string name, Handle callback)
    {
        BreakLoad(name, AssetType.additive_scene, callback);
    }

    public Handle LoadAudio(string name, Handle callback)
    {
        return Load(name, AssetType.audio, callback);
    }

    public void BreakLoadAudio(string name, Handle callback)
    {
        BreakLoad(name, AssetType.audio, callback);
    }

    #endregion

    #region internal loader method

    /*byte[] ReadFile(string filename)
    {
        byte[] data = null;

        if (Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.OSXPlayer
            || Application.platform == RuntimePlatform.OSXEditor)
        {
            string url = filename.Replace("file://", "");

            FileStream fs = File.OpenRead(url);
            data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            fs.Close();
            fs.Dispose();
        }
        else
        {
            TextAsset text = Resources.Load(filename) as TextAsset;
            data = text.bytes;
        }
        return data;
    }*/


    void BreakLoad(string name, AssetType type, Handle callback)
    {
        string url = string.Empty;

        url = GetFileUrl(name);

        for (int i = 0; i < mLoadingTasks.Count; ++i)
        {
            if (mLoadingTasks[i].url == url)
            {
                mLoadingTasks[i].CallBacks.Remove(callback);
                return;
            }
        }

        for (int i = 0; i < mTasks.Count; ++i)
        {
            if (mTasks[i].url == url)
            {
                mTasks[i].CallBacks.Remove(callback);
                break;
            }
        }
    }

    Handle Load(string name, AssetType type, Handle callback)
    {
        if (string.IsNullOrEmpty(name))
        {
            Logger.instance.Warning("empty res name!\n");
            return callback;
        }

        string url = string.Empty;

        if(type == AssetType.image)
        {
            url = name;
        }
        else
        {
            url = GetFileUrl(name);
        }

        // is download ?
        U3DAsset asset = FindAsset(url);
        if (asset != null)
        {
            if (callback != null)
            {
                callback(asset.Asset);
                callback = null;
            }
            return callback;
        }

        // is loading ?
        for (int i = 0; i < mLoadingTasks.Count; ++i)
        {
            if (mLoadingTasks[i].url == url)
            {
                mLoadingTasks[i].CallBacks.Add(callback);
                return callback;
            }
        }

        bool b = false;

        for (int i = 0; i < mTasks.Count; ++i)
        {
            if (mTasks[i].url == url)
            {
                mTasks[i].CallBacks.Add(callback);
                b = true;
                break;
            }
        }

        if (!b) // not in task list
        {
            AssetLoadTask tk = new AssetLoadTask();
            tk.resName = name;
            tk.assetType = type;
            tk.url = url;
            tk.CallBacks.Add(callback);
            mTasks.Add(tk);
        }

        return callback;
    }

    #endregion

    #region internal request bundle method

    IEnumerator RequestManifest()
    {
        string url = GetFileUrl( ManifestFile, ResFolder, string.Empty );
        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(string.Format("no manifest file : {0} \n", www.error));
            yield break;
        }

        AssetBundle ab = www.assetBundle;
        mManifest = (AssetBundleManifest)ab.LoadAsset("AssetBundleManifest");
        ab.Unload(false);
        www.Dispose();

        yield break;
    }


    public class  AssetDependencies
    {
        public string Url = string.Empty;
        public bool Done = false;
        public AssetBundle Bundle = null;
        public int Counter = 0;
    }

   // Dictionary<string, AssetDependencies> mAssetDependencies = new Dictionary<string, AssetDependencies>();

    List<AssetDependencies> mAssetDependencies = new List<AssetDependencies>();

    AssetDependencies FindAssetDependencies(string url)
    {
        for (int i=0; i<mAssetDependencies.Count; ++i)
        {
            if (mAssetDependencies[i].Url == url)
            {
                return mAssetDependencies[i];
            }
        }

        return null;
    }

    IEnumerator Request(AssetLoadTask task)
    {
        if (mManifest == null)
        {
            UnityEngine.Debug.LogError("manifest is null !\n");
            yield break;
        }

        List<string> tmplist = new List<string>();

        if (task.assetType != AssetType.image) // 非bundle的图片没依赖。
        {
            string n = task.resName.ToLower() + BundleExtension;
            string[] deps = mManifest.GetAllDependencies(n);

            // 找出所有要加载的依赖
            for (int j = 0; j < deps.Length; ++j)
            {
//                 if (mAssetDependencies.ContainsKey(deps[j])
//                     && mAssetDependencies[deps[j]].Done)
//                     continue;

                if (string.IsNullOrEmpty(deps[j]))
                    continue;

                AssetDependencies depend = FindAssetDependencies(deps[j]);

                if (depend != null
                    && depend.Done)
                {
                    depend.Counter++;
                    continue;
                }

                tmplist.Add(deps[j]);
            }
        }

        // 先加载所有依赖
        for (int i = 0; i < tmplist.Count; ++i)
        {
            string surl = tmplist[i];
            AssetDependencies ad = FindAssetDependencies(surl);
            if (ad != null)
            {
                ad.Counter++;
                if (ad.Done)
                {
                    continue;
                }
                else
                {
                    while (!ad.Done)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            else
            {
                string url2 = GetFileUrl(surl, ResFolder, string.Empty);
                WWW www1 = WWW.LoadFromCacheOrDownload(url2, 0); //new WWW(url2);

                //Debug.Log(string.Format("begin : {0}\n", url2));

                AssetDependencies newad = new AssetDependencies();
                newad.Url = surl;
                newad.Done = false;
                newad.Counter++;

                mAssetDependencies.Add(newad);

                yield return www1;

                //Debug.Log(string.Format("end : {0}\n", url2));


                if (!string.IsNullOrEmpty(www1.error))
                {
                    Debug.LogError(string.Format("load www error : {0} \n", www1.error));
                    continue;
                }

                AssetBundle ab1 = www1.assetBundle;

                newad.Bundle = ab1;
                newad.Done = true;

                www1.Dispose();
            }
        }


        string name = Path.GetFileName(task.url);
        AssetBundle ab = null;
        WWW wwwmain = null;
        Object asset = null;
        bool b = false;

       // Logger.instance.Error(name);
        AssetDependencies mainAsset = FindAssetDependencies(name);
        if (mainAsset != null)
        {
            // 当前主要bundle也可能是某个资源的依赖
            ab = mainAsset.Bundle;
            mAssetDependencies.Remove(mainAsset);
            b = true;
        }
        else
        {
#if UNITY_EDITOR
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string tuurl = task.url.Replace("file://", "").Trim();
                if (!File.Exists(tuurl))
                {
                    Logger.instance.Error("找不到资源 {0} !\n", tuurl);
                    yield break;
                }
            }
#endif
            wwwmain = new WWW(task.url);

            //Debug.Log(string.Format("begin : {0}\n", task.url));

            yield return wwwmain;

            //Debug.Log(string.Format("end : {0}\n", task.url));


            if (!string.IsNullOrEmpty(wwwmain.error))
            {
                Debug.LogError(string.Format("load www error : {0} \n", wwwmain.error));
                yield break;
            }


            if (task.assetType == AssetType.image)
            {
                Texture2D tex = wwwmain.textureNonReadable;
                asset = tex;
                task.OnLoaded(tex);
                CacheAssets(task, asset);
            }
            else
                ab = wwwmain.assetBundle;
        }

        if (ab == null)
            yield break;

        if (task.assetType == AssetType.scene)
        {
            if (USE_CLEAR_SCENE)
            {
                Application.LoadLevel("clear_scene");
                yield return new WaitForSeconds(0.01f);
            }

            //Debug.Log(string.Format("step : {0}\n", 0));

            AsyncOperation async = Application.LoadLevelAsync(task.resName);
            yield return async;

            //Debug.Log(string.Format("step : {0}\n", 1));

        }
        else if (task.assetType == AssetType.additive_scene)
        {
            if (USE_CLEAR_SCENE)
            {
                Application.LoadLevel("clear_scene");
                yield return new WaitForSeconds(0.01f);
            }

            AsyncOperation async = Application.LoadLevelAdditiveAsync(task.resName);
            yield return async;
        }
        else
        {
            Logger.instance.Log("ffffff", string.Format("begin load asset : {0}\n", task.resName));
            
            asset = ab.LoadAsset(task.resName, task.GetUnityType());
            task.OnLoaded(asset);
        }

        if (!b)
            ab.Unload(false);

        if (wwwmain != null)
        {
            wwwmain.Dispose();
        }

        if (task.assetType != AssetType.scene
            && task.assetType != AssetType.additive_scene)
        {
            CacheAssets(task, asset);
        }

        mLoadingTasks.Remove(task);

        if (task.assetType == AssetType.scene
            || task.assetType == AssetType.additive_scene)
        {
            yield return new WaitForSeconds(0.1f);

            //Debug.Log(string.Format("step : {0}\n", 3));

            task.OnLoaded(null);
        }

//         if (task.assetType != AssetType.image) // 非bundle的图片没依赖。
//         {
//             string n = task.resName.ToLower() + BundleExtension;
//             string[] deps = mManifest.GetAllDependencies(n);
// 
//             for (int i = 0; i < deps.Length; ++i)
//             {
//                 AssetDependencies depend = FindAssetDependencies(deps[i]);
//                 if (depend == null)
//                     continue;
// 
//                 if (depend.Url == "shader.bundle")
//                     continue;
// 
//                 depend.Counter--;
// 
// //                 if (depend.Counter <= 0 && depend.Bundle != null)
// //                 {
// //                     depend.Bundle.Unload(false);
// //                     mAssetDependencies.Remove(depend);
// //                 }
// 
//                 yield return new WaitForSeconds(0.1f);
//             }
// 
//         }
    }


    void ClearDependencies()
    {
        for (int i = 0; i < mAssetDependencies.Count; ++i)
        {
            AssetDependencies dpd = mAssetDependencies[i];
            if (dpd.Counter <= 0 && dpd.Bundle != null)
            {
                dpd.Bundle.Unload(false);
                mAssetDependencies.RemoveAt(i);
                --i;
            }
        }
    }

    /*
    IEnumerator MulRequest(AssetLoadTask[] tasks)
    {
        if (mManifest == null)
        {
            UnityEngine.Debug.LogError("manifest is null !\n");
            yield break;
        }

        mTempbundleDic.Clear();
        mTempDepDic.Clear();

        for (int i = 0; i < tasks.Length; ++i)
        {
            AssetLoadTask task = tasks[i];

            if (task.assetType == AssetType.image) // 非bundle的图片没依赖。
            {
                continue;
            }

            string n = task.resName.ToLower() + BundleExtension;
            string[] deps = mManifest.GetAllDependencies(n);

            for (int j = 0; j < deps.Length; ++j)
            {
                if (mTempDepDic.ContainsKey(deps[j]))
                    continue;
                mTempDepDic.Add(deps[j], string.Empty);
            }
        }

        // 先加载所有依赖
        foreach (KeyValuePair<string, string> pair in mTempDepDic)
        {
            string url2 = GetFileUrl(pair.Key, ResFolder, string.Empty, true, true);
            WWW www1 = new WWW(url2);
            yield return www1;

            if (!string.IsNullOrEmpty(www1.error))
            {
                Debug.LogError(string.Format("load www error : {0} \n", www1.error));
                continue;
            }

            AssetBundle ab1 = www1.assetBundle;
            mTempbundleDic.Add(pair.Key, ab1);
            www1.Dispose();
        }

        for (int i = 0; i < tasks.Length; ++i)
        {
            AssetLoadTask task = tasks[i];
            string name = Path.GetFileName(task.url);
            AssetBundle ab = null;
            WWW www1 = null;
            Object asset = null;
            bool b = false;

            if (mTempbundleDic.ContainsKey(name))
            {
                ab = mTempbundleDic[name];
                b = true;
            }
            else
            {
                www1 = new WWW(task.url);
                yield return www1;
                if (task.assetType == AssetType.image)
                {
                    Texture2D tex = www1.textureNonReadable;
                    asset = tex;
                    task.OnLoaded(tex);
                    CacheAssets(task, asset);
                }
                else
                    ab = www1.assetBundle;

                www1.Dispose();
            }

            if (ab == null)
                continue;

            if (task.assetType == AssetType.scene)
            {
                AsyncOperation async = Application.LoadLevelAsync(task.resName);
                yield return async;

                //Application.LoadLevel(task.resName);

                //task.OnLoaded(null);
            }
            else
            {
                asset = ab.LoadAsset(task.resName, task.GetUnityType());
                task.OnLoaded(asset);
            }

            if (!b)
                ab.Unload(false);

            if (www1 != null)
            {
                www1.Dispose();
            }

            if (task.assetType != AssetType.scene)
            {
                CacheAssets(task, asset);
            }

            mLoadingTasks.Remove(task);

            if (task.assetType == AssetType.scene)
            {
                yield return new WaitForSeconds(0.1f);
                task.OnLoaded(null);
            }
        }

        foreach (KeyValuePair<string, AssetBundle> pair in mTempbundleDic)
        {
            if (pair.Value != null)
                pair.Value.Unload(false);
        }

        mTempbundleDic.Clear();
        mTempDepDic.Clear();
    }
    */

    void CacheAssets(AssetLoadTask task, Object asset)
    {
        U3DAsset uasset = new U3DAsset();
        uasset.Asset = asset;
        uasset.Url = task.url;
        uasset.ResName = task.resName;

        mAssets.Add(uasset);
    }
    #endregion

    #region helpers

    public void RemoveAsset(string name, AssetType type)
    {
        string url = string.Empty;     
        if(type == AssetType.image)
        {
            url = name;
        }
        else
        {
            url = GetFileUrl(name);
        }
        U3DAsset ast = FindAsset(url);
        mAssets.Remove(ast);

        string n = ast.ResName.ToLower() + BundleExtension;
        string[] deps = mManifest.GetAllDependencies(n);

        for (int i = 0; i < deps.Length; ++i)
        {
            AssetDependencies depend = FindAssetDependencies(deps[i]);
            if (depend == null)
                continue;

            if (depend.Url == "shader.bundle")
                continue;

            depend.Counter--;

            if (depend.Counter <= 0 && depend.Bundle != null)
            {
                depend.Bundle.Unload(true);
                mAssetDependencies.Remove(depend);
            }
        }

    }

    public GameObject GetOriginalObject(string name)
    {
        string url = GetFileUrl(name);
        U3DAsset ast = FindAsset(url);
        if (ast != null)
        {
            return (GameObject)ast.Asset;
        }
        return null;
    }

    public U3DAsset FindAsset(string url)
    {
        for (int i = 0; i < mAssets.Count; ++i)
        {
            if (mAssets[i].Url == url)
            {
                return mAssets[i];
            }
        }
        return null;
    }

    public string GetFileUrl(string name)
    {
        return GetFileUrl( name, ResFolder, BundleExtension );
    }

    public string GetFileUrl( string name, string folder, string ext )
    {
        string sFileURL = "";
        string sRealFile = "";
        for( int i = 0; i < m_listWorkingPaths.Count; i++ )
        {
            sFileURL = m_listWorkingPaths[ i ];
            sFileURL += folder + name + ext;

            int iIdx = sFileURL.IndexOf( "://" );
            if( iIdx >= 0 )
            {
                iIdx += 3;
                sRealFile = sFileURL.Substring( iIdx, sFileURL.Length - iIdx );
            }
            else sRealFile = sFileURL;

            if (File.Exists(sRealFile)) return sFileURL;
            //if( Star.Foundation.CFile.IsFileExist( sRealFile ) ) return sFileURL;
        }

        return sFileURL; // return the last one if cannot find any

        /*//string url = Path.Combine( mUrlPrefix, folder ) + "/";
        string url = mUrlPrefix;

        if( Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.OSXEditor
            || Application.platform == RuntimePlatform.OSXPlayer )
        {
            // file://...resource/patchable_resource/platform/name.ext
            return "file://" + url + folder + name + ext;
        }
        else if( Application.platform == RuntimePlatform.Android )
        {
            return "jar:file://" + Application.dataPath + "!/assets/patchable_resources/" + folder + name + ext;
        }
        else if( Application.platform == RuntimePlatform.IPhonePlayer )
        {
            return "file://" + Application.dataPath + "/Raw/patchable_resources/" + folder + name + ext;
        }
        else if( Application.platform == RuntimePlatform.WP8Player )
        {
            // to do.
            return string.Empty;
        }
        else
        {
            return "file://" + mUrlPrefix + name + ext;
        }*/
    }

    /*string GetManifestFileName()
    {
        //return ManifestFile;
#if UNITY_STANDALONE_WIN
            return "windows";
#elif UNITY_ANDROID
            return "android";
#elif UNITY_IPHONE
            return "ios";
#elif UNITY_WP8
            return "wp";
#else
            return "windows";
#endif
    }*/

    bool ContainsFolder(string path, string folderName)
    {
        path += "/" + folderName + "/";
        DirectoryInfo di = new DirectoryInfo(path);
        return di.Exists;
    }

    #endregion
}
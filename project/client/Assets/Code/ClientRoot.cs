using UnityEngine;
using System.Collections;


public class ClientRoot : MonoBehaviour
{

#region instance
    public static ClientRoot instance { get; private set; }
#endregion

    public Camera mainCamera = null;
    public UIRoot nguiRoot = null;
    public Camera uiCamera = null;

    private string m_sPersistentPatchableResourcesPath = "";
    private CFileSystem m_theFileSystem = new CFileSystem();

    private GameClient mClient = null;
    public GameClient Client
    {
        get { return mClient; }
    }

    public string PersistentPatchableResourcesPath
    {
        get { return m_sPersistentPatchableResourcesPath; }
    }

    void Awake()
    {
        DontDestroyOnLoad(mainCamera.gameObject);


        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _StarEngineInit();

        mClient = new GameClient();
        mClient.OnAwake();
    }

    void Start()
    {
        mClient.OnStart();
    }

    void Update()
    {
        mClient.OnUpdate(Time.deltaTime);
    }

    void OnDestroy()
    {
        mClient.OnDestroy();
    }

    void OnApplicationQuit()
    {
        mClient.OnDestroy();
    }

    void OnGUI()
    {
        mClient.OnGUI();
    }

    void _StarEngineInit()
    {
        Star.Foundation.Log.OutputDebugString(false);
        Star.Foundation.Log.ShowToConsole(false);
        Star.Foundation.Log.LogToFile(false);

        // set local DB
        string sSQLiteDBConnectionPath = Application.persistentDataPath;
        Star.Foundation.CPath.AddRightSlash(ref sSQLiteDBConnectionPath);
        sSQLiteDBConnectionPath += "Database/";
        Star.Foundation.CPath.CreateDir(sSQLiteDBConnectionPath + "SQLite/", true);
        Star.Database.CSystem.CommonConnectionPath(Star.Database.CConnection.EType.SQLITE, sSQLiteDBConnectionPath);
        //Debug.LogError( "Local DB path: " + sCommonDBConnectionPath );

        // set persistent patchable resources path
        m_sPersistentPatchableResourcesPath = Application.persistentDataPath;
        Star.Foundation.CPath.AddRightSlash(ref m_sPersistentPatchableResourcesPath);
        m_sPersistentPatchableResourcesPath += "patchable_resources/";
        Star.Foundation.CPath.CreateDir(m_sPersistentPatchableResourcesPath, true);

        // set streaming patchable resources path
        string sStreamingAssetPath = Application.streamingAssetsPath;
        Star.Foundation.CPath.AddRightSlash(ref sStreamingAssetPath);
        sStreamingAssetPath += "patchable_resources/";

        // set file system
        System.Collections.Generic.List<string> listWorkingPath = new System.Collections.Generic.List<string>();
        if (Application.isEditor)
        {
            string sEditorGameTablePath = Application.dataPath;
            Star.Foundation.CPath.AddRightSlash(ref sEditorGameTablePath);
            sEditorGameTablePath += "../../../resource/resources_1";
            listWorkingPath.Add(sEditorGameTablePath);
            sEditorGameTablePath += "../../../resource/resources_0";
            listWorkingPath.Add(sEditorGameTablePath);
            sEditorGameTablePath += "../../../resource/PatchableResources_1";
            listWorkingPath.Add(sEditorGameTablePath);
            sEditorGameTablePath += "../../../resource/PatchableResources_0";
            listWorkingPath.Add(sEditorGameTablePath);

            /// test
            //string sApkFile = sEditorGameTablePath + "/nuolan.apk";
            //string sApkPath = sEditorGameTablePath + "/nuolan";
            //Star.Foundation.CCompress.Decompress( sApkFile, sApkPath );
        }
        else
        {
            listWorkingPath.Add(m_sPersistentPatchableResourcesPath);
            listWorkingPath.Add(sStreamingAssetPath);
        }

        // insert working path into filesystem
        m_theFileSystem.SetWoringPathList(listWorkingPath);
        Star.Foundation.CFile.SetCustomFileSystem(m_theFileSystem);

        //
        // initial the client part
        //
        //_InitGameClient();
    }
}


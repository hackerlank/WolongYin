using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using ProtoBuf;

public class AssetDownloader : Singleton<AssetDownloader>
{
    private PatchableIndexSetupInfo mIndexData = null;
    private PatchableIndexSetupInfo mServerIndexData = null;
    private List<string> mDownloadUrls = new List<string>();
    private List<PatchableIndexInfo> mToDownloadList = new List<PatchableIndexInfo>(); 

    // 下载地址
    public List<string> DownloadUrls
    {
        get { return mDownloadUrls; }
        set { mDownloadUrls = value; }
    }

    public PatchableIndexSetupInfo IndexData
    {
        get { return mIndexData; }
    }

    public PatchableIndexSetupInfo ServerIndexData
    {
        get { return mServerIndexData; }
    }

    public string IndexFileName
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return "index_android.bytes";
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return "index_ios.bytes";
            else
            {
                return "index_windows.bytes";
            }
        }
    }

    public string ServerIndexFileName
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return "patchable_index/index_android.bytes";
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return "patchable_index/index_ios.bytes";
            else
            {
                return "patchable_index/index_windows.bytes";
            }
        }
    }

    public string GetServerRootPath(int rootType)
    {
        if (rootType == 0)
        {
            return "/patchable_resources_0/";
        }
        else
        {
            return "/patchable_resources_1/";
        }
    }

    public string ClientRootPath
    {
        get { return ClientRoot.instance.PersistentPatchableResourcesPath; }
    }

    public void Init()
    {
        mIndexData = null;
        mServerIndexData = null;

        // 读取本地index文件
        mIndexData = Utility.LoadProtoFile<PatchableIndexSetupInfo>(IndexFileName);

        if (IndexData == null)
        {
            Logger.instance.Error("无法读取本地 index 文件，可能被删除或者读取错误！\n");
        }
        else
        {
            Logger.instance.Log("本地 index 信息： version : {0}, 日期 : {1}!\n", IndexData.Version, IndexData.SystemTimeDate);            
        }
    }

    public void MakeDownloadList()
    {
        mToDownloadList.Clear();

        if (ServerIndexData == null)
        {
            Logger.instance.Error("还没获取 server index 文件！\n");
            return;
        }

        for (int i = 0; i < ServerIndexData.Datas.Count; ++i)
        {
            PatchableIndexInfo newInfo = ServerIndexData.Datas[i];
            bool badd = true;

            if (IndexData != null)
            {
                for (int j = 0; j < IndexData.Datas.Count; ++j)
                {
                    PatchableIndexInfo oldInfo = IndexData.Datas[j];
                    if (oldInfo.MatchCRC == newInfo.MatchCRC)
                    {
                        badd = false; // 不是新文件
                        if (oldInfo.CRCID != newInfo.CRCID)
                        {
                            // 文件有更新
                            mToDownloadList.Add(newInfo);
                        }
                        break;
                    }
                }
            }

            if (badd)
            {
                mToDownloadList.Add(newInfo);
            }
        }
    }

    public void WriteDownloadFile(PatchableIndexInfo info, byte[] bytes)
    {
        string filePath = ClientRootPath + info.AssetName;
        using (MemoryStream ms = new MemoryStream())
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (FileStream stream = File.Open(filePath, FileMode.OpenOrCreate/*, FileAccess.Write*/))
            {
                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write(bytes);
                bw.Flush();
                bw.Close();
                stream.Close();
            }
            ms.Close();
        }
    }
}
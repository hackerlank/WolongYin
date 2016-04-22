using UnityEngine;
using System.Collections;

public class CFileSystem : Star.Foundation.CFileSystem
{
    public class CFile : Star.Foundation.CFileSystem.CSystemFile
    {
        public CFile(CFileSystem theFileSystem)
            : base(theFileSystem)
        {
            m_theFileSystem = theFileSystem;
        }

        public override bool Open(string sFilename, int iMode, bool bSuppressOpenFileError = true)
        {
            if ((iMode & Star.Foundation.CFile.MODE_WRITE) != 0)
            {
                return base.Open(sFilename, iMode, bSuppressOpenFileError);
            }
            else if (Application.isEditor)
            {
                if (base.Open(sFilename, iMode, bSuppressOpenFileError))
                {
                    return true;
                }
            }

            string sTheFilename = string.Empty;
            foreach (string sLoopFilename in m_theFileSystem.m_listWorkingPaths)
            {
                sTheFilename = sLoopFilename; //m_theFileSystem.GetWoringPath( 0 );
                Star.Foundation.CPath.AddRightSlash(ref sTheFilename);
                sTheFilename += sFilename;

                //Debug.LogError( "try read file from working path - " + sTheFilename );
                if (base.Open(sTheFilename, iMode, bSuppressOpenFileError))
                {
                    //Debug.LogError( "Cool, read file from working path - " + sTheFilename );
                    return true;
                }
            }

            // try read from Resources
            //Debug.LogError( "try read from Resources: " + sFilename + ", iMode: " + iMode );
            if ((iMode & Star.Foundation.CFileSystem.IFile.MODE_READ) == Star.Foundation.CFileSystem.IFile.MODE_READ)
            {
                //Debug.LogError( "Hello, try read file from resources path - " + sFilename );
                sTheFilename = Star.Foundation.CPath.DriverDir(sFilename);
                Star.Foundation.CPath.AddRightSlash(ref sTheFilename);
                sTheFilename += Star.Foundation.CPath.Name(sFilename);

                if ((iMode & Star.Foundation.CFileSystem.IFile.MODE_TEXT) == Star.Foundation.CFileSystem.IFile.MODE_TEXT)
                {
                    //Debug.LogError( "try read text file from resources path - " + sFilename );
                    TextAsset asset = Resources.Load(sTheFilename, typeof(TextAsset)) as TextAsset;
                    if (asset != null)
                    {
                        byte[] utf8BOM = new byte[3] { 0xEF, 0xBB, 0xBF };
                        m_aAssetBytes = new byte[asset.bytes.Length + 3];
                        System.Buffer.BlockCopy(utf8BOM, (int)0, m_aAssetBytes, 0, utf8BOM.Length);
                        System.Buffer.BlockCopy(asset.bytes, (int)0, m_aAssetBytes, 3, asset.bytes.Length);
                        //Debug.LogError( "Cool, read file from resource - " + sTheFilename + ", filze size: " + m_aAssetBytes.Length );
                        return true;
                    }
                    else return false;
                }
                else if ((iMode & Star.Foundation.CFileSystem.IFile.MODE_BINARY) == Star.Foundation.CFileSystem.IFile.MODE_BINARY)
                {
                    //Debug.LogError( "try read binary file from resources path - " + sFilename );
                    TextAsset asset = Resources.Load(sTheFilename, typeof(TextAsset)) as TextAsset;
                    if (asset != null)
                    {
                        m_aAssetBytes = asset.bytes;
                        //Debug.LogError( "Cool, read file from resource - " + sTheFilename + ", filze size: " + m_aAssetBytes.Length );
                        return true;
                    }
                    else return false;
                }
            }

            return false;
        }

        public override void Close()
        {
            base.Close();
            m_aAssetBytes = null;
        }
        public override long FileSize()
        {
            if (base.IsOpened()) return base.FileSize();
            else if (m_aAssetBytes != null) return m_aAssetBytes.Length;
            else return 0;
        }

        public override bool SeekBegin(long lDistanceToMove)
        {
            if (base.IsOpened()) return base.SeekBegin(lDistanceToMove);
            else if (m_aAssetBytes != null)
            {
                if (lDistanceToMove >= 0 && lDistanceToMove <= m_aAssetBytes.Length)
                {
                    m_lCurrentIndex = lDistanceToMove;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public override bool SeekCurrent(long lDistanceToMove)
        {
            if (base.IsOpened()) return base.SeekCurrent(lDistanceToMove);
            else if (m_aAssetBytes != null)
            {
                long l = m_lCurrentIndex + lDistanceToMove;
                if (l >= 0 && l <= m_aAssetBytes.Length)
                {
                    m_lCurrentIndex = l;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public override bool SeekEnd(long lDistanceToMove)
        {
            if (base.IsOpened()) return base.SeekEnd(lDistanceToMove);
            else if (m_aAssetBytes != null)
            {
                long l = m_aAssetBytes.Length + lDistanceToMove;
                if (l >= 0 && l <= m_aAssetBytes.Length)
                {
                    m_lCurrentIndex = l;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public override long Tell()
        {
            if (base.IsOpened()) return base.Tell();
            else if (m_aAssetBytes != null) return m_lCurrentIndex;
            else return 0;
        }

        public override bool IsOpened()
        {
            if (base.IsOpened()) return true;
            else if (m_aAssetBytes != null) return true;
            else return false;
        }

        public override int Write(byte[] byData, int iSizeInBytes)
        {
            if (base.IsOpened()) return base.Write(byData, iSizeInBytes);
            else if (m_aAssetBytes != null) return 0;
            else return 0;
        }
        public override int Read(byte[] byData, int iSizeInBytes)
        {
            if (base.IsOpened()) return base.Read(byData, iSizeInBytes);
            else if (m_aAssetBytes != null)
            {
                if (byData.Length < iSizeInBytes) iSizeInBytes = byData.Length;
                if (m_aAssetBytes.Length - m_lCurrentIndex < iSizeInBytes) iSizeInBytes = m_aAssetBytes.Length - (int)m_lCurrentIndex;

                System.Buffer.BlockCopy(m_aAssetBytes, (int)m_lCurrentIndex, byData, 0, iSizeInBytes);

                m_lCurrentIndex += iSizeInBytes;
                return iSizeInBytes;
            }
            else return 0;
        }

        //
        byte[] m_aAssetBytes = null;
        long m_lCurrentIndex = 0;
        CFileSystem m_theFileSystem = null;
    }

    //
    public override IFile NewFile() { return new CFile(this); }
    /*public override bool IsFileExist( string sFilename )
    {
        string sTheFilename = GetWoringPath( 0 );
        Star.Foundation.CPath.AddRightSlash( ref sTheFilename );
        sTheFilename += sFilename;

        if( base.IsFileExist( sTheFilename ) ) return true;
        else 
        {
            if( Resources.Load( sFilename ) != null ) return true;
        }
        return false;
    }*/


    //
    //
    public void SetWoringPathList(System.Collections.Generic.List<string> listWorkingPath)
    {
        m_listWorkingPaths = listWorkingPath;
    }
    public string GetWoringPath(int iIdx)
    {
        if (iIdx >= m_listWorkingPaths.Count)
        {
            Star.Foundation.Log.LogErrorMsg("CFileSystem error! GetWoringPath index out of range!(" + iIdx + ")");
            return "";
        }
        else return m_listWorkingPaths[iIdx];
    }

    //
    //
    System.Collections.Generic.List<string> m_listWorkingPaths = new System.Collections.Generic.List<string>();
}

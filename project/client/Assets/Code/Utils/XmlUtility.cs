using System.Collections.Generic;


public static class XmlUtility
{
    public static string LoadFile(string folder, string fileName)
    {
        string sFilename = folder;
        Star.Foundation.CPath.AddRightSlash(ref sFilename);
        sFilename += fileName;

        string sText = null;
        Star.Foundation.CFile file = new Star.Foundation.CFile();
        if (file.Open(sFilename, Star.Foundation.CFile.MODE_READ | Star.Foundation.CFile.MODE_TEXT | Star.Foundation.CFile.MODE_UTF8))
        {
            file.Read(out sText, (int)file.FileSize());
            file.Dispose();
        }

        return sText;
    }
}
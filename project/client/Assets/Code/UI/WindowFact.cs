using UnityEngine;
using System.Collections.Generic;


public enum EWindowType
{
    none,

    LoadingWindow = 1,

    max,
};

public class WindowData
{
    public System.Type ClassType;
    public string WindowName = string.Empty;
    public string AssetName = string.Empty;
    public bool Popup = true;
    public bool ShowReturnButton = false;
    public bool UseBackgroundCollider = true;
};


public class WindowFact : Singleton<WindowFact>
{
    public Dictionary<EWindowType, WindowData> sWindowTypeMap = new Dictionary<EWindowType, WindowData>()
        {
             {EWindowType.LoadingWindow, new WindowData()
             { ClassType=typeof(LoadingWindow), WindowName ="加载界面",AssetName="level_loading", Popup =true, ShowReturnButton=false, UseBackgroundCollider=true}},
        };
}

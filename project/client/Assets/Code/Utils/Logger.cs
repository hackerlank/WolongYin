//#define NO_LOG

using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Logger : SingletonMonoBehavior<Logger>
{
    
    enum eMsgType
    {
        Log,
        Warning,
        Error,
    }
    class SMsg
    {
        public string strMsg;
        public eMsgType eType;
    }

    List<SMsg> m_listMsg = new List<SMsg>();
    public int LogMaxNum = 30;

    public bool showLog = true;
    public bool showWarning = true;
    public bool showError = true;

    public bool showPanel = false; 
    public int panelWidth = 350;
    Vector2 scrollPosition;
    //bool mouseOver = false;
    //public bool IsMouseOver
    //{
    //    get
    //    {
    //        return mouseOver;
    //    }
    //}

    //public static bool Release
    //{
    //    get { return m_bRelease; }
    //    set { m_bRelease = value; }
    //}

    //static DreamTown.CDebugLogWindow m_winDebugLog;

    //public static void SetLogWindow(DreamTown.CDebugLogWindow winDebugLog)
    //{
    //    m_winDebugLog = winDebugLog;
    //}

    #region Logger窗口处理函数
    void OnGUI()
    {
        if (showPanel)
        {
            //Debug.LogError("OnGUI OnGUI OnGUI OnGUI");
            UpdateLog();
            UpdateButton();
        }
    }

    //public bool IsMouseOver()
    //{
    //    //Rect rect = GUILayoutUtility.GetLastRect();
    //    Rect rect = new Rect(0, 0, panelWidth, Screen.height);

    //    return Event.current != null
    //        && Event.current.type == EventType.Repaint 
    //        && rect.Contains(Event.current.mousePosition);
    //}
    //void OnMouseEnter()
    //{
    //    mouseOver = true;
    //    Debug.LogError("OnMouseEnter OnMouseEnter OnMouseEnter OnMouseEnter");
    //    //rend.material.color = Color.red;
    //}
    ////void OnMouseOver()
    ////{
    ////    rend.material.color -= new Color(0.1F, 0, 0) * Time.deltaTime;
    ////}
    //void OnMouseExit()
    //{
    //    mouseOver = false;
    //    Debug.LogError("OnMouseExit OnMouseExit OnMouseExit OnMouseExit");
    //    //rend.material.color = Color.white;
    //}

    private void UpdateLog()
    {
        //GUILayout.BeginArea(new Rect(0, 0, 250, Screen.height - 50));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(panelWidth), GUILayout.Height(Screen.height - 50));
        GUILayout.BeginVertical();
        for (int i = 0; i < m_listMsg.Count; i++)
        {
            if (m_listMsg[i].eType == eMsgType.Log && showLog)
            {
                GUI.color = Color.white;
                GUILayout.Label(m_listMsg[i].strMsg);
            }
            if (m_listMsg[i].eType == eMsgType.Warning && showWarning)
            {
                GUI.color = Color.yellow;
                GUILayout.Label(m_listMsg[i].strMsg);
            }
            if (m_listMsg[i].eType == eMsgType.Error && showError)
            {
                GUI.color = Color.red;
                GUILayout.Label(m_listMsg[i].strMsg);
            }
        }
        GUILayout.EndVertical();
        //GUILayout.EndArea();
        GUILayout.EndScrollView();
    }

    private void UpdateButton()
    {
        GUI.color = Color.black;
        GUILayout.BeginArea(new Rect(0, Screen.height - 50, panelWidth, 50));
        GUILayout.BeginHorizontal();
        showLog = GUILayout.Toggle(showLog, "Log");
        showWarning = GUILayout.Toggle(showWarning, "Warning");
        showError = GUILayout.Toggle(showError, "Error");
        if (GUILayout.Button("Clear"))
        {
            m_listMsg.Clear();
        }
        if (GUILayout.Button("Close"))
        {
            ShowDebugPanel(false);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void Print(string msg, eMsgType type)
    {
        SMsg sMsg = new SMsg();
        sMsg.strMsg = msg;
        sMsg.eType = type;

        if (m_listMsg.Count > LogMaxNum)
            m_listMsg.Remove(m_listMsg[0]);

        m_listMsg.Add(sMsg);
    }
    #endregion

    #region 公开接口
    public void ShowDebugPanel(bool bShow)
    {
        showPanel = bShow;
    }

    public void Warning(string msg)
    {
        if (showWarning)
        {
            UnityEngine.Debug.LogWarning(msg);
            if (showPanel)
            {
                Print(msg, eMsgType.Warning);
            }
        }
    }

    public void Warning(string format, params object[] arg)
    {
        Warning(string.Format(format, arg));
    }

    public void Error(string msg)
    {
        if (showError)
        {
            UnityEngine.Debug.LogError(msg);
            if (showPanel)
            {
                Print(msg, eMsgType.Error);
            }
        }
    }

    public void Error(string format, params object[] arg)
    {
        Error(string.Format(format, arg));
    }

    public void Log(string msg)
    {
        if (showLog)
        {
            UnityEngine.Debug.Log(msg);
            if (showPanel)
            {
                Print(msg, eMsgType.Log);
            }
        }
    }

    public void Log(string format, params object[] arg)
    {
        Log(string.Format(format, arg));
    }

    public void Log(string color, string msg)
    {
        Log("<color=#" + color + ">" + msg + "</color>");
    }
    #endregion
}
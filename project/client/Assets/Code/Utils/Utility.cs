using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using ProtoBuf;


public static class Utility
{
    public enum ECompareType
    {
        Greater, // 大于
        Equal,   // 等于
        Less,    // 小于
        Greater_And_Equal,
        Less_And_Equal,
    }

    public static ProtoSerializer ProtobufSerializer = new ProtoSerializer();

    public delegate void ObjectDelegate(UnityEngine.Object obj);
    public delegate void VoidDelegate();

    static GameObject mTempGo = null;

    public static Dictionary<string, uint> WordIDDic = new Dictionary<string, uint>();

    public static void SetActiveObject(GameObject go, bool active)
    {
        if (go == null)
            return;

        go.transform.localScale = active ? Vector3.one : Vector3.zero;
    }

    public static float Sqr(float x)
    {
        return x * x;
    }

    public static void ToSpherical(Vector3 dir, out float rotX, out float rotZ)
    {
        var xyLen = Mathf.Sqrt(Sqr(dir.x) + Sqr(dir.z));
        rotX = Mathf.Atan2(dir.x, dir.z); // yaw
        rotZ = Mathf.Atan2(dir.y, xyLen); // pitch
    }

    public static void ToCartesian(float rotX, float rotZ, out Vector3 dir)
    {
        var sinZ = Mathf.Sin(rotZ);
        var cosZ = Mathf.Cos(rotZ);
        var sinX = Mathf.Sin(rotX);
        var cosX = Mathf.Cos(rotX);

        dir.x = sinX * cosZ;
        dir.y = sinZ;
        dir.z = cosX * cosZ;
    }

    public static void ResetUITweeners(GameObject go)
    {
        if (go == null)
            return;

        UITweener[] tweeners = go.GetComponentsInChildren<UITweener>();
        for (int i = 0; i < tweeners.Length; ++i)
        {
            UITweener tw = tweeners[i];
            tw.ResetToBeginning();
            tw.PlayForward();
        }
    }

    public static UITweener[] ResetUITweeners(GameObject go, out float maxdur, out UITweener last)
    {
        maxdur = 0f;
        last = null;

        UITweener[] tweeners = go.GetComponentsInChildren<UITweener>();
        for (int i = 0; i < tweeners.Length; ++i)
        {
            UITweener tw = tweeners[i];
            tw.ResetToBeginning();
            tw.PlayForward();

            float mdur = tw.delay + tw.duration;
            if (maxdur < mdur)
            {
                maxdur = mdur;
                last = tw;
            }
        }

        return tweeners;
    }

    public static Vector3 VectorSmoothSlerp(ref Vector3 SrcPos, ref Vector3 DstPos, float DeltaTime, float PowParam)
    {
        return SrcPos + (DstPos - SrcPos) * (1 - Mathf.Pow(0.5f, PowParam * DeltaTime));
    }

    public static UnityEngine.Object Instantiate(UnityEngine.Object obj)
    {
        GameObject go = (GameObject)UnityEngine.Object.Instantiate(obj);
#if UNITY_EDITOR
        ResetShader(go);
#endif
        return go;
    }

    public static void Destroy(UnityEngine.Object obj)
    {
        if (obj == null)
            return;


        if (Application.isEditor)
            UnityEngine.Object.DestroyImmediate(obj);
        else
            UnityEngine.Object.Destroy(obj);
    }


    public static void Destroy(GameObject node)
    {
        if (node == null)
            return;

        node.transform.parent = null;

        if (Application.isEditor)
            UnityEngine.Object.DestroyImmediate(node);
        else
            UnityEngine.Object.Destroy(node);
    }

    public static void Destroy(Transform node)
    {
        if (node == null)
            return;

        node.parent = null;

        if (Application.isEditor)
            UnityEngine.Object.DestroyImmediate(node.gameObject);
        else
            UnityEngine.Object.Destroy(node.gameObject);
    }

    public static bool NearlyEqual(ref Vector3 point1, ref Vector3 point2, float sqrValue = 0.00001f)
    {
        return (Vector3.SqrMagnitude(point1 - point2) < sqrValue);
    }

    public static float Distance2D(Vector3 target, Vector3 original)
    {
        return Vector2.Distance(new Vector2(target.x, target.z), new Vector2(original.x, original.z));
    }

    public static int GetAreaIDByName(string name)
    {
        int idx = 0;
        if (int.TryParse(name.Remove(0, 1), out idx))
            return idx;
        else
        {
            Logger.instance.Error("错误的区域名字 {0} ！\n", name);
            return 0;
        }
    }

    public static void SetIdentityChild(GameObject parent, GameObject child)
    {
        if (parent == null || child == null)
            return;

        SetIdentityChild(parent.transform, child.transform);
    }

    public static void SetIdentityChild(Transform parent, Transform child)
    {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    //public static T DeepClone<T>(T source)
    //{
    //    System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

    //    MemoryStream stream = new MemoryStream();
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    formatter.Serialize(stream, source);
    //    stream.Position = 0;
    //    return (T)formatter.Deserialize(stream);
    //}

    public static void SetIdentityPosition(GameObject gameObj)
    {
        gameObj.transform.localPosition = Vector3.zero;
        gameObj.transform.localRotation = Quaternion.identity;
        gameObj.transform.localScale = Vector3.one;
    }

    public static void SetObjectLayer(GameObject go, int layer)
    {
        Transform[] nodes = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i].gameObject.layer = layer;
        }

        go.layer = layer;
    }

//     public static UIPanel CreatePanel(string name, Transform uiroot, int layer)
//     {
//         GameObject child = new GameObject(name);
//         child.layer = layer;
// 
//         Transform ct = child.transform;
//         ct.parent = uiroot;
//         ct.localPosition = Vector3.zero;
//         ct.localRotation = Quaternion.identity;
//         ct.localScale = Vector3.one;
// 
//         UIPanel pl = child.AddComponent<UIPanel>();
// 
//         return pl;
//     }

    public static T ScriptGet<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;

        T script = go.GetComponent<T>();
        if (script == null)
            script = go.AddComponent<T>();

        return script;
    }

    public static Bounds ConvertToBound(Vector3 vecPosition, float length, float width, float height = 0)
    {
        Bounds b = new Bounds()
        {
            center = vecPosition/* + new Vector3(length / 2, width / 2, height / 2)*/,
            size = new Vector3(length, height, width)
        };
        return b;
    }

    public static GameObject FindNode(GameObject go, string nodeName)
    {
        if (go == null)
            return null;

        if (string.IsNullOrEmpty(nodeName))
            return go;

        Transform ret = go.transform.FindChild(nodeName);
        if (ret != null)
            return ret.gameObject;

        Transform[] nodes = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < nodes.Length; ++i)
        {
            if (nodes[i] == null)
                continue;

            if (nodes[i].name == nodeName)
                return nodes[i].gameObject;
        }

        return null;
    }

    public static T FindNode<T>(GameObject go, string childname) where T : Component
    {
        GameObject ret = FindNode(go, childname);
        if (ret == null)
            return null;

        return ret.GetComponent<T>();
    }

    public static Transform FindNode(Transform[] nodes, string name)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].name == name) return nodes[i];
        }
        return null;
    }

    public static List<Transform> FindNodes(Transform[] nodes, string name)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].name == name)
            {
                list.Add(nodes[i]);
            }
        }
        return list;
    }

    public static T FindNode<T>(Transform[] nodes, string name) where T : Component
    {
        Transform tr = FindNode(nodes, name);
        if (tr) return tr.GetComponent<T>();
        return null;
    }

    public static List<T> FindNodes<T>(Transform[] nodes, string name) where T : Component
    {
        List<T> list = new List<T>();
        List<Transform> trs = FindNodes(nodes, name);
        if (trs.Count > 0)
        {
            for (int i = 0; i < trs.Count; i++)
            {
                list.Add(trs[i].GetComponent<T>());
            }
        }
        return list;
    }

    public static List<T> FindNodes<T>(GameObject go, string name) where T : Component
    {
        List<T> list = new List<T>();
        GameObject ret = FindNode(go, name);
        for (int i = 0; i < ret.transform.childCount; i++)
        {
            list.Add(ret.transform.GetChild(i).GetComponent<T>());
        }
        return list;
    }

    public static void PlayEffect(GameObject go)
    {
        if (go == null)
            return;

        go.SetActive(true);
    }

    public static void ScaleEffect(GameObject go, float scale)
    {
        ScaleEffect(go, new Vector3(scale, scale, scale));
    }

    public static void ScaleEffect(GameObject go, Vector3 scale)
    {
        if (go == null)
            return;
    }

    public static void ResetShader(GameObject go)
    {
#if UNITY_EDITOR
        if (go == null)
        {
            Logger.instance.Log("无法设置Shader，因为对象为空");
            return;
        }

        Renderer[] rArray = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < rArray.Length; ++i)
        {
            foreach (Material mat in rArray[i].sharedMaterials)
            {
                if (mat == null)
                    continue;

                Shader sd = Shader.Find(mat.shader.name);
                if (sd != null)
                    mat.shader = sd;
            }
        }
#endif
    }

    public static bool RectIntersect(Vector2 rect0Min, Vector2 rect0Max, Vector2 rect1Min, Vector2 rect1Max)
    {
        Vector2 intersectMin = Vector3.zero;
        intersectMin.x = Math.Max(rect0Min.x, rect1Min.x);
        intersectMin.y = Math.Max(rect0Min.y, rect1Min.y);

        Vector2 intersectMax = Vector3.zero;
        intersectMax.x = Math.Min(rect0Max.x, rect1Max.x);
        intersectMax.y = Math.Min(rect0Max.y, rect1Max.y);

        if (intersectMax.x <= intersectMin.x) return false;
        if (intersectMax.y <= intersectMin.y) return false;

        return true;
    }

    public static bool RectContains2DPoint(Vector2 rectMin, Vector2 rectMax, Vector3 pos)
    {
        if (pos.x <= rectMin.x
            || pos.z <= rectMin.y
            || pos.x >= rectMax.x
            || pos.z >= rectMax.y)
            return false;

        return true;
    }

//     public static void Vector3_Copy(ProtoBuf.Vector3Data v1, ref Vector3 v2, float scale = 1)
//     {
//         v2.x = v1.Vector3Data_X;
//         v2.y = v1.Vector3Data_Y;
//         v2.z = v1.Vector3Data_Z;
// 
//         v2 *= scale;
//     }
// 
//     public static void Vector3_CopyXY(ProtoBuf.Vector3Data v1, ref Vector2 v2)
//     {
//         v2.x = v1.Vector3Data_X;
//         v2.y = v1.Vector3Data_Y;
//     }

    public static void Rotate(ref float x, ref float z, float angle)
    {
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        float locationX = sin * z + cos * x;
        float locationZ = cos * z - sin * x;
        x = locationX;
        z = locationZ;
    }

    public static Vector3 GetCameraVec(float CameraViewAngle, float CameraLookAngle, float CameraLength)
    {
        Quaternion quat = Quaternion.Euler(CameraViewAngle, CameraLookAngle, 0);
        return quat * Vector3.forward * CameraLength;
    }

    public static long TickToMilliSec(long tick)
    {
        return tick / (10 * 1000);
    }

    public static long MilliSecToTick(long time)
    {
        return time * 10 * 1000;
    }

    public static float MilliSecToSec(long ms)
    {
        return ((float)ms) / 1000;
    }

    public static float MilliSecToMinute(long ms)
    {
        return ((float)ms) / 60000;
    }

    public static long SecToMilliSec(float s)
    {
        return (long)(s * 1000);
    }

    public static long MilliSecToHour(long ms)
    {
        return ms / 3600000;
    }

    public static long HourToMilliSec(long h)
    {
        return h * 3600000;
    }

    //万进制
    public static long CalculateWanHex(long number)
    {
        return number / 10000;
    }

    //亿进制
    public static long CalculateHundredMillionHex(long number)
    {
        return number / 100000000;
    }

    public static string FormatNumber(long number)
    {
        if (number < 10000)
            return number.ToString();

        number /= 1000;
        number *= 1000;

        float w = (float)number / 10000.0f;

        return w.ToString() + "W";
    }

    public enum FormatTimeFlags
    {
        Day = 1,
        Hour = 2,
        Minute = 4,
        Seconds = 8,
        All = Day | Hour | Minute | Seconds,
        All2 = Hour | Minute | Seconds,
        All3 = Hour | Minute,
        All4 = Minute | Seconds,
    }

    public static void SetRenderQueue(GameObject go, int queue)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] shareMaterials = renderers[i].sharedMaterials;
            for (int j = 0; j < shareMaterials.Length; j++)
            {
                if (shareMaterials[j])
                    shareMaterials[j].renderQueue = queue;
            }
            renderers[i].sharedMaterials = shareMaterials;
        }
    }



    public static string GetWordString(string str)
    {
        //         if (!WordIDDic.ContainsKey(str))
        //         {
        //             Logger.instance.Error("翻译表没有内容： {0}！\n", str);
        //             return str;
        //         }
        //         else
        //         {
        //             TranslateClient tb = TranslateClientManager.instance.Find(WordIDDic[str]);
        // 
        //             // to do. 根据语言版本返回文字
        //             return tb.showWords;
        //         }

        return str;
    }

    //     public static string GetWordString(uint id)
    //     {
    //         TranslateClient tableTranslate = TranslateClientManager.instance.Find(id);
    //         if (null == tableTranslate)
    //         {
    //             Logger.instance.Error("翻译表没有id为[{0}]的内容\n", id);
    //             return "";
    //         }
    // 
    //         return tableTranslate.showWords;
    //     }

    public static string FormatTime(float seconds, FormatTimeFlags flags = FormatTimeFlags.All)
    {
        string str = "";
        int day = (int)(seconds / 86400.0f);
        if (day > 0 && (flags & FormatTimeFlags.Day) != 0)
        {
            str = day + Utility.GetWordString("天");
            seconds -= (float)day * 86400.0f;
        }
        int hour = (int)(seconds / 3600.0f);
        if (hour > 0 && (flags & FormatTimeFlags.Hour) != 0)
        {
            str += hour + Utility.GetWordString("小时");
            seconds -= (float)hour * 3600.0f;
        }
        int minute = (int)(seconds / 60.0f);
        if (minute > 0 && (flags & FormatTimeFlags.Minute) != 0)
        {
            str += minute + Utility.GetWordString("分");
            seconds -= (float)minute * 60.0f;
        }
        if ((seconds > 0 && (flags & FormatTimeFlags.Seconds) != 0) ||
            string.IsNullOrEmpty(str)) // 没有任何值就显示0秒
        {
            str += (int)seconds + Utility.GetWordString("秒");
        }

        return str;
    }

    public static string FormatTime2(float seconds, FormatTimeFlags flags = FormatTimeFlags.All)
    {
        string str = "";
        int day = (int)(seconds / 86400.0f);
        if ((flags & FormatTimeFlags.Day) != 0)
        {
            str = day + ":";
            seconds -= (float)day * 86400.0f;
        }
        int hour = (int)(seconds / 3600.0f);
        if ((flags & FormatTimeFlags.Hour) != 0)
        {
            str += hour >= 10 ? hour + ":" : "0" + hour + ":";
            seconds -= (float)hour * 3600.0f;
        }
        int minute = (int)(seconds / 60.0f);
        if ((flags & FormatTimeFlags.Minute) != 0)
        {
            str += minute >= 10 ? minute + ":" : "0" + minute + ":";
            seconds -= (float)minute * 60.0f;
        }
        if ((flags & FormatTimeFlags.Seconds) != 0) // 没有任何值就显示0秒
        {
            str += (int)seconds >= 10 ? ((int)seconds).ToString() : "0" + ((int)seconds).ToString();
        }

        return str;
    }


    public static Vector2 ScreenPosToNGUI(Vector2 pos)
    {
        Vector2 p = pos;
        p.x = pos.x - Screen.width / 2.0f;
        p.y = pos.y - Screen.height / 2.0f;
        return p;
    }

    public static float CrossProduct(float x0, float y0, float x1, float y1) { return x0 * y1 - y0 * x1; }

    public static string ToString(byte[] array)
    {
        int size = 0;
        foreach (byte item in array)
        {
            if (item == 0)
                break;

            ++size;
        }

        return System.Text.Encoding.UTF8.GetString(array, 0, size);
    }

    public static void SaveProtoFile<T>(string filePath, bool isDelete, T t)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            if (File.Exists(filePath) && isDelete)
            {
                File.Delete(filePath);
            }

            ProtobufSerializer.Serialize(ms, t);
            //Serializer.Serialize<T>(ms, t);

            using (FileStream stream = File.Open(filePath, FileMode.OpenOrCreate/*, FileAccess.Write*/))
            {
                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write(ms.ToArray());
                bw.Flush();
                bw.Close();
                stream.Close();
            }
            ms.Close();
        }
    }

    public static T LoadProtoFile<T>(string filePath)
    {
        byte[] aBytes = Star.Foundation.CFile.ReadAllBytes(filePath, Star.Foundation.CFile.MODE_READ | Star.Foundation.CFile.MODE_BINARY);
        if (aBytes != null)
        {
            T t = System.Activator.CreateInstance<T>();
            MemoryStream stream = new MemoryStream(aBytes);
            try
            {
                t = (T)ProtobufSerializer.Deserialize(stream, null, t.GetType());
                stream.Close();
                stream.Dispose();
            }
            catch (System.Exception ex)
            {
                //error
                Logger.instance.Error("GetFromFile error [" + filePath + "] " + ex.Message);
            }

            return t;
        }
        else return default(T);
    }

    public static Vector3 StringToVector3(string value)
    {
        string tmp = value.Substring(1, value.Length - 2);
        //Logger.instance.Log(tmp);
        string[] s = tmp.Split(new char[] { ',' });

        float x, y, z;
        x = float.Parse(s[0]);
        y = float.Parse(s[1]);
        z = float.Parse(s[2]);

        return new Vector3(x, y, z);
    }

    public static string Vector3ToString(Vector3 vec)
    {
        string s = string.Format("({0},{1},{2})", vec.x.ToString(), vec.y.ToString(), vec.z.ToString());
        return s;
    }

    public static Quaternion StringToQuaternion(string value)
    {
        string tmp = value.Substring(1, value.Length - 2);
        //Logger.instance.Log(tmp);
        string[] s = tmp.Split(new char[] { ',' });

        float x, y, z, w;
        x = float.Parse(s[0]);
        y = float.Parse(s[1]);
        z = float.Parse(s[2]);
        w = float.Parse(s[3]);

        return new Quaternion(x, y, z, w);
    }

    public static string QuaternionToString(Quaternion qua)
    {
        string s = "(" + qua.x.ToString() + "," + qua.y.ToString()
            + "," + qua.z.ToString() + "," + qua.w.ToString() + ")";

        return s;
    }


    /// <summary>
    /// 世界坐标转换NGUI坐标
    /// </summary>
    /// <param name="WorldPos">世界坐标</param>
    /// <returns></returns>
    public static Vector3 WorldToNGUI(Vector3 WorldPos)
    {
        Vector3 pt = Camera.main.WorldToScreenPoint(WorldPos);
        Vector3 ff = ClientRoot.instance.uiCamera.ScreenToWorldPoint(pt);
        ff.z = 0;
        return ff;
    }


    /// <summary>
    /// 世界缩放NGUI，血条等UI用
    /// </summary>
    /// <param name="headPos">头顶坐标</param>
    /// <param name="startFormat">开始距离</param>
    /// <returns></returns>
    public static Vector3 ScaleUI(Vector3 headPos, float startFormat)
    {
        float newFomat = startFormat / Vector3.Distance(headPos, Camera.main.transform.position);
        return Vector3.one * newFomat;
    }


    /// <summary>
    /// 判断屏幕坐标是否在UI上。
    /// </summary>
    /// <param name="pos">屏幕坐标</param>
    /// <returns></returns>
    //public static bool IsGUIContains(Vector2 pos)
    //{
    //    if (XuanYuan.ClientRoot.instance.uiCamera != null)
    //    {
    //        Ray ray = XuanYuan.ClientRoot.instance.uiCamera.ScreenPointToRay(pos);
    //        RaycastHit hit = new RaycastHit();

    //        int iLayer = 1 << XuanYuan.GameDef.UI_LAYER;
    //        iLayer |= XuanYuan.GameDef.UIDIALOG_LAYER;
    //        if (Physics.Raycast(ray, out hit, iLayer)
    //            && (hit.collider.gameObject.layer == XuanYuan.GameDef.UI_LAYER
    //                || hit.collider.gameObject.layer == XuanYuan.GameDef.UIDIALOG_LAYER)
    //            )
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    /// <summary>
    /// 延迟执行
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="callback"></param>
    public delegate void DelayFinishCallback(params object[] Parameters);
    public static void DelayFinish(float delayTime, DelayFinishCallback callback, params object[] Parameters)
    {
        DaikonForge.Tween.Tween<float> tween = new DaikonForge.Tween.Tween<float>()
            .SetStartValue(0)
            .SetEndValue(1)
            .SetDuration(delayTime);
        tween.OnCompleted((value) =>
        {
            if (callback != null)
                callback(Parameters);
        }).Play();
    }



    static float[] m_HitDefPointX = new float[4];
    static float[] m_HitDefPointZ = new float[4];
    static float[] m_AttackeePointX = new float[4];
    static float[] m_AttackeePointZ = new float[4];

    public static bool RectangleHitDefineCollision(
        Vector3 HitDefPos, float HitDefOrientation,
        Vector3 HitDef,
        Vector3 AttackeePos, float AttackeeOrientation,
        Vector3 AttackeeBounding)
    {
        //Debug.Log("11111111111111111111" + HitDefPos + "   AttackeePos:" + AttackeePos + "     AttackeeBounding" + AttackeeBounding + "   HitDef" + HitDef);
        //排除高度影响，以XZ平面坐标作为判定基准
        if (HitDefPos.y > AttackeePos.y + AttackeeBounding.y ||
            AttackeePos.y > HitDefPos.y + HitDef.y)
        {
            return false;
        }

        // 计算出第一个四边形的四个定点
        float x0 = -HitDef.x * 0.5f, z0 = -HitDef.z * 0.5f;
        float x1 = -HitDef.x * 0.5f, z1 = HitDef.z * 0.5f;
        Rotate(ref x0, ref z0, HitDefOrientation);
        Rotate(ref x1, ref z1, HitDefOrientation);
        Vector2 maxHit = new Vector2(Mathf.Max(Mathf.Abs(x0), Mathf.Abs(x1)), Mathf.Max(Mathf.Abs(z0), Mathf.Abs(z1)));

        m_HitDefPointX[0] = HitDefPos.x - x0;
        m_HitDefPointX[1] = HitDefPos.x - x1;
        m_HitDefPointX[2] = HitDefPos.x + x0;
        m_HitDefPointX[3] = HitDefPos.x + x1;

        m_HitDefPointZ[0] = HitDefPos.z - z0;
        m_HitDefPointZ[1] = HitDefPos.z - z1;
        m_HitDefPointZ[2] = HitDefPos.z + z0;
        m_HitDefPointZ[3] = HitDefPos.z + z1;


        // 计算出第二个四边形的四个顶点
        x0 = -AttackeeBounding.x * 0.5f;
        z0 = -AttackeeBounding.z * 0.5f;
        x1 = -AttackeeBounding.x * 0.5f;
        z1 = AttackeeBounding.z * 0.5f;
        Rotate(ref x0, ref z0, AttackeeOrientation);
        Rotate(ref x1, ref z1, AttackeeOrientation);
        Vector2 maxAtk = new Vector2(Mathf.Max(Mathf.Abs(x0), Mathf.Abs(x1)), Mathf.Max(Mathf.Abs(z0), Mathf.Abs(z1)));

        m_AttackeePointX[0] = AttackeePos.x - x0;
        m_AttackeePointX[1] = AttackeePos.x - x1;
        m_AttackeePointX[2] = AttackeePos.x + x0;
        m_AttackeePointX[3] = AttackeePos.x + x1;

        m_AttackeePointZ[0] = AttackeePos.z - z0;
        m_AttackeePointZ[1] = AttackeePos.z - z1;
        m_AttackeePointZ[2] = AttackeePos.z + z0;
        m_AttackeePointZ[3] = AttackeePos.z + z1;

        if (HitDefPos.x > AttackeePos.x + maxHit[0] + maxAtk[0] ||
            HitDefPos.x < AttackeePos.x - maxHit[0] - maxAtk[0] ||
            HitDefPos.z > AttackeePos.z + maxHit[1] + maxAtk[1] ||
            HitDefPos.z < AttackeePos.z - maxHit[1] - maxAtk[1])
            return false;

        // 拿四边形的四个顶点判断，是否在另外一个四边形的四条边的一侧
        for (int i = 0; i < 4; i++)
        {
            x0 = m_HitDefPointX[i];
            x1 = m_HitDefPointX[(i + 1) % 4];
            z0 = m_HitDefPointZ[i];
            z1 = m_HitDefPointZ[(i + 1) % 4];

            bool hasSameSidePoint = false;
            for (int j = 0; j < 4; j++)
            {
                float v = CrossProduct(x1 - x0, z1 - z0, m_AttackeePointX[j] - x0, m_AttackeePointZ[j] - z0);
                if (v < 0)
                {
                    hasSameSidePoint = true;
                    break;
                }
            }

            // 如果4个定点都在其中一条边的另外一侧，说明没有交点
            if (!hasSameSidePoint)
                return false;
        }
        // 所有边可以分割另外一个四边形，说明有焦点。
        return true;
    }


    public static bool CylinderHitDefineCollision(
        Vector3 HitDefPos, float HitDefOrientation,
        float HitRadius, float HitDefHeight,
        Vector3 AttackeePos, float AttackeeOrientation,
        Vector3 AttackeeBounding)
    {
        //排除高度影响，以XZ平面坐标作为判定基准
        if (HitDefPos.y > AttackeePos.y + AttackeeBounding.y ||
            AttackeePos.y > HitDefPos.y + HitDefHeight)
            return false;

        float vectz = HitDefPos.z - AttackeePos.z;
        float vectx = HitDefPos.x - AttackeePos.x;
        if (vectx != 0 || vectz != 0)
            Rotate(ref vectx, ref vectz, -AttackeeOrientation);

        if ((Mathf.Abs(vectx) > (HitRadius + AttackeeBounding.z)) || (Mathf.Abs(vectz) > (HitRadius + AttackeeBounding.x)))
            return false;

        return true;
    }

    public static bool RingHitDefineCollision(
        Vector3 HitDefPos, float HitDefOrientation,
        float HitInnerRadius, float HitDefHeight, float HitOutRadius,
        Vector3 AttackeePos, float AttackeeOrientation,
        Vector3 AttackeeBounding)
    {
        //排除高度影响，以XZ平面坐标作为判定基准
        if (HitDefPos.y > AttackeePos.y + AttackeeBounding.y ||
            AttackeePos.y > HitDefPos.y + HitDefHeight)
            return false;

        float radius = Mathf.Min(AttackeeBounding.x, AttackeeBounding.z);
        float distance = (AttackeePos - HitDefPos).magnitude;
        if (distance + radius < HitInnerRadius || distance - radius > HitOutRadius)
            return false;

        return true;
    }

    public static bool FanHitDefineCollision(
        Vector3 HitDefPos, float HitDefOrientation,
        float HitRadius, float HitDefHeight, float HitStartAngle, float HitEndAngle,
        Vector3 AttackeePos, float AttackeeOrientation,
        Vector3 AttackeeBounding)
    {
        //排除高度影响，以XZ平面坐标作为判定基准
        //         if (HitDefPos.y > AttackeePos.y + AttackeeBounding.y ||
        //             AttackeePos.y > HitDefPos.y + HitDefHeight)
        //             return false;

        float vectz = HitDefPos.z - AttackeePos.z;
        float vectx = HitDefPos.x - AttackeePos.x;
        if (vectx != 0 || vectz != 0)
            Rotate(ref vectx, ref vectz, -AttackeeOrientation);
        //         if ((Mathf.Abs(vectx) > (HitRadius + AttackeeBounding.z)) || (Mathf.Abs(vectz) > (HitRadius + AttackeeBounding.x)))
        //             return false;

        float angle = Mathf.Abs(Mathf.Atan2(vectz, vectx));
        float startAngle = HitDefOrientation + 4.12f;

        if (startAngle > Mathf.PI * 2)
            startAngle -= Mathf.PI;

        float endAngle = startAngle + HitStartAngle * Mathf.Deg2Rad;

        if (endAngle > Mathf.PI * 2)
            endAngle -= Mathf.PI;

        Logger.instance.Log("{0} - {1} - {2}", angle * Mathf.Rad2Deg, startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg);

        //if (mTempGo != null)
        //    Utility.Destroy(mTempGo);

        //mTempGo = new GameObject("FanAttackFrame");
        //JMLEllipsoidCurve jml = mTempGo.AddComponent<JMLEllipsoidCurve>();
        //jml.Radius = new Vector2(HitRadius, HitRadius);
        //jml.EllipsoidAmplitude = HitStartAngle * Mathf.Deg2Rad;
        //jml.Offset = HitDefOrientation + 4.12f > Mathf.PI * 2 ? HitDefOrientation + 4.12f - Mathf.PI : HitDefOrientation + 4.12f;
        //mTempGo.transform.position = HitDefPos;
        //mTempGo.transform.rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));

        if (angle > Mathf.Min(startAngle, endAngle) && angle < Mathf.Max(startAngle, endAngle))
            return true;

        return false;
    }

    /// <summary>
    /// 获得百分比数值
    /// </summary>
    /// <returns></returns>
    public static float GetPercentValue(float current , float max)
    {
        return current / max;
    }


    public static float DistanceMax(float x1, float y1, float x2, float y2)
    {
        return Mathf.Max(Mathf.Abs(x1 - x2), Mathf.Abs(y1 - y2));
    }

    public static float DistanceXZ(Vector3 p0, Vector3 p1)
    {
        Vector3 v = p0 - p1;
        v.y = 0;
        return v.magnitude;
    }

}
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : Singleton<PoolManager>
{
    public enum PoolFlag
    {
        normal,
        effect,
        actor,
        item_ui,
        property_ui,
        max,
    };

    Dictionary<PoolFlag, Dictionary<string, Queue<GameObject>>> m_ObjectPools = new Dictionary<PoolFlag, Dictionary<string, Queue<GameObject>>>();

    GameObject m_poolObj = null;

    public void Initialize()
    {
        m_poolObj = new GameObject("_GamePool");
        Object.DontDestroyOnLoad(m_poolObj);
    }

    public void ClearAll()
    {
        for (int i = 0; i < (int)PoolFlag.max; ++i)
        {
            ClearPool((PoolFlag)i);
        }
        m_ObjectPools.Clear();
    }

    public void OnDestroy()
    {
        // ClearAll();
        Object.Destroy(m_poolObj);
    }

    public void PreSpawn(PoolFlag flag, string keyName, Object srcAssets, int count, bool deActive = true)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject go = Utility.Instantiate(srcAssets) as GameObject;
            if (!deActive) Utility.SetActiveObject(go, false);
            Despawn(flag, keyName, go, deActive);
        }
    }

    public GameObject Spawn(PoolFlag flag, string keyName, Object srcAssets)
    {
        Queue<GameObject> pool = null;
        if (!m_ObjectPools.ContainsKey(flag))
            m_ObjectPools.Add(flag, new Dictionary<string, Queue<GameObject>>());
        if (m_ObjectPools[flag].TryGetValue(keyName, out pool))
        {
            if (pool.Count > 0)
            {
                GameObject ret = pool.Dequeue();
                ret.transform.parent = null;
                return ret;
            }
            else
                return Utility.Instantiate(srcAssets) as GameObject;
        }
        return Utility.Instantiate(srcAssets) as GameObject;
    }

    public GameObject Spawn(PoolFlag flag, Object srcAssets)
    {
        return Spawn(flag, srcAssets.name, srcAssets);
    }

    public void Despawn(PoolFlag flag, string resName, GameObject go, bool deActive = true)
    {
        if (go == null
            || string.IsNullOrEmpty(resName)
            || m_poolObj == null)
            return;

        if (!m_ObjectPools.ContainsKey(flag))
        {
            Queue<GameObject> tmp = new Queue<GameObject>();
            Dictionary<string, Queue<GameObject>> dic = new Dictionary<string, Queue<GameObject>>();
            tmp.Enqueue(go);
            dic.Add(resName, tmp);
            m_ObjectPools.Add(flag, dic);
        }

        go.transform.parent = null;
        go.transform.position = Vector3.zero;
        //go.transform.localScale = Vector3.one;

        go.transform.parent = m_poolObj.transform;

        Queue<GameObject> pool = null;
        if (m_ObjectPools[flag].TryGetValue(resName, out pool))
        {
            if (deActive)
                go.SetActive(false);
            pool.Enqueue(go);
        }
        else
        {
            pool = new Queue<GameObject>();
            if (deActive)
                go.SetActive(false);
            pool.Enqueue(go);
            m_ObjectPools[flag].Add(resName, pool);
        }

        go = null;
    }

    public void ClearPool(PoolFlag flag)
    {
        if (!m_ObjectPools.ContainsKey(flag))
            return;

        foreach (string res in m_ObjectPools[flag].Keys)
        {
            while (m_ObjectPools[flag][res].Count > 0)
            {
                GameObject go = m_ObjectPools[flag][res].Dequeue();
                if (go == null)
                    continue;

                go.transform.parent = null;
                Object.Destroy(go);
            }
            m_ObjectPools[flag][res].Clear();
        }
        m_ObjectPools[flag].Clear();
        m_ObjectPools.Remove(flag);
    }
}

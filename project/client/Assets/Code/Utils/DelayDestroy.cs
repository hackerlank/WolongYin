using UnityEngine;
using System.Collections;

public class DelayDestroy : MonoBehaviour
{

    public delegate void OnDestroy(string resName, GameObject obj);
    public int Time = 1000;
    public string mResName;
    public OnDestroy onDestroy = null;

    void OnEnable()
    {
        StartCoroutine(StartTime());
    }

    // Use this for initialization
    IEnumerator StartTime()
    {
        yield return new WaitForSeconds(Time / 1000f);
        if (onDestroy == null)
            Utility.Destroy(gameObject);
        else
            onDestroy(mResName, gameObject);
    }
}

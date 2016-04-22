using UnityEngine;
using System.Collections;



public class AutoDestroy : MonoBehaviour
{
    public delegate void OnDestroy(string resName, GameObject obj);
    public int Time = 1000;
    public string mResName;
    public OnDestroy OnDestroyFunc = null;

    void OnEnable()
    {
        StartCoroutine(StartTime());
    }


    IEnumerator StartTime()
    {
        yield return new WaitForSeconds(Time / 1000f);
        if (OnDestroyFunc == null)
            Utility.Destroy(gameObject);
        else
            OnDestroyFunc(mResName, gameObject);
    }
}

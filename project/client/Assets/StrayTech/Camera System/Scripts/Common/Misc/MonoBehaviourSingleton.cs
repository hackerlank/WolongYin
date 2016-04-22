using UnityEngine;
using System.Collections;

namespace StrayTech
{
    /// <summary>
    /// <para>Inherit from this if you're a monobehaviour and you want to be a singleton.</para>
    /// <para>Represents a singleton class that is also a MonoBehaviour, and has all the functionality of a MonoBehaviour.</para>
    /// <para>Takes advantage of how C# creates static members of generic classes.</para>
    /// 
    /// 
    /// Because of the challanges of creating a new singelton from code (scene leaks from OnDestroy/OnDisable), the workflow for this class assumes
    /// that an instance of this class will exist in the scene. The instance accessor differs from a traditional singelton because it will not create
    /// a new instance for you. If you need an instance, it has to exist in the scene. /// 
    /// 
    /// </summary>
    /// <typeparam name="T">The type that is a MonoBehaviour we want to be a Singleton.</typeparam>
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        #region members
            /// <summary>
            /// The singleton instance.
            /// </summary>
            private static T _instance;
        #endregion members

        #region properties
            public static T Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        var inScene = GameObject.FindObjectOfType<T>();

                        if (inScene != null)
                        {
                            _instance = inScene;
                        }
                    }

                    return _instance;
                }
            }
        #endregion properties

        #region constructors
            protected virtual void Awake()
            {
                if (_instance != null && _instance != this)
                {
                    Debug.LogFormat("{0} is being destroyed because a singelton reference was already set!", this.name);
                    Destroy(this);
                    return;
                }

                //If we got here then we are the singelton. Get the reference to the component. 
                _instance = this.GetComponent<T>();
            }
        #endregion construcors


        #region methods
            protected virtual void OnDestroy()
            {
                if (_instance == this)
                {
                    _instance = null;
                }
            }

            protected virtual void OnApplicationQuit()
            {
                if (_instance != this)
                    return;
            }
        #endregion methods
    }
}
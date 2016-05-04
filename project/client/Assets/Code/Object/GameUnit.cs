using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public class GameUnit : BaseGameMono, IActionControllerPlayable, IUIEventListener
{
    private string mID = string.Empty;
    private int mTableID = -1;
    protected GameObject mModel = null;
    protected bool mDone = false;
    private string mActiveModelName = string.Empty;
    private bool mDestroyed = false;
    private Utility.VoidDelegate mOnLoadedCallback = null;
    private EActionState mActionState = EActionState.stop;
    private CharacterController mCharacterController = null;
    private AnimatorController mAnimatorController = null;
    private ActionController mActionController = null;

    #region Get&Set
    public string ID
    {
        get { return mID; }
        set { mID = value; }
    }

    public int TableID
    {
        get { return mTableID; }
        set { mTableID = value; }
    }

    public string ActiveModelName
    {
        get { return mActiveModelName; }
    }

    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Quaternion rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }

    public Transform parent
    {
        get { return transform.parent; }
        set { transform.parent = value; }
    }

    public Utility.VoidDelegate OnLoadedCallback
    {
        get { return mOnLoadedCallback; }
        set { mOnLoadedCallback = value; }
    }

    public GameObject Model
    {
        get { return mModel; }
    }

    public bool Done
    {
        get { return mDone; }
    }

    public UnityEngine.CharacterController characterController
    {
        get { return mCharacterController; }
    }

    public AnimatorController animatorController
    {
        get { return mAnimatorController; }
    }

    public ActionController actionController
    {
        get { return mActionController; }
    }
    #endregion

    #region mono func

    public override void Awake()
    {
    }

    public override void Start()
    {
    }

    public override void Update(float deltaTime)
    {
    }

    #endregion

    #region Model Set

    public virtual void ChangeModel(string model)
    {
        if (string.IsNullOrEmpty(model))
            return;

        mActiveModelName = model;
        ResourceCenter.instance.LoadObject(model,
            _OnLoadedModel);
    }

    protected virtual void _OnLoadedModel(Object asset)
    {
        GameObject model = Utility.Instantiate(asset) as GameObject;
        _SetModel(model);

        if (Model.GetComponent<CharacterController>() != null)
        {
            _SetCharacterController();
        }

        //Utility.SetObjectLayer(gameObject, LayerMask.NameToLayer("Unit"));

        if (OnLoadedCallback != null)
        {
            OnLoadedCallback();
            OnLoadedCallback = null;
        }

    }

    protected virtual void _InitModelTransform()
    {
        transform.localScale = Model.transform.localScale;
        //transform.localRotation = Quaternion.identity;

        Utility.SetIdentityChild(transform, Model.transform);
    }

    protected virtual void _ResetCollider()
    {
        BoxCollider dstBc = AddComponentIfMissing<BoxCollider>();
        BoxCollider srcBc = Model.GetComponent<BoxCollider>();
        if (srcBc == null)
        {
            Debug.LogError("Can't found BoxCollider in model " + name + "\n");
            return;
        }

        srcBc.enabled = false;
        dstBc.center = srcBc.center;
        dstBc.size = srcBc.size;


        //         if (Get_Model().GetComponent<CharacterController>() != null)
        //         {
        //             dstBc.enabled = false;
        //         }
    }

    void _SetCharacterController()
    {
        if (characterController == null)
        {
            CharacterController srcCC = Model.GetComponent<CharacterController>();
            CharacterController DstCC = AddComponentIfMissing<CharacterController>();
            if (srcCC != null)
            {
                srcCC.enabled = false;

                DstCC.center = srcCC.center;
                DstCC.radius = srcCC.radius;
                DstCC.height = srcCC.height;
                DstCC.skinWidth = srcCC.skinWidth;
                DstCC.stepOffset = srcCC.stepOffset;
                DstCC.slopeLimit = srcCC.slopeLimit;
                //DstCC.detectCollisions = false;
            }

            mCharacterController = DstCC;
        }
    }

    protected void _SetModel(GameObject model)
    {
        if (model == null)
        {
            Logger.instance.Error("空的模型 {0} - {1}!\n", name, TableID);
            return;
        }

        if (model == Model)
            return;

        if (Model != null)
        {
            Utility.Destroy(Model);
        }

        mModel = model;

        mDone = true;

        _InitModelTransform();

        // 不设置碰撞框
        //_ResetCollider();
    }
    #endregion

    #region helper
    public T AddComponentIfMissing<T>() where T : Component
    {
        T mono = gameObject.GetComponent<T>();
        if (mono == null)
            gameObject.AddComponent<T>();
        return mono;
    }
    
    #endregion

    #region game func

    public static T Create<T>(string name, int tableid, SceneLoader loader, params object[] args) where T : GameUnit
    {
        GameObject go = new GameObject(name);
        GameMonoAgent agent = go.AddComponent<GameMonoAgent>();
        T unit = agent.AddGameMonoComponent<T>();
        go.RegisterGameUnit<T>(unit);
        bool b = unit._OnCreate(tableid, loader, args);
        if (b)
        {
            return unit;
        }
        else
        {
            Utility.Destroy(go);
            return null;
        }
    }

    protected virtual bool _OnCreate(int tableid, SceneLoader loader, params object[] args)
    {
        return false;
    }
    #endregion

    #region IActionControllerPlayable

    public EActionState actionState
    {
        get { return mActionState;  }
        protected set { mActionState = value; }
    }

    public virtual void CrossFade(string name, float blendtime = 0.3f, float normalizedTime = 0f)
    {
        actionState = EActionState.playing;
        
        if (animatorController != null)
            animatorController.CrossFade(name, blendtime, normalizedTime);
    }

    public virtual void Pause()
    {
        actionState = EActionState.pause;
        
        if (animatorController != null)
            animatorController.Pause();

        if (actionController != null)
            actionController.Pause();
    }

    public virtual void Resume()
    {
        actionState = EActionState.playing;
        
        if (animatorController != null)
            animatorController.Resume();

        if (actionController != null)
            actionController.Resume();
    }

    public virtual void Stop()
    {
        actionState = EActionState.stop;
        
        if (animatorController != null)
            animatorController.Stop();

        if (actionController != null)
            actionController.Stop();
    }
    #endregion

    #region IUIEventListener
    public virtual void OnClick(GameObject go)
    {
    }

    public virtual void OnHover(GameObject go, bool state)
    {
    }

    public virtual void OnPress(GameObject go, bool press)
    {
    }

    public virtual void OnDragStart(GameObject go)
    {
    }

    public virtual void OnDrag(GameObject go, Vector2 delta)
    {
    }

    public virtual void OnDragEnd(GameObject go)
    {
    }
    #endregion
}

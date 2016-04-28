using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class PlayEffectEvent : GameEvent
{
    public enum EBindMode
    {
        none,
        bindModel,
        bindBone,
    }

    public const int DELETE_MODE_NORMAL = 1 << 0;
    public const int DELETE_MODE_WITH_ACTION = 1 << 1;
    public const int DELETE_MODE_WITH_HURT = 1 << 2;

    private EffectProto mProtoData = null;
    private GameUnit mModel = null;

    protected override void _SetType()
    {
        mType = EGameEventType.PlayEffect;
    }

    protected override void _OnDelete()
    {
    }

    protected override void _Reset()
    {
        mModel = null;
        mProtoData = null;
    }

    public override void SetData(params object[] args)
    {
        mProtoData = (EffectProto)args[0];
        mModel = (GameUnit) args[1];
    }

    public override void Execute()
    {
        if (string.IsNullOrEmpty(mProtoData.assetName)
            || mModel == null)
            return;

        ResourceCenter.instance.LoadObject(mProtoData.assetName,
            (value) =>
            {
                GameObject go = (GameObject) Utility.Spawn(value);

                Transform parent = null;

                EBindMode bindMode = (EBindMode) mProtoData.bindMode;
                if (bindMode == EBindMode.bindModel)
                {
                    parent = mModel.transform;
                }
                else if (bindMode == EBindMode.bindBone)
                {
                    parent = Utility.FindNode<Transform>(mModel.gameObject, mProtoData.bindBone);
                }

                if (!mProtoData.deleteMode.CheckMask(DELETE_MODE_NORMAL))
                {
                    if (mProtoData.deleteMode.CheckMask(DELETE_MODE_WITH_ACTION))
                    {
                        if (mModel.actionController != null)
                            mModel.actionController.AddBindEffect(go);
                    }
                    if (mProtoData.deleteMode.CheckMask(DELETE_MODE_WITH_HURT))
                    {
                        // to do.
                    }
                }

                Vector3 offset = Utility.ToVector3(mProtoData.offset);
                Vector3 euler = Utility.ToVector3(mProtoData.rotate);
                Vector3 scale = new Vector3(mProtoData.scale, mProtoData.scale, mProtoData.scale);

                go.transform.parent = parent;
                go.transform.localPosition = offset;
                go.transform.localScale = scale;
                go.SetActive(true);
            });
    }

}
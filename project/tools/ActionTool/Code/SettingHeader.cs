using System.Collections.Generic;
using ProtoBuf;

#region UnitActionHeader
public class UnitActionHeader
{
    public int roleID { get; set; }
    public int idleState { get; set; }
    public List<ActionStateHeader> StateList { get; set; }
    public List<AttackDefHeader> DefList { get; set; }

    public void Parse(UnitActionProto proto)
    {
        roleID = proto.roleID;
        idleState = proto.idleState;

        StateList.Clear();
        foreach (var actionStateProto in proto.actions)
        {
            ActionStateHeader ash = new ActionStateHeader();
            ash.Parse(actionStateProto);

            StateList.Add(ash);
        }

        DefList.Clear();
        foreach (var attackDefProto in proto.atkDefList)
        {
            AttackDefHeader adh = new AttackDefHeader();
            adh.Parse(attackDefProto);

            DefList.Add(adh);
        }
    }

    public UnitActionProto ConvertToProto()
    {
        UnitActionProto pd = new UnitActionProto();

        pd.roleID = roleID;
        pd.idleState = idleState;

        return pd;
    }
}
#endregion


#region ActionStateHeader

public class ActionStateHeader
{
    public string stateName { get; set; }
    public int stateID { get; set; }
    public float stateTime { get; set; }
    public int nextStateID { get; set; }
    public List<AnimSlotHeader> slotList { get; set; }
    public List<GameEventHeader> eventList { get; set; }

    public void Parse(ActionStateProto proto)
    {

    }

    public ActionStateProto ConvertToProto()
    {
        ActionStateProto pd = new ActionStateProto();

        return pd;
    }
}

#endregion


#region AnimSlotHeader

public class AnimSlotHeader
{
    public string animName { get; set; }
    public float startTime { get; set; }
    public float endTime { get; set; }
    public int weight { get; set; }

    public void Parse(AnimSlotProto proto)
    {

    }

    public AnimSlotProto ConvertToProto()
    {
        AnimSlotProto pd = new AnimSlotProto();

        return pd;
    }
}

#endregion


#region AttackDefHeader

public class AttackDefHeader
{
    public string attackDefName { get; set; }
    public int attackDefID { get; set; }
    public float triggerTime { get; set; }
    public float duration { get; set; }
    public AttackDefFxGroupHeader normalFx { get; set; }
    public AttackDefFxGroupHeader critFx { get; set; }
    public List<GameEventHeader> eventList { get; set; }
    public List<GameEventHeader> hitedEvents { get; set; }
    public float hitedTime { get; set; }
    public HitDefHeader hitedData { get; set; }

    public void Parse(AttackDefProto proto)
    {

    }

    public AttackDefProto ConvertToProto()
    {
        AttackDefProto pd = new AttackDefProto();

        return pd;
    }
}

#endregion


#region HitDefHeader
public class HitDefHeader
{
    public float triggerTime { get; set; }

    public void Parse(HitDefProto proto)
    {

    }

    public HitDefProto ConvertToProto()
    {
        HitDefProto pd = new HitDefProto();

        return pd;
    }
}
#endregion


#region AttackDefFxGroupHeader
public class AttackDefFxGroupHeader
{
    public EffectHeader HitedEffect { get; set; }
    public SoundHeader HitedSound { get; set; }
    public EffectHeader SelfEffect { get; set; }
    public SoundHeader SelfSound { get; set; }

    public void Parse(AttackDefProto proto)
    {

    }

    public AttackDefProto ConvertToProto()
    {
        AttackDefProto pd = new AttackDefProto();

        return pd;
    }
}
#endregion


#region Vector3Header
public class Vector3Header
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public void Parse(Vector3Proto proto)
    {

    }

    public Vector3Proto ConvertToProto()
    {
        Vector3Proto pd = new Vector3Proto();

        return pd;
    }
}
#endregion


#region GameEventHeader

public class GameEventHeader
{
    public int eventType { get; set; }
    public float triggerTime { get; set; }
    public EffectHeader effectData { get; set; }
    public SoundHeader soundData { get; set; }

    public void Parse(GameEventProto proto)
    {

    }

    public GameEventProto ConvertToProto()
    {
        GameEventProto pd = new GameEventProto();

        return pd;
    }
}

#endregion


#region EffectHeader

public class EffectHeader
{
    public Vector3Header offset { get; set; }
    public Vector3Header rotate { get; set; }
    public float scale { get; set; }
    public string assetName { get; set; }
    public string bindBone { get; set; }
    public int bindMode { get; set; }
    public int deleteMode { get; set; }

    public void Parse(EffectProto proto)
    {

    }

    public EffectProto ConvertToProto()
    {
        EffectProto pd = new EffectProto();

        return pd;
    }
}

#endregion


#region SoundHeader

public class SoundHeader
{
    public string assetName { get; set; }

    public void Parse(SoundProto proto)
    {

    }

    public SoundProto ConvertToProto()
    {
        SoundProto pd = new SoundProto();

        return pd;
    }
}

#endregion

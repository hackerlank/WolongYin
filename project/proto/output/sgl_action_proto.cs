//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: sgl_action_proto.proto
// Note: requires additional types generated from: common_proto.proto
namespace ProtoBuf
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UnitActionSetupProto")]
  public partial class UnitActionSetupProto : global::ProtoBuf.IExtensible
  {
    public UnitActionSetupProto() {}
    
    private readonly global::System.Collections.Generic.List<ProtoBuf.UnitActionProto> _ProtoList = new global::System.Collections.Generic.List<ProtoBuf.UnitActionProto>();
    [global::ProtoBuf.ProtoMember(1, Name=@"ProtoList", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.UnitActionProto> ProtoList
    {
      get { return _ProtoList; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UnitActionProto")]
  public partial class UnitActionProto : global::ProtoBuf.IExtensible
  {
    public UnitActionProto() {}
    
    private int _roleID = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"roleID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int roleID
    {
      get { return _roleID; }
      set { _roleID = value; }
    }
    private int _idleState = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"idleState", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int idleState
    {
      get { return _idleState; }
      set { _idleState = value; }
    }
    private readonly global::System.Collections.Generic.List<ProtoBuf.ActionStateProto> _actions = new global::System.Collections.Generic.List<ProtoBuf.ActionStateProto>();
    [global::ProtoBuf.ProtoMember(11, Name=@"actions", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.ActionStateProto> actions
    {
      get { return _actions; }
    }
  
    private readonly global::System.Collections.Generic.List<ProtoBuf.AttackDefProto> _atkDefList = new global::System.Collections.Generic.List<ProtoBuf.AttackDefProto>();
    [global::ProtoBuf.ProtoMember(12, Name=@"atkDefList", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.AttackDefProto> atkDefList
    {
      get { return _atkDefList; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ActionStateProto")]
  public partial class ActionStateProto : global::ProtoBuf.IExtensible
  {
    public ActionStateProto() {}
    
    private string _stateName = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"stateName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string stateName
    {
      get { return _stateName; }
      set { _stateName = value; }
    }
    private int _stateID = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"stateID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int stateID
    {
      get { return _stateID; }
      set { _stateID = value; }
    }
    private float _stateTime = default(float);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"stateTime", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float stateTime
    {
      get { return _stateTime; }
      set { _stateTime = value; }
    }
    private int _nextStateID = (int)0;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"nextStateID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int nextStateID
    {
      get { return _nextStateID; }
      set { _nextStateID = value; }
    }
    private readonly global::System.Collections.Generic.List<ProtoBuf.AnimSlotProto> _slotList = new global::System.Collections.Generic.List<ProtoBuf.AnimSlotProto>();
    [global::ProtoBuf.ProtoMember(11, Name=@"slotList", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.AnimSlotProto> slotList
    {
      get { return _slotList; }
    }
  
    private readonly global::System.Collections.Generic.List<ProtoBuf.GameEventProto> _eventList = new global::System.Collections.Generic.List<ProtoBuf.GameEventProto>();
    [global::ProtoBuf.ProtoMember(12, Name=@"eventList", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.GameEventProto> eventList
    {
      get { return _eventList; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AnimSlotProto")]
  public partial class AnimSlotProto : global::ProtoBuf.IExtensible
  {
    public AnimSlotProto() {}
    
    private string _animName = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"animName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string animName
    {
      get { return _animName; }
      set { _animName = value; }
    }
    private float _startTime = (float)0;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"startTime", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue((float)0)]
    public float startTime
    {
      get { return _startTime; }
      set { _startTime = value; }
    }
    private float _endTime = (float)100;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"endTime", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue((float)100)]
    public float endTime
    {
      get { return _endTime; }
      set { _endTime = value; }
    }
    private int _weight = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"weight", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int weight
    {
      get { return _weight; }
      set { _weight = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AttackDefProto")]
  public partial class AttackDefProto : global::ProtoBuf.IExtensible
  {
    public AttackDefProto() {}
    
    private string _attackDefName = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"attackDefName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string attackDefName
    {
      get { return _attackDefName; }
      set { _attackDefName = value; }
    }
    private int _attackDefID = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"attackDefID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int attackDefID
    {
      get { return _attackDefID; }
      set { _attackDefID = value; }
    }
    private float _triggerTime = default(float);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"triggerTime", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float triggerTime
    {
      get { return _triggerTime; }
      set { _triggerTime = value; }
    }
    private float _duration = default(float);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"duration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float duration
    {
      get { return _duration; }
      set { _duration = value; }
    }
    private ProtoBuf.AttackDefFxGroupProto _normalFx = null;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"normalFx", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.AttackDefFxGroupProto normalFx
    {
      get { return _normalFx; }
      set { _normalFx = value; }
    }
    private ProtoBuf.AttackDefFxGroupProto _critFx = null;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"critFx", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.AttackDefFxGroupProto critFx
    {
      get { return _critFx; }
      set { _critFx = value; }
    }
    private readonly global::System.Collections.Generic.List<ProtoBuf.GameEventProto> _eventList = new global::System.Collections.Generic.List<ProtoBuf.GameEventProto>();
    [global::ProtoBuf.ProtoMember(7, Name=@"eventList", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.GameEventProto> eventList
    {
      get { return _eventList; }
    }
  
    private readonly global::System.Collections.Generic.List<ProtoBuf.GameEventProto> _hitedEvents = new global::System.Collections.Generic.List<ProtoBuf.GameEventProto>();
    [global::ProtoBuf.ProtoMember(8, Name=@"hitedEvents", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.GameEventProto> hitedEvents
    {
      get { return _hitedEvents; }
    }
  
    private float _hitedTime = default(float);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"hitedTime", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float hitedTime
    {
      get { return _hitedTime; }
      set { _hitedTime = value; }
    }
    private ProtoBuf.HitDefProto _hitedData = null;
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"hitedData", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.HitDefProto hitedData
    {
      get { return _hitedData; }
      set { _hitedData = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HitDefProto")]
  public partial class HitDefProto : global::ProtoBuf.IExtensible
  {
    public HitDefProto() {}
    
    private float _triggerTime = default(float);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"triggerTime", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float triggerTime
    {
      get { return _triggerTime; }
      set { _triggerTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AttackDefFxGroupProto")]
  public partial class AttackDefFxGroupProto : global::ProtoBuf.IExtensible
  {
    public AttackDefFxGroupProto() {}
    
    private ProtoBuf.EffectProto _HitedEffect = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"HitedEffect", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.EffectProto HitedEffect
    {
      get { return _HitedEffect; }
      set { _HitedEffect = value; }
    }
    private ProtoBuf.SoundProto _HitedSound = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"HitedSound", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.SoundProto HitedSound
    {
      get { return _HitedSound; }
      set { _HitedSound = value; }
    }
    private ProtoBuf.EffectProto _SelfEffect = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"SelfEffect", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.EffectProto SelfEffect
    {
      get { return _SelfEffect; }
      set { _SelfEffect = value; }
    }
    private ProtoBuf.SoundProto _SelfSound = null;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"SelfSound", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.SoundProto SelfSound
    {
      get { return _SelfSound; }
      set { _SelfSound = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: sgl_battle_proto.proto
// Note: requires additional types generated from: common_proto.proto
namespace ProtoBuf
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BattleUnitProto")]
  public partial class BattleUnitProto : global::ProtoBuf.IExtensible
  {
    public BattleUnitProto() {}
    
    private string _Guid = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Guid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Guid
    {
      get { return _Guid; }
      set { _Guid = value; }
    }
    private int _TableID = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"TableID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int TableID
    {
      get { return _TableID; }
      set { _TableID = value; }
    }
    private bool _MainCommander = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"MainCommander", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool MainCommander
    {
      get { return _MainCommander; }
      set { _MainCommander = value; }
    }
    private float _HP = default(float);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"HP", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float HP
    {
      get { return _HP; }
      set { _HP = value; }
    }
    private float _Power = default(float);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"Power", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float Power
    {
      get { return _Power; }
      set { _Power = value; }
    }
    private int _MainTileIndex = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"MainTileIndex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int MainTileIndex
    {
      get { return _MainTileIndex; }
      set { _MainTileIndex = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _SkillList = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(7, Name=@"SkillList", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> SkillList
    {
      get { return _SkillList; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BattleFactionProto")]
  public partial class BattleFactionProto : global::ProtoBuf.IExtensible
  {
    public BattleFactionProto() {}
    
    private string _Guid = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Guid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Guid
    {
      get { return _Guid; }
      set { _Guid = value; }
    }
    private ProtoBuf.EBattleFactionType _FactionType = ProtoBuf.EBattleFactionType.FT_Player;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"FactionType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoBuf.EBattleFactionType.FT_Player)]
    public ProtoBuf.EBattleFactionType FactionType
    {
      get { return _FactionType; }
      set { _FactionType = value; }
    }
    private readonly global::System.Collections.Generic.List<ProtoBuf.BattleUnitProto> _UnitList = new global::System.Collections.Generic.List<ProtoBuf.BattleUnitProto>();
    [global::ProtoBuf.ProtoMember(3, Name=@"UnitList", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.BattleUnitProto> UnitList
    {
      get { return _UnitList; }
    }
  
    private int _FactionPower = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"FactionPower", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int FactionPower
    {
      get { return _FactionPower; }
      set { _FactionPower = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"StartBattleCmdReceive")]
  public partial class StartBattleCmdReceive : global::ProtoBuf.IExtensible
  {
    public StartBattleCmdReceive() {}
    
    private ProtoBuf.BattleFactionProto _PlayerFaction = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"PlayerFaction", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.BattleFactionProto PlayerFaction
    {
      get { return _PlayerFaction; }
      set { _PlayerFaction = value; }
    }
    private ProtoBuf.BattleFactionProto _EnemyFaction = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"EnemyFaction", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoBuf.BattleFactionProto EnemyFaction
    {
      get { return _EnemyFaction; }
      set { _EnemyFaction = value; }
    }
    private float _RandomSeed = default(float);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"RandomSeed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float RandomSeed
    {
      get { return _RandomSeed; }
      set { _RandomSeed = value; }
    }
    private int _SceneID = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"SceneID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int SceneID
    {
      get { return _SceneID; }
      set { _SceneID = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DoAttackCmd")]
  public partial class DoAttackCmd : global::ProtoBuf.IExtensible
  {
    public DoAttackCmd() {}
    
    private string _Guid = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Guid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Guid
    {
      get { return _Guid; }
      set { _Guid = value; }
    }
    private int _SkillID = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"SkillID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int SkillID
    {
      get { return _SkillID; }
      set { _SkillID = value; }
    }
    private ProtoBuf.EBattleFactionType _FactionType = ProtoBuf.EBattleFactionType.FT_Player;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"FactionType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoBuf.EBattleFactionType.FT_Player)]
    public ProtoBuf.EBattleFactionType FactionType
    {
      get { return _FactionType; }
      set { _FactionType = value; }
    }
    private int _RoundCount = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"RoundCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int RoundCount
    {
      get { return _RoundCount; }
      set { _RoundCount = value; }
    }
    private bool _IsCrit = default(bool);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"IsCrit", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool IsCrit
    {
      get { return _IsCrit; }
      set { _IsCrit = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"EBattleFactionType")]
    public enum EBattleFactionType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"FT_Player", Value=0)]
      FT_Player = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FT_Enemy", Value=1)]
      FT_Enemy = 1
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"EBattleResultType")]
    public enum EBattleResultType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"BR_Win", Value=0)]
      BR_Win = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BR_Lose", Value=1)]
      BR_Lose = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BR_Tie", Value=2)]
      BR_Tie = 2
    }
  
}
﻿using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;

public class BattleFaction
{
    private List<BattleUnit> mUnits = new List<BattleUnit>();
    private BattleFactionField mField = null;
    private BattleFactionProto mProtoData = null;
    private int mPower = 0;
    private SceneLoader mBindLoader = null;

    #region Get&Set
    public SceneLoader BindLoader
    {
        get { return mBindLoader; }
        set { mBindLoader = value; }
    }
    public List<BattleUnit> Units
    {
        get { return mUnits; }
    }

    public int Power
    {
        get { return mPower; }
        set { mPower = value; }
    }

    public ProtoBuf.BattleFactionProto ProtoData
    {
        get { return mProtoData; }
        private set { mProtoData = value; }
    }

    public BattleFactionField theField
    {
        get { return mField; }
        set { mField = value; }
    }
    #endregion

    public void Parse(BattleFactionProto proto)
    {
        ProtoData = proto;
        Power = ProtoData.FactionPower;

        Clear();
        _CreateUnits();
    }

    public void Clear()
    {
        for (int i = 0; i < Units.Count; ++i)
        {
            Units[i].Destroy();
        }

        Units.Clear();
        BindLoader = null;
    }

    public BattleUnit Find(string guid)
    {
        for (int i = 0; i < mUnits.Count; ++i)
        {
            BattleUnit ut = mUnits[i];

            if (ut.Guid == guid)
                return ut;
        }

        return null;
    }


    void _CreateUnits()
    {
        if (ProtoData == null || theField == null)
            return;

        for (int i = 0; i < ProtoData.UnitList.Count; i++)
        {
            BattleUnitProto data = ProtoData.UnitList[i];
            BattleUnit unit = BattleUnit.Create(data, theField.FactionType, BindLoader);
            if (unit == null)
                continue;

            BattleTile tile = theField.GetTile(data.MainTileIndex);
            if (tile == null)
                continue;

            tile.TheUnit = unit;

            Units.Add(unit);
        }

        Units.Sort((BattleUnit lhs, BattleUnit rhs) => { return lhs.theTile.Index.CompareTo(rhs.theTile.Index); });
    }
}
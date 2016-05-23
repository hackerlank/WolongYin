using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public static class UnitActionHelper
{
    private static Dictionary<int, UnitActionProto> mUnitActionDatas = new Dictionary<int, UnitActionProto>();

    public static ActionStateProto FindState(int roleID, int stateID)
    {
        UnitActionProto proto = null;
        mUnitActionDatas.TryGetValue(roleID, out proto);
        if (proto != null)
        {
            return proto.actions.Find((value) => { return value.stateID == stateID; });
        }
        else
        {
            return null;
        }
    }

    public static AttackDefProto FindAttackDef(int roleID, int atkDefID)
    {
        UnitActionProto proto = null;
        mUnitActionDatas.TryGetValue(roleID, out proto);
        if (proto != null)
        {
            return proto.atkDefList.Find((value) => { return value.attackDefID == atkDefID; });
        }
        else
        {
            return null;
        }
    }

    public static UnitActionProto FindProto(int roleID)
    {
        UnitActionProto proto = null;
        mUnitActionDatas.TryGetValue(roleID, out proto);
        return proto;
    }
}
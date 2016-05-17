using UnityEngine;
using System.Collections.Generic;

public class BattleScene : GameSceneBase
{
    private BattleField mField = null;

    public BattleField theField
    {
        get { return mField; }
        private set { mField = value; }
    }

    protected override void _OnSceneWasLoaded()
    {
        theField = GameObject.FindObjectOfType<BattleField>();
        if (theField == null)
        {
            Logger.instance.Error("can't found battle field! scene : {0}\n", table.baseid);
            return;
        }
    }
}
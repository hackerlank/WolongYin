using UnityEngine;
using System.Collections.Generic;

public class SkillActionController : ActionStateController
{
    private BattleUnit mMainUnit = null;

    #region Get&Set
    public BattleUnit MainUnit
    {
        get { return mMainUnit; }
        set { mMainUnit = value; }
    }
    #endregion
}
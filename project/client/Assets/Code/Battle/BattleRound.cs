using UnityEngine;
using System.Collections.Generic;


public class BattleRound : IPoolable
{
    private int mCurTurns = 0;
    private List<BattleUnit> mRedUnits = new List<BattleUnit>();
    private List<BattleUnit> mBlueUnits = new List<BattleUnit>(); 

    public void OnUpdate(float deltaTime)
    { }

    public void Reset()
    {
        mCurTurns = 0;
    }

    void IPoolable.Create()
    {
    }

    void IPoolable.New()
    {
    }

    void IPoolable.Delete()
    {
        Reset();
    }
}
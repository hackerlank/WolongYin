using UnityEngine;
using System.Collections.Generic;

public static class BattleFormula
{
    public static bool CalcCrit(BattleUnit unit)
    {
        return Random.Range(0f, 1f) <= 0.2f;
    }
}
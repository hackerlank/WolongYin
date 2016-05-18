using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class BattleField : SingletonMonoBehavior<BattleField>
{
    public BattleFactionField PlayerField = null;
    public BattleFactionField EnemyField = null;
    public int Rows = 3;
    public int Cols = 3;
    public float GridWidth = 1f;
    public float GridHeight = 1f;

    //630 036
    //741 147
    //852 258
    public void Create()
    {
        if (PlayerField != null)
            Utility.Destroy(PlayerField.gameObject);

        if (EnemyField != null)
            Utility.Destroy(EnemyField.gameObject);

        GameObject pgo = new GameObject("_PlayerFaction");
        Utility.SetIdentityChild(this.gameObject, pgo);

        BattleFactionField pbf = pgo.AddComponent<BattleFactionField>();
        pbf.FactionType = EBattleFactionType.FT_Player;
        pbf.Cols = Cols;
        pbf.Rows = Rows;
        pbf.GridWidth = GridWidth;
        pbf.GridHeight = GridHeight;
        pbf.Create(pgo);

        PlayerField = pbf;

        GameObject ego = new GameObject("_EnemyFaction");
        Utility.SetIdentityChild(this.gameObject, ego);

        BattleFactionField ebf = ego.AddComponent<BattleFactionField>();
        ebf.FactionType = EBattleFactionType.FT_Enemy;
        ebf.Cols = Cols;
        ebf.Rows = Rows;
        ebf.GridWidth = GridWidth;
        ebf.GridHeight = GridHeight;
        ebf.Create(ego);

        EnemyField = ebf;
    }
}
using UnityEngine;
using System.Collections.Generic;


public class BattleField : MonoBehaviour
{
    public List<BattleTile> Tiles = new List<BattleTile>(); 
    public GameDef.EBattleFaction Faction = GameDef.EBattleFaction.player;
    public int Rows = 3;
    public int Cols = 3;
    public float GridWidth = 0.5f;
    public float GridHeight = 0.5f;
}
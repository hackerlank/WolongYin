using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class BattleFactionField : MonoBehaviour
{
    public EBattleFactionType FactionType = EBattleFactionType.FT_Player;
    public int Rows = 3;
    public int Cols = 3;
    public float GridWidth = 0.5f;
    public float GridHeight = 0.5f;
    public List<BattleTile> Tiles = new List<BattleTile>();

    public void Clear()
    {
        for (int i = 0; i < Tiles.Count; ++i)
        {
            Utility.Destroy(Tiles[i].gameObject);
        }
        Tiles.Clear();
    }

    public void Create(GameObject rootGo)
    {
        Clear();

        for (int i = 0; i < Rows; ++i)
        {
            for (int j = 0; j < Cols; ++j)
            {
                GameObject tgo = new GameObject(string.Format("_tile[{0},{1}]", i, j));
                Utility.SetIdentityChild(rootGo, tgo);

                BattleTile tile = Utility.ScriptGet<BattleTile>(tgo);
                tile.Index = i * Cols + j;
                tile.theField = this;

                float x = j * GridWidth + GridWidth / 2;
                float z = i * GridHeight + GridHeight / 2;

                tgo.transform.localPosition = new Vector3(x, 0f, z);
                tgo.transform.localRotation = Quaternion.identity;
                tgo.transform.localScale = Vector3.one;

                Tiles.Add(tile);
            }
        }
    }


    public BattleTile GetTile(int index)
    {
        if (index < 0 || index >= Tiles.Count)
            return null;

        return Tiles[index];
    }

    public BattleTile GetTile(int row, int col)
    {
        int idx = row * Cols + col;
        return GetTile(idx);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = FactionType == EBattleFactionType.FT_Player ? Color.green : Color.red;
        foreach (var battleTile in Tiles)
        {
            Gizmos.DrawWireCube(battleTile.transform.position, new Vector3(GridWidth, 0f, GridHeight));
        }
    }
}
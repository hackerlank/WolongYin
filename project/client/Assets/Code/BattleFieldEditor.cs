using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BattleField))]
public class BattleFieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();

        BattleField bf = this.target as BattleField;

        if (GUILayout.Button("Create"))
        {
            bf.Create();
        }
    }
}
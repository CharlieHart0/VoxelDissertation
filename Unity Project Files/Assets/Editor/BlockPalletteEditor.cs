using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockPallette))]
public class BlockPalletteEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add Block entry"))
        {
            BlockPallette blockPallette = (BlockPallette)target;
            blockPallette.addEntry();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputHillMax : MenuInputFloat
{
    public WorldGeneration worldGen;
    private void Start()
    {
        inputField.text = worldGen.hillMaxHeight.ToString();
    }
}

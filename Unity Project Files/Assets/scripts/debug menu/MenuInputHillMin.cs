using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputHillMin : MenuInputFloat
{
    public WorldGeneration worldGen;
    private void Start()
    {
        inputField.text = worldGen.valleyMinHeight.ToString();
    }
}

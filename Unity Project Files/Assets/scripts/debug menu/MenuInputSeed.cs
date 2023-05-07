using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputSeed : MenuInputFloat
{
    public void Start()
    {
        inputField.text = WorldSettings.Instance.seedInt.ToString();
    }
    
}

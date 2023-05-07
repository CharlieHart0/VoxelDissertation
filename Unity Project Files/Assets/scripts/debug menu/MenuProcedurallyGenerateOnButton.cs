using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuProcedurallyGenerateOnButton : MenuInputBool
{
    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = WorldSettings.Instance.generateTerrainOnQPress;
    }

   
}

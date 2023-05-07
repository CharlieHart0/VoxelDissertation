using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenugenerateBlocksAsCameraMoves : MenuInputBool
{
    private void Start()
    {
        toggle.isOn = WorldSettings.Instance.generateTerrainAsCameraMoves;
    }
}

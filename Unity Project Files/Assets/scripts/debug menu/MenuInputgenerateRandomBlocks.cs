using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputgenerateRandomBlocks : MenuInputBool
{
    private void Start()
    {
        toggle.isOn = WorldSettings.Instance.generateRandomBlocks;
    }
}

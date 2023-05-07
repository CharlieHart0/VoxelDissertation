using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputRandomSeed : MenuInputBool
{
    private void Start()
    {
        toggle.isOn = WorldSettings.Instance.useRandomSeed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuobjectPoolingPillars : MenuInputBool
{
    private void Start()
    {
        toggle.isOn = WorldSettings.Instance.objectPoolingPillars;
    }
}

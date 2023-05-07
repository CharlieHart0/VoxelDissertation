using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputCullInternal : MenuInputBool
{
    private void Start()
    {
        toggle.isOn = WorldSettings.Instance.cullHiddenVoxelFaces;
    }
}

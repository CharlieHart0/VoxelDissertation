using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputUseWorldGen : MenuInputBool
{
    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = WorldSettings.Instance.useChunkInputBlocks;
    }

}

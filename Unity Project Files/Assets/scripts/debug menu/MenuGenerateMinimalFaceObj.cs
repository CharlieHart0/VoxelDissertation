using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGenerateMinimalFaceObj : MenuInputBool
{
    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = WorldSettings.Instance.generateMinimalFaceObjects;
    }
}

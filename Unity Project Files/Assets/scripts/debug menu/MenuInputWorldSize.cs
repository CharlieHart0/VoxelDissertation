using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputWorldSize : MenuInputVec3
{
    private void Start()
    {
        x.text = WorldSettings.Instance.renderedWorldSizeinChunks.x.ToString();
        y.text = WorldSettings.Instance.renderedWorldSizeinChunks.y.ToString();
        z.text = WorldSettings.Instance.renderedWorldSizeinChunks.z.ToString();
    }
}

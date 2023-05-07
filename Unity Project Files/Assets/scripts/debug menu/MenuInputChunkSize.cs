using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputChunkSize : MenuInputVec3
{
    private void Start()
    {
        x.text = WorldSettings.Instance.chunkSizeinVoxels.x.ToString();
        y.text = WorldSettings.Instance.chunkSizeinVoxels.y.ToString();
        z.text = WorldSettings.Instance.chunkSizeinVoxels.z.ToString();
    }

}

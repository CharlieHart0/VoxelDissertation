using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class enableDepthTexture : MonoBehaviour
{
    private void OnEnable()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
}

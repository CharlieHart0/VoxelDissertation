using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class occlusionCullingCameras : MonoBehaviour
{
    Camera HZBcamera;
    GameObject HZBcamObject;
    public RenderTexture hzbTexture;
    public LayerMask cullingMask;
    public Material testMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
        hzbTexture = new RenderTexture(
            Screen.width,
            Screen.height,
            0,
            RenderTextureFormat.RFloat
        );
        hzbTexture.filterMode = FilterMode.Point;
        hzbTexture.useMipMap = true;
        hzbTexture.autoGenerateMips = true;
        hzbTexture.format = RenderTextureFormat.RGFloat;

        HZBcamObject = new GameObject();

        HZBcamObject.transform.SetParent(this.gameObject.transform, false);
        HZBcamObject.transform.localPosition = Vector3.zero;
        HZBcamObject.transform.localEulerAngles = Vector3.zero;

        HZBcamera = HZBcamObject.AddComponent<Camera>();


        HZBcamera.fieldOfView = Camera.main.fieldOfView;
        HZBcamera.targetTexture = hzbTexture;
        HZBcamera.depthTextureMode = DepthTextureMode.Depth;
        HZBcamera.enabled = false;
        HZBcamera.orthographic = Camera.main.orthographic;
        HZBcamera.nearClipPlane = Camera.main.nearClipPlane;
        HZBcamera.farClipPlane = Camera.main.farClipPlane;
        HZBcamera.cullingMask = cullingMask;
        HZBcamera.backgroundColor = Color.green;
        HZBcamera.clearFlags = CameraClearFlags.SolidColor;

        testMaterial.SetTexture("_CamDepthTex", hzbTexture);
       
    }

    private void Update()
    {
        HZBcamera.Render();
    }



}

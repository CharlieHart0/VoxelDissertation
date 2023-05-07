using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script was used in prototyping, and is not used anywhere in the final product
public class testFrustumCulling : MonoBehaviour
{
    [SerializeField]
    MeshRenderer r;

    [SerializeField]
    Vector3 v;

    [SerializeField]
    bool b;
    
    bool isInViewFrustum(Vector3 worldPos)
    {
       Vector3 ScreenPoint = Camera.main.WorldToScreenPoint(worldPos);


        // return not (too far left or right or down or up or behind camera)
        return !(
             // too far left
             ScreenPoint.x < 0 ||
            // too far right
            ScreenPoint.x > Screen.currentResolution.width ||
            // too far down
            ScreenPoint.y < 0 ||
            // too far up
            ScreenPoint.y > Screen.currentResolution.height ||
            // behind camera near clip plane
            ScreenPoint.z < Camera.main.nearClipPlane
            );
       ;
    }

    // Update is called once per frame
    void Update()
    {
        v = Camera.main.WorldToScreenPoint(this.transform.position);
        b = isInViewFrustum(this.transform.position);

        r.enabled = b;
    }
}

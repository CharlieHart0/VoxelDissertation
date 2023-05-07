using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// class contains some useful functions
public static class HelpfulFunctions
{

    // returns true if point (world space) is inside camera view frustum, false if not
    public static bool isInViewFrustum(Vector3 worldPos)
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
}

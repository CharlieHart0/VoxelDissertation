using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public WorldGeneration gen;
    public Camera camera;
    public float moveSpeed;
    public float lookSpeed;
    [Range(0, 10f)]
    [SerializeField]
    float cameraSensitivityY;

    [Range(0, 10f)]
    [SerializeField]
    float cameraSensitivityX;

    [Range(0, 90f)]
    [SerializeField]
    float maxXRotation;

    [Range(0, -90f)]
    [SerializeField]
    float minXRotation;

    float sqrt05;

    float chunkXSize;
    float chunkZSize;

    
    void Awake()
    {
        sqrt05 = Mathf.Sqrt(0.5f);
        chunkXSize = WorldSettings.Instance.voxelSize.x * WorldSettings.Instance.chunkSizeinVoxels.x;
        chunkZSize = WorldSettings.Instance.voxelSize.z * WorldSettings.Instance.chunkSizeinVoxels.z;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        cameraRotation();

        // if generate terrain only on keypress setting is enabled, update terrain only on press of Q key
        if (!WorldSettings.Instance.generateTerrainOnQPress || (WorldSettings.Instance.generateTerrainOnQPress && Input.GetKeyDown(KeyCode.Q)))
        {
            updateTerrain();
        }
    }

    void cameraRotation()
    {
        Vector3 startingRot;
        // get rotation of camera at start of frame
        startingRot = camera.transform.localEulerAngles;

        // get x and y angles to rotate camera by, times by sensitivity

        float angleY = Input.GetAxis("Mouse Y") * cameraSensitivityY;
        float angleX = Input.GetAxis("Mouse X") * cameraSensitivityX;

        // make X rotation (y axis camera movement) within limits

        float xRot = startingRot.x - angleY;
        if (xRot < (360 + minXRotation) && xRot > 180f)   // if x rotation is too low (negative rotation, because -1 = 359)
        {
            //Debug.Log("upper cam limit" + xRot);
            xRot = minXRotation;
        }
        if (xRot > maxXRotation && xRot < 180f) // if x rotation is too high
        {
            //Debug.Log("lower cam limit" + xRot);
            xRot = maxXRotation;
        }

        //perform camera rotation
        camera.transform.localEulerAngles = new Vector3(xRot, startingRot.y + angleX, 0f);
    }
    void FixedUpdate()
    {
        float z = 0;
        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.W))
        {
            z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            z -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x += 1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            y += 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            y -= 1;
        }

        if (!((x * z) == 0)) // if not (one or more of them is zero), or if both keys are being pressed
        {
            // times both by  root 0.5  , so that the magnitude of the movement is 1
            x *= sqrt05;
            z *= sqrt05;
        }

        Vector3 Zmovement = transform.forward * z * moveSpeed;
        Vector3 Xmovement = transform.right * x * moveSpeed;
        Vector3 Ymovement = transform.up * y * moveSpeed;

        this.transform.position += (Xmovement + Zmovement + Ymovement);
    }

    public void updateTerrain()
    {
        

        // origin of pillar the camera is currently in
        Vector3 currentPillarPos = gen.worldPosToPillarOrigin(this.transform.position);
        // as Vec2
        Vector2 currentPillarv2 = new Vector2(currentPillarPos.x, currentPillarPos.z);

        // remove far away pillars first, so object pooling can be used for new generated pillars

        // if should remove pillars outside of the render area
        if (WorldSettings.Instance.removeTerrainAsCameraMoves)
        {
            List<Vector2> pillarsToKill = new();
            // for all instantiated pillars
            foreach(KeyValuePair<Vector2,Pillar> pillar in gen.pillars)
            {
                
                // if location is outside current render area, remove it
                if(Mathf.Abs(pillar.Key.x - currentPillarv2.x) > (WorldSettings.Instance.renderedWorldSizeinChunks.x * chunkXSize))
                {
                    pillarsToKill.Add(pillar.Key);

                }
                else if (Mathf.Abs(pillar.Key.y - currentPillarv2.y) > (WorldSettings.Instance.renderedWorldSizeinChunks.z * chunkZSize))
                {
                    pillarsToKill.Add(pillar.Key);
                }
            }

            foreach(Vector2 v2 in pillarsToKill)
            {
                gen.killPillar(new Vector3(v2.x,0,v2.y));
            }
        }

        // if should generate terrain as camera moves into new area
        if (WorldSettings.Instance.generateTerrainAsCameraMoves)
        {
            // for all pillar positions in the render area
            for (float x = currentPillarv2.x - (WorldSettings.Instance.renderedWorldSizeinChunks.x * chunkXSize);
                x <= currentPillarv2.x + (chunkXSize * WorldSettings.Instance.renderedWorldSizeinChunks.x);
                x += chunkXSize)
            {
                for (float z = currentPillarv2.y - (WorldSettings.Instance.renderedWorldSizeinChunks.z * chunkZSize);
                z <= currentPillarv2.y + (chunkZSize * WorldSettings.Instance.renderedWorldSizeinChunks.z);
                z += chunkZSize)
                {
                    // if pillar does not exist yet
                    if (!gen.pillars.ContainsKey(new Vector2(x, z)))
                    {
                        gen.generateNewPillar(new Vector3(x, 0, z));
                    }
                }
            }
        }

    }

}

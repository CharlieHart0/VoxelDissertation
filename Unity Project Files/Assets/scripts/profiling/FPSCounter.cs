using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// this component drives an fps counter ui element
public class FPSCounter : MonoBehaviour
{
    TextMeshProUGUI counter;
    [SerializeField] string prefix = "FPS: ";
    [SerializeField] int counterUpdatesPerSecond = 5;
    float timeSinceLastUpdate = 0f;
    [SerializeField] Gradient gradient;
    [SerializeField] float targetFps = 60f;
    [SerializeField] WorldGeneration gen;

    [Header("Object pooling debug counters")]
    [SerializeField]
    TextMeshProUGUI pPool;
    [SerializeField]
    TextMeshProUGUI nPool;
    [SerializeField]
    TextMeshProUGUI sPool;
    [SerializeField]
    TextMeshProUGUI ePool;
    [SerializeField]
    TextMeshProUGUI wPool;
    [SerializeField]
    TextMeshProUGUI uPool;
    [SerializeField]
    TextMeshProUGUI dPool;


    private void Start()
    {
         counter = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // if time since counter was last updated is more than (1/update rate), update
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate > Mathf.Pow(counterUpdatesPerSecond, -1f))
        {
            timeSinceLastUpdate = 0f;
            updateCounter();
        }
    }

    // updates the value on the counter
    void updateCounter()
    {
        // calculates the fps as the multiplicative inverse of time taken to render last frame, and rounds to integer
        int fps = Mathf.RoundToInt(Mathf.Pow(Time.deltaTime, -1f));
        counter.text = prefix + fps.ToString();

        // sets colour of the counter to represent how close the fps is to the target fps
        counter.color = gradient.Evaluate(fps / targetFps);

        poolCounters();
    }

    void poolCounters()
    {
        if (WorldSettings.Instance.objectPoolingPillars)
        {
            pPool.text = "P Pool: " + gen.pillarPool.Count.ToString();
        }
        else
        {
            pPool.text = "";
        }

        if (WorldSettings.Instance.objectPoolingVoxelFaces)
        {
            nPool.text = "N Pool: " + gen.facePoolDict[VoxelFace.FaceDirection.North].Count.ToString();
            sPool.text = "S Pool: " + gen.facePoolDict[VoxelFace.FaceDirection.South].Count.ToString();
            ePool.text = "E Pool: " + gen.facePoolDict[VoxelFace.FaceDirection.East].Count.ToString();
            wPool.text = "W Pool: " + gen.facePoolDict[VoxelFace.FaceDirection.West].Count.ToString();
            uPool.text = "U Pool: " + gen.facePoolDict[VoxelFace.FaceDirection.Up].Count.ToString();
            dPool.text = "D Pool: " + gen.facePoolDict[VoxelFace.FaceDirection.Down].Count.ToString();
        }
        else
        {
            nPool.text = "";
            sPool.text = "";
            ePool.text = "";
            wPool.text = "";
            uPool.text = "";
            dPool.text = "";
        }
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : MonoBehaviour
{
    public static WorldSettings Instance;

    
    [Header("Enable/Disable Features")] // these properties can be used to enable and disable implemented features, when evaluating their impact on performance

    [Tooltip("Do not render internal voxel faces which contact a non transparent voxel")]
    public bool cullHiddenVoxelFaces = true;

    [Tooltip("Cull faces which are not fully or partially within the view frustum of the main camera")]
    public bool viewFrustumCulling = false;

    [Tooltip("Cull faces which are not fully or partially within the view frustum of the main camera - on a chunk by chunk basis")]
    public bool viewFrustumCullingChunkwise = false;

    public bool viewFrustumCullingColliders = true; // view frustum culling using colliders

    [Tooltip("World Generation generates random blocks in random positions. For debugging only")]
    public bool generateRandomBlocks = true;

    [Tooltip("Generate a random seed each time the game runs")]
    public bool useRandomSeed;

    [Tooltip("Use world Generation defined in WorldGeneration Class")]
    public bool useChunkInputBlocks;

    [Tooltip("Use Voxel based Lighting")]
    public bool useVoxelLighting = true;



    [Header("World Dimensions")]
    public Vector3 voxelSize = new Vector3(1f, 1f, 1f);

    public Vector3 chunkSizeinVoxels = new Vector3(16, 16, 16);


    // the size around the camera which is to be rendered/generated. y value is the world height in chunks.
    public Vector3 renderedWorldSizeinChunks = new(5, 4, 5);

    [Header("World Generation Settings")]
    public int seedInt;
    public bool generateMinimalFaceObjects = true;
    public bool generateTerrainAsCameraMoves = true;
    public bool removeTerrainAsCameraMoves = true;
    public bool objectPoolingPillars = true;
    public bool objectPoolingVoxelFaces = true;
    public bool useHillCurve = true;
    public bool cavesNarrowAtSurface = true;
    public bool generateTerrainOnQPress = false;

    [Header("Lighting Settings")]
    public int sunMaxStrength = 16;

    


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        
        // if needs to generate random seed, or if given seed is too big to be used
        if (useRandomSeed || seedInt > 9999999)
        {
            seedInt = Random.Range(0, 9999999);
        }

        
    }
}

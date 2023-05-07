using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public GameObject pillarPrefab;
    public Testing testingClass;

    [Header("Hill Generation")]
    public short hillMaxHeight = 64;
    public short valleyMinHeight = 40;
    public float hillNoiseScale = 1f;
    public AnimationCurve hillCurve;

    [Header("Cave Carving")]
    public float caveThreshold = 0.6f;
    public float caveScale = 0.03f;
    // the curve which controls the rate at which the caves narrow as they reach the narrowing region at the surface
    public AnimationCurve caveNarrowingAtSurfaceCurve;
    // the size of the area under the surface in which the caves will become narrower as they reach the surface
    public float caveNarrowingAtSurfaceSize = 8f;


    [Header("Block References")]
    public Block rockBlock;
    public Block grassyEarthBlock;
    public Block earthBlock;
    public Block airBlock;
    public Block rockBumpyBlock;
    public Block rockLavaBlock;

    // counter for debugging, counts how much time it has taken to generate voxels
    public double timeTakenForVoxels = 0f;

    [Header("Object references")]

    public MeshCollider cameraCollider;

    public GameObject voxelFacePool;

    // dictionary which holds lists of face object pools for each orientation of face
    public Dictionary<VoxelFace.FaceDirection, List<VoxelFace>> facePoolDict = new(); 


    // vector2 is world position of pillar, xz.   y value does not need to be included, as a pillar is a tall structure
    public Dictionary<Vector2, Pillar> pillars = new();

    // holds all pooled pillars which are to be used for object pooling of pillars.
    public List<Pillar> pillarPool = new();

    public Pillar getPillar(Vector2 pillarPos)
    {
        if (!pillars.ContainsKey(pillarPos))
        {
            return null;
        }
        else
        {
            return pillars[pillarPos];
        }
    }
    public void Awake()
    {
        UnityEngine.Random.InitState(WorldSettings.Instance.seedInt);

        facePoolDict.Add(VoxelFace.FaceDirection.North, new List<VoxelFace>());
        facePoolDict.Add(VoxelFace.FaceDirection.South, new List<VoxelFace>());
        facePoolDict.Add(VoxelFace.FaceDirection.East, new List<VoxelFace>());
        facePoolDict.Add(VoxelFace.FaceDirection.West, new List<VoxelFace>());
        facePoolDict.Add(VoxelFace.FaceDirection.Up, new List<VoxelFace>());
        facePoolDict.Add(VoxelFace.FaceDirection.Down, new List<VoxelFace>());
    }

    private void Update()
    {
        // generates and removes a pillar at specific location to test rendering
        if (Input.GetKeyDown(KeyCode.G))
        {
            generateNewPillar(new Vector3(64, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            killPillar(new Vector3(64, 0, 0));
        }
    }

    public void killPillar(Vector3 _pos)
    {
        Vector2 _posv2 = new Vector2(_pos.x, _pos.z);
        // if position was given incorrectly, snap it to pillar origin point
        if (!pillars.ContainsKey(_posv2))
        {
            _posv2 = new Vector2(worldPosToPillarOrigin(_pos).x, worldPosToPillarOrigin(_pos).z);
        }

        // try to kill the pillar
        if (pillars.ContainsKey(_posv2))
        {
            // if object pooling enabled for pillars, hide it and add to pillarPool, else destroy it-
            if (WorldSettings.Instance.objectPoolingPillars)
            {
               
                
                    
                    foreach(KeyValuePair<Vector3,Chunk> c in pillars[_posv2].data.chunks)
                    {
                        foreach(KeyValuePair<Vector3,Voxel> v in c.Value.data.voxels)
                        {
                        // unrender all faces, and re set block to air as default
                            v.Value.unRenderVoxel();
                            v.Value.data.block = airBlock;
                        }
                    }
                
                pillars[_posv2].gameObject.SetActive(false);
                pillarPool.Add(pillars[_posv2]);
            }
            else
            {
                Destroy(pillars[_posv2].gameObject);
            }
           
            pillars.Remove(_posv2);

        }
        else
        {
            Debug.Log("cannot remove pillar " + _posv2.ToString());
            return;
        }


        // X- West  X+ East
        // Z- South Z+ North
        // Y- Down Y+ Up 

        // if there is a west pillar from the killed one
        if (pillars.ContainsKey(new Vector2(_posv2.x - WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x, _posv2.y)))
        {
            foreach(KeyValuePair<Vector3,Chunk> chunkKVP in pillars[new Vector2(
                _posv2.x - WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x, _posv2.y)].data.chunks)
            {
                
                foreach(KeyValuePair<Vector3,Voxel> voxelKVP in chunkKVP.Value.data.voxels){
                    // if voxel is on the east face of the chunk, re render it
                    if(voxelKVP.Key.x == _posv2.x -1)
                    {
                        // remove reference to now deleted voxel
                        // important that unrenders before removing reference, or voxel face will not be hidden
                        voxelKVP.Value.unRenderVoxel();
                        voxelKVP.Value.data.eastVoxel = null;
                        voxelKVP.Value.renderVoxel();
                    } 
                }
            }
        }

        // if there is an east pillar from the killed one
        if (pillars.ContainsKey(new Vector2(_posv2.x + WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x, _posv2.y)))
        {
            foreach (KeyValuePair<Vector3, Chunk> chunkKVP in pillars[new Vector2(_posv2.x +
                WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x, _posv2.y)].data.chunks)
            {
                
                foreach (KeyValuePair<Vector3, Voxel> voxelKVP in chunkKVP.Value.data.voxels)
                {
                    // if voxel is on the west face of the chunk, re render it
                    if (voxelKVP.Key.x == _posv2.x + (WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x))
                    {
                        // remove reference to now deleted voxel
                        voxelKVP.Value.unRenderVoxel();
                        voxelKVP.Value.data.westVoxel = null;
                        voxelKVP.Value.renderVoxel();
                    }
                }
            }
        }

        // if there is a south pillar from the killed one
        if (pillars.ContainsKey(new Vector2(_posv2.x, _posv2.y - WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z)))
        {
            foreach (KeyValuePair<Vector3, Chunk> chunkKVP in pillars[new Vector2(_posv2.x, _posv2.y - WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z)].data.chunks)
            {
                
                foreach (KeyValuePair<Vector3, Voxel> voxelKVP in chunkKVP.Value.data.voxels)
                {
                    // if voxel is on the north face of the chunk, re render it
                    if (voxelKVP.Key.z == _posv2.y - 1 )
                    {
                        // remove reference to now deleted voxel
                        
                        voxelKVP.Value.unRenderVoxel();
                        voxelKVP.Value.data.northVoxel = null;
                        voxelKVP.Value.renderVoxel();
                    }
                }
            }
        }

        // if there is a north pillar from the killed one
        if (pillars.ContainsKey(new Vector2(_posv2.x, _posv2.y + WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z)))
        {
            foreach (KeyValuePair<Vector3, Chunk> chunkKVP in pillars[new Vector2(_posv2.x, _posv2.y + WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z)].data.chunks)
            {
                
                foreach (KeyValuePair<Vector3, Voxel> voxelKVP in chunkKVP.Value.data.voxels)
                {
                    // if voxel is on the south face of the chunk, re render it
                    if (voxelKVP.Key.z == _posv2.y + (WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x))
                    {
                        // remove reference to now deleted voxel


                        voxelKVP.Value.unRenderVoxel();
                        voxelKVP.Value.data.southVoxel = null;
                        voxelKVP.Value.renderVoxel();
                    }
                }
            }
        }
    }

    public void generateNewPillar(Vector3 _pos)
    {
        double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
        Vector2 _posv2 = new Vector2(_pos.x, _pos.z);
        // if position was given incorrectly, snap it to pillar origin point
        if (!pillars.ContainsKey(_posv2))
        {
            _posv2 = new Vector2(worldPosToPillarOrigin(_pos).x, worldPosToPillarOrigin(_pos).z);
        }

        // if pillar already exists
        if (pillars.ContainsKey(_posv2))
        {
            return;
        }
        else
        {
            GameObject spawnedPillar;
            bool pooled = false;
            // if using object pooling-
            if (WorldSettings.Instance.objectPoolingPillars && pillarPool.Count > 0)
            {                
                // get last pillar in pool list
                spawnedPillar  = pillarPool[pillarPool.Count - 1].gameObject;
                pillarPool.RemoveAt(pillarPool.Count - 1);
                spawnedPillar.gameObject.SetActive(true);
                pooled = true;

                //Debug.Log("pooled - getting pillar at time " + (DateTime.Now.TimeOfDay.TotalMilliseconds - startTime).ToString() + "ms");
            }
            else
            {
                // spawn pillar
                spawnedPillar = Instantiate(pillarPrefab, this.transform);                
            }

            spawnedPillar.transform.position = new Vector3(
                    _posv2.x
                    , 0,
                    _posv2.y);

            Pillar spawnedPillarComponent = spawnedPillar.GetComponent<Pillar>();
            spawnedPillarComponent.world = this;
            pillars.Add(_posv2, spawnedPillarComponent);

            if (!pooled)
            {
                // populate pillar with voxel objects
                spawnedPillarComponent.generation.generatePillar();
            }
            else
            {
                spawnedPillarComponent.data.resetChunkPositionRecords();

                // reset worldspace position records of all voxels in pooled pillar to their new location
                foreach(KeyValuePair<Vector3,Chunk> c in spawnedPillarComponent.data.chunks)
                {
                    c.Value.generation.resetVoxelPositionRecords();


                    // re evaluate block values for pillar (do world generation for new pillar position)
                    c.Value.data.generateBlocks();
                }

                //Debug.Log("pooled - resetting values and generating world at time " + (DateTime.Now.TimeOfDay.TotalMilliseconds - startTime).ToString() + "ms");

            }
            
            //set block value for all voxels (world generation)
            spawnedPillarComponent.generation.setBlocks();

            //Debug.Log("setting blocks at time " + (DateTime.Now.TimeOfDay.TotalMilliseconds - startTime).ToString() + "ms");

            // do find neighbours pass on all voxels            
            spawnedPillarComponent.generation.findNeighbouringVoxels();

            //Debug.Log("finding neighbours at time " + (DateTime.Now.TimeOfDay.TotalMilliseconds - startTime).ToString() + "ms");

            // render pillar
            spawnedPillarComponent.generation.renderPillar();


            string s = "";
            if (pooled)
            {
                s = "Pooled -- ";
            }
            Debug.Log(s+"Generating Pillar at "+ _pos.ToString()+" took " + (DateTime.Now.TimeOfDay.TotalMilliseconds - startTime).ToString() + "ms");

            testingClass.pillarLoadTimes.Add(DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);
        }
    }


    // generates the initial set of pillars for the world;
    public void generate()
    {
        timeTakenForVoxels = 0;
        double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
        double lastTime = startTime;

        // spawn empty pillar objects
        for (int x = 0; x < WorldSettings.Instance.renderedWorldSizeinChunks.x; x++)
        {
            for (int z = 0; z < WorldSettings.Instance.renderedWorldSizeinChunks.z; z++)
            {
                GameObject spawnedPillar = Instantiate(pillarPrefab, this.transform);
                spawnedPillar.transform.position = new Vector3(
                    x * WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x
                    , 0,
                    z * WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z);

                Pillar spawnedPillarComponent = spawnedPillar.GetComponent<Pillar>();
                spawnedPillarComponent.world = this;
                pillars.Add(new Vector2(spawnedPillar.transform.position.x, spawnedPillar.transform.position.z), spawnedPillarComponent);
            }
        }
        Debug.Log("Spawning Empty pillars took " + (DateTime.Now.TimeOfDay.TotalMilliseconds - lastTime).ToString() + "ms");
        lastTime = DateTime.Now.TimeOfDay.TotalMilliseconds;



        // populate pillars with voxels
        foreach (KeyValuePair<Vector2, Pillar> entry in pillars)
        {
            entry.Value.generation.generatePillar();
        }
        Debug.Log("populating pillars took " + (DateTime.Now.TimeOfDay.TotalMilliseconds - lastTime).ToString() + "ms");
        lastTime = DateTime.Now.TimeOfDay.TotalMilliseconds;

        Debug.Log("     - of which voxels took " + timeTakenForVoxels.ToString() + "ms");


        //set block value for all voxels (world generation)
        foreach (KeyValuePair<Vector2, Pillar> entry in pillars)
        {
            entry.Value.generation.setBlocks();
        }
        Debug.Log("setting blocks/worldgen took " + (DateTime.Now.TimeOfDay.TotalMilliseconds - lastTime).ToString() + "ms");
        lastTime = DateTime.Now.TimeOfDay.TotalMilliseconds;



        // do find neighbours pass on all voxels
        foreach (KeyValuePair<Vector2, Pillar> entry in pillars)
        {
            entry.Value.generation.findNeighbouringVoxels();
        }
        Debug.Log("finding Neighbour voxels took " + (DateTime.Now.TimeOfDay.TotalMilliseconds - lastTime).ToString() + "ms");
        lastTime = DateTime.Now.TimeOfDay.TotalMilliseconds;



        // render all pillars
        foreach (KeyValuePair<Vector2, Pillar> entry in pillars)
        {
            entry.Value.generation.renderPillar();
        }
        Debug.Log("rendering pillars took " + (DateTime.Now.TimeOfDay.TotalMilliseconds - lastTime).ToString() + "ms");
        lastTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
    }

    // given a world space position of a voxel, returns the block which should be there. (does all major world generation)
    public Block evaluateBlock(int x, int y, int z)
    {


        Block b = null;

        // coordinates transformed by the seed, to be used when a unique value is needed
        float xSeeded = x + WorldSettings.Instance.seedInt;
        float ySeeded = y + WorldSettings.Instance.seedInt;
        float zSeeded = z + WorldSettings.Instance.seedInt;


        #region hill

            // how far between valley and hill is this block? between 0 and 1
            float _hillHeightNormalised = Mathf.PerlinNoise(xSeeded * hillNoiseScale, zSeeded * hillNoiseScale);


        // if hill curve is used to change profile of hills
        if (WorldSettings.Instance.useHillCurve)
        {
            _hillHeightNormalised = hillCurve.Evaluate(_hillHeightNormalised);
        }

        // the maximum height of that current coordinate
        short _hillHeight = (short)(valleyMinHeight + (_hillHeightNormalised * (hillMaxHeight - valleyMinHeight)));

        // if the evaluated position is higher than the max hill height at that position
        if(y > _hillHeight)
        {
            
            return null;
        }
        // if at surface
        else if(y == _hillHeight){
            b = grassyEarthBlock;
        }
        else if(y<_hillHeight && y >= (_hillHeight - UnityEngine.Random.Range(2, 5)))
        {
            b = earthBlock;
        }
        // if below surface and earth layer
        else
        {
            b = rockBlock;
        }

        #endregion


        #region caves
        // cave carving
        if (y <= _hillHeight)
        {
            if (caveEvaluate(xSeeded, ySeeded, zSeeded,y, _hillHeight) && ! (y == 0))
            {
                b = airBlock;
            }
            if(y == 0)
            {
                b = rockLavaBlock;
            }
        }

        #endregion

        return b;
    }

    // evaluates cave generation and returns true if block at position should be carved out (turned to air)
    bool caveEvaluate(float xSeeded,float ySeeded,float zSeeded,float yTrue,float surfaceLevel)
    {

        // val = perlin1^2 + perlin2^2, where perlin2 is arbitrarily offset, essentially a sample using another seed, as unity's perlin noise system
        // Firstlydoes not support seeds in the conventional manner
        float val = Mathf.Pow(Perlin3D.PerlinNoise3D(xSeeded * caveScale, ySeeded * caveScale, zSeeded * caveScale), 2) +
            Mathf.Pow(Perlin3D.PerlinNoise3D((xSeeded -3000) * caveScale, (ySeeded + 4000) * caveScale, (zSeeded + 7000) * caveScale), 2);

        // lower cave threshold results in less air blocks being carved out, and more stone remaining
        float _caveThreshold =  caveThreshold;

        // if near the surface, perform narrowing on the cave, so that smaller chunks of surface are cut out
        if (yTrue >= surfaceLevel - caveNarrowingAtSurfaceSize && WorldSettings.Instance.cavesNarrowAtSurface)
        {
            // the relative height (between 0 and 1) of how far up the block is from the start of the narrowing zone to the surface
            float normHeightInNarrowingZone = (yTrue - (surfaceLevel - caveNarrowingAtSurfaceSize)) / caveNarrowingAtSurfaceSize;

            // cave threshold lower means less air, so as normheight is higher, we want the total to get lower, so less cave is carved out.
            _caveThreshold = caveThreshold - caveNarrowingAtSurfaceCurve.Evaluate(normHeightInNarrowingZone);
        }

        return (val <= _caveThreshold);

    }


    // takes a world position, and returns the origin of the pillar which should contain the object. not guarunteed to be an instantiated pillar;
    public Vector3 worldPosToPillarOrigin(Vector3 _worldPos)
    {
        float flooredX = Mathf.Floor(_worldPos.x / (WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x))
            * (WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x);

        float flooredZ = Mathf.Floor(_worldPos.z / (WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z)) 
            * (WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z);

        return new Vector3(flooredX, 0, flooredZ);
    }

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VoxelData))]
public class Voxel : MonoBehaviour
{
    
    public float xSize = 1;
    public float ySize = 1;
    public float zSize = 1; 
    [HideInInspector]
    public Vector3 coords = new(0f, 0f, 0f); // coords of centre of the voxel
    [HideInInspector]
    public Vector3 bounds;

    public VoxelData data;
    public Chunk chunk;

    // Face references
    public VoxelFace northFace;
    public VoxelFace southFace;
    public VoxelFace eastFace;
    public VoxelFace westFace;
    public VoxelFace upFace;
    public VoxelFace downFace;

    [Header("Face prefab references")]
    public GameObject northFacePrefab;
    public GameObject southFacePrefab;
    public GameObject eastFacePrefab;
    public GameObject westFacePrefab;
    public GameObject upFacePrefab;
    public GameObject downFacePrefab;

    public void Start()
    {
        

    }

    public void Awake()
    {

    }

    public void setBlock()
    {
        // if world settings state that voxels are to be set to a random block
        if (WorldSettings.Instance.generateRandomBlocks)
        {

            // choose random block from blockpallette
            data.block = BlockPallette.Instance.blockPalletteEntries[UnityEngine.Random.Range(0, BlockPallette.Instance.blockPalletteEntries.Count)].block;
        }


        // if world settings dictate that voxel will be given a block value by the world generation
        if (WorldSettings.Instance.useChunkInputBlocks)
        {
            Vector3 chunkPos = chunk.data.worldPosToChunkPos(this.transform.position);
            data.block = chunk.data.chunkInputBlocks.evaluate((short)chunkPos.x, (short)chunkPos.y, (short)chunkPos.z);
        }
    }

    public void unRenderVoxel()
    {
        
        // if every voxel always has 6 face children objects
        if (!WorldSettings.Instance.generateMinimalFaceObjects)
        {
           // TODO add voxel face pooling for this case
                northFace.unRenderFace();
                southFace.unRenderFace();
                eastFace.unRenderFace();
                westFace.unRenderFace();
                upFace.unRenderFace();
                downFace.unRenderFace();
           
        }
        else // if voxel only has face objects for faces that need to be rendered
        {
            VoxelFace[] faces = { northFace, southFace, eastFace, westFace, upFace, downFace };
            
            foreach(VoxelFace face in faces)
            {
                if (face != null)
                {
                    // if face object pooling is enabled
                    if (WorldSettings.Instance.objectPoolingVoxelFaces)
                    {
                        
                        //remove reference to voxel in face component
                        face.voxel = null;
                        // move it to face pool object
                        face.gameObject.transform.SetParent(chunk.pillar.world.voxelFacePool.transform);
                        face.gameObject.transform.localPosition = new Vector3(0,0,0);
                        // add it to list of faces in face pool list
                        chunk.pillar.world.facePoolDict[face.direction].Add(face);
                        // set object to inactive
                        face.gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(face.gameObject);
                    }
                    
                }
            }

            
                northFace = null;
                southFace = null;
                westFace = null;
                eastFace = null;
                upFace = null;
                downFace = null;

        }
    }

    public void renderVoxel()
    {


        double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;


        if (data.block.doNotRender) // if block is not to be rendered, such as air
        {
            return;
        }

        


        bool renderNorthFace = false;
        bool renderSouthFace = false;
        bool renderEastFace = false;
        bool renderWestFace = false;
        bool renderUpFace = false;
        bool renderDownFace = false;

        // if non visible faces are to be culled, or just not generated in the first place
        if (WorldSettings.Instance.cullHiddenVoxelFaces)
        {
            // calculate the number of faces that are to be rendered

            // if no voxel connected to north face of this voxel
            if (data.northVoxel == null)
            {
                renderNorthFace = true;
            }
            else
            {
                // or if voxel connected to north face is transparent
                if (data.northVoxel.data.block.transparent)
                {
                    renderNorthFace = true;
                }
            }

            if (data.southVoxel == null)
            {
                renderSouthFace = true;
            }
            else
            {
                if (data.southVoxel.data.block.transparent)
                {
                    renderSouthFace = true;
                }
            }

            if (data.eastVoxel == null)
            {
                renderEastFace = true;
            }
            else
            {
                if (data.eastVoxel.data.block.transparent)
                {
                    renderEastFace = true;
                }
            }

            if (data.westVoxel == null)
            {
                renderWestFace = true;
            }
            else
            {
                if (data.westVoxel.data.block.transparent)
                {
                    renderWestFace = true;
                }
            }

            if (data.upVoxel == null)
            {
                renderUpFace = true;
            }
            else
            {
                if (data.upVoxel.data.block.transparent)
                {
                    renderUpFace = true;
                }
            }

            if (data.downVoxel == null)
            {
                renderDownFace = true;
            }
            else
            {
                if (data.downVoxel.data.block.transparent)
                {
                    renderDownFace = true;
                }
            }
        }
        else
        {
            renderNorthFace = true;
            renderSouthFace = true;
            renderEastFace = true;
            renderWestFace = true;
            renderUpFace = true;
            renderDownFace = true;
        }

        // if optimised default faceless voxel, instantiate faces
        if (renderNorthFace && WorldSettings.Instance.generateMinimalFaceObjects && northFace == null) { instantiateFace(VoxelFace.FaceDirection.North); }
        if (renderSouthFace && WorldSettings.Instance.generateMinimalFaceObjects && southFace == null) { instantiateFace(VoxelFace.FaceDirection.South); }
        if (renderEastFace && WorldSettings.Instance.generateMinimalFaceObjects&& eastFace == null) { instantiateFace(VoxelFace.FaceDirection.East); }
        if (renderWestFace && WorldSettings.Instance.generateMinimalFaceObjects && westFace == null) { instantiateFace(VoxelFace.FaceDirection.West); }
        if (renderUpFace && WorldSettings.Instance.generateMinimalFaceObjects && upFace == null) { instantiateFace(VoxelFace.FaceDirection.Up); }
        if (renderDownFace && WorldSettings.Instance.generateMinimalFaceObjects && downFace == null) { instantiateFace(VoxelFace.FaceDirection.Down); }


        //render faces

        if (renderNorthFace) { northFace.renderFace(); }
        if(renderSouthFace ) { southFace.renderFace(); }
        if(renderEastFace ) { eastFace.renderFace(); }
        if(renderWestFace ) { westFace.renderFace(); }
        if(renderUpFace ) { upFace.renderFace(); }
        if(renderDownFace) { downFace.renderFace(); }

        chunk.pillar.world.testingClass.voxelRenderTimes.Add(DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);

    }

    // instantiates and sets up a face gameobject, or grabs one from the face pool if enabled and available
    public void instantiateFace(VoxelFace.FaceDirection dir)
    {
       

        GameObject faceObj = null;
        VoxelFace face = null;


        // if faces arent using object pooling optimisation, or no available faces in pool, instantiate and setup new face object
        if (!WorldSettings.Instance.objectPoolingVoxelFaces || chunk.pillar.world.facePoolDict[dir].Count == 0)
        {
            switch (dir)
            {
                case VoxelFace.FaceDirection.North:
                    faceObj = GameObject.Instantiate(northFacePrefab, this.transform);
                    face = faceObj.GetComponent<VoxelFace>();
                    face.voxel = this;
                    this.northFace = face;
                    break;

                case VoxelFace.FaceDirection.South:
                    faceObj = GameObject.Instantiate(southFacePrefab, this.transform);
                    face = faceObj.GetComponent<VoxelFace>();
                    face.voxel = this;
                    this.southFace = face;
                    break;

                case VoxelFace.FaceDirection.East:
                    faceObj = GameObject.Instantiate(eastFacePrefab, this.transform);
                    face = faceObj.GetComponent<VoxelFace>();
                    face.voxel = this;
                    this.eastFace = face;
                    break;

                case VoxelFace.FaceDirection.West:
                    faceObj = GameObject.Instantiate(westFacePrefab, this.transform);
                    face = faceObj.GetComponent<VoxelFace>();
                    face.voxel = this;
                    this.westFace = face;
                    break;

                case VoxelFace.FaceDirection.Up:
                    faceObj = GameObject.Instantiate(upFacePrefab, this.transform); ;
                    face = faceObj.GetComponent<VoxelFace>();
                    face.voxel = this;
                    this.upFace = face;
                    break;

                case VoxelFace.FaceDirection.Down:
                    faceObj = GameObject.Instantiate(downFacePrefab, this.transform);
                    face = faceObj.GetComponent<VoxelFace>();
                    face.voxel = this;
                    this.downFace = face;
                    break;

                default:
                    break;

            }
        }
        else
        {
            // grab a face from face pool, at the last position in list
            face = chunk.pillar.world.facePoolDict[dir][chunk.pillar.world.facePoolDict[dir].Count - 1];
            // remove it from the pool
            chunk.pillar.world.facePoolDict[dir].RemoveAt(chunk.pillar.world.facePoolDict[dir].Count - 1);
            // set up face
            face.gameObject.SetActive(true);
            face.voxel = this;
            face.gameObject.transform.SetParent(transform);
            face.gameObject.transform.localPosition = Vector3.zero;
            switch (dir) {
                case VoxelFace.FaceDirection.North:
                    northFace = face;
                    break;

                case VoxelFace.FaceDirection.South:
                    southFace = face;
                    break;

                case VoxelFace.FaceDirection.West:
                    westFace = face;
                    break;

                case VoxelFace.FaceDirection.East:
                    eastFace = face;
                    break;

                case VoxelFace.FaceDirection.Down:
                    downFace = face;
                    break;

                case VoxelFace.FaceDirection.Up:
                    upFace = face;
                    break;

                default:
                    Debug.LogError("Setting up pooled voxel face, but it has no direction value!");
                    break;
            }

        }

        
    }

    // this method is not used, but could be part of a voxel lighting system in the future
    public void lighting()
    {
        //calculate if block is exposed to the sky
        isExposedToSky();


    }

    // this function recursively calculates if there is a clear line of sight from the voxel to the upper limit of the world,
    // ignoring transparent blocks.
    // a null voxel is the world boundary
    public bool isExposedToSky()
    {
        // if reached the world's upper boundary
        if (data.upVoxel == null)
        {
            data.exposedToSky = 1;
            return true;
        }
        // if above block is opaque
        else if (!data.upVoxel.data.block.transparent)
        {
            data.exposedToSky = -1;
            return false;
        }
        // if there is a transparent block above this one
        else
        {
            // if above block has already been calculated, use its value
            if (data.upVoxel.data.exposedToSky != 0)
            {
                data.exposedToSky = data.upVoxel.data.exposedToSky;
                return (data.upVoxel.data.exposedToSky == 1);
            }


            return data.upVoxel.isExposedToSky();
        }
    }

}

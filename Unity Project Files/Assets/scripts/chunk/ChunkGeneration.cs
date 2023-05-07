using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// handles the generation of chunks of voxels
public class ChunkGeneration : MonoBehaviour
{
    
    [SerializeField] private GameObject voxelPrefabFaced;
    [SerializeField] private GameObject voxelPrefabFaceless;
    public short xSize = 16;
    public short ySize = 16;
    public short zSize = 16;
    [SerializeField] Chunk chunk;


    private void Awake()
    {
        xSize = (short)WorldSettings.Instance.chunkSizeinVoxels.x;
        ySize = (short)WorldSettings.Instance.chunkSizeinVoxels.y;
        zSize = (short)WorldSettings.Instance.chunkSizeinVoxels.z;


        // set up collider for frustum culling
        if (WorldSettings.Instance.viewFrustumCullingColliders)
        {
            chunk.boxCollider = gameObject.AddComponent<BoxCollider>();
            chunk.boxCollider.isTrigger = false;

            float xSizeinUnits = xSize * WorldSettings.Instance.voxelSize.x;
            float ySizeinUnits = ySize * WorldSettings.Instance.voxelSize.y;
            float zSizeinUnits = zSize * WorldSettings.Instance.voxelSize.z;

            chunk.boxCollider.center = new Vector3(xSizeinUnits / 2,
                ySizeinUnits / 2,
                zSizeinUnits / 2)  +  new Vector3(-0.5f,-0.5f,-0.5f);

            // add a little border around the collider, so chunk is rendered just before it enters the view frustum
            chunk.boxCollider.size = new Vector3(xSizeinUnits* 1.1f, ySizeinUnits * 1.1f, zSizeinUnits * 1.1f);
        }
       
         
    }


    // used to update the new world space positions of voxels in a chunk which has been moved as part of pillar object pooling
    public void resetVoxelPositionRecords()
    {
        List<Vector3> posList = new();
        List<Voxel> voxList = new();
        foreach(KeyValuePair<Vector3,Voxel> kvpV in chunk.data.voxels)
        {
            voxList.Add(kvpV.Value);
            posList.Add(kvpV.Value.gameObject.transform.position);

            // also reset all neightbour references for voxels
            kvpV.Value.data.northVoxel = null;
            kvpV.Value.data.southVoxel = null;
            kvpV.Value.data.westVoxel = null;
            kvpV.Value.data.eastVoxel = null;
            kvpV.Value.data.upVoxel = null;
            kvpV.Value.data.downVoxel = null;
        }

        if(voxList.Count != posList.Count)
        {
            Debug.LogError("Resetting voxel position records in pooled pillar, pos list and vox list are of inequal lengths");
            return;
        }

        chunk.data.voxels.Clear();
        for(int i = 0; i < voxList.Count;i++)
        {
            
            chunk.data.voxels.Add(posList[i], voxList[i]);
        }
    }

    public void generateChunk()

    {
        // do the world generation for this chunk and get the blocks to be generated
        chunk.data.generateBlocks();


        Vector3 VoxelSize = WorldSettings.Instance.voxelSize;

        // - - - est voxel in chunk is at 0,0,0 relative to chunk object

      

        // for loop to repeat for all Y values in chunk
        for (short y = 0; y < (ySize * VoxelSize.y); y += (short)VoxelSize.y)
        {
            // generate all voxels in flat plane of chunk

            for (short x = 0; x < (xSize * VoxelSize.x); x += (short)VoxelSize.x)
            {

                for (short z = 0; z < (zSize * VoxelSize.z); z += (short)VoxelSize.z)
                {

                    Vector3 relativePosition = new Vector3(x, y, z); // voxel's position relative to chunk

                    double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
                    GameObject generatedVoxel = generateVoxel(relativePosition);
                    chunk.pillar.world.timeTakenForVoxels += (DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);
                    
                    Voxel generatedVoxelComponent = generatedVoxel.GetComponent<Voxel>();
                    generatedVoxelComponent.chunk = chunk;
                    // register voxel in chunk.data's voxel dictionary, key is voxel's world position, value is the voxel component
                    chunk.data.voxels.Add(transform.position + relativePosition, generatedVoxelComponent);
                }

            }


        }

        // all the voxels in the chunk have now been generated

       
    }

    public void setBlocks()
    {
        foreach(KeyValuePair<Vector3,Voxel> kvp in chunk.data.voxels)
        {
            kvp.Value.setBlock();
        }
    }

    public void findNeighbouringVoxels()
    {
        //TODO add option to set neighbours inverse neighbour chunk so algorithm doesnt need to run again to find the same pair in reverse

        // give voxeldata components references to neighbouring voxels
        foreach (KeyValuePair<Vector3, Voxel> entry in chunk.data.voxels)
        {
            //Debug.Log("voxel " + entry.Key.ToString() + " being checked for neighbours");

            // X- West  X+ East
            // Z- South Z+ North
            // Y- Down Y+ Up
            Vector3 relativePosition = entry.Key - this.transform.position; // voxel's relative position to chunk

            

            //Debug.Log("For voxel at world position " + entry.Key.ToString());

            // relative position of voxel 1 voxel in - x direction
            Vector3 westPosition = new(relativePosition.x - 1, relativePosition.y, relativePosition.z);
            entry.Value.data.westVoxel = chunk.data.chunkPositionToVoxel(westPosition,"west");

            // relative position of voxel 1 voxel in + x direction
            Vector3 eastPosition = new(relativePosition.x + 1, relativePosition.y, relativePosition.z);
            entry.Value.data.eastVoxel = chunk.data.chunkPositionToVoxel(eastPosition,"east");

            // relative position of voxel 1 voxel in -z direction
            Vector3 southPosition = new(relativePosition.x, relativePosition.y, relativePosition.z - 1);
            entry.Value.data.southVoxel = chunk.data.chunkPositionToVoxel(southPosition,"south");

            // relative position of voxel 1 voxel in +z direction
            Vector3 northPosition = new(relativePosition.x, relativePosition.y, relativePosition.z + 1);
            entry.Value.data.northVoxel = chunk.data.chunkPositionToVoxel(northPosition,"north");

            // relative position of voxel 1 voxel in +y direction
            Vector3 upPosition = new(relativePosition.x, relativePosition.y + 1, relativePosition.z);
            entry.Value.data.upVoxel = chunk.data.chunkPositionToVoxel(upPosition,"up");

            // relative position of voxel 1 voxel in -y direction
            Vector3 downPosition = new(relativePosition.x, relativePosition.y - 1, relativePosition.z);
            entry.Value.data.downVoxel = chunk.data.chunkPositionToVoxel(downPosition,"down");



            // if voxel is at west boundary of chunk
            if(relativePosition.x == 0 && entry.Value.data.westVoxel != null)
            {
                entry.Value.data.westVoxel.data.eastVoxel = entry.Value;
            }
            // if voxel is at east boundary of chunk
            if (relativePosition.x == (WorldSettings.Instance.voxelSize.x * WorldSettings.Instance.chunkSizeinVoxels.x) -1
                && entry.Value.data.eastVoxel != null)
            {
                entry.Value.data.eastVoxel.data.westVoxel = entry.Value;
            }
            // if voxel is at north boundary of chunk
            if (relativePosition.z == (WorldSettings.Instance.voxelSize.z * WorldSettings.Instance.chunkSizeinVoxels.z) -1
                && entry.Value.data.northVoxel != null)
            {
                entry.Value.data.northVoxel.data.southVoxel = entry.Value;
            }
            // if voxel is at south boundary of chunk
            if (relativePosition.z == 0 && entry.Value.data.southVoxel != null)
            {
                entry.Value.data.southVoxel.data.northVoxel = entry.Value;
            }
        }
    }

    public void renderChunk()
    {
        double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;


        // render the voxels in the chunk

        foreach (KeyValuePair<Vector3, Voxel> entry in chunk.data.voxels)
        {
            entry.Value.renderVoxel();

            Vector3 relativePosition = entry.Key - this.transform.position; // voxel's relative position to chunk

            // if voxel is at west boundary of chunk
            if (relativePosition.x == 0 && entry.Value.data.westVoxel != null)
            {
                entry.Value.data.westVoxel.unRenderVoxel();
                entry.Value.data.westVoxel.renderVoxel();
            }
            // if voxel is at east boundary of chunk
            if (relativePosition.x == (WorldSettings.Instance.voxelSize.x * WorldSettings.Instance.chunkSizeinVoxels.x) -1
                 && entry.Value.data.eastVoxel != null)
            {
                entry.Value.data.eastVoxel.unRenderVoxel();
                entry.Value.data.eastVoxel.renderVoxel();
            }
            // if voxel is at north boundary of chunk
            if (relativePosition.z == (WorldSettings.Instance.voxelSize.z * WorldSettings.Instance.chunkSizeinVoxels.z) -1
                && entry.Value.data.northVoxel != null)
            {
                entry.Value.data.northVoxel.unRenderVoxel();
                entry.Value.data.northVoxel.renderVoxel();
            }
            // if voxel is at south boundary of chunk
            if (relativePosition.z == 0 && entry.Value.data.southVoxel != null)
            {
                entry.Value.data.southVoxel.unRenderVoxel();
                entry.Value.data.southVoxel.renderVoxel();
            }
        }

        chunk.pillar.world.testingClass.chunkRenderTimes.Add(DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);
    }

    // spawns voxel at given local position
    private GameObject generateVoxel(Vector3 localPos)
    {
        GameObject voxelPrefabChoice = null;

        if (WorldSettings.Instance.generateMinimalFaceObjects)
        {
            voxelPrefabChoice = voxelPrefabFaceless;
        }
        else
        {
            voxelPrefabChoice = voxelPrefabFaced;
        }

        GameObject voxelObj = Instantiate(voxelPrefabChoice, gameObject.transform); // instantiates a voxel as child of chunk object
        voxelObj.transform.localPosition = localPos;
        voxelObj.GetComponent<Voxel>().chunk = chunk;

        return voxelObj;
    }
}

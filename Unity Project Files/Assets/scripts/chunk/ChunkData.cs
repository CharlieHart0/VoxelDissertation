using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this class holds data for a chunk, and several methods which are used in locating voxels within a chunk.
public class ChunkData : MonoBehaviour
{
   
    public Chunk chunk;
    public Dictionary<Vector3,Voxel> voxels = new(); // contains a list of all voxels in the chunk, key is voxel's world space position
    public Block defaultBlock; // the default block, probably air, to be used in world generation
    public ChunkInputBlocks chunkInputBlocks;
    public Vector3 dimensions;
    public Vector3[] chunkVertices;

    

    private void Start()
    {
        dimensions = new Vector3 (WorldSettings.Instance.chunkSizeinVoxels.x * WorldSettings.Instance.voxelSize.x,
                                    WorldSettings.Instance.chunkSizeinVoxels.y * WorldSettings.Instance.voxelSize.y,
                                    WorldSettings.Instance.chunkSizeinVoxels.z * WorldSettings.Instance.voxelSize.z
            );

        chunkVertices = new Vector3[8]
        {
            // each vertex of chunk, add 0.5 in each dimension, as voxels extend 0.5 units in each direction further than their centres.
            transform.position + new Vector3(-0.5f,-0.5f,-0.5f),
            new Vector3(dimensions.x,0,0) +  transform.position + new Vector3(0.5f,-0.5f,-0.5f),
            new Vector3(dimensions.x,dimensions.y,0) +  transform.position + new Vector3(0.5f,0.5f,-0.5f),
            new Vector3(dimensions.x,dimensions.y,dimensions.z) +  transform.position+ new Vector3(0.5f,0.5f,0.5f),
            new Vector3(0,dimensions.y,0) +  transform.position+ new Vector3(-0.5f,0.5f,-0.5f),
            new Vector3(0,dimensions.y,dimensions.z) +  transform.position+ new Vector3(-0.5f,0.5f,0.5f),
            new Vector3(0,0,dimensions.z) +  transform.position+ new Vector3(-0.5f,-0.5f,0.5f),
            new Vector3(dimensions.x,0,dimensions.z) +  transform.position+ new Vector3(0.5f,-0.5f,0.5f)
        };

    }

    private void Update()
    {
        
    }

    public Vector3 worldPosToChunkPos(Vector3 worldPos)
    {
        return worldPos - this.transform.position;
    }
   

    // sets the chunkinputblocks variable of the chunk to the values they should be for the current position of each voxel in world space
    public void generateBlocks()
    {
        double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
        chunkInputBlocks = new(defaultBlock, (short)WorldSettings.Instance.chunkSizeinVoxels.x,
            (short)WorldSettings.Instance.chunkSizeinVoxels.y,
            (short)WorldSettings.Instance.chunkSizeinVoxels.z);
        
        // evaluates world generation output for each voxel
        for(short x = 0; x < WorldSettings.Instance.chunkSizeinVoxels.x; x++)
        {
            for (short y = 0; y < WorldSettings.Instance.chunkSizeinVoxels.y; y++)
            {
                for (short z = 0; z < WorldSettings.Instance.chunkSizeinVoxels.z; z++)
                {
                    //get worldpos of voxel
                    Vector3 _worldPos = this.transform.position + new Vector3(x, y, z);
                    //evaluate block from world generation
                    Block b = chunk.pillar.world.evaluateBlock(Mathf.FloorToInt(_worldPos.x),
                        Mathf.FloorToInt(_worldPos.y),
                        Mathf.FloorToInt(_worldPos.z));
                    // if there is a block to place here (it is not the default block)
                    if(b != null)
                    {
                        chunkInputBlocks.setBlock(x,y,z,b);
                    }
                }
            }
        }
        chunk.pillar.world.testingClass.chunkBlockGenerationTimes.Add(DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);
    }



    // given a relative chunk position, this method returns a voxel component, or null if not found
    // this function is only garunteed to work for voxels in the chunk, or directly contacting the chunk (not diagonally)
    public Voxel chunkPositionToVoxel(Vector3 chunkPosition,string debugDirection = "Recursive")
    {
        //Debug.Log("looking for neighbour in direciton: " + debugDirection);
        Vector3 worldPos = this.transform.position + chunkPosition;
        if (!voxels.ContainsKey(worldPos)) {
            Pillar pillarToSearch = null;

            
            if(chunkPosition.x < 0)
            {
                int xPositionOfPillarToSearch = (int)(chunk.pillar.transform.position.x - (WorldSettings.Instance.voxelSize.x * WorldSettings.Instance.chunkSizeinVoxels.x));
                pillarToSearch = chunk.pillar.world.getPillar(new Vector2(xPositionOfPillarToSearch, this.transform.position.z));
                
            }
            else if(chunkPosition.x >= (WorldSettings.Instance.voxelSize.x * WorldSettings.Instance.chunkSizeinVoxels.x))
            {
                int xPositionOfPillarToSearch = (int)(chunk.pillar.transform.position.x +(WorldSettings.Instance.voxelSize.x * WorldSettings.Instance.chunkSizeinVoxels.x));
                pillarToSearch = chunk.pillar.world.getPillar(new Vector2(xPositionOfPillarToSearch, this.transform.position.z));
            }
            else if (chunkPosition.z < 0)
            {
                int zPositionOfPillarToSearch = (int)(chunk.pillar.transform.position.z - (WorldSettings.Instance.voxelSize.z * WorldSettings.Instance.chunkSizeinVoxels.z));
                pillarToSearch = chunk.pillar.world.getPillar(new Vector2(this.transform.position.x,zPositionOfPillarToSearch));
            }
            else if (chunkPosition.z >= (WorldSettings.Instance.voxelSize.z * WorldSettings.Instance.chunkSizeinVoxels.z))
            {
                int zPositionOfPillarToSearch = (int)(chunk.pillar.transform.position.z + (WorldSettings.Instance.voxelSize.z * WorldSettings.Instance.chunkSizeinVoxels.z));
                pillarToSearch = chunk.pillar.world.getPillar(new Vector2(this.transform.position.x, zPositionOfPillarToSearch));
            }

            // if could be a voxel in a different chunk inside the pillar
            else if (chunkPosition.y < 0 || chunkPosition.y >= chunk.generation.ySize)
            {
                pillarToSearch = chunk.pillar;
               
            }
            
           
            if(pillarToSearch != null)
            {
                Chunk chunkToSearch = pillarToSearch.data.findChunkInPillar(worldPos);
                // if there exists no chunk this voxel could exist in, return null
                if (chunkToSearch == null)
                {
                   // Debug.Log("----NULL CHUNK at face "+ debugDirection+", the pillar position is "+pillarToSearch.transform.position.ToString());
                    return null;
                }

                //Debug.Log("----found chunk to search " + chunkToSearch.transform.position);

                return chunkToSearch.data.chunkPositionToVoxel(chunkToSearch.data.worldPosToChunkPos(worldPos));
            }
            

            return null; }

        
        return voxels[worldPos];
            
    }
}


// this class holds a 3d list (3 layers of nested lists) of every position in a chunk, and a value of the block to be spawned there
public class ChunkInputBlocks
{
   

    List<List<List<Block>>> xBlocks = new();

    public ChunkInputBlocks(Block defaultBlock,short xSize = 16, short ySize = 16, short zSize = 16)
    {
        // for x  0 to 15 (default)
        for (int x = 0; x < xSize; x++)
        {
            List<List<Block>> yBlocks = new();
            for (int y = 0; y < ySize; y++)
            {

                List<Block> zBlocks = new();
                for (int z = 0; z < xSize; z++)
                {

                    zBlocks.Add(defaultBlock);

                }
                yBlocks.Add(zBlocks);

            }
            xBlocks.Add(yBlocks);
        }
    }

    public Block evaluate(short x, short y, short z)
    {
        return xBlocks[x][y][z];
    }


    // sets a block value in the list
    public bool setBlock(short x, short y, short z, Block block, bool check = false)
    {
        // if trying to set a block outside of range x
        if (xBlocks.Count < x + 1 || x < 0)
        {
            return false;
        }
        // if trying to set a block outside of range y
        if (xBlocks[x].Count < y + 1 || y < 0)
        {
            return false;
        }
        // if trying to set a block outside of range y
        if (xBlocks[x][y].Count < z + 1 || z < 0)
        {
            return false;
        }

        xBlocks[x][y][z] = block;


        // if check, checks that block was set correctly.
        if (!check) { return true; }
        else
        {
            return (block == evaluate(x, y, z));
        }
    }
}


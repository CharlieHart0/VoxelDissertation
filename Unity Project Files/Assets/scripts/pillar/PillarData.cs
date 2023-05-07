using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarData : MonoBehaviour
{
    [SerializeField]
    Pillar pillar;

    [SerializeField]
    int debugChunkCount = 0;

    public Dictionary<Vector3, Chunk> chunks = new(); // contains a list of all chunks in the pillar, key is chunk's world space position

    private void Update()
    {
        // used for debugging, because using a debugger is hard
        debugChunkCount = chunks.Count;
    }
    // given a relative pillar position, this method returns a chunk component, or null if not found
    public Chunk pillarPositionToChunk(Vector3 pillarPosition)
    {
        // convert relative pillar position to world position
        Vector3 worldPos = pillar.transform.position + pillarPosition;
        if (!chunks.ContainsKey(worldPos)) { return null; }

        return chunks[worldPos];

    }

    // used to update the new world space positions of chunks in pillar which has been moved as part of pillar object pooling
    public void resetChunkPositionRecords()
    {
        List<Vector3> posList = new();
        List<Chunk> chunkList = new();
        foreach (KeyValuePair<Vector3, Chunk> kvpC in chunks)
        {
            chunkList.Add(kvpC.Value);
            posList.Add(kvpC.Value.gameObject.transform.position);
        }

        // check that extracted value lists are of equal length
        if (chunkList.Count != posList.Count)
        {
            Debug.LogError("Resetting chunk position records in pooled pillar, pos list and chunk list are of inequal lengths");
            return;
        }


        chunks.Clear();
        for (int i = 0; i < chunkList.Count; i++)
        {
            
            chunks.Add(posList[i], chunkList[i]);
        }
    }


    // given the world space position of a voxel, returns the chunk inside the pillar which the voxel may be in
    public Chunk findChunkInPillar(Vector3 voxelPosition)
    {
        //Debug.Log("find chunk in pillar for " + voxelPosition.ToString());
        // if voxel position is not within bounds of pillar
        // too negative
        if(voxelPosition.x < this.transform.position.x)
        {
            //Debug.Log("X too low");
            return null;
        }
        if (voxelPosition.z < this.transform.position.z)
        {
            //Debug.Log("z too low");
            return null;
        }
        if (voxelPosition.y < this.transform.position.y)
        {
            //Debug.Log("y too low");
            return null;
        }
        // too positive
        if (voxelPosition.x >= this.transform.position.x + WorldSettings.Instance.chunkSizeinVoxels.x)
        {
            //Debug.Log("X too high");
            return null;
        }
        if (voxelPosition.z >= this.transform.position.z + WorldSettings.Instance.chunkSizeinVoxels.z)
        {
            //Debug.Log("z too high");
            return null;
        }
        if (voxelPosition.y >= this.transform.position.y + WorldSettings.Instance.chunkSizeinVoxels.y * pillar.generation.ySize)
        {
            //Debug.Log("y too high");
            return null;
        }

        //find which chunk the voxel must be in

        // round y position down to nearest chunk.ysize, to find y position of chunk
        float chunkY = Mathf.FloorToInt(voxelPosition.y / WorldSettings.Instance.chunkSizeinVoxels.y) * WorldSettings.Instance.chunkSizeinVoxels.y;
        //Debug.Log("found chunk, is at y value "+ chunkY.ToString());

        return pillarPositionToChunk(new Vector3(0f, chunkY, 0f));

        
    }
}

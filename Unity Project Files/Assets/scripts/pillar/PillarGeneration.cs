using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this script handles the generation of pillars, which are a series of chunks on top of each other.
public class PillarGeneration : MonoBehaviour
{
    [SerializeField]
    Pillar pillar;

    
    public GameObject chunkPrefab;

    [Tooltip("How many chunks tall is this pillar?")]
    [SerializeField]
    public int ySize = 3;

    public void Awake()
    {
        ySize = (short)WorldSettings.Instance.renderedWorldSizeinChunks.y;
    }

    private void Start()
    {
        
    }

    public void generatePillar()
    {
        // for each chunk to be generated in the pillar
        for (int y = 0; y < ySize; y++)
        {
            GameObject chunkObj = Instantiate(chunkPrefab);
            Chunk chunk = chunkObj.GetComponent<Chunk>();
            chunkObj.transform.parent = transform;
            // place chunk with correct offset from pillar origin
            chunkObj.transform.localPosition = new Vector3(0, y * chunk.generation.ySize, 0);
            chunk.pillar = pillar;
            pillar.data.chunks.Add(chunkObj.transform.position, chunk);

           chunk.generation.generateChunk();
        }

    }

    public void setBlocks()
    {
        foreach (KeyValuePair<Vector3, Chunk> kvp in pillar.data.chunks)
        {
            kvp.Value.generation.setBlocks();
        }
    }

    public void findNeighbouringVoxels()
    {
        foreach (KeyValuePair<Vector3, Chunk> entry in pillar.data.chunks)
        {
            entry.Value.generation.findNeighbouringVoxels();
        }
    }

    public void renderPillar()
    {
        foreach (KeyValuePair<Vector3, Chunk> entry in pillar.data.chunks)
        {
            entry.Value.generation.renderChunk();
        }
    }


}

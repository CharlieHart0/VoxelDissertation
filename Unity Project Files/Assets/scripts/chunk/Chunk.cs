using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChunkData))]
[RequireComponent(typeof(ChunkGeneration))]
public class Chunk : MonoBehaviour
{
    public ChunkData data;
    public ChunkGeneration generation;
    public Pillar pillar;
    public BoxCollider boxCollider;

    private bool renderedLastFrame = false;
    public bool renderTheChunk = false;

    private void Start()
    {

        // turn collider on and off to make sure ontriggerenter is called if chunk spawned inside camera view frustum collider
        boxCollider.enabled = false;
        boxCollider.enabled = true;
    }

    private void Update()
    {
        if (WorldSettings.Instance.viewFrustumCullingChunkwise)
        {
            bool renderThisChunk = false;
            // if any of the chunk vertices are in the view frustum, make the chunk render
            foreach(Vector3 vertex in data.chunkVertices)
            {
                if (HelpfulFunctions.isInViewFrustum(vertex))
                {
                    renderThisChunk = true;
                    break;
                }
            }

            //TODO, this can sometimes not work well, if viewing a small part of a chunk where none of the corners are in frame,
            //also it will not work if you are close enough to a large chunk that you cannot see any of the corners

            // if render instruction is different to last frame, update all voxel faces
            if(renderThisChunk != renderedLastFrame)
            {
                foreach(Voxel v in data.voxels.Values)
                {
                    // voxel faces should really be in a dictionary or array, but this will have to do for now

                    if(v.northFace != null)
                    {
                        v.northFace.meshRenderer.enabled = renderThisChunk;
                    }

                    if (v.southFace != null)
                    {
                        v.southFace.meshRenderer.enabled = renderThisChunk;
                    }

                    if (v.eastFace != null)
                    {
                        v.eastFace.meshRenderer.enabled = renderThisChunk;
                    }

                    if (v.westFace != null)
                    {
                        v.westFace.meshRenderer.enabled = renderThisChunk;
                    }

                    if (v.upFace != null)
                    {
                        v.upFace.meshRenderer.enabled = renderThisChunk;
                    }

                    if (v.downFace != null)
                    {
                        v.downFace.meshRenderer.enabled = renderThisChunk;
                    }
                }
            }


            renderedLastFrame = renderThisChunk;
        }

        if (WorldSettings.Instance.viewFrustumCullingColliders)
        {
            renderChunk(renderTheChunk);
        }
    }


    // ontriggerexit doesnt seem to work with non standard collider shapes, and ontriggerenter seems to be calling every physics tick,
    // so we will just set renderTheChunk to false with fixedupdate, and then later in the excecution order when ontriggerenter is called, it will be set to true
    // if it is overlapping with camera view frustum

    // it is probably a bad idea to write this feature taking advantage of a bug

    private void FixedUpdate()
    {
        if (WorldSettings.Instance.viewFrustumCullingColliders)
        {
            renderTheChunk = false;
        }
    }

    public void renderChunk(bool input)
    {
        bool renderThisChunk = input;

        if (renderThisChunk == renderedLastFrame) { return; }

        foreach (Voxel v in data.voxels.Values)
        {
            // voxel faces should really be in a dictionary or array, but this will have to do for now

            if (v.northFace != null)
            {
                v.northFace.meshRenderer.enabled = renderThisChunk;
            }

            if (v.southFace != null)
            {
                v.southFace.meshRenderer.enabled = renderThisChunk;
            }

            if (v.eastFace != null)
            {
                v.eastFace.meshRenderer.enabled = renderThisChunk;
            }

            if (v.westFace != null)
            {
                v.westFace.meshRenderer.enabled = renderThisChunk;
            }

            if (v.upFace != null)
            {
                v.upFace.meshRenderer.enabled = renderThisChunk;
            }

            if (v.downFace != null)
            {
                v.downFace.meshRenderer.enabled = renderThisChunk;
            }
        }

        renderedLastFrame = renderThisChunk;
    }
}

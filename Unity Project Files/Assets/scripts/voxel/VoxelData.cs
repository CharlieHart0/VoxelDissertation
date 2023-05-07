using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// contains data for the voxel
public class VoxelData : MonoBehaviour
{
    public enum TextureUnwrapType
    {
        SingleTexture,
        Net
    }

    [Header("Neighbouring Voxels")]
    // X- West  X+ East
    // Z- South Z+ North
    // Y- Down Y+ Up
    public Voxel westVoxel;
    public Voxel eastVoxel;
    public Voxel southVoxel;
    public Voxel northVoxel;
    public Voxel downVoxel;
    public Voxel upVoxel;

    [Header("Voxel Properties")]
    public Block block;
    // records whether block is exposed to the sky or not. 0 is unknown, -1 is false and 1 is true.
    // a nullable boolean would be better suited to this
    public short exposedToSky = 0;
    public short sunLightLevel;
    public short resultantLightLevel;

    
}

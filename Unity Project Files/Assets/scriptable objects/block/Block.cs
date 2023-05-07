using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName= "New Block",menuName ="Block")]
public class Block : ScriptableObject
{
    public new string name;
    public bool transparent;
    public bool doNotRender;
    public VoxelData.TextureUnwrapType textureUnwrapType;
    public Material material;
}

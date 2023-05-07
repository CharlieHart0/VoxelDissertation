using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PillarData))]
[RequireComponent(typeof(PillarGeneration))]
public class Pillar : MonoBehaviour
{
    public PillarData data;
    public PillarGeneration generation;
    public WorldGeneration world;
}

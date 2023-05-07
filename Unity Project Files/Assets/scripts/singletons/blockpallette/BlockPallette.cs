using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct BlockPalletteEntry
{
    public BlockPallette.Block id;
    public Block block;
}


// this script contains references to all the blocks which can be used in the scene
public class BlockPallette : MonoBehaviour
{
    public enum Block
    {
        Air,
        Rock,
        Earth,
        GrassyEarth
    }

    public static BlockPallette Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public List<BlockPalletteEntry> blockPalletteEntries = new();

    public BlockPalletteEntry entryToAdd;

    public void addEntry()
    {
        while(blockPalletteEntries.Count <= (int)entryToAdd.id)
        {
            blockPalletteEntries.Add(new BlockPalletteEntry());
        }
        blockPalletteEntries[(int)entryToAdd.id] = entryToAdd;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System.Text;
using System;

// This script controls the recording of testing data/ results
public class Testing : MonoBehaviour
{
    [SerializeField] string filePath;
    [SerializeField] bool runTestingCode = false;
    public bool gameStarted = false; // whether or not the game has started fully - after main menu screen
    [SerializeField] string testName; // the name of the current test, by which the file can be identified later
    [SerializeField] string testComment; // a comment describing the current test
    [SerializeField] WorldSettings worldSettings;
    [SerializeField] WorldGeneration worldGen;
    [SerializeField] KeyCode saveKey = KeyCode.T;


    public double testStartTime = 0;
    public List<double> pillarLoadTimes = new();     // times taken to generate populate and render new pillars in the world
    public List<double> chunkBlockGenerationTimes = new(); // times taken to generate the list of block states for each chunk in the world
    public List<double> voxelRenderTimes = new();   // times taken to generates face objects and render a voxel
    public List<double> chunkRenderTimes = new(); // times taken to run renderFace() function on facesin a chunk 
    public List<int> fpsRecords = new(); // individual frame rate values at various points during runnning


    [SerializeField] int counterUpdatesPerSecond = 2; // rate per second at which the frame rate should be recorded
    float timeSinceLastUpdate = 0f;


    

    // gets the current frame rate of the game
    int currentFPS()
    {
        // calculates the fps as the multiplicative inverse of time taken to render last frame, and rounds to integer
        // (Time.deltaTime is basically Seconds per Frame, and the value we want here is Frames per Second)
        return Mathf.RoundToInt(Mathf.Pow(Time.deltaTime, -1f));
    }

    // Update is called once per frame
    void Update()
    {
        // if runtestingcode setting is false, or if game has not started,
        // or game has started but generate terrain only on Q press setting is sctive, and there is no terrain yet
        if (!runTestingCode || 
            (!gameStarted && !(worldSettings.generateTerrainOnQPress ^ worldGen.pillars.Count >0)))
        {
            return;
        }

        

        // if time since counter was last updated is more than (1/update rate), update
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate > Mathf.Pow(counterUpdatesPerSecond, -1f))
        {
            timeSinceLastUpdate = 0f;
            // record the current frame rate and add it to list
            fpsRecords.Add(currentFPS());
        }



        if (Input.GetKeyDown(saveKey))
        {
            // save the values which have been recorded
            saveValues();
        }
    }

    // gets mean average of an integer list
    int intListAverage(List<int> _list)
    {
        int total = 0;

        foreach(int i in _list)
        {
            total += i;
        }
        return total / _list.Count;
    }

    // gets mean average of double list
    double doubleListAverage(List<double> _list)
    {
        double total = 0;

        foreach (double i in _list)
        {
            total += i;
        }
        return total / _list.Count;
    }


    private void OnApplicationQuit()
    {
        if (runTestingCode)
        {
            saveValues();
        }
    }

    //save all the data recorded to a file
    void saveValues()
    {
        JSONObject json = new();

        // information about the environment
        json["Test Comment"] = testComment;
        json["Date and Time"] = DateTime.Now.ToString();
        json["Environment Type"] = Application.isEditor ? "Editor": "Build";
        json["CPU"]["Name"] = SystemInfo.processorType;
        json["CPU"]["Cores"] = SystemInfo.processorCount / 2; // 2 logical processors per core
        json["CPU"]["Frequency (MHz)"] = SystemInfo.processorFrequency;
        json["RAM Total (MB)"] = SystemInfo.systemMemorySize;
        json["Graphics"]["Graphics Device Name"] = SystemInfo.graphicsDeviceName;
        json["Graphics"]["GPU VRAM (MB)"] = SystemInfo.graphicsMemorySize;
        json["Test Duration (ms)"] = DateTime.Now.TimeOfDay.TotalMilliseconds - testStartTime;

        // record information about enabled settings

            // optimisations
        json["Settings"]["Optimisations"]["Object Pooling"]["Pillars"] = worldSettings.objectPoolingPillars;
        json["Settings"]["Optimisations"]["Object Pooling"]["Faces"] = worldSettings.objectPoolingVoxelFaces;
        json["Settings"]["Optimisations"]["Internal Face Culling"] = worldSettings.cullHiddenVoxelFaces;
        json["Settings"]["Optimisations"]["View Frustum Culling"]["Screen Coordinates - Faces"] = worldSettings.viewFrustumCulling;
        json["Settings"]["Optimisations"]["View Frustum Culling"]["Screen Coordinates - Chunks"] = worldSettings.viewFrustumCullingChunkwise;
        json["Settings"]["Optimisations"]["View Frustum Culling"]["Colliders - Chunks"] = worldSettings.viewFrustumCullingColliders;


            // world properties
        json["Settings"]["World Properties"]["Chunk Dimensions (in voxels)"]["X"] = worldSettings.chunkSizeinVoxels.x;
        json["Settings"]["World Properties"]["Chunk Dimensions (in voxels)"]["Y"] = worldSettings.chunkSizeinVoxels.y;
        json["Settings"]["World Properties"]["Chunk Dimensions (in voxels)"]["Z"] = worldSettings.chunkSizeinVoxels.z;

        json["Settings"]["World Properties"]["Render Distance (in voxels)"]["X"] = (worldSettings.renderedWorldSizeinChunks.x * worldSettings.chunkSizeinVoxels.x);
        json["Settings"]["World Properties"]["Render Distance (in voxels)"]["Y"] = (worldSettings.renderedWorldSizeinChunks.y * worldSettings.chunkSizeinVoxels.y);
        json["Settings"]["World Properties"]["Render Distance (in voxels)"]["Z"] = (worldSettings.renderedWorldSizeinChunks.z * worldSettings.chunkSizeinVoxels.z);

        json["Settings"]["World Properties"]["World Seed"] = worldSettings.seedInt;

        json["Settings"]["World Properties"]["Hill Properties"]["Max Height"] = worldGen.hillMaxHeight;
        json["Settings"]["World Properties"]["Hill Properties"]["Min Height"] = worldGen.valleyMinHeight;


            // other options / features
        json["Settings"]["Misc Options"]["Generate Random Blocks"] = worldSettings.generateRandomBlocks;
        json["Settings"]["Misc Options"]["Randomise Seed"] = worldSettings.useRandomSeed;
        json["Settings"]["Misc Options"]["Use World Generation System"] = worldSettings.useChunkInputBlocks;
        json["Settings"]["Misc Options"]["Remove Pillars Dynamically"] = worldSettings.removeTerrainAsCameraMoves;
        json["Settings"]["Misc Options"]["Generate Pillars Dynamically"] = worldSettings.generateTerrainAsCameraMoves;
        json["Settings"]["Misc Options"]["Use Hill Shape Curve"] = worldSettings.useHillCurve;
        json["Settings"]["Misc Options"]["Generate Terrain only when Q pressed"] = worldSettings.generateTerrainOnQPress;
        json["Settings"]["Misc Options"]["Generate face objects only where needed"] = worldSettings.generateMinimalFaceObjects;
        json["Settings"]["Misc Options"]["Narrow Caves at Surface"] = worldSettings.cavesNarrowAtSurface;




        // recorded and averaged values

            // frame rate
        json["FPS"]["Average"] = intListAverage(fpsRecords);
        foreach(int i in fpsRecords)
        {
            json["FPS"]["Data"].Add(i);
        }

            // pillar load times
        if(pillarLoadTimes.Count == 0) { 
            json["Pillar Generation and Render Times (ms)"]["Average"] = "NULL";
            json["Pillar Generation and Render Times (ms)"]["Data"] = "NULL";
        }
        else
        {
            json["Pillar Generation and Render Times (ms)"]["Average"] = doubleListAverage(pillarLoadTimes);
            foreach (int i in pillarLoadTimes)
            {
                json["Pillar Generation and Render Times (ms)"]["Data"].Add(i);
            }
        }


            // chunk block generation times
        if (chunkBlockGenerationTimes.Count == 0)
        {
            json["Chunk Block List Generation Times (ms)"]["Average"] = "NULL";
            json["Chunk Block List Generation Times (ms)"]["Data"] = "NULL";
        }
        else
        {
            json["Chunk Block List Generation Times (ms)"]["Average"] = doubleListAverage(chunkBlockGenerationTimes);
            foreach (int i in chunkBlockGenerationTimes)
            {
                json["Chunk Block List Generation Times (ms)"]["Data"].Add(i);
            }
        }


            // voxel rendering times
        if (voxelRenderTimes.Count == 0)
        {
            json["Voxel Rendering Time (ms)"]["Average"] = "NULL";
        }
        else
        {
            json["Voxel Rendering Time (ms)"]["Average"] = doubleListAverage(voxelRenderTimes);
            // individual data points are not saved here, as there may be hundreds of thousands of them, and only the average is needed
        }

            // chunk render times
        if (chunkRenderTimes.Count == 0)
        {
            json["Chunk Rendering Times (ms)"]["Average"] = "NULL";
            json["Chunk Rendering Times (ms)"]["Data"] = "NULL";
        }
        else
        {
            json["Chunk Rendering Times (ms)"]["Average"] = doubleListAverage(chunkRenderTimes);
            foreach (int i in chunkRenderTimes)
            {
                json["Chunk Rendering Times (ms)"]["Data"].Add(i);
            }
        }




        // write and save to JSON file
        StringBuilder sb = new();
        json.WriteToStringBuilder(sb,0,4,JSONTextMode.Indent);
        System.IO.File.WriteAllText(filePath+testName+".json", sb.ToString());
    }



}

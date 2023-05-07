using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MenuSettings : MonoBehaviour
{
    [SerializeField] Testing testingClass;
    public WorldGeneration worldGen;
    public CameraController cameraController;
    [Header("Setting input fields")]
    public MenuInputChunkSize menuInputChunkSize;
    public MenuInputWorldSize menuInputWorldSize;
    public MenuInputSeed menuInputSeed;
    public MenuInputHillMax menuInputHillMax;
    public MenuInputHillMin menuInputHillMin;
    public MenuInputCullInternal menuInputCullInternal;
    public MenuInputRandomSeed menuInputRandomSeed;
    public MenuInputgenerateRandomBlocks menuInputgenerateRandomBlocks;
    public MenuInputUseWorldGen menuInputUseWorldGen;
    public MenuGenerateMinimalFaceObj menuGenerateMinimalFaceObj;
    public MenugenerateBlocksAsCameraMoves MenugenerateBlocksAsCameraMoves;
    public MenuRemoveBlocksAsCameraMoves menuRemoveBlocksAsCamera;
    public MenuobjectPoolingPillars menuobjectPoolingPillars;
    public MenuObjectPoolingFaces menuObjectPoolingFaces;
    public MenuhillCurve menuHillCurve;
    public MenuCaveNarrowAtSurface menuCaveNarrowAtSurface;
    public MenuProcedurallyGenerateOnButton menuGenQPress;
    public menuViewFrustumCulling menuViewFrustumCulling;
    public MenuViewFrustumCullingChunks MenuViewFrustumCullingChunks;
    public MenuViewFrustumCullingCollider MenuViewFrustumCullingCollider;



    int currentPage = 1;
    public GameObject[] pages;
    public TextMeshProUGUI pageCounter;

    public void prevPage()
    {
        pages[currentPage - 1].SetActive(false);

        currentPage--;
        if(currentPage <= 0)
        {
            currentPage = pages.Length;
        }
        pages[currentPage - 1].SetActive(true);
        pageCounter.text = "Page " + currentPage.ToString() + "/" + pages.Length.ToString();
    }


    public void nextPage()
    {
        pages[currentPage - 1].SetActive(false);

        currentPage++;
        if (currentPage > pages.Length)
        {
            currentPage = 1;
        }
        pages[currentPage - 1].SetActive(true);
        pageCounter.text = "Page " + currentPage.ToString() + "/" + pages.Length.ToString();
    }

    public void Start()
    {

        pageCounter.text = "Page " + currentPage.ToString() + "/" + pages.Length.ToString();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void generateWorld()
    {
        WorldSettings.Instance.chunkSizeinVoxels = new Vector3(int.Parse(menuInputChunkSize.x.text),
            int.Parse(menuInputChunkSize.y.text),
            int.Parse(menuInputChunkSize.z.text));

        WorldSettings.Instance.renderedWorldSizeinChunks = new Vector3(int.Parse(menuInputWorldSize.x.text),
            int.Parse(menuInputWorldSize.y.text),
            int.Parse(menuInputWorldSize.z.text));

        WorldSettings.Instance.seedInt = int.Parse(menuInputSeed.inputField.text);

        worldGen.hillMaxHeight = short.Parse(menuInputHillMax.inputField.text);

        worldGen.valleyMinHeight = short.Parse(menuInputHillMin.inputField.text);

        WorldSettings.Instance.cullHiddenVoxelFaces = menuInputCullInternal.toggle.isOn;

        WorldSettings.Instance.generateRandomBlocks = menuInputgenerateRandomBlocks.toggle.isOn;

        WorldSettings.Instance.useRandomSeed = menuInputRandomSeed.toggle.isOn;

        if (menuInputRandomSeed.toggle.isOn)
        {
            WorldSettings.Instance.seedInt = UnityEngine.Random.Range(0, 9999999);
        }
        

        WorldSettings.Instance.useChunkInputBlocks = menuInputUseWorldGen.toggle.isOn;

        WorldSettings.Instance.generateMinimalFaceObjects = menuGenerateMinimalFaceObj.toggle.isOn;

        WorldSettings.Instance.generateTerrainAsCameraMoves = MenugenerateBlocksAsCameraMoves.toggle.isOn;

        WorldSettings.Instance.removeTerrainAsCameraMoves = menuRemoveBlocksAsCamera.toggle.isOn;

        WorldSettings.Instance.objectPoolingPillars = menuobjectPoolingPillars.toggle.isOn;

        WorldSettings.Instance.objectPoolingVoxelFaces = menuObjectPoolingFaces.toggle.isOn;

        WorldSettings.Instance.useHillCurve = menuHillCurve.toggle.isOn;

        WorldSettings.Instance.cavesNarrowAtSurface = menuCaveNarrowAtSurface.toggle.isOn;

        WorldSettings.Instance.generateTerrainOnQPress=    menuGenQPress.toggle.isOn;

        WorldSettings.Instance.viewFrustumCulling = menuViewFrustumCulling.toggle.isOn;

        WorldSettings.Instance.viewFrustumCullingChunkwise = MenuViewFrustumCullingChunks.toggle.isOn;

        WorldSettings.Instance.viewFrustumCullingColliders = MenuViewFrustumCullingCollider.toggle.isOn;


        cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        testingClass.gameStarted = true;
        testingClass.testStartTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
        Destroy(this.gameObject);
    }
}

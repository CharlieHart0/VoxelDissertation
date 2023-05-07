using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFrustumCullingColliders : MonoBehaviour
{
    [SerializeField] Chunk chunk;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Enter " + gameObject.transform.position.ToString());
        if (other.CompareTag("MainCamera"))
        {
            chunk.renderTheChunk = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Trigger Exit " + gameObject.transform.position.ToString());
        if (other.CompareTag("MainCamera"))
        {
            
            chunk.renderTheChunk = false;

        }
    }
}

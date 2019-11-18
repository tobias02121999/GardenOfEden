using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram : MonoBehaviour
{
    // Initialize the public variables
    public GameObject spawnPrefab;
    public PaintRenderer paintRenderer;

    // Initialize the private variables
    bool hasCollided;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasCollided)
        {
            Instantiate(spawnPrefab, transform.position, transform.rotation);
            paintRenderer.lineRenderer.positionCount = 0;
            Destroy(this.gameObject);
        }
    }

    // Check if the player is touching the hologram
    void OnTriggerEnter(Collider other)
    {
        hasCollided = true;
    }
}

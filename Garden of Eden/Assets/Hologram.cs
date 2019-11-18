using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram : MonoBehaviour
{
    // Initialize the public variables
    public GameObject spawnPrefab;
    public PaintRenderer paintRenderer;
    public float paintCost;

    // Initialize the private variables
    bool hasCollided;
    PlayerInventory inventory;
    NetworkPlayers players;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.Find("Network Manager").GetComponent<NetworkPlayers>();
        inventory = players.localPlayer.GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        inventory.paintToSpend = paintCost;

        if (hasCollided && inventory.paint >= paintCost)
        {
            Instantiate(spawnPrefab, transform.position, transform.rotation);
            paintRenderer.lineRenderer.positionCount = 0;
            inventory.paint -= paintCost;
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        inventory.paintToSpend = 0f;
    }

    // Check if the player is touching the hologram
    void OnTriggerEnter(Collider other)
    {
        hasCollided = true;
    }
}

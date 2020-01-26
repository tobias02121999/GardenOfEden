using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hologram : NetworkBehaviour
{
    // Initialize the public variables
    public GameObject spawnPrefab;
    public PaintRenderer paintRenderer;
    public float paintCost;
    public bool isHome;
    public bool isFarm;
    public Transform animationTransform;

    // Initialize the private variables
    bool hasCollided;
    PlayerInventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        inventory.paintToSpend = paintCost;

        if (hasCollided && inventory.paint >= paintCost && spawnPrefab != null)
        {
            //Instantiate(spawnPrefab, transform.position, transform.rotation); Instantiate(spawnPrefab);
            var setup = GetComponent<HologramSetup>();
            CmdSpawnObject(isHome, isFarm, setup.teamID);

            paintRenderer.lineRenderer.positionCount = 0;
            inventory.paint -= paintCost;
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
            inventory.paintToSpend = 0f;
    }

    // Check if the player is touching the hologram
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
            hasCollided = true;
    }

    [Command]
    void CmdSpawnObject(bool _isHome, bool _isFarm, int teamID)
    {
        var obj = Instantiate(spawnPrefab, animationTransform.position, animationTransform.rotation); Instantiate(spawnPrefab);

        if (_isHome)
        {
            if (teamID == 0)
                GameManager.Instance.teamOneHomes.Add(obj);

            if (teamID == 1)
                GameManager.Instance.teamTwoHomes.Add(obj);
        }

        if (_isFarm)
        {
            if (teamID == 0)
                GameManager.Instance.teamOneFarms.Add(obj);

            if (teamID == 1)
                GameManager.Instance.teamTwoFarms.Add(obj);
        }

        NetworkServer.Spawn(obj);
    }
}

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

        if (hasCollided && inventory.paint >= paintCost && spawnPrefab != null)
        {
            var teamID = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerSetup>().teamID;
            CmdSpawnObject(teamID, isHome, isFarm);

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
    void CmdSpawnObject(int teamID, bool _isHome, bool _isFarm)
    {
        var obj = Instantiate(spawnPrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(obj);

        if (teamID == 0)
        {
            if (_isHome)
                GameManager.Instance.teamOneHomes.Add(obj);

            if (_isFarm)
                GameManager.Instance.teamOneFarms.Add(obj);
        }

        if (teamID == 1)
        {
            if (_isHome)
                GameManager.Instance.teamTwoHomes.Add(obj);

            if (_isFarm)
                GameManager.Instance.teamTwoFarms.Add(obj);
        }
    }
}

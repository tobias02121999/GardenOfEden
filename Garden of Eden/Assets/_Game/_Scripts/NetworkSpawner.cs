using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSpawner : NetworkBehaviour
{
    // Initialize the public variables
    public GameObject prefab;
    public Transform spawnPos;

    // Initialize the private variables
    bool hasSpawned;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();

        if (isServer)
            Debug.Log("Konten");
    }

    // Spawn the prefab based on user input
    [Client]
    void Spawn()
    {
        if (!hasSpawned)
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0f || Input.GetMouseButtonDown(0))
            {
                CmdSpawn(isLocalPlayer);
                hasSpawned = true;
            }
        }
        else
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 0f)
                hasSpawned = false;
        }
    }

    // Spawn the prefab over the network
    [Command]
    void CmdSpawn(bool isLocal)
    {
        //var obj = Instantiate(prefab, spawnPos.position, Quaternion.identity);
        var obj = ObjectPooler.Instance.SpawnFromPool("Humans", spawnPos.position, Quaternion.identity);
        NetworkServer.Spawn(obj);

        if (isLocal)
            obj.GetComponentInChildren<InteractionDetection>().localPlayer = GetComponent<PlayerControls>();
    }
}

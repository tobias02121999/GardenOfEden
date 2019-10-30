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
    }

    // Spawn the prefab based on user input
    [Client]
    void Spawn()
    {
        if (!hasSpawned)
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0f || Input.GetMouseButtonDown(0))
            {
                CmdSpawn("Humans", spawnPos.position);
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
    public void CmdSpawn(string poolName, Vector3 spawnPosition)
    {
        //var obj = Instantiate(prefab, spawnPos.position, Quaternion.identity);
        var obj = ObjectPooler.Instance.SpawnFromPool(poolName, spawnPos.position, Quaternion.identity);
        NetworkServer.Spawn(obj);
    }

    [Command]
    public void CmdSpawnHouse(Transform spawnPosition, bool spawnedByHuman)
    {
        if (spawnedByHuman)
        {
            var house = ObjectPooler.Instance.SpawnFromPool("House", spawnPosition.position + new Vector3(0, 0, 6), Quaternion.identity);
            NetworkServer.Spawn(house);
        }
        else 
        {
            var house = ObjectPooler.Instance.SpawnFromPool("House", spawnPosition.position, Quaternion.identity);
            NetworkServer.Spawn(house);
        }
    }
}

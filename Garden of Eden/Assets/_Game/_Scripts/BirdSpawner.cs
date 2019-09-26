using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BirdSpawner : NetworkBehaviour
{
    // Initialize the public variables
    public GameObject bird;
    public Transform[] spawnPoints;
    public Vector2 spawnDuration, spawnHeight;
    public Transform target;
    public float flightDeviation;

    // Initialize the private variables
    int spawnAlarm;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnBird(); // Spawn a bird
    }

    // Spawn a bird
    void SpawnBird()
    {
        if (spawnAlarm <= 0f)
        {
            var length = spawnPoints.Length;
            var index = Mathf.RoundToInt(Random.Range(0f, length - 1f));
            var spawn = spawnPoints[index].position;
            spawn.y += Random.Range(spawnHeight.x, spawnHeight.y);

            CmdSpawnBird(spawn);

            spawnAlarm = Mathf.RoundToInt(Random.Range(spawnDuration.x, spawnDuration.y));
        }
        else
            spawnAlarm--;
    }

    // Spawn a bird over the network
    [Command]
    void CmdSpawnBird(Vector3 pos)
    {
        var obj = Instantiate(bird, pos, Quaternion.identity);
        var targetPos = target.position + new Vector3(Random.Range(-flightDeviation, flightDeviation), 0f, Random.Range(-flightDeviation, flightDeviation));
        var lookPos = targetPos - obj.transform.position;
        lookPos.y = 0f;
        var rotation = Quaternion.LookRotation(lookPos);
        obj.transform.rotation = rotation;

        NetworkServer.Spawn(obj);
    }
}

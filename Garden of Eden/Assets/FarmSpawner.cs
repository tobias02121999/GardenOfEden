using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FarmSpawner : NetworkBehaviour
{
    // Initialize the public enums
    public enum UnitType { NEUTRAL, TEAM_1, TEAM_2 }

    // Initialize the public variables
    public GameObject farmPrefab;
    public UnitType type;
    public int spawnDuration;
    public bool loop;

    // Initialize the private variables
    int spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
            Timer(); // Control the spawn timer and spawn a unit when it reaches 0
        else
            Debug.Log("Not running spawner!");
    }

    // Control the spawn timer and spawn a unit when it reaches 0
    void Timer()
    {
        spawnTimer--;

        if (spawnTimer == 0)
        {
            Spawn(); // Spawn the desired unit

            if (loop)
                spawnTimer = spawnDuration;
            else
                spawnTimer = -1;
        }
    }

    // Spawn the desired unit
    void Spawn()
    {
        switch (type)
        {
            case UnitType.NEUTRAL:
                //CmdSpawnServer(); // Spawn a neutral object (server-side)
                break;

            case UnitType.TEAM_1:
                CmdSpawnTeam1(); // Spawn an object in the name of team 1 (server-side)
                break;

            case UnitType.TEAM_2:
                CmdSpawnTeam2(); // Spawn an object in the name of team 2 (client-side)
                break;
        }
    }

    [Command]
    void CmdSpawnTeam1()
    {
        var obj = Instantiate(farmPrefab, transform);
        obj.transform.parent = null;

        GameManager.Instance.teamOneFarms.Add(obj);

        NetworkServer.Spawn(obj);
    }

    [Command]
    void CmdSpawnTeam2()
    {
        var obj = Instantiate(farmPrefab, transform);
        obj.transform.parent = null;

        GameManager.Instance.teamTwoFarms.Add(obj);

        NetworkServer.Spawn(obj);
    }
}

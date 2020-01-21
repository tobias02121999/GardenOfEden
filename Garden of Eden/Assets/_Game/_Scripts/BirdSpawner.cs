using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BirdSpawner : NetworkBehaviour
{
    // Initialize the public variables
    public GameObject bird, human;
    public Transform[] spawnPoints;
    public Vector2 spawnDuration, spawnHeight;
    public Transform target;
    public float flightDeviation;
    public bool isStork;

    // Initialize the private variables
    int spawnAlarm = 500;
    
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

            if (isStork)
            {
                for (var i = 0; i < length; i++)
                {
                    var pos = spawnPoints[i].position;
                    CmdSpawnBird(pos, i, true); // Spawn a bird over the network
                }
            }
            else
            {
                var index = Mathf.RoundToInt(Random.Range(0f, length - 1f));
                var spawn = spawnPoints[index].position;
                spawn.y += Random.Range(spawnHeight.x, spawnHeight.y);

                CmdSpawnBird(spawn, 0, false); // Spawn a bird over the network
            }

            spawnAlarm = Mathf.RoundToInt(Random.Range(spawnDuration.x, spawnDuration.y));
        }
        else
            spawnAlarm--;
    }

    [Command] // Spawn a bird over the network
    void CmdSpawnBird(Vector3 pos, int teamID, bool isStork)
    {
        // Spawn a bird (or stork)
        var obj = Instantiate(bird, pos, Quaternion.identity);
        var targetPos = target.position + new Vector3(Random.Range(-flightDeviation, flightDeviation), 0f, Random.Range(-flightDeviation, flightDeviation));
        var lookPos = targetPos - obj.transform.position;
        lookPos.y = 0f;
        var rotation = Quaternion.LookRotation(lookPos);
        obj.transform.rotation = rotation;

        if (isStork)
        {
            obj.GetComponent<Stork>().teamID = teamID;

            // Spawn a human
            var _parent = obj.GetComponent<Stork>().humanParent;
            var _obj = Instantiate(human, _parent.position, Quaternion.identity);
            _obj.GetComponent<HumanAI>().storkID = GetComponent<NetworkIdentity>().netId;
            _obj.GetComponent<HumanAI>().isStork = true;

            NetworkServer.Spawn(obj);
            NetworkServer.Spawn(_obj);

            if (teamID == 0)
            {
                GameManager.Instance.teamOneStorks.Add(obj);
                GameManager.Instance.TeamOneHumans.Add(_obj);

                _obj.transform.parent = obj.GetComponent<Stork>().humanParent;
            }

            if (teamID == 1)
            {
                GameManager.Instance.teamTwoStorks.Add(obj);
                GameManager.Instance.TeamTwoHumans.Add(_obj);

                var storkID = obj.GetComponent<NetworkIdentity>().netId;
                var humanID = _obj.GetComponent<NetworkIdentity>().netId;
                RpcParentStorkClient(storkID, humanID); // Parent the spawned client human to the corresponding client stork
            }
        }
    }

    [ClientRpc] // Parent the spawned client human to the corresponding client stork
    void RpcParentStorkClient(NetworkInstanceId storkID, NetworkInstanceId humanID)
    {
        var stork = ClientScene.FindLocalObject(storkID);
        var human = ClientScene.FindLocalObject(humanID);
        human.transform.parent = stork.GetComponent<Stork>().humanParent;

        Debug.Log("Parent Human To Stork");
    }
}

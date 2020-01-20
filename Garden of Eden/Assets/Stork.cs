using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Stork : NetworkBehaviour
{
    // Initialize the public variables
    public GameObject humanPrefab;
    public Transform humanParent;
    public GameObject human;
    public Rigidbody humanHips;

    [SyncVar]
    public int teamID;

    // Initialize the private variables
    int timer = 500;
    bool humanSpawned;

    // Start is called before the first frame update
    void Start()
    {
        if ((isServer && teamID == 0) || (!isServer && teamID == 1))
        {
            if (teamID == 0)
                CmdSpawnTeam1();

            if (teamID == 1)
            {
                var id = GetComponent<NetworkIdentity>().netId;
                CmdSpawnTeam2(id);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((isServer && teamID == 0) || (!isServer && teamID == 1))
        {
            humanHips = human.transform.Find("mixamorig:Hips").GetComponent<Rigidbody>();
            humanHips.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            human.GetComponent<RagdollAnimator>().impactRecovery = 0f;
        }
    }

    // Spawn the human prefab
    [Command]
    void CmdSpawnTeam1()
    {
        var obj = Instantiate(humanPrefab, transform);
        human = obj;

        GameManager.Instance.TeamOneHumans.Add(obj);

        NetworkServer.Spawn(obj);
    }

    [Command]
    void CmdSpawnTeam2(NetworkInstanceId storkID)
    {
        var obj = Instantiate(humanPrefab, transform);

        GameManager.Instance.TeamTwoHumans.Add(obj);

        NetworkServer.Spawn(obj);

        var id = obj.GetComponent<NetworkIdentity>().netId;
        GameManager.Instance.RpcBounceHuman(storkID, id);
    }
}

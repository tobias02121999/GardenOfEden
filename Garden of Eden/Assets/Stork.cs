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
    public int teamID;
    public Material[] teamMaterial;

    [HideInInspector]
    public GameManager gameManager;

    // Initialize the private variables
    NetworkPlayers players;

    // Start is called before the first frame update
    void Start()
    {
        teamID = Mathf.RoundToInt(Random.Range(0f, 1f));

        CmdSpawnHuman();

        humanHips = human.transform.Find("mixamorig:Hips").GetComponent<Rigidbody>();
        humanHips.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        human.GetComponent<RagdollAnimator>().impactRecovery = 0f;

        players = GameObject.Find("Network Manager").GetComponent<NetworkPlayers>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawn the human
    [Command]
    void CmdSpawnHuman()
    {
        human = Instantiate(humanPrefab, humanParent.transform.position, Quaternion.identity);
        human.transform.parent = humanParent;

        if (teamID == 0)
        {
            gameManager.TeamOneHumans.Add(human);
            NetworkServer.Spawn(human);
        }
        else
        {
            gameManager.TeamTwoHumans.Add(human);
            NetworkServer.SpawnWithClientAuthority(human, players.otherPlayer);
        }

        human.transform.Find("Human_BaseMesh").GetComponent<SkinnedMeshRenderer>().material = teamMaterial[teamID];
    }
}

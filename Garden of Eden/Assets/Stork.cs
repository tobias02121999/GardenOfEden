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

    // Start is called before the first frame update
    void Start()
    {
        //teamID = Mathf.RoundToInt(Random.Range(0f, 1f));
        teamID = 0;

        CmdSpawnHuman();

        humanHips = human.transform.Find("mixamorig:Hips").GetComponent<Rigidbody>();
        humanHips.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        human.GetComponent<RagdollAnimator>().impactRecovery = 0f;
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
            gameManager.TeamOneHumans.Add(human);
        else
            gameManager.TeamTwoHumans.Add(human);

        human.transform.Find("Human_BaseMesh").GetComponent<SkinnedMeshRenderer>().material = teamMaterial[teamID];

        NetworkServer.Spawn(human);
    }
}

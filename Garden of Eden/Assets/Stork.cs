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

    // Start is called before the first frame update
    void Start()
    {
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

        NetworkServer.Spawn(human);
    }
}

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
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        humanHips = human.transform.Find("mixamorig:Hips").GetComponent<Rigidbody>();
        humanHips.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        human.GetComponent<RagdollAnimator>().impactRecovery = 0f;
        */
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallSetup : NetworkBehaviour
{
    // Initialize the public variables
    [SyncVar]
    public int teamID;

    // Initialize the private variables
    Rigidbody rb;
    Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the ball object
    }

    // Update is called once per frame
    void Update()
    {
        ToggleComponents(); // Disable the local components if this object is not controlled by the local player
    }

    // Initialize the ball object
    void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponentInChildren<Collider>();
    }

    // Disable the local components if this object is not controlled by the local player
    void ToggleComponents()
    {
        var localPlayerID = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerSetup>().teamID;
        if (teamID != localPlayerID)
        {
            rb.isKinematic = true;
            coll.enabled = false;
        }
        else
        {
            rb.isKinematic = false;
            coll.enabled = true;
        }
    }
}

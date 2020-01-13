using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HouseSetup : NetworkBehaviour
{
    // Initialize the public variables
    [SyncVar]
    public int teamID;

    public Behaviour[] nonLocalComponents;

    // Initialize the private variables
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ToggleComponents(); // Disable the local components if this object is not controlled by the local player
    }

    // Disable the local components if this object is not controlled by the local player
    void ToggleComponents()
    {
        var localPlayerID = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerSetup>().teamID;
        var isLocal = (teamID == localPlayerID);

        var length = nonLocalComponents.Length;
        for (var i = 0; i < length; i++)
            nonLocalComponents[i].enabled = isLocal;
    }
}

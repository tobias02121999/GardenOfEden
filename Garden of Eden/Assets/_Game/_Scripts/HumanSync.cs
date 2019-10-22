using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HumanSync : NetworkBehaviour
{
    // Initialize the public variables
    [HideInInspector]
    public NetworkPlayers networkPlayers;

    [HideInInspector]
    public NetworkIdentity identity;

    [HideInInspector]
    public InteractionDetection detection;

    void Start()
    {
        networkPlayers = FindObjectOfType<NetworkPlayers>();
        identity = GetComponent<NetworkIdentity>();
        detection = GetComponentInChildren<InteractionDetection>();
    }

    void Update()
    {
        if (detection.otherPlayerColliding)
            networkPlayers.localPlayer.GetComponent<PlayerControls>().CmdSetAuthorityClient(identity);
        else
            networkPlayers.localPlayer.GetComponent<PlayerControls>().CmdClearAuthority(identity);
    }
}

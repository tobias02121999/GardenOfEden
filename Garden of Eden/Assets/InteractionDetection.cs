using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetection : MonoBehaviour
{
    // Initialize the public variables
    [HideInInspector]
    public NetworkPlayers networkPlayers;

    public bool localPlayerColliding;
    public bool otherPlayerColliding;

    // Start is called before the first frame update
    void Start()
    {
        networkPlayers = FindObjectOfType<NetworkPlayers>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        var _local = other.gameObject.GetComponentInParent<PlayerControls>().gameObject;
        var _other = other.gameObject.GetComponentInParent<PlayerControls>().gameObject;

        if (_local != null && _local == networkPlayers.localPlayer)
            localPlayerColliding = true;
        else
            localPlayerColliding = false;

        if (_other != null && _other == networkPlayers.otherPlayer)
            otherPlayerColliding = true;
        else
            otherPlayerColliding = false;
    }

    void OnTriggerExit()
    {
        localPlayerColliding = false;
        otherPlayerColliding = false;
    }
}

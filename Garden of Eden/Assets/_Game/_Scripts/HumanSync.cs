using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HumanSync : MonoBehaviour
{
    // Initialize the public variables
    [HideInInspector]
    public NetworkPlayers networkPlayers;

    [HideInInspector]
    public NetworkIdentity identity;

    // Initialize the private variables
    bool serverInControl;

    // Start is called before the first frame update
    void Start()
    {
        networkPlayers = FindObjectOfType<NetworkPlayers>();
        identity = GetComponentInParent<NetworkIdentity>();
    }

    // Update is called once per frame
    void Update()
    {
        identity.localPlayerAuthority = serverInControl;
    }

    // 
    void OnTriggerStay(Collider other)
    {
        var obj = other.GetComponentInParent<PlayerController>().gameObject;
        if (obj == networkPlayers.otherPlayer)
            serverInControl = false;
        else
            serverInControl = true;
    }

    void OnTriggerExit(Collider other)
    {
        serverInControl = true;
    }
}

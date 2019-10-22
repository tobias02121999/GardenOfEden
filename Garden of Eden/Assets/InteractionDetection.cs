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
        localPlayerColliding = (other.GetComponentInParent<PlayerControls>().gameObject == networkPlayers.localPlayer);
        otherPlayerColliding = (other.GetComponentInParent<PlayerControls>().gameObject == networkPlayers.otherPlayer);
    }
}

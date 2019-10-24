using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetection : MonoBehaviour
{
    // Initialize the public variables
    public PlayerControls localPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponentInParent<PlayerControls>();

        if (!localPlayer.isServer)
        {
            if (!player.isLocalPlayer)
                localPlayer.CmdSetClientAuthority(this.gameObject);
            else
                localPlayer.CmdClearAuthority(this.gameObject);
        }
    }
}

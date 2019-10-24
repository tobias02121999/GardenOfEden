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
        if (!localPlayer.isServer)
            localPlayer.CmdSetClientAuthority(this.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (!localPlayer.isServer)
            localPlayer.CmdClearAuthority(this.gameObject);
    }
}

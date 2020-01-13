using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetection : MonoBehaviour
{
    // Initialize the public variables
    public PlayerControls localPlayer;
    public bool clientAuthority;

    // Initialize the private variables
    RagdollSetup ragdollSetup;

    // Start is called before the first frame update
    void Start()
    {
        var players = FindObjectsOfType<PlayerControls>();
        for (var i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer)
                localPlayer = players[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 
}

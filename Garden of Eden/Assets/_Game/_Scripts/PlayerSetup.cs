using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    // Initialize the public variables
    public bool isOther;
    public Behaviour[] nonLocalComponents;
    public GameObject[] nonLocalObjects;
    public GameObject[] playerMasks;
    public Material[] teamMaterials;
    public Renderer[] handRenderers;
    public GameObject maskCloud;

    //[HideInInspector]
    public int teamID;

    // Start is called before the first frame update
    void Start()
    {
        DisableComponents(); // Disable the local components if this object is not controlled by the local player
        AssignTeamID(); // Assign the correct team ID to each player

        var length = playerMasks.Length;
        for (var i = 0; i < length; i++)
            playerMasks[i].SetActive(i == teamID);

        length = handRenderers.Length;
        for (var i = 0; i < length; i++)
            handRenderers[i].material = teamMaterials[teamID];
    }

    // Disable the local components if this object is not controlled by the local player
    void DisableComponents()
    {
        if (!isLocalPlayer)
        {
            var length = nonLocalComponents.Length;
            for (var i = 0; i < length; i++)
                nonLocalComponents[i].enabled = false;

            length = nonLocalObjects.Length;
            for (var i = 0; i < length; i++)
                nonLocalObjects[i].SetActive(false);

            NetworkPlayers.Instance.otherPlayer = gameObject;

            isOther = true;

            length = playerMasks.Length;
            for (var i = 0; i < length; i++)
                playerMasks[i].layer = 0;

            maskCloud.layer = 0;
        }
        else
        {
            NetworkPlayers.Instance.localPlayer = gameObject;

            var length = playerMasks.Length;
            for (var i = 0; i < length; i++)
                playerMasks[i].layer = 31;

            maskCloud.layer = 31;
        }
    }

    // Assign the correct team ID to each player
    void AssignTeamID()
    {
        if (isLocalPlayer)
        {
            if (!isServer)
                teamID = 1;
        }
        else
        {
            if (isServer)
                teamID = 1;
        }
    }

    [Command] // Send the client variables over to the server
    public void CmdManagerToServer(float score)
    {
        GameManager.Instance.teamTwoFoodScore = score;
    }
}

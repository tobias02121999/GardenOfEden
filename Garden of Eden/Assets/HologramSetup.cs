using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramSetup : MonoBehaviour
{
    // Initialize the public variables
    public int teamID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var comp = GetComponent<Hologram>();

        comp.enabled = (teamID == NetworkPlayers.Instance.localPlayer.GetComponent<PlayerSetup>().teamID);
    }
}

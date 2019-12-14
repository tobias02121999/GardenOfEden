using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    // Initialize the public variables
    public AuraAPI.AuraLight[] auraLights;
    public Sun sun;
    public NetworkPlayers networkPlayers;

    // Initialize the private variables
    GameObject localPlayer;

    // Initialize the private variables
    bool auraIsLit;

    // Start is called before the first frame update
    void Start()
    {
        localPlayer = networkPlayers.localPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        AuraLightsInit(); // Initialize the aura lights
    }

    // Initialize the aura lights
    void AuraLightsInit()
    {
        if (localPlayer != null)
        {
            var cam = localPlayer.transform.Find("CenterEyeAnchor").gameObject;
            cam.AddComponent<AuraAPI.Aura>();
            Debug.Log("Component Added");

            //sun.aura = localPlayer.GetComponentInChildren<AuraAPI.Aura>();

            var length = auraLights.Length;

            for (var i = 0; i < length; i++)
            {
                auraLights[i].enabled = true;

                if (i == length - 1)
                    auraIsLit = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    // Initialize the public variables
    public AuraAPI.AuraLight[] auraLights;
    public Sun sun;

    // Initialize the private variables
    bool auraIsLit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AuraLightsInit(); // Initialize the aura lights
    }

    // Initialize the aura lights
    void AuraLightsInit()
    {
        var player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            sun.aura = player.GetComponentInChildren<AuraAPI.Aura>();

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

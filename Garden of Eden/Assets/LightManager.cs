using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    // Initialize the public variables
    public AuraAPI.AuraLight[] sceneLights;
    public Sun sun;

    // Initialize the private variables
    bool lightsAreAwake;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WakeLights(); // Wake the scene lights as soon as the local player has spawned in
    }

    // Wake the scene lights as soon as the local player has spawned in
    void WakeLights()
    {
        var playerExists = (NetworkPlayers.Instance.localPlayer != null);

        if (playerExists && !lightsAreAwake)
        {
            var length = sceneLights.Length;
            for (var i = 0; i < length; i++)
                sceneLights[i].enabled = true;

            sun.aura = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerInventory>().headTransform.GetComponent<AuraAPI.Aura>();

            lightsAreAwake = true;
        }
    }
}

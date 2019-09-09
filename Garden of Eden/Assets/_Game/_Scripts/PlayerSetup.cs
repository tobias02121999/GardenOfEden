using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] nonLocalComponents;

    Camera lobbyCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < nonLocalComponents.Length; i++)
            {
                nonLocalComponents[i].enabled = false;
            }
        }
        else
        {
            lobbyCamera = Camera.main;
            lobbyCamera.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (lobbyCamera != null)
        {
            lobbyCamera.gameObject.SetActive(true);
        }
    }
}

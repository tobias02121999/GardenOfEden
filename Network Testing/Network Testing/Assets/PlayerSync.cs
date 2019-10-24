using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSync : NetworkBehaviour
{
    // Initialize the public variables
    public GameObject cube;
    public Text cubeInfo1, cubeInfo2;

    // Initialize the private variables
    bool cubeSpawned, authoritySet;
    GameObject cubeInstance;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            var cam = GetComponent<Camera>();
            cam.enabled = false;

            var player = GetComponent<Player>();
            player.enabled = false;

            var canvas = GetComponentInChildren<Canvas>().gameObject;
            canvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetButtonDown("Fire1") && !cubeSpawned)
            {
                CmdSpawnCube();
                cubeSpawned = true;
            }

            if (Input.GetButtonDown("Fire2") && ! authoritySet)
            {
                CmdSetClientAuthority();
                authoritySet = true;
            }

            if (cubeSpawned)
                cubeInfo1.text = "Cube Spawned: True";
            else
                cubeInfo1.text = "Cube Spawned: False";

            if (authoritySet)
                cubeInfo2.text = "Authority: Client";
            else
                cubeInfo2.text = "Authority: Host";
        }
    }

    [Command]
    void CmdSpawnCube()
    {
        cubeInstance = (GameObject)Instantiate(cube, new Vector3(0f, 0f, 0f), Quaternion.identity);
        //NetworkServer.SpawnWithClientAuthority(obj, connectionToClient);
        NetworkServer.Spawn(cubeInstance);
    }

    [Command]
    void CmdSetClientAuthority()
    {
        var identity = cubeInstance.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(connectionToClient);
    }
}

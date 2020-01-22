using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Monument : NetworkBehaviour
{
    // Initialize the public variables
    [SyncVar]
    public int teamID;

    public GameObject[] masks;
    public int buildState;
    public int buildTarget;

    //[HideInInspector]
    public int buildProgress;

    // Initialize the private variables
    bool hasRun;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var length = masks.Length;
        for (var i = 0; i < length; i++)
            masks[i].SetActive(i == teamID);

        var chunk = buildTarget / 4;
        if (buildProgress >= chunk && buildProgress < (chunk * 2))
            buildState = 1;

        if (buildProgress >= (chunk * 2) && buildProgress < (chunk * 3))
            buildState = 2;

        if (buildProgress >= buildTarget)
            buildState = 3;

        animator.SetInteger("buildState", buildState);

        if (teamID == 0 && isServer)
            RpcSyncToClient(buildProgress);

        if (teamID == 1 && !isServer)
            CmdSyncToServer(buildProgress);
    }

    [Command]
    void CmdSyncToServer(int value)
    {
        buildProgress = value;
    }

    [ClientRpc]
    void RpcSyncToClient(int value)
    {
        buildProgress = value;
    }
}

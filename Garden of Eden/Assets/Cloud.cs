using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Cloud : NetworkBehaviour
{
    // Initialize the public variables
    public float cloudDuration;
    public GameObject target;
    public bool hasTarget;
    public NetworkIdentity identity;

    // Initialize the private variables
    CloudSetup setup;

    // Start is called before the first frame update
    void Start()
    {
        setup = GetComponent<CloudSetup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (setup.teamID == NetworkPlayers.Instance.localPlayer.GetComponent<PlayerSetup>().teamID)
        {
            cloudDuration--;

            if (cloudDuration <= 0f)
                Destroy(this.gameObject);

            if (target != null)
            {
                var ID = target.GetComponentInParent<NetworkIdentity>().netId;
                identity = target.GetComponentInParent<NetworkIdentity>();

                if (target.CompareTag("Farm"))
                    CmdFarmWet(ID, hasTarget);

                if (target.CompareTag("Sapling"))
                    CmdSaplingWet(ID, hasTarget);
            }
        }
    }

    // Tell the target object if its wet or not
    [Command]
    public void CmdFarmWet(NetworkInstanceId ID, bool toggle)
    {
        var obj = NetworkServer.FindLocalObject(ID);
        obj.GetComponent<Farm>().isWet = toggle;
    }

    [Command]
    public void CmdSaplingWet(NetworkInstanceId ID, bool toggle)
    {
        var obj = NetworkServer.FindLocalObject(ID);
        obj.GetComponent<Tree>().isWet = toggle;
    }
}

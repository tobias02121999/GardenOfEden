using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RainCollider : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ResetRotation();
    }

    // Make sure the collider always faces down
    void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    // Tell the object it's colliding with that it's getting rained on
    void OnTriggerStay(Collider other)
    {
        var cloud = GetComponentInParent<Cloud>();
        var ID = other.GetComponentInParent<NetworkIdentity>().netId;

        if (other.CompareTag("Farm"))
            cloud.CmdFarmWet(ID, true);

        if (other.CompareTag("Sapling"))
            cloud.CmdSaplingWet(ID, true);
    }

    // Tell the object it's colliding with that it's not getting rained on
    void OnTriggerExit(Collider other)
    {
        var cloud = GetComponentInParent<Cloud>();
        var ID = other.GetComponentInParent<NetworkIdentity>().netId;

        if (other.CompareTag("Farm"))
            cloud.CmdFarmWet(ID, false);

        if (other.CompareTag("Sapling"))
            cloud.CmdSaplingWet(ID, false);
    }
}

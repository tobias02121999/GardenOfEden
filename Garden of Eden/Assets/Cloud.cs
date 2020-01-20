using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Cloud : NetworkBehaviour
{
    // Initialize the public variables
    public float cloudDuration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cloudDuration--;

        if (cloudDuration <= 0f)
            Destroy(this.gameObject);
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

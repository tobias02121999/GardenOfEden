using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyAtBorder : NetworkBehaviour
{
    public Vector3 borderSize;
    public Transform targetTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((targetTransform.position.x < -borderSize.x || targetTransform.position.x > borderSize.x) ||
            (targetTransform.position.y < -borderSize.y || targetTransform.position.y > borderSize.y) ||
            (targetTransform.position.z < -borderSize.z || targetTransform.position.z > borderSize.z))
            CmdDestroy(); // Destroy the object over the network
    }

    // Destroy the object over the network
    [Command]
    void CmdDestroy()
    {
        Debug.Log("Destroy!");
        NetworkServer.Destroy(this.gameObject);
    }
}

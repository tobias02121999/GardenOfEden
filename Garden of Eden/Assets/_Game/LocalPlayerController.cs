using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayerController : NetworkBehaviour
{
    public Camera localCamera;

    public float movementSpeed;

    Vector3 localPosition;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            Destroy(localCamera);
        }
        else
        {
            // Taking care of the camera when another player joins.
            if (localCamera.tag != "MainCamera")
            {
                localCamera.tag = "MainCamera";
                localCamera.enabled = true;
            }
        }

        localPosition.x = Input.GetAxis("Horizontal");
        localPosition.z = Input.GetAxis("Vertical");

        transform.position += localPosition * movementSpeed * Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBelt : MonoBehaviour
{
    // Initialize the public variables
    public Transform cameraTransform;
    public float flexibility, turnability;
    public float distToHead;
    public float moveSpd, turnSpd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowCam(); // Follow the camera transform
        RotateCam(); // Rotate towards the camera transform
    }

    // Follow the camera transform
    void FollowCam()
    {
        var t = new Vector3();
        var h = cameraTransform.position;
        var tf = flexibility;
        var l = distToHead;

        t.x = Mathf.Clamp(transform.position.x, h.x - tf, h.x + tf);
        t.z = Mathf.Clamp(transform.position.z, h.z - tf, h.z + tf);
        t.y = h.y - l;

        var step = moveSpd * Time.deltaTime;

        transform.position = Vector3.Slerp(transform.position, t, step);
    }

    // Rotate towards the camera transform
    void RotateCam()
    {
        var r = transform.localEulerAngles.y;
        var tr = cameraTransform.localEulerAngles.y;
        var tf = turnability;

        var rotY = Mathf.Clamp(r, tr - tf, tr + tf);

        var step = turnSpd * Time.deltaTime;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, rotY, 0f), step);
    }
}

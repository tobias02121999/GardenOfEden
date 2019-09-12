using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    // Initialize the public variables
    public string inputAxis;
    public float rotSpeed;

    // Initialize the private variables
    float rotY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotY += Input.GetAxis(inputAxis) * rotSpeed;
        transform.rotation = Quaternion.Euler(transform.rotation.x, rotY, transform.rotation.z);
    }
}

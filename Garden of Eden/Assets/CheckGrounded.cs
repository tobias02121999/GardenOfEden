using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{
    // Initialize the public variables
    public Grab grab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check if the house is grounded
    void OnTriggerEnter(Collider other)
    {
        grab.isGrounded = true;
    }

    // Check if the house is not grounded
    void OnTriggerExit(Collider other)
    {
        grab.isGrounded = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{
    // Initialize the public variables
    public Grab grab;
    public HumanAI humanAI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check if the house is grounded
    void OnTriggerStay(Collider other)
    {
        if (grab != null)
            grab.isGrounded = true;

        if (humanAI != null)
            humanAI.isGrounded = true;
    }

    // Check if the house is not grounded
    void OnTriggerExit(Collider other)
    {
        if (grab != null)
            grab.isGrounded = false;

        if (humanAI != null)
            humanAI.isGrounded = false;
    }
}

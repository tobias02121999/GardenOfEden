using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerCollider : MonoBehaviour
{
    // Initialize the public variables
    public PlayerControls playerControls;
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check for collision
    void OnCollisionEnter(Collision collision)
    {
        playerControls.FingerIsColliding[ID] = true;
    }

    // Check for no collision
    void OnCollisionExit(Collision collision)
    {
        playerControls.FingerIsColliding[ID] = false;
    }
}

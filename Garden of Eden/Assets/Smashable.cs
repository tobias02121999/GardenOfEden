using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smashable : MonoBehaviour
{
    // Initialize the public variables
    public float sturdiness;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check if being smashed
    void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Hammer") && collision.relativeVelocity.magnitude >= sturdiness)
            Smashed();
    }

    // Get destroyed
    void Smashed()
    {
        Destroy(this.gameObject);
    }
}

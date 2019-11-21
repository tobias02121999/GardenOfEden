using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smashable : MonoBehaviour
{
    // Initialize the public variables
    public float sturdiness;
    public GameObject poofEffect;
    public Transform poofLocation;

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
        var obj = Instantiate(poofEffect, poofLocation.position, Quaternion.identity);
        obj.transform.parent = null;

        Destroy(this.gameObject);
    }
}

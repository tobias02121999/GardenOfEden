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
        var hand = collision.gameObject.GetComponentInParent<FollowPivot>();

        if (hand != null && collision.relativeVelocity.magnitude >= sturdiness)
        {
            var controls = hand.GetComponentInParent<PlayerControls>();

            if (hand.CompareTag("HandLeft") && controls.isFistL)
                Smashed();

            if (hand.CompareTag("HandRight") && controls.isFistR)
                Smashed();
        }
    }

    // Get destroyed
    void Smashed()
    {
        var obj = Instantiate(poofEffect, poofLocation.position, Quaternion.identity);
        obj.transform.parent = null;

        Destroy(this.gameObject);
    }
}

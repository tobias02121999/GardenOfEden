using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    // Initialize the public variables
    public float speed;
    public GameObject animation, ragdoll;

    // Initialize the private variables
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called once per fixed frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    // Check for collision and if so, turn into a ragdoll
    void OnCollisionEnter(Collision collision)
    {
        animation.SetActive(false);
        ragdoll.SetActive(true);
    }
}

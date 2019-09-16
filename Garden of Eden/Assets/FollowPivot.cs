using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPivot : MonoBehaviour
{
    // Initialize the public variables
    public Transform target;
    public Transform reference;
    public float speed;

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
        Reference(); // Rotate the reference transform towards the target
        Move(); // Move towards the target pivot
    }

    // Rotate the reference transform towards the target
    void Reference()
    {
        reference.LookAt(target);
    }

    // Move towards the target pivot
    void Move()
    {
        var dist = Vector3.Distance(transform.position, target.position);
        var step = dist * speed;

        rb.velocity = reference.forward * step;
    }
}

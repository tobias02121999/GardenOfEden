using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPivot : MonoBehaviour
{
    // Initialize the public variables
    public Transform target;
    public float speed;

    // Initialize the private variables
    Rigidbody rb;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(); // Move towards the target pivot
    }

    // Move towards the target pivot
    void Move()
    {
        var dist = Vector3.Distance(transform.position, target.position);
        var step = dist * speed;

        rb.position = Vector3.Lerp(transform.position, target.position, step);
        rb.rotation = target.rotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPivot : MonoBehaviour
{
    // Initialize the public variables
    public Transform target;
    public Transform reference;
    public float speed, rotationSpeed;

    // Initialize the private variables
    Rigidbody rb;
    float distanceThreshold = 50f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        Reference(); // Rotate the reference transform towards the target
    }

    // FixedUpdate is called once per fixed frame
    private void FixedUpdate()
    {
        Move(); // Move towards the target pivot
        Rotate(); // Rotate towards the target pivot
    }

    // Rotate the reference transform towards the target
    void Reference()
    {
        reference.LookAt(target);
    }

    // Calculate the required torque to equal the rotation to a target rotation
    Vector3 ComputeTorque(Transform targetTransform, Quaternion desiredRotation)
    {
        //q will rotate from our current rotation to desired rotation
        Quaternion q = desiredRotation * Quaternion.Inverse(targetTransform.rotation);

        //convert to angle axis representation so we can do math with angular velocity
        Vector3 x;
        float xMag;
        q.ToAngleAxis(out xMag, out x);
        x.Normalize();

        var rb = targetTransform.GetComponent<Rigidbody>();

        //w is the angular velocity we need to achieve
        Vector3 w = x * xMag * Mathf.Deg2Rad / Time.fixedDeltaTime;
        w -= rb.angularVelocity;

        //to multiply with inertia tensor local then rotationTensor coords
        Vector3 wl = targetTransform.InverseTransformDirection(w);
        Vector3 Tl;
        Vector3 wll = wl;

        wll = rb.inertiaTensorRotation * wll;
        wll.Scale(rb.inertiaTensor);
        Tl = Quaternion.Inverse(rb.inertiaTensorRotation) * wll;
        Vector3 T = targetTransform.TransformDirection(Tl);

        return T;
    }

    // Move towards the target pivot
    void Move()
    {
        var dist = Vector3.Distance(transform.position, target.position);
        var step = dist * speed;

        if (dist <= distanceThreshold || !GameManager.Instance.playersReady)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = reference.forward * step * Time.deltaTime;
        }
        else
            rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Rotate towards the target pivot
    void Rotate()
    {
        var angle = Vector3.Angle(target.forward, transform.forward);
        var step = angle * rotationSpeed;

        rb.AddTorque(ComputeTorque(transform, target.rotation) * step * Time.deltaTime);
    }
}

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RagdollAnimator : MonoBehaviour
{ 
    // Initialize the public variables
    public Transform[] bones, targets;
    public float[] animationControl;
    public float collisionImpact, impactRecovery, collapseMinimum, collapseDuration;
    public BoxCollider feetCollider;
    public Transform movementParent;
    //[System.NonSerialized]
    public bool hasCollapsed;
    [System.NonSerialized]
    public float targetRot, currentRot;
    public AudioSource audioSource;

    // Initialize the private variables
    float maxForce = 2750f, collapseAlarm;

    // Run this code once at the start
    void Start()
    {
        audioSource.pitch = Random.Range(.75f, 1.25f);

        for (var i = 0; i < bones.Length; i++)
        {
            bones[i].gameObject.AddComponent<RagdollCollider>();

            bones[i].GetComponent<RagdollCollider>().ragdollAnimator = GetComponent<RagdollAnimator>();
            bones[i].GetComponent<RagdollCollider>().boneID = i;

            bones[i].GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Recover(); // Recover from recent impacts
        Collapse(); // Collapse when the average animation control value gets too low
    }

    // Run this code every single frame
    void FixedUpdate()
    {
        Follow(); // Follow the target
    }

    // Follow the target
    void Follow()
    {
        for (var i = 0; i < bones.Length; i++)
        {
            var rb = bones[i].GetComponent<Rigidbody>();
            var force = ((animationControl[i] / 100f) * maxForce) * Time.deltaTime;
            rb.AddTorque(ComputeTorque(bones[i], targets[i].rotation) * force);
        }
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

    // Recover from recent impacts
    void Recover()
    {
        if (collapseAlarm <= 0f)
        {
            for (var i = 0; i < bones.Length; i++)
            {
                if (animationControl[i] <= 100f - impactRecovery)
                    animationControl[i] += impactRecovery;
                else
                    animationControl[i] = 100f;
            }
        }
    }

    // Collapse when the average animation control value gets too low
    void Collapse()
    {
        var average = animationControl.Average();

        if (average <= collapseMinimum)
        {
            if (!hasCollapsed)
            {
                for (var i = 0; i < bones.Length; i++)
                    animationControl[i] = 0f;

                collapseAlarm = collapseDuration;
                audioSource.Play(); // AAA!

                feetCollider.enabled = false;
                hasCollapsed = true;
            }
        }
        else
        {
            feetCollider.enabled = true;
            hasCollapsed = false;
        }

        collapseAlarm--;
    }
}

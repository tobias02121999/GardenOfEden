using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Panda;

public class RagdollAnimator : MonoBehaviour
{
    // Initialize the public variables
    public Transform[] bones, targets;
    public float[] animationControl;
    public float collisionImpact, impactRecovery, collapseMinimum, collapseDuration, speed, wanderDuration, turnSpeed, fearGauge, fearMultiplier;
    [HideInInspector] float baseSpeed;
    public BoxCollider feetCollider;
    public Transform movementParent;

    // Initialize the private variables
    float maxForce = 65f, collapseAlarm, wanderAlarm;
    bool hasCollapsed;
    float targetRot, currentRot;

    // Run this code once at the start
    void Start()
    {
        baseSpeed = speed;

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
        Wander(); // Randomize the movement direction

        if (fearMultiplier >= 7)
        {
            fearMultiplier = 7;
        }
    }

    // Run this code every single frame
    void FixedUpdate()
    {
        Follow(); // Follow the target
        Move(); // Apply force to the players rigidbodies
    }

    // Follow the target
    void Follow()
    {
        for (var i = 0; i < bones.Length; i++)
        {
            var rb = bones[i].GetComponent<Rigidbody>();
            var force = (animationControl[i] / 100f) * maxForce;
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

    // Apply force to the players rigidbodies
    void Move()
    {
        if (!hasCollapsed)
        {
            for (var i = 0; i < bones.Length; i++)
            {
                var rb = bones[i].GetComponent<Rigidbody>();
                var forward = movementParent.forward;

                rb.AddForce(forward * speed);
            }
        }
    }

    // Randomize the movement direction
    void Wander()
    {
        if (!hasCollapsed)
        {
            if (wanderAlarm <= 0f)
            {
                targetRot = Random.Range(0f, 360f);
                wanderAlarm = wanderDuration;

                Debug.Log("Randomize");
            }

            wanderAlarm--;

            if (currentRot >= targetRot + turnSpeed)
                currentRot -= turnSpeed;

            if (currentRot <= targetRot - turnSpeed)
                currentRot += turnSpeed;

            movementParent.rotation = Quaternion.Euler(0f, currentRot, 0f);
        }
    }


    // AI BEHAVIOR
    [Task]
    bool MeteorHasSpawned() 
    {
        if (DrawRecognition.returnShape == "Circle" || Input.GetMouseButtonDown(0))    // Checking if the player has drawn a circle
        {
            fearGauge += 2;                             // If so, increase fear among the humans
            Task.current.Succeed();
            return true;
        }
        else
        {
            Task.current.Fail();                        // Else fail this task
            return false;        // DEBUG, FOR ACTUAL RESULT REVERT TO FALSE
        }
    }

    [Task]
    void GaugeFear()
    {
        float closestObject = 10000f;
        Vector3 closestObjectPosition = Vector3.zero;

        for (int i = 0; i < GameManager.fearObjects.Count; i++)
        {
            float newObjectDistance = Vector3.Distance(this.transform.position, GameManager.fearObjects[i].transform.position);

            if (newObjectDistance < closestObject)
            {
                closestObject = newObjectDistance;
                closestObjectPosition = GameManager.fearObjects[i].transform.position;
            }
        }

        fearGauge = Mathf.Clamp(fearGauge + 100 - Vector3.Distance(this.transform.position, closestObjectPosition) / 0.75f , 0, 100);
        if (fearGauge <= 100)
        {
            speed = Mathf.Clamp(speed + fearGauge / 4, 0, 12);
        }
    }

//    [Task]
//    void Panic()        // What happens when humans panic
//    {
//        Mathf.Lerp(fearGauge, 100, fearMultiplier * Time.deltaTime);    // Fear increases over time
//        wanderDuration = 50f;
//        speed = 5f;
//        fearMultiplier += fearMultiplier;   // Fear multiplier to make fear incrase faster.
//    }
}

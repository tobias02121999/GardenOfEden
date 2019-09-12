using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCollider : MonoBehaviour
{
    // Initialize the public variables
    public RagdollAnimator ragdollAnimator;
    public int boneID;

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
        var magnitude = collision.relativeVelocity.magnitude;
        var impact = ragdollAnimator.collisionImpact;
        var value = Mathf.Clamp(ragdollAnimator.animationControl[boneID] - (magnitude * impact), 0f, 100f);

        ragdollAnimator.animationControl[boneID] = value;
    }
}

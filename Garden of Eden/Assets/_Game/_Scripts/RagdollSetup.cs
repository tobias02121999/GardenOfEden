using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RagdollSetup : NetworkBehaviour
{
    // Initialize the private variables
    InteractionDetection detection;

    // Start is called before the first frame update
    void Start()
    {
        detection = GetComponentInChildren<InteractionDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        bool check;

        if (isServer)
            check = detection.clientAuthority;
        else
            check = !detection.clientAuthority;

        ControlRigidBodies(check);
    }

    // Enable or disable all rigidbodies
    public void ControlRigidBodies(bool state)
    {
        var animator = GetComponent<RagdollAnimator>();
        var size = animator.bones.Length;

        for (var i = 0; i < size; i++)
        {
            var characterJoint = animator.bones[i].GetComponent<CharacterJoint>();
            var configurableJoint = animator.bones[i].GetComponent<ConfigurableJoint>();
            var rigidbody = animator.bones[i].GetComponent<Rigidbody>();
            var collider = animator.bones[i].GetComponent<Collider>();

            if (rigidbody != null)
                rigidbody.isKinematic = state;

            if (collider != null)
                collider.enabled = state;
        }

        animator.feetCollider.enabled = state;
        animator.enabled = state;
    }
}

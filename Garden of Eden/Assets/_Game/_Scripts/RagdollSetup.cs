using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RagdollSetup : NetworkBehaviour
{
    // Initialize the public variables
    [SyncVar]
    public int teamID;

    public Behaviour[] nonLocalComponents;
    public Material[] teamMaterials;
    public Renderer modelRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ToggleComponents(); // Disable the local components if this object is not controlled by the local player
        ApplyMaterial(); // Apply the correct team material to the unit based on its networked teamID
    }

    // Disable the local components if this object is not controlled by the local player
    void ToggleComponents()
    {
        var localPlayerID = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerSetup>().teamID;
        var isLocal = (teamID == localPlayerID);

        ControlRigidBodies(!isLocal);

        var length = nonLocalComponents.Length;
        for (var i = 0; i < length; i++)
            nonLocalComponents[i].enabled = isLocal;
    }

    // Enable or disable all rigidbodies
    void ControlRigidBodies(bool state)
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
                collider.enabled = !state;
        }

        animator.feetCollider.enabled = !state;
        animator.enabled = !state;
    }

    // Apply the correct team material to the unit based on its networked teamID
    void ApplyMaterial()
    {
        modelRenderer.material = teamMaterials[teamID];
    }
}

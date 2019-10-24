using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using InputTracking = UnityEngine.XR.InputTracking;
using Node = UnityEngine.XR.XRNode;

public class PlayerControls : NetworkBehaviour
{
    public GameObject cameraRig;
    public GameObject cursor;
    public Transform leftPivot;
    public Transform rightPivot;
    public Camera leftEye;
    public Camera rightEye;
    public Animation handAnimationL, handAnimationR;
    public NetworkPlayers networkPlayers;

    void Start()
    {
        networkPlayers = GameObject.FindObjectOfType<NetworkPlayers>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            cameraRig.SetActive(false);
            cursor.SetActive(false);
            GetComponent<NetworkSpawner>().enabled = false;

            networkPlayers.otherPlayer = this.gameObject;
        }
        else
        {
            // Makes sure the local camera's are the main camera's.
            if (leftEye.tag != "MainCamera")
            {
                leftEye.tag = "MainCamera";
                leftEye.enabled = true;
            }

            if (rightEye.tag != "MainCamera")
            {
                rightEye.tag = "MainCamera";
                rightEye.enabled = true;
            }

            // Takes care of the local hand tracking.
            leftPivot.localRotation = InputTracking.GetLocalRotation(Node.LeftHand);
            leftPivot.localPosition = InputTracking.GetLocalPosition(Node.LeftHand);

            rightPivot.localRotation = InputTracking.GetLocalRotation(Node.RightHand);
            rightPivot.localPosition = InputTracking.GetLocalPosition(Node.RightHand);

            var duration = handAnimationL["Hand Close"].length;
            handAnimationL["Hand Close"].time = Mathf.Clamp(duration * OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger), 0f, duration - .1f);
            handAnimationR["Hand Close"].time = Mathf.Clamp(duration * OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger), 0f, duration - .1f);

            networkPlayers.localPlayer = this.gameObject;
        }
    }

    // Shift the authority over to the client
    [Command]
    public void CmdSetClientAuthority(GameObject instance)
    {
        var identity = instance.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(connectionToClient);
    }

    // Clear the authority
    [Command]
    public void CmdClearAuthority(GameObject instance)
    {
        var identity = instance.GetComponent<NetworkIdentity>();
        identity.RemoveClientAuthority(connectionToClient);
    }
}

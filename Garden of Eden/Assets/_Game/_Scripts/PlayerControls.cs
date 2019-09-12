using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using InputTracking = UnityEngine.XR.InputTracking;
using Node = UnityEngine.XR.XRNode;

public class PlayerControls : NetworkBehaviour
{
    public GameObject ovrCameraRig;
    public GameObject cursor;
    public Transform leftHand;
    public Transform rightHand;
    public Camera leftEye;
    public Camera rightEye;

    private void Update()
    {
        if (!isLocalPlayer)
        {
            ovrCameraRig.SetActive(false);
            cursor.SetActive(false);
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
            leftHand.localRotation = InputTracking.GetLocalRotation(Node.LeftHand);
            leftHand.localPosition = InputTracking.GetLocalPosition(Node.LeftHand);

            rightHand.localRotation = InputTracking.GetLocalRotation(Node.RightHand);
            rightHand.localPosition = InputTracking.GetLocalPosition(Node.RightHand);
        }
    }
}

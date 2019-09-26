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
    public Transform leftPivot;
    public Transform rightPivot;
    public GameObject physicsBrush;
    public GameObject drawingBrush;
    public Camera leftEye;
    public Camera rightEye;
    public Animation handAnimation;

    // Update is called once per frame
    void Update()
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
            leftPivot.localRotation = InputTracking.GetLocalRotation(Node.LeftHand);
            leftPivot.localPosition = InputTracking.GetLocalPosition(Node.LeftHand);

            rightPivot.localRotation = InputTracking.GetLocalRotation(Node.RightHand);
            rightPivot.localPosition = InputTracking.GetLocalPosition(Node.RightHand);

            physicsBrush.SetActive(!cursor.GetComponent<Brush>().isDrawing);
            drawingBrush.SetActive(cursor.GetComponent<Brush>().isDrawing);

            var duration = handAnimation["Hand Close"].length;
            handAnimation["Hand Close"].time = Mathf.Clamp(duration * OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger), 0f, duration - .1f);
        }
    }
}

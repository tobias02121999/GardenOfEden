using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using InputTracking = UnityEngine.XR.InputTracking;
using Node = UnityEngine.XR.XRNode;

public class PlayerControls : NetworkBehaviour
{
    // Initialize the public variables
    public GameObject headModel;
    public Transform leftPivot;
    public Transform rightPivot;
    public Camera leftEye;
    public Camera rightEye;
    public Animation handAnimationL, handAnimationR;

    [HideInInspector]
    public bool isFistL, isFistR;

    // Initialize the private variables
    PlayerSetup setup;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the player object
        CamInit(); // Makes sure the local cameras are the main cameras
    }

    // Update is called once per frame
    void Update()
    {
        if (!setup.isOther)
            HandTracking(); // Takes care of the local hand tracking
    }

    // Initialize the player object
    void Initialize()
    {
        setup = GetComponent<PlayerSetup>();
    }

    // Makes sure the local cameras are the main cameras
    void CamInit()
    {
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
    }

    // Takes care of the local hand tracking
    void HandTracking()
    {
        leftPivot.localRotation = InputTracking.GetLocalRotation(Node.LeftHand);
        leftPivot.localPosition = InputTracking.GetLocalPosition(Node.LeftHand);

        rightPivot.localRotation = InputTracking.GetLocalRotation(Node.RightHand);
        rightPivot.localPosition = InputTracking.GetLocalPosition(Node.RightHand);

        var duration = handAnimationL["Hand Close"].length;
        handAnimationL["Hand Close"].time = Mathf.Clamp(duration * OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger), 0f, duration - .1f);
        handAnimationR["Hand Close"].time = Mathf.Clamp(duration * OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger), 0f, duration - .1f);

        isFistL = (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) >= .75f);
        isFistR = (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) >= .75f);
    }
}

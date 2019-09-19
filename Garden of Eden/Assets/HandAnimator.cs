using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    // Initialize the public variables
    public Transform indexFinger;
    public Transform middleFinger;
    public Transform ringFinger;
    public Transform pinky;
    public Transform thumb;

    public Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim["Hand Middle Finger"].layer = 1;
        anim["Hand Ring Finger"].layer = 2;
        anim["Hand Pinky"].layer = 3;
        anim["Hand Thumb"].layer = 4;

        anim["Hand Index Finger"].AddMixingTransform(indexFinger);
        anim["Hand Middle Finger"].AddMixingTransform(middleFinger);
        anim["Hand Ring Finger"].AddMixingTransform(ringFinger);
        anim["Hand Pinky"].AddMixingTransform(pinky);
        anim["Hand Thumb"].AddMixingTransform(thumb);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // Initialize the public variables
    public bool touchingHandL, touchingHandR;
    public bool isGrounded;

    // Initialize the private variables
    Rigidbody rb;
    Floatable floatable;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        floatable = GetComponent<Floatable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded && !(touchingHandL && touchingHandR))
            rb.constraints = RigidbodyConstraints.FreezeAll;
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            if (floatable != null)
                floatable.isFloating = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        var hand = collision.gameObject.GetComponentInParent<FollowPivot>();

        if (hand != null)
        {
            if (hand.CompareTag("HandLeft"))
                touchingHandL = true;

            if (hand.CompareTag("HandRight"))
                touchingHandR = true;
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        var hand = collision.gameObject.GetComponentInParent<FollowPivot>();

        if (hand != null)
        {
            if (hand.CompareTag("HandLeft"))
                touchingHandL = false;

            if (hand.CompareTag("HandRight"))
                touchingHandR = false;
        }
    }
}

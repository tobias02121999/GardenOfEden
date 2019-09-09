using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float movementSpeed;

    Vector3 localPosition;

    // Update is called once per frame
    void Update()
    {
        localPosition.x = Input.GetAxis("Horizontal");
        localPosition.z = Input.GetAxis("Vertical");

        transform.position += localPosition * movementSpeed * Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    // Initialize the public variables
    public float speed;
    public string moveAxisHor, moveAxisVer;
    public string lookAxisHor, lookAxisVer;
    public Vector2 lookSensitivity;

    // Initialize the private variables
    float rotX, rotY;
    Transform camTransform;

    // Start is called before the first frame update
    void Start()
    {
        camTransform = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move(); // Move the spectator around based on input
        Look(); // Turn the camera based on input
    }

    // Move the spectator around based on input
    void Move()
    {
        transform.position += ((transform.forward * Input.GetAxis(moveAxisVer) * speed) + (transform.right * Input.GetAxis(moveAxisHor) * speed)) * Time.deltaTime;
    }

    // Turn the camera based on input
    void Look()
    {
        rotX += Input.GetAxis(lookAxisVer) * lookSensitivity.y * Time.deltaTime;
        rotY += Input.GetAxis(lookAxisHor) * lookSensitivity.x * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        camTransform.rotation = Quaternion.Euler(rotX, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}

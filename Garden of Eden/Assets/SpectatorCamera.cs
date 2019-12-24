using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    // Initialize the public enums
    public enum States { STATIC }

    // Initialize the public variables
    public States state = States.STATIC;
    public bool freeCamActive;
    public float freeCamSpeed, staticCamSpeed;
    public float freeCamSensitivity, staticCamSensitivity;
    public GameObject freeCam;
    public GameObject[] cineCams;

    // Initialize the private variables
    int inputAlarm;
    bool userIsActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RunState(); // Run the current camera state
        SwitchState(); // Switch to the next camera state
        ToggleFreeCam(); // Toggle the free cam mode on and off
        CheckUserActivity(); // Check if the user is actively using the spectator
    }

    // Run the current camera state
    void RunState()
    {
        if (!freeCamActive) // Run the current state functions
        {
            switch (state)
            {
                case States.STATIC:
                    ToggleCams(0); // Toggle cameras on and off
                    StaticTurn(); // Turn the static camera using user input
                    break;
            }
        }
        else // Run the free cam functions
        {
            FreeLook(); // Look around using user input
            FreeMove(); // Move around using user input
            ToggleCams(0); // Toggle cameras on and off
        }
    }

    // Switch to the next camera state
    void SwitchState()
    {

    }

    // Toggle the free cam mode on and off
    void ToggleFreeCam()
    {
        if (Input.GetButtonDown("Submit"))
        {
            var cam = GetComponentInChildren<Camera>();
            var pos = cam.transform.position;
            var rot = cam.transform.rotation;

            freeCam.transform.position = pos;
            freeCam.transform.rotation = rot;
            freeCamActive = !freeCamActive;
        }
    }

    // Look around using user input
    void FreeLook()
    {
        var step = freeCamSensitivity * Time.deltaTime;
        var rotX = freeCam.transform.eulerAngles.x - (Input.GetAxis("Mouse Y") * step);
        var rotY = freeCam.transform.eulerAngles.y + (Input.GetAxis("Mouse X") * step);

        freeCam.transform.rotation = Quaternion.Euler(rotX, rotY, transform.eulerAngles.z);
    }

    // Move around using user input
    void FreeMove()
    {
        var step = freeCamSpeed * Time.deltaTime;
        freeCam.transform.position += (freeCam.transform.forward * (Input.GetAxis("Vertical") * step)) + 
            (freeCam.transform.right * (Input.GetAxis("Horizontal") * step));
    }

    // Turn the static camera using user input
    void StaticTurn()
    {
        var step = staticCamSensitivity * Time.deltaTime;
        var rotY = cineCams[0].transform.eulerAngles.y + (Input.GetAxis("Mouse X") * step);

        if (!userIsActive)
            rotY += staticCamSpeed;

        cineCams[0].transform.rotation = Quaternion.Euler(cineCams[0].transform.eulerAngles.x, rotY, cineCams[0].transform.eulerAngles.z);
    }

    // Toggle cameras on and off
    void ToggleCams(int camID)
    {
        if (!freeCamActive)
        {
            var length = cineCams.Length;
            for (var i = 0; i < length; i++)
                cineCams[i].SetActive(i == camID);

            freeCam.SetActive(false);
        }
        else
        {
            var length = cineCams.Length;
            for (var i = 0; i < length; i++)
                cineCams[i].SetActive(false);

            freeCam.SetActive(true);
        }
    }

    // Check if the user is actively using the spectator
    void CheckUserActivity()
    {
        var inputCheckThreshold = 100;

        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f || Input.GetButtonDown("Submit"))
            inputAlarm = inputCheckThreshold;

        userIsActive = (inputAlarm > 0);

        inputAlarm--;
    }
}

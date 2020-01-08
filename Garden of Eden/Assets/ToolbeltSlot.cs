using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbeltSlot : MonoBehaviour
{
    // Initialize the public variables
    public GameObject[] toolMesh;
    public int toolID;
    public int handType;
    public string handTag;

    // Initialize the private variables
    int interactionAlarm;
    int interactionAlarmDuration = 60;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayMesh(); // Display the correct mesh
        interactionAlarm--;
    }

    // Set the tool ID
    void OnTriggerStay(Collider other)
    {
        if (interactionAlarm <= 0)
        {
            var hand = other.GetComponentInParent<FollowPivot>();
            
            if ((hand.CompareTag("HandLeft") && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) != 0f) || 
            (hand.CompareTag("HandRight") && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) != 0f))
            {
                if (hand.CompareTag("HandLeft"))
                    handType = 0;
                
                if (hand.CompareTag("HandRight"))
                    handType = 1;

                handTag = hand.tag;

                var ID = GetToolID(other.tag);
                var inv = other.GetComponentInParent<PlayerInventory>();
                SwitchTool(ID, inv, handType);

                interactionAlarm = interactionAlarmDuration;
            }
        }
    }

    // Switch tools
    void SwitchTool(int targetID, PlayerInventory targetInv, int handType)
    {
        /*
        int newID;

        if (handType == 0)
        {
            newID = targetInv.currentToolL;
            targetInv.currentToolL = toolID;
        }
        else
        {
            newID = targetInv.currentToolR;
            targetInv.currentToolR = toolID;
        }

        toolID = newID;

        audioSource.Play();
        */
    }

    // Display the correct mesh
    void DisplayMesh()
    {
        var length = toolMesh.Length;
        for (var i = 0; i < length; i++)
        {
            var isActive = (i == toolID);
            toolMesh[i].SetActive(isActive);
        }
    }

    // Convert a tool name into a tool ID
    int GetToolID(string name)
    {
        var ID = 0;

        switch (name)
        {
            case "Hand":
                ID = 0;
                break;

            case "Paintbrush":
                ID = 1;
                break;

            case "Hammer":
                ID = 2;
                break;

            case "Paintjar":
                ID = 3;
                break;

            case "Book":
                ID = 4;
                break;
        }

        return ID;
    }
}

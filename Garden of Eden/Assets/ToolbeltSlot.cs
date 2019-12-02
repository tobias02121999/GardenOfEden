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
    bool hasInteracted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayMesh(); // Display the correct mesh
    }

    // Set the tool ID
    void OnTriggerEnter(Collider other)
    {
        if (!hasInteracted)
        {
            var hand = other.GetComponentInParent<FollowPivot>();
            
            if (hand.CompareTag("HandLeft"))
                handType = 0;
            
            if (hand.CompareTag("HandRight"))
                handType = 1;

            handTag = hand.tag;

            var ID = GetToolID(other.tag);
            var inv = other.GetComponentInParent<PlayerInventory>();
            SwitchTool(ID, inv, handType);

            hasInteracted = true;
        }
    }

    // Reset the interaction bool
    void OnTriggerExit(Collider other)
    {
        hasInteracted = false;
    }

    // Switch tools
    void SwitchTool(int targetID, PlayerInventory targetInv, int handType)
    {
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

            Debug.Log("A!");
        }

        toolID = newID;
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
        }

        return ID;
    }
}

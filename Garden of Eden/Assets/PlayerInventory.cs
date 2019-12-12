using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Initialize the public variables
    public GameObject[] toolsL, toolsR;
    public int currentToolL, currentToolR;
    public float maxPaint;
    public bool debugMode;
    public ToolbeltSlot[] slots;

    // Initialize the private variables
    bool switchButtonLPressed, switchButtonRPressed;
    public float paint, paintToSpend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var size = toolsL.Length;
        for (var i = 0; i < size; i++)
        {
            if (i == currentToolL && !toolsL[i].activeSelf)
                toolsL[i].SetActive(true);

            if (i == currentToolR && !toolsR[i].activeSelf)
                toolsR[i].SetActive(true);

            if (i != currentToolL && toolsL[i].activeSelf)
                toolsL[i].SetActive(false);

            if (i != currentToolR && toolsR[i].activeSelf)
                toolsR[i].SetActive(false);
        }

        if (debugMode)
        {
            var inputL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            if (inputL.x > 0f && !switchButtonLPressed)
            {
                if (currentToolL <= size - 2)
                    currentToolL++;
                else
                    currentToolL = 0;

                switchButtonLPressed = true;
            }

            if (inputL.x < 0f && !switchButtonLPressed)
            {
                if (currentToolL >= 1)
                    currentToolL--;
                else
                    currentToolL = size - 1;

                switchButtonLPressed = true;
            }

            if (inputL.x == 0f)
                switchButtonLPressed = false;

            var inputR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            if (inputR.x > 0f && !switchButtonRPressed)
            {
                if (currentToolR <= size - 2)
                    currentToolR++;
                else
                    currentToolR = 0;

                switchButtonRPressed = true;
            }

            if (inputR.x < 0f && !switchButtonRPressed)
            {
                if (currentToolR >= 1)
                    currentToolR--;
                else
                    currentToolR = size - 1;

                switchButtonRPressed = true;
            }

            if (inputR.x == 0f)
                switchButtonRPressed = false;
        }
    }

    // Return the currently equipped tools to the toolbelt
    public void returnTools()
    {
        var length = slots.Length;
        for (var i = 0; i < length; i++)
        {
            if (slots[i].toolID == 0)
            {
                if (currentToolL != 0)
                {
                    slots[i].toolID = currentToolL;
                    currentToolL = 0;
                }
                else
                {
                    if (currentToolR != 0)
                    {
                        slots[i].toolID = currentToolR;
                        currentToolR = 0;
                    }
                }
            }
        }
    }
}

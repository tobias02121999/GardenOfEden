using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Initialize the public variables
    public GameObject[] hands, brushes, magicLines;
    public float maxPaint;
    public float summonInitiateDistance, summonDistance;
    public GameObject deskPrefab;
    public Transform headTransform;

    //[HideInInspector]
    public float paint, paintToSpend;

    /* >> OLD INVENTORY SYSTEM PUBLIC VARIABLES <<
    public GameObject[] toolsL, toolsR;
    public int currentToolL, currentToolR;
    public float maxPaint;
    public bool debugMode;
    public ToolbeltSlot[] slots;
    */

    // Initialize the private variables
    bool inputL, inputR;
    bool isSummoningL, isSummoningR;
    GameObject currentDesk;

    /* >> OLD INVENTORY SYSTEM PRIVATE VARIABLES <<
    bool switchButtonLPressed, switchButtonRPressed;
    public float paint, paintToSpend;
    */

    // Start is called before the first frame update
    void Start()
    {
        paint = maxPaint;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playersReady)
        {
            GetInput(); // Get the touch controller input
            Switch(); // Switch between hands and paintbrushes
            SummonDesk(); // Summon the god desk
        }
    }

    // Get the touch controller input
    void GetInput()
    {
        // Get the left and right controller input
        inputL = (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0f);
        inputR = (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) != 0f);
    }

    // Switch between hands and paintbrushes
    void Switch()
    {
        // Toggle the hand and brush objects on and off
        if (!inputL)
        {
            var brush = brushes[0].GetComponentInChildren<Brush>();
            brush.RecognitionInit();

            hands[0].SetActive(true);
            brushes[0].SetActive(false);
        }
        else
        {
            hands[0].SetActive(false);
            brushes[0].SetActive(true);
        }

        if (!inputR)
        {
            var brush = brushes[1].GetComponentInChildren<Brush>();
            brush.RecognitionInit();

            hands[1].SetActive(true);
            brushes[1].SetActive(false);
        }
        else
        {
            hands[1].SetActive(false);
            brushes[1].SetActive(true);
        }
    }

    // Summon the god desk
    void SummonDesk()
    {
        var dist = Vector3.Distance(hands[0].transform.position, hands[1].transform.position);
        var controls = GetComponent<PlayerControls>();

        if (dist <= summonInitiateDistance && !(controls.isFistL && controls.isFistR))
        {
            if (controls.isFistL)
            {
                magicLines[1].SetActive(true);
                isSummoningL = true;
            }
                
            if (controls.isFistR)
            {
                magicLines[0].SetActive(true);
                isSummoningR = true;
            }
        }

        if (!controls.isFistL)
        {
            var line = magicLines[1].GetComponent<ParticleLine>();
            line.ResetLine();

            magicLines[1].SetActive(false);
            isSummoningL = false;
        }

        if (!controls.isFistR)
        {
            var line = magicLines[0].GetComponent<ParticleLine>();
            line.ResetLine();

            magicLines[0].SetActive(false);
            isSummoningR = false;
        }

        if (dist >= summonDistance && (isSummoningL || isSummoningR))
        {
            var a = hands[0].transform.position;
            var b = hands[1].transform.position;

            var ab = b - a;
            ab = Vector3.Normalize(ab);

            var pos = a + (ab * (dist / 2f));
            var rot = Quaternion.Euler(0f, headTransform.eulerAngles.y, 0f);

            if (currentDesk != null)
                Destroy(currentDesk);

            currentDesk = Instantiate(deskPrefab, pos, rot);

            if (controls.isFistL)
            {
                var line = magicLines[1].GetComponent<ParticleLine>();
                line.ResetLine();

                magicLines[1].SetActive(false);
            }

            if (controls.isFistR)
            {
                var line = magicLines[0].GetComponent<ParticleLine>();
                line.ResetLine();

                magicLines[0].SetActive(false);
            }

            isSummoningL = false;
            isSummoningR = false;
        }
    }
}

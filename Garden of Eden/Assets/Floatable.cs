using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floatable : MonoBehaviour
{
    // Initialize the public variables
    public bool isFloating;
    public GameObject[] modelStates;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFloating)
        {
            modelStates[0].SetActive(false);
            modelStates[1].SetActive(true);
        }
        else
        {
            modelStates[0].SetActive(true);
            modelStates[1].SetActive(false);
        }
    }
}

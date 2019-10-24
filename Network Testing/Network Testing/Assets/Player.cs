using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Initialize the private variables
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = Random.ColorHSV();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

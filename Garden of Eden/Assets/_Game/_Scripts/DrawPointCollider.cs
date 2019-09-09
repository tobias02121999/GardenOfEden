using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPointCollider : MonoBehaviour
{
    // Initialize the public variables
    public int xx, yy;
    public GameObject cube;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check for collision
    void OnTriggerStay()
    {
        var drawRecognition = GetComponentInParent<DrawRecognition>();
        drawRecognition.drawData[xx, yy] = 1;
        cube.SetActive(true);
    }
}

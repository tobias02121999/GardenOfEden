using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbeltPivot : MonoBehaviour
{
    // Initialize the public variables
    public Transform headTransform;

    // Initialize the private variables
    float startPosY;

    // Start is called before the first frame update
    void Start()
    {
        startPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = headTransform.position;
        transform.rotation = Quaternion.Euler(0f, headTransform.localEulerAngles.y, 0f);
    }
}

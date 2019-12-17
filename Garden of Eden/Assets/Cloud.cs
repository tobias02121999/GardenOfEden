using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    // Initialize the public variables
    public float cloudDuration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cloudDuration--;

        if (cloudDuration <= 0f)
            Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ResetRotation();
    }

    // Make sure the collider always faces down
    void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
}

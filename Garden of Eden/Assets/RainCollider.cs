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

    // Tell the object it's colliding with that it's getting rained on
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Farm"))
            other.GetComponentInParent<Farm>().isWet = true;

        if (other.CompareTag("Tree"))
            other.GetComponentInParent<Tree>().isWet = true;
    }

    // Tell the object it's colliding with that it's not getting rained on
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Farm"))
            other.GetComponentInParent<Farm>().isWet = false;

        if (other.CompareTag("Tree"))
            other.GetComponentInParent<Tree>().isWet = false;
    }
}

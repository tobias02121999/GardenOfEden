using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRetrieval : MonoBehaviour
{
    // Initialize the public variables
    public Transform[] retrievalPoints;
    public string[] tagsToCheck;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check if buildings have fallen into the water
    void OnTriggerEnter(Collider other)
    {
        var length = tagsToCheck.Length;
        for (var i = 0; i < length; i++)
        {
            var tag = tagsToCheck[i];
            if (other.CompareTag(tag))
            {
                var obj = other.gameObject;
                RetrieveObject(obj);
                break;
            }
        }
    }

    // Reposition the target object to one of the retrieval points
    void RetrieveObject(GameObject target)
    {
        var length = retrievalPoints.Length;
        var rand = Mathf.RoundToInt(Random.Range(0f, length - 1f));
        var pos = retrievalPoints[rand].position;

        target.GetComponentInParent<Rigidbody>().transform.position = pos;
        target.GetComponentInParent<Rigidbody>().transform.rotation = Quaternion.identity;
        target.GetComponentInParent<Grab>().isGrounded = true;
        target.GetComponentInParent<Floatable>().isFloating = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdWaypoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Tell the bird its in range of a waypoint
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bird"))
        {
            var obj = other.GetComponentInParent<Bird>();
            if (obj.waypoint == null)
                obj.waypoint = transform;
        }
    }

    // Tell the bird its no longer in the waypoints range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bird"))
        {
            var obj = other.GetComponentInParent<Bird>();
            if (obj.waypoint == transform)
                obj.waypoint = null;
        }
    }
}

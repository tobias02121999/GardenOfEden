using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorkDropzone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check for storks dropping humans
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stork"))
        {
            var stork = other.GetComponentInParent<Stork>();

            stork.humanHips.constraints = RigidbodyConstraints.None;
            stork.human.GetComponent<RagdollAnimator>().impactRecovery = 1f;

            stork.human.transform.parent = null;
        }
    }
}

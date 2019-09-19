using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 5)
        {
            // Trigger some kind of explosion first.

            var lingeringObject = new GameObject();             // Spawn a new GameObject for the lingering effect.
            lingeringObject.name = "Lingering Fear Object";     // Name it accordingly.

            GameManager.Instance.lingeringFearObjects.Add(lingeringObject); // Add the new lingering object to the corresponding list in GameManager.

            Destroy(gameObject);   // Destroy this GameObject.
        }
    }
}

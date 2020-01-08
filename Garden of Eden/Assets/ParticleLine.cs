using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLine : MonoBehaviour
{
    // Initialize the public variables
    public Transform targetPoint;
    public float lineDensity;
    public GameObject particlePrefab;

    // Initialize the private variables
    [HideInInspector]
    public List<GameObject> particleSystems = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine(); // Draw the particle line
        RotateLine(); // Rotate the line towards the target point
    }

    // Draw the particle line
    void DrawLine()
    {
        var dist = Vector3.Distance(transform.position, targetPoint.position);
        var count = Mathf.RoundToInt(dist / lineDensity);

        for (var i = 0; i < count; i++)
        {
            if (particleSystems.Count < i)
            {
                var pos = transform.position + (transform.forward * (i * lineDensity));
                var obj = Instantiate(particlePrefab, pos, transform.rotation);
                obj.transform.parent = transform;

                particleSystems.Add(obj);
            }
            else
            {
                if (count < particleSystems.Count && i == count - 1)
                {
                    Destroy(particleSystems[particleSystems.Count - 1]);
                    particleSystems.RemoveAt(particleSystems.Count - 1);
                }
            }
        }
    }

    // Rotate the line towards the target point
    void RotateLine()
    {
        transform.LookAt(targetPoint);
    }

    // Reset the drawn particle line
    public void ResetLine()
    {
        var size = particleSystems.Count;
        for (var i = 0; i < size; i++)
        {
            Destroy(particleSystems[particleSystems.Count - 1]);
            particleSystems.RemoveAt(particleSystems.Count - 1);
        }
    }
}

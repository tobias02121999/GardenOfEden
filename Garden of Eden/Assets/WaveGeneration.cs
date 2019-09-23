using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGeneration : MonoBehaviour
{
    // Initialize the public variables
    public float scale, speed, strength, height;

    // Initialize the private variables
    Mesh mesh;
    Vector3[] baseHeight;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (baseHeight == null)
            baseHeight = mesh.vertices;

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];

            vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x, baseHeight[i].y + Mathf.Sin(Time.time * .1f)) * strength;

            vertices[i] = vertex;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}

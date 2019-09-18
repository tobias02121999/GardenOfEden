using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshCollider : MonoBehaviour
{
    // Initialize the public variables
    public SkinnedMeshRenderer meshRenderer;
    public MeshCollider _collider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCollider();
    }

    // Update the mesh collider
    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        _collider.sharedMesh = null;
        _collider.sharedMesh = colliderMesh;
    }
}

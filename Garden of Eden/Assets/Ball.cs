using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Initialize the public variables
    public Material[] teamMaterials;

    // Initialize the private variables
    Renderer modelRenderer;
    bool isOther;
    BallSetup setup;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the ball object
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMaterial(); // Apply the correct team material to the unit based on its networked teamID
    }

    // Initialize the ball object
    void Initialize()
    {
        modelRenderer = GetComponentInChildren<Renderer>();
        setup = GetComponent<BallSetup>();
    }

    // Apply the correct team material to the unit based on its networked teamID
    void ApplyMaterial()
    {
        modelRenderer.material = teamMaterials[setup.teamID];
    }
}

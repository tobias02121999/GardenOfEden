using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialScroller : MonoBehaviour
{
    // Initialize the public variables
    public float scrollSpeedX, scrollSpeedY;
    public Material targetMaterial;
    public string materialName;

    // Initialize the private variables
    float posX, posY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        posX += scrollSpeedX * Time.deltaTime;
        posY += scrollSpeedY * Time.deltaTime;

        targetMaterial.SetTextureOffset("_DetailAlbedoMap", new Vector2(posX, posY));
    }
}

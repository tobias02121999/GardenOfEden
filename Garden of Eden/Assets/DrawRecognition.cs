using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRecognition : MonoBehaviour
{
    // Initialize the private variables
    public GameObject drawPointCollider;
    public Vector2 size;

    // Initialize the private variables
    GameObject[,] colliderGrid;
    int gridWidth, gridHeight;

    // Start is called before the first frame update
    void Start()
    {
        gridWidth = Mathf.RoundToInt(size.x);
        gridHeight = Mathf.RoundToInt(size.y);

        colliderGrid = new GameObject[gridWidth, gridHeight];

        CreateGrid(); // Instantiate the collider grid
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Instantiate the collider grid
    void CreateGrid()
    {
        var colliderWidth = drawPointCollider.transform.localScale.x;
        var colliderHeight = drawPointCollider.transform.localScale.y;

        var offsetX = -(((gridWidth - 1) / 2f) * colliderWidth);
        var offsetY = -(((gridHeight - 1) / 2f) * colliderHeight);

        for (var xx = 0; xx < size.x; xx++)
        {
            for (var yy = 0; yy < size.y; yy++)
            {
                var posX = (xx * colliderWidth) + offsetX;
                var posY = (yy * colliderHeight) + offsetY;

                var obj = Instantiate(drawPointCollider, transform);
                obj.transform.position = new Vector3(posX, posY, transform.position.z);

                colliderGrid[xx, yy] = obj;
            }
        }
    }
}

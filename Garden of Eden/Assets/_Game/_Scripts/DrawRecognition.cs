using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRecognition : MonoBehaviour
{
    // Initialize the private variables
    public GameObject drawPointCollider;
    public Vector2 gridSize;
    public int[,] drawData;
    public int symbolCount, symbolVariation;
    public TextMesh symbolText, drawDataText;

    [HideInInspector]
    public float scale;

    // Initialize the private variables
    GameObject[,] colliderGrid;
    int gridWidth, gridHeight;
    int[][][,] drawLibrary;

    // Start is called before the first frame update
    void Start()
    {
        gridWidth = Mathf.RoundToInt(gridSize.x);
        gridHeight = Mathf.RoundToInt(gridSize.y);

        colliderGrid = new GameObject[gridWidth, gridHeight];

        DrawLibraryInit(); // Initialize the draw library
        CreateGrid(); // Instantiate the collider grid

        // Initialize the draw data
        drawData = new int[gridWidth, gridHeight];

        SetLibrary(); // Set the symbol library
    }

    // Update is called once per frame
    void Update()
    {
        CheckLibrary();
    }

    // Instantiate the collider grid
    void CreateGrid()
    {
        var colliderWidth = drawPointCollider.transform.localScale.x;
        var colliderHeight = drawPointCollider.transform.localScale.y;

        var offsetX = -(((gridWidth - 1) / 2f) * colliderWidth);
        var offsetY = -(((gridHeight - 1) / 2f) * colliderHeight);

        for (var xx = 0; xx < gridSize.x; xx++)
        {
            for (var yy = 0; yy < gridSize.y; yy++)
            {
                var posX = transform.position.x + ((xx * colliderWidth) + offsetX);
                var posY = transform.position.y + ((yy * colliderHeight) + offsetY);

                var obj = Instantiate(drawPointCollider, transform);
                obj.transform.position = new Vector3(posX, posY, transform.position.z);

                var script = obj.GetComponent<DrawPointCollider>();
                script.xx = xx;
                script.yy = yy;

                colliderGrid[xx, yy] = obj;
            }
        }

        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Initialize the draw library
    void DrawLibraryInit()
    {
        drawLibrary = new int[symbolCount][][,];
        for (var xx = 0; xx < symbolCount; xx++)
        {
            drawLibrary[xx] = new int[symbolVariation][,];
            for (var yy = 0; yy < symbolVariation; yy++)
                drawLibrary[xx][yy] = new int[gridWidth, gridHeight];
        }
    }

    // Set the symbol library
    void SetLibrary()
    {
        // Square
        drawLibrary[0][0][0, 0] = 1; drawLibrary[0][0][0, 1] = 1; drawLibrary[0][0][0, 2] = 1; drawLibrary[0][0][0, 3] = 1; drawLibrary[0][0][0, 4] = 1; drawLibrary[0][0][1, 0] = 1; drawLibrary[0][0][1, 1] = 0; drawLibrary[0][0][1, 2] = 0; drawLibrary[0][0][1, 3] = 0; drawLibrary[0][0][1, 4] = 1; drawLibrary[0][0][2, 0] = 1; drawLibrary[0][0][2, 1] = 0; drawLibrary[0][0][2, 2] = 0; drawLibrary[0][0][2, 3] = 0; drawLibrary[0][0][2, 4] = 1; drawLibrary[0][0][3, 0] = 1; drawLibrary[0][0][3, 1] = 0; drawLibrary[0][0][3, 2] = 0; drawLibrary[0][0][3, 3] = 0; drawLibrary[0][0][3, 4] = 1; drawLibrary[0][0][4, 0] = 1; drawLibrary[0][0][4, 1] = 1; drawLibrary[0][0][4, 2] = 1; drawLibrary[0][0][4, 3] = 1; drawLibrary[0][0][4, 4] = 1;
        drawLibrary[0][1][0, 0] = 1; drawLibrary[0][1][0, 1] = 1; drawLibrary[0][1][0, 2] = 1; drawLibrary[0][1][0, 3] = 1; drawLibrary[0][1][0, 4] = 0; drawLibrary[0][1][1, 0] = 1; drawLibrary[0][1][1, 1] = 0; drawLibrary[0][1][1, 2] = 0; drawLibrary[0][1][1, 3] = 1; drawLibrary[0][1][1, 4] = 1; drawLibrary[0][1][2, 0] = 1; drawLibrary[0][1][2, 1] = 0; drawLibrary[0][1][2, 2] = 0; drawLibrary[0][1][2, 3] = 0; drawLibrary[0][1][2, 4] = 1; drawLibrary[0][1][3, 0] = 1; drawLibrary[0][1][3, 1] = 0; drawLibrary[0][1][3, 2] = 0; drawLibrary[0][1][3, 3] = 0; drawLibrary[0][1][3, 4] = 1; drawLibrary[0][1][4, 0] = 1; drawLibrary[0][1][4, 1] = 1; drawLibrary[0][1][4, 2] = 1; drawLibrary[0][1][4, 3] = 1; drawLibrary[0][1][4, 4] = 1;
        drawLibrary[0][2][0, 0] = 0; drawLibrary[0][2][0, 1] = 0; drawLibrary[0][2][0, 2] = 0; drawLibrary[0][2][0, 3] = 1; drawLibrary[0][2][0, 4] = 0; drawLibrary[0][2][1, 0] = 0; drawLibrary[0][2][1, 1] = 1; drawLibrary[0][2][1, 2] = 1; drawLibrary[0][2][1, 3] = 1; drawLibrary[0][2][1, 4] = 0; drawLibrary[0][2][2, 0] = 0; drawLibrary[0][2][2, 1] = 1; drawLibrary[0][2][2, 2] = 0; drawLibrary[0][2][2, 3] = 1; drawLibrary[0][2][2, 4] = 0; drawLibrary[0][2][3, 0] = 0; drawLibrary[0][2][3, 1] = 1; drawLibrary[0][2][3, 2] = 0; drawLibrary[0][2][3, 3] = 1; drawLibrary[0][2][3, 4] = 0; drawLibrary[0][2][4, 0] = 0; drawLibrary[0][2][4, 1] = 1; drawLibrary[0][2][4, 2] = 1; drawLibrary[0][2][4, 3] = 1; drawLibrary[0][2][4, 4] = 0;
        drawLibrary[0][3][0, 0] = 1; drawLibrary[0][3][0, 1] = 1; drawLibrary[0][3][0, 2] = 1; drawLibrary[0][3][0, 3] = 1; drawLibrary[0][3][0, 4] = 1; drawLibrary[0][3][1, 0] = 1; drawLibrary[0][3][1, 1] = 1; drawLibrary[0][3][1, 2] = 0; drawLibrary[0][3][1, 3] = 0; drawLibrary[0][3][1, 4] = 1; drawLibrary[0][3][2, 0] = 1; drawLibrary[0][3][2, 1] = 0; drawLibrary[0][3][2, 2] = 0; drawLibrary[0][3][2, 3] = 0; drawLibrary[0][3][2, 4] = 1; drawLibrary[0][3][3, 0] = 1; drawLibrary[0][3][3, 1] = 0; drawLibrary[0][3][3, 2] = 0; drawLibrary[0][3][3, 3] = 0; drawLibrary[0][3][3, 4] = 1; drawLibrary[0][3][4, 0] = 1; drawLibrary[0][3][4, 1] = 1; drawLibrary[0][3][4, 2] = 1; drawLibrary[0][3][4, 3] = 1; drawLibrary[0][3][4, 4] = 1;
        drawLibrary[0][4][0, 0] = 1; drawLibrary[0][4][0, 1] = 1; drawLibrary[0][4][0, 2] = 1; drawLibrary[0][4][0, 3] = 1; drawLibrary[0][4][0, 4] = 1; drawLibrary[0][4][1, 0] = 1; drawLibrary[0][4][1, 1] = 0; drawLibrary[0][4][1, 2] = 0; drawLibrary[0][4][1, 3] = 1; drawLibrary[0][4][1, 4] = 1; drawLibrary[0][4][2, 0] = 1; drawLibrary[0][4][2, 1] = 0; drawLibrary[0][4][2, 2] = 0; drawLibrary[0][4][2, 3] = 0; drawLibrary[0][4][2, 4] = 1; drawLibrary[0][4][3, 0] = 1; drawLibrary[0][4][3, 1] = 0; drawLibrary[0][4][3, 2] = 0; drawLibrary[0][4][3, 3] = 0; drawLibrary[0][4][3, 4] = 1; drawLibrary[0][4][4, 0] = 1; drawLibrary[0][4][4, 1] = 1; drawLibrary[0][4][4, 2] = 1; drawLibrary[0][4][4, 3] = 1; drawLibrary[0][4][4, 4] = 1;
        drawLibrary[0][5][0, 0] = 1; drawLibrary[0][5][0, 1] = 1; drawLibrary[0][5][0, 2] = 1; drawLibrary[0][5][0, 3] = 1; drawLibrary[0][5][0, 4] = 0; drawLibrary[0][5][1, 0] = 1; drawLibrary[0][5][1, 1] = 1; drawLibrary[0][5][1, 2] = 0; drawLibrary[0][5][1, 3] = 1; drawLibrary[0][5][1, 4] = 0; drawLibrary[0][5][2, 0] = 1; drawLibrary[0][5][2, 1] = 1; drawLibrary[0][5][2, 2] = 0; drawLibrary[0][5][2, 3] = 1; drawLibrary[0][5][2, 4] = 0; drawLibrary[0][5][3, 0] = 1; drawLibrary[0][5][3, 1] = 1; drawLibrary[0][5][3, 2] = 0; drawLibrary[0][5][3, 3] = 1; drawLibrary[0][5][3, 4] = 1; drawLibrary[0][5][4, 0] = 1; drawLibrary[0][5][4, 1] = 1; drawLibrary[0][5][4, 2] = 1; drawLibrary[0][5][4, 3] = 1; drawLibrary[0][5][4, 4] = 1;

        // Circle
        drawLibrary[1][0][0, 0] = 1; drawLibrary[1][0][0, 1] = 1; drawLibrary[1][0][0, 2] = 1; drawLibrary[1][0][0, 3] = 1; drawLibrary[1][0][0, 4] = 0; drawLibrary[1][0][1, 0] = 1; drawLibrary[1][0][1, 1] = 0; drawLibrary[1][0][1, 2] = 0; drawLibrary[1][0][1, 3] = 1; drawLibrary[1][0][1, 4] = 1; drawLibrary[1][0][2, 0] = 1; drawLibrary[1][0][2, 1] = 1; drawLibrary[1][0][2, 2] = 0; drawLibrary[1][0][2, 3] = 0; drawLibrary[1][0][2, 4] = 1; drawLibrary[1][0][3, 0] = 1; drawLibrary[1][0][3, 1] = 1; drawLibrary[1][0][3, 2] = 0; drawLibrary[1][0][3, 3] = 1; drawLibrary[1][0][3, 4] = 1; drawLibrary[1][0][4, 0] = 0; drawLibrary[1][0][4, 1] = 1; drawLibrary[1][0][4, 2] = 1; drawLibrary[1][0][4, 3] = 1; drawLibrary[1][0][4, 4] = 0;
        drawLibrary[1][1][0, 0] = 0; drawLibrary[1][1][0, 1] = 1; drawLibrary[1][1][0, 2] = 1; drawLibrary[1][1][0, 3] = 1; drawLibrary[1][1][0, 4] = 0; drawLibrary[1][1][1, 0] = 1; drawLibrary[1][1][1, 1] = 1; drawLibrary[1][1][1, 2] = 0; drawLibrary[1][1][1, 3] = 1; drawLibrary[1][1][1, 4] = 1; drawLibrary[1][1][2, 0] = 1; drawLibrary[1][1][2, 1] = 0; drawLibrary[1][1][2, 2] = 0; drawLibrary[1][1][2, 3] = 0; drawLibrary[1][1][2, 4] = 1; drawLibrary[1][1][3, 0] = 1; drawLibrary[1][1][3, 1] = 1; drawLibrary[1][1][3, 2] = 0; drawLibrary[1][1][3, 3] = 1; drawLibrary[1][1][3, 4] = 1; drawLibrary[1][1][4, 0] = 0; drawLibrary[1][1][4, 1] = 1; drawLibrary[1][1][4, 2] = 1; drawLibrary[1][1][4, 3] = 1; drawLibrary[1][1][4, 4] = 0;
        drawLibrary[1][2][0, 0] = 0; drawLibrary[1][2][0, 1] = 1; drawLibrary[1][2][0, 2] = 1; drawLibrary[1][2][0, 3] = 1; drawLibrary[1][2][0, 4] = 0; drawLibrary[1][2][1, 0] = 1; drawLibrary[1][2][1, 1] = 1; drawLibrary[1][2][1, 2] = 0; drawLibrary[1][2][1, 3] = 1; drawLibrary[1][2][1, 4] = 1; drawLibrary[1][2][2, 0] = 1; drawLibrary[1][2][2, 1] = 0; drawLibrary[1][2][2, 2] = 0; drawLibrary[1][2][2, 3] = 0; drawLibrary[1][2][2, 4] = 1; drawLibrary[1][2][3, 0] = 1; drawLibrary[1][2][3, 1] = 1; drawLibrary[1][2][3, 2] = 0; drawLibrary[1][2][3, 3] = 1; drawLibrary[1][2][3, 4] = 1; drawLibrary[1][2][4, 0] = 0; drawLibrary[1][2][4, 1] = 1; drawLibrary[1][2][4, 2] = 1; drawLibrary[1][2][4, 3] = 1; drawLibrary[1][2][4, 4] = 1;
        drawLibrary[1][3][0, 0] = 0; drawLibrary[1][3][0, 1] = 1; drawLibrary[1][3][0, 2] = 1; drawLibrary[1][3][0, 3] = 1; drawLibrary[1][3][0, 4] = 0; drawLibrary[1][3][1, 0] = 1; drawLibrary[1][3][1, 1] = 0; drawLibrary[1][3][1, 2] = 0; drawLibrary[1][3][1, 3] = 1; drawLibrary[1][3][1, 4] = 1; drawLibrary[1][3][2, 0] = 1; drawLibrary[1][3][2, 1] = 0; drawLibrary[1][3][2, 2] = 0; drawLibrary[1][3][2, 3] = 0; drawLibrary[1][3][2, 4] = 1; drawLibrary[1][3][3, 0] = 1; drawLibrary[1][3][3, 1] = 1; drawLibrary[1][3][3, 2] = 0; drawLibrary[1][3][3, 3] = 1; drawLibrary[1][3][3, 4] = 1; drawLibrary[1][3][4, 0] = 0; drawLibrary[1][3][4, 1] = 1; drawLibrary[1][3][4, 2] = 1; drawLibrary[1][3][4, 3] = 1; drawLibrary[1][3][4, 4] = 1;
        drawLibrary[1][4][0, 0] = 0; drawLibrary[1][4][0, 1] = 1; drawLibrary[1][4][0, 2] = 1; drawLibrary[1][4][0, 3] = 1; drawLibrary[1][4][0, 4] = 0; drawLibrary[1][4][1, 0] = 1; drawLibrary[1][4][1, 1] = 1; drawLibrary[1][4][1, 2] = 0; drawLibrary[1][4][1, 3] = 1; drawLibrary[1][4][1, 4] = 1; drawLibrary[1][4][2, 0] = 1; drawLibrary[1][4][2, 1] = 0; drawLibrary[1][4][2, 2] = 0; drawLibrary[1][4][2, 3] = 0; drawLibrary[1][4][2, 4] = 1; drawLibrary[1][4][3, 0] = 1; drawLibrary[1][4][3, 1] = 0; drawLibrary[1][4][3, 2] = 0; drawLibrary[1][4][3, 3] = 1; drawLibrary[1][4][3, 4] = 1; drawLibrary[1][4][4, 0] = 1; drawLibrary[1][4][4, 1] = 1; drawLibrary[1][4][4, 2] = 1; drawLibrary[1][4][4, 3] = 1; drawLibrary[1][4][4, 4] = 0;
        drawLibrary[1][5][0, 0] = 0; drawLibrary[1][5][0, 1] = 1; drawLibrary[1][5][0, 2] = 1; drawLibrary[1][5][0, 3] = 1; drawLibrary[1][5][0, 4] = 0; drawLibrary[1][5][1, 0] = 1; drawLibrary[1][5][1, 1] = 1; drawLibrary[1][5][1, 2] = 0; drawLibrary[1][5][1, 3] = 1; drawLibrary[1][5][1, 4] = 1; drawLibrary[1][5][2, 0] = 1; drawLibrary[1][5][2, 1] = 0; drawLibrary[1][5][2, 2] = 0; drawLibrary[1][5][2, 3] = 0; drawLibrary[1][5][2, 4] = 1; drawLibrary[1][5][3, 0] = 1; drawLibrary[1][5][3, 1] = 0; drawLibrary[1][5][3, 2] = 0; drawLibrary[1][5][3, 3] = 0; drawLibrary[1][5][3, 4] = 1; drawLibrary[1][5][4, 0] = 0; drawLibrary[1][5][4, 1] = 1; drawLibrary[1][5][4, 2] = 1; drawLibrary[1][5][4, 3] = 1; drawLibrary[1][5][4, 4] = 1;
    }

    // Scroll through the library to find matches
    void CheckLibrary()
    {
        for (var c = 0; c < symbolCount; c ++)
        {
            for (var v = 0; v < symbolVariation; v++)
            {
                for (var xx = 0; xx < gridWidth; xx++)
                {
                    for (var yy = 0; yy < gridHeight; yy++)
                    {
                        if (drawData[xx, yy] != drawLibrary[c][v][xx, yy])
                        {
                            if (v == symbolVariation - 1)
                            {
                                if (c == symbolCount - 1)
                                {
                                    symbolText.text = "Symbol not found...";
                                    goto End;
                                }
                                else
                                {
                                    xx = 0;
                                    yy = 0;
                                    v = 0;
                                    c++;
                                }
                            }
                            else
                            {
                                xx = 0;
                                yy = 0;
                                v++;
                            }
                        }
                        else
                        {
                            if (xx == gridWidth - 1 && yy == gridHeight - 1)
                            {
                                symbolText.text = "This is a " + GetSymbolData(c) + "!";
                                goto End;
                            }
                        }
                    }
                }
            }
        }
        End: ;

        var str = "";
        for (var xx = 0; xx < gridWidth; xx++)
        {
            for (var yy = 0; yy < gridHeight; yy++)
            {
                str = str + "drawLibrary[1][5][" + xx + ", " + yy + "] = " + drawData[xx, yy] + "; ";
            }
        }

        drawDataText.text = str;
    }

    // Get the symbol data
    string GetSymbolData(int category)
    {
        switch (category)
        {
            // Square
            case 0:
                return "Square";

            // Circle
            case 1:
                return "Circle";

            // Failsafe
            default:
                return "None";
        }
    }
}

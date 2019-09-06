using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    // Initialize the public variables
    public float sensitivity, lineOffset, recognitionScale;
    public string axisHor, axisVer, drawButton;
    public GameObject drawPoint, drawRecognition;
    public Transform drawPointParent;
    public TextMesh symbolText;

    // Initialize the private variables
    float inkAlarm;
    List<Transform> drawPoints = new List<Transform>();
    LineRenderer lineRenderer;
    Vector2 drawEdgeHor, drawEdgeVer, drawPivot, drawSize;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(); // Move the cursor around
        SetDrawPoints(); // Instantiate the draw points
        DrawLine(); // Draw the line between the draw points
        RecognitionInit(); // Initialize the draw recognition
        GetDrawData(); // Get data from the drawing
    }

    // Move the cursor around
    void Move()
    {
        var spdHor = Input.GetAxis(axisHor) * sensitivity;
        var spdVer = Input.GetAxis(axisVer) * sensitivity;

        transform.position += (transform.right * spdHor) + (transform.up * spdVer);
    }

    // Instantiate the draw points
    void SetDrawPoints()
    {
        if (inkAlarm <= 0f)
        {
            if (Input.GetButton(drawButton))
            {
                var obj = Instantiate(drawPoint, transform.position, Quaternion.identity);
                obj.transform.parent = drawPointParent;
                drawPoints.Add(obj.transform);
            }

            inkAlarm = lineOffset;
        }
        else
            inkAlarm--;
    }

    // Draw the line between the draw points
    void DrawLine()
    {
        var amount = drawPoints.Count;
        lineRenderer.positionCount = amount;

        for (var i = 0; i < amount; i++)
            lineRenderer.SetPosition(i, drawPoints[i].position);
    }

    // Initialize the draw recognition
    void RecognitionInit()
    {
        if (Input.GetButtonUp(drawButton))
        {
            var scale = Mathf.Max(drawSize.x, drawSize.y);

            var obj = Instantiate(drawRecognition, drawPivot, Quaternion.identity);
            obj.GetComponent<DrawRecognition>().scale = scale * recognitionScale;
            obj.GetComponent<DrawRecognition>().symbolText = symbolText;
        }
    }

    // Get data from the drawing
    void GetDrawData()
    {
        if (!Input.GetButton(drawButton))
        {
            drawEdgeHor = new Vector2(transform.position.x, transform.position.x);
            drawEdgeVer = new Vector2(transform.position.y, transform.position.y);
        }

        for (var i = 0; i < drawPoints.Count; i++)
        {
            var posX = drawPoints[i].position.x;
            var posY = drawPoints[i].position.y;

            if (posX < drawEdgeHor.x)
                drawEdgeHor.x = posX;

            if (posX > drawEdgeHor.y)
                drawEdgeHor.y = posX;

            if (posY < drawEdgeVer.x)
                drawEdgeVer.x = posY;

            if (posY > drawEdgeVer.y)
                drawEdgeVer.y = posY;
        }

        drawSize.x = drawEdgeHor.y - drawEdgeHor.x;
        drawSize.y = drawEdgeVer.y - drawEdgeVer.x;

        drawPivot.x = drawEdgeHor.x + (drawSize.x / 2f);
        drawPivot.y = drawEdgeVer.x + (drawSize.y / 2f);
    }

    // Calculate the length towards a direction with a given speed for the X position
    float lengthdir_x(float len, Vector2 dir)
    {
        dir = dir.normalized * len;
        return dir.x;
    }

    // Calculate the length towards a direction with a given speed for the Y position
    float lengthdir_y(float len, Vector2 dir)
    {
        dir = dir.normalized * len;
        return dir.y;
    }
}

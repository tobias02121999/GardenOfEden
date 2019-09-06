using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    // Initialize the public variables
    public float sensitivity, lineOffset;
    public string axisHor, axisVer, drawButton;
    public GameObject drawPoint;
    public Transform drawPointParent;

    // Initialize the private variables
    float inkAlarm;
    List<Transform> drawPoints = new List<Transform>();
    LineRenderer lineRenderer;

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

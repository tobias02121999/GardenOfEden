using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintRenderer : MonoBehaviour
{
    // Initialize the public variables
    public LineRenderer lineRenderer;
    public GameObject targetObject;
    public bool isDrawing;
    public List<Transform> drawPoints = new List<Transform>();
    public int confirmDuration;

    // Initialize the private variables
    int confirmAlarm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.positionCount <= 0)
            Destroy(targetObject);

        DrawLine(); // Draw the line between the draw points
    }

    // Draw the line between the draw points
    void DrawLine()
    {
        if (isDrawing)
        {
            var amount = drawPoints.Count;
            lineRenderer.positionCount = amount;

            for (var i = 0; i < amount; i++)
                lineRenderer.SetPosition(i, drawPoints[i].position);
        }
        else
        {
            if (confirmAlarm <= 0)
            {
                lineRenderer.positionCount--;

                if (lineRenderer.positionCount >= 1)
                    confirmAlarm = confirmDuration;
            }

            confirmAlarm--;
        }
    }
}

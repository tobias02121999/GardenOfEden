using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{
    // Initialize the public variables
    public float sensitivity, lineOffset, recognitionScale, drawFrequency;
    public string axisHor, axisVer, drawButton;
    public bool debugMode, isDrawing;
    public GameObject drawPoint, drawRecognition;
    public Transform rightHand, head;
    public int handID, confirmDuration;

    // Initialize the private variables
    float posAlarm;
    List<Transform> drawPoints = new List<Transform>();
    LineRenderer lineRenderer;
    PaintRenderer paintRenderer;
    Vector2 drawEdgeHor, drawEdgeVer, drawPivot, drawSize;
    Vector3 posOld, posNew;
    AudioSource audioSource;

    GameObject drawPointObject;
    Transform drawPointParent;

    int confirmAlarm;

    float input;

    // Start is called before the first frame update
    void Start()
    {
        drawPointObject = new GameObject();
        drawPointParent = drawPointObject.transform;

        paintRenderer = GameObject.Find("Paint Renderer").GetComponent<PaintRenderer>();
        lineRenderer = paintRenderer.lineRenderer;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(); // Move the cursor around
        SetDrawPoints(); // Instantiate the draw points
        GetDrawData(); // Get data from the drawing
        GetPos();

        if (handID == 0)
            input = (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger));
        else
            input = (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));

        paintRenderer.isDrawing = isDrawing;
        paintRenderer.drawPoints = drawPoints;

        if (input != 0f)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
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
        for (var i = 0; i < drawFrequency; i++)
        {
            if (input != 0f || (Input.GetButton(drawButton) && debugMode))
            {
                if (!isDrawing)
                {
                    drawPoints.Clear();

                    if (!debugMode)
                        drawPointParent.rotation = Quaternion.Euler(0f, head.rotation.eulerAngles.y, 0f);

                    if (paintRenderer.targetObject != null)
                        Destroy(paintRenderer.targetObject);

                    drawPointParent.position = transform.position;
                    isDrawing = true;
                }

                var dist = Vector3.Distance(posOld, transform.position);

                var obj = Instantiate(drawPoint, transform.position, Quaternion.identity);
                obj.transform.parent = drawPointParent;
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0f);
                drawPoints.Add(obj.transform);

                if (drawPoints.Count <= 1)
                {
                    drawEdgeHor = new Vector2(obj.transform.localPosition.x, obj.transform.localPosition.x);
                    drawEdgeVer = new Vector2(obj.transform.localPosition.y, obj.transform.localPosition.y);
                }
            }
        }
    }

    // Initialize the draw recognition
    public void RecognitionInit()
    {
        if ((!debugMode && isDrawing) || (!Input.GetButton(drawButton) && debugMode && isDrawing))
        {
            var scale = Mathf.Max(drawSize.x, drawSize.y);

            var obj = Instantiate(drawRecognition, transform.position, Quaternion.identity);
            obj.transform.parent = drawPointParent;
            obj.transform.localPosition = new Vector3(drawPivot.x, drawPivot.y, 0f);

            var drawRecog = obj.GetComponent<DrawRecognition>();
            drawRecog.scale = scale * recognitionScale;
            drawRecog.drawPointParent = drawPointParent;
            drawRecog.debugMode = debugMode;
            drawRecog.paintRenderer = paintRenderer;

            isDrawing = false;
        }
    }

    // Get data from the drawing
    void GetDrawData()
    {
        if (isDrawing)
        {
            for (var i = 0; i < drawPoints.Count; i++)
            {
                var posX = drawPoints[i].localPosition.x;
                var posY = drawPoints[i].localPosition.y;

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
    }

    void GetPos()
    {
        if (posAlarm <= 0f)
        {
            posOld = transform.position;
            posAlarm = 1f;
        }
        else
            posAlarm--;
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

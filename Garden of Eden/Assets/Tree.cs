using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    // Initialize the public variables
    public Vector2 scale, rotation;

    // Start is called before the first frame update
    void Start()
    {
        var s = Random.Range(scale.x, scale.y);
        transform.localScale = new Vector3(s, s, s);

        var rx = Random.Range(rotation.x, rotation.y);
        var ry = Random.Range(0f, 360f);
        var rz = Random.Range(rotation.x, rotation.y);
        transform.rotation = Quaternion.Euler(rx, ry, rz);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(GameObject.Find("CenterEyeAnchor").transform);
    }
}

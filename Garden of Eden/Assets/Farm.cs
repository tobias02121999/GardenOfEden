using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.teamOneFarms.Add(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

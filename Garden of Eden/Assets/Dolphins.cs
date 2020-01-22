using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dolphins : MonoBehaviour
{
    // Initialize the public variables
    public Vector2 timerDuration;

    // Initialize the private variables
    Animator dolphinTrigger;
    int timer;

    // Start is called before the first frame update
    void Start()
    {
        dolphinTrigger = GetComponent<Animator>();
        timer = Mathf.RoundToInt(Random.Range(timerDuration.x, timerDuration.y));

        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
            dolphinTrigger.SetBool("isActive", true);

        timer--;
    }
}

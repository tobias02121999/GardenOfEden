using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSchool : MonoBehaviour
{
    // Initialize the public variables
    public Animation[] fishAnimation;
    public int[] animationOffset;
    public float speed, turnSpeed;
    public Vector2 wanderDuration;

    // Initialize the private variables
    float direction, target;
    int timer, strafeDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Main(); // Run the main state

        // Randomize the fish animation starts
        for (var i = 0; i < fishAnimation.Length; i++)
        {
            if (animationOffset[i] <= 0)
                fishAnimation[i].Play();

            animationOffset[i]--;
        }
    }

    // Run the current state
    void Main()
    {
        Rotate(); // Rotate towards the target rotation
        Move();  // Move towards its current direction

        if (timer <= 0)
        {
            timer = Mathf.RoundToInt(Random.Range(wanderDuration.x, wanderDuration.y));
            target = Random.Range(0f, 360f);
        }

        timer--;
    }

    // Rotate towards the target rotation
    void Rotate()
    {
        if (direction <= target - turnSpeed)
            direction += turnSpeed;

        if (direction >= target + turnSpeed)
            direction -= turnSpeed;

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, direction, transform.eulerAngles.z);
    }

    // Move towards its current direction
    void Move()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}

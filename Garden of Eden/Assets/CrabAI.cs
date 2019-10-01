using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAI : MonoBehaviour
{
    // Initialize the public enumerators
    public enum States { IDLE, WANDER };

    // Initialize the public variables
    public States state;
    public float speed, turnSpeed;
    public Vector2 idleDuration, wanderDuration;
    public Animator animator;

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
        RunState(); // Run the current state
        Animate(); // Animate the crab
    }

    // Run the current state
    void RunState()
    {
        switch (state)
        {
            // The idle state
            case States.IDLE:
                if (timer <= 0)
                {
                    bool rand = (Mathf.RoundToInt(Random.Range(0f, 1f)) == 0);

                    if (rand)
                        timer = Mathf.RoundToInt(Random.Range(idleDuration.x, idleDuration.y));
                    else
                    {
                        timer = Mathf.RoundToInt(Random.Range(wanderDuration.x, wanderDuration.y));
                        target = Random.Range(0f, 360f);
                        strafeDirection = Mathf.RoundToInt(Random.Range(0f, 1f));
                        state = States.WANDER;
                    }
                }
                break;

            // The wander state
            case States.WANDER:
                if (timer <= 0)
                {
                    bool rand = (Mathf.RoundToInt(Random.Range(0f, 1f)) == 0);

                    if (rand)
                    {
                        timer = Mathf.RoundToInt(Random.Range(wanderDuration.x, wanderDuration.y));
                        target = Random.Range(0f, 360f);
                    }
                    else
                    {
                        timer = Mathf.RoundToInt(Random.Range(idleDuration.x, idleDuration.y));
                        state = States.IDLE;
                    }
                }

                Rotate(); // Rotate towards the target rotation
                Move();  // Move towards its current direction
                break;
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
        var dir = 1;
        if (strafeDirection == 0)
            dir = -1;

        transform.position += (transform.right * dir) * speed * Time.deltaTime;
    }

    // Animate the crab
    void Animate()
    {
        var isWalking = (state != States.IDLE);
        animator.SetBool("IsWalking", isWalking);
    }
}

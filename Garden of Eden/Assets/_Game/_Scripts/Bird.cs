using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    // Initialize the public enums
    public enum States { FLYING, GOTOWAYPOINT, RESTING, TAKEOFF };

    // Initialize the public variables
    public float speed;
    public GameObject anim, ragdoll;
    public Transform waypoint;
    public int maxEnergy;
    public float restDistance;
    public float takeoffAngle;
    public Vector2 restDuration;
    public Animation modelAnimation;

    // Initialize the private variables
    public int energy;
    public States state;
    float flyingHeight;
    public int restAlarm;

    // Initialize the private variables
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        energy = Mathf.RoundToInt(Random.Range(-.49f, maxEnergy + .49f));

        flyingHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // FixedUpdate is called once per fixed frame
    void FixedUpdate()
    {
        RunState(); // Run the current bird state
    }

    // Check for collision and if so, turn into a ragdoll
    void OnCollisionEnter(Collision collision)
    {
        anim.SetActive(false);
        ragdoll.SetActive(true);
        GetComponent<Bird>().enabled = false;
    }

    // Run the current bird state
    void RunState()
    {
        switch (state)
        {
            case States.FLYING:
                Fly(); // Fly forward
                CheckForRest(); // Check for available waypoints if resting is required
                break;

            case States.GOTOWAYPOINT:
                Fly(); // Fly forward
                LookAtWaypoint(); // Rotate the bird to look at the waypoint
                CheckIfLanded(); // Check if the bird has landed at the waypoint
                break;

            case States.RESTING:
                Rest(); // Go to the nearest waypoint and rest
                break;

            case States.TAKEOFF:
                Fly(); // Fly forward
                LookAtSky(); // Rotate the bird to look at the waypoint
                CheckIfAirborne(); // Check if the bird is airborne
                break;
        }
    }

    // Fly forward
    void Fly()
    {
        rb.velocity = transform.forward * speed;
    }

    // Rotate the bird to look at the waypoint
    void LookAtWaypoint()
    {
        transform.LookAt(waypoint);
    }

    // Rotate the bird to look at the sky
    void LookAtSky()
    {
        transform.rotation = Quaternion.Euler(-takeoffAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    // Check for available waypoints if resting is required
    void CheckForRest()
    {
        energy--;

        if (energy <= 0 && waypoint != null)
            state = States.GOTOWAYPOINT;
    }

    // Check if the bird has landed at the waypoint
    void CheckIfLanded()
    {
        if (waypoint != null)
        {
            var dist = Vector3.Distance(transform.position, waypoint.position);
            if (dist <= restDistance)
            {
                rb.velocity = new Vector3(0f, 0f, 0f);
                restAlarm = Mathf.RoundToInt(Random.Range(restDuration.x - .45f, restDuration.y + .45f));
                modelAnimation.Play("Bird Resting");
                state = States.RESTING;
            }
        }
        else
            state = States.TAKEOFF;
    }

    // Check if the bird is airborne
    void CheckIfAirborne()
    {
        if (transform.position.y >= flyingHeight)
        {
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);
            state = States.FLYING;
        }
    }

    // Go to the nearest waypoint and rest
    void Rest()
    {
        restAlarm--;

        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);

        if (restAlarm <= 0 || waypoint == null)
        {
            energy = Mathf.RoundToInt(Random.Range(-.49f, maxEnergy + .49f));
            modelAnimation.Play("Bird Flying");
            state = States.TAKEOFF;
        }
    }
}

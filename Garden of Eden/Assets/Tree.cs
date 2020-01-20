using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tree : NetworkBehaviour
{
    // Initialize the public enums
    public enum States { SAPLING, GROWN, CHOPPED }

    // Initialize the public variables
    public Vector2 scale, rotation;
    public GameObject[] modelStates;
    public Vector2 baseGrowSpeed, respawnDuration;
    public States state;
    public bool isWet;
    public ParticleSystem buffEffect;

    // Initialize the private variables
    public float growth, growSpeed, respawnAlarm;

    // Start is called before the first frame update
    void Start()
    {
        respawnAlarm = Random.Range(respawnDuration.x, respawnDuration.y);
        growSpeed = Random.Range(baseGrowSpeed.x, baseGrowSpeed.y);

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
        RunState(); // Run the current tree state

        if (isWet && !buffEffect.isPlaying)
            buffEffect.Play();

        if (!isWet && buffEffect.isPlaying)
            buffEffect.Stop();
    }

    // Run the current tree state
    void RunState()
    {
        switch (state)
        {
            case States.SAPLING:
                DisplayModel(0); // Display the chosen model state from the model state array
                Grow(); // Grow as a sapling into a grown tree
                break;

            case States.GROWN:
                DisplayModel(1); // Display the chosen model state from the model state array
                break;

            case States.CHOPPED:
                DisplayModel(2); // Display the chosen model state from the model state array
                Respawn(); // Respawn as a sapling after being chopped
                break;
        }
    }

    // Display the chosen model state from the model state array
    void DisplayModel(int index)
    {
        var length = modelStates.Length;
        for (var i = 0; i < length; i++)
            modelStates[i].SetActive(i == index);
    }

    // Grow as a sapling into a grown tree
    void Grow()
    {
        if (isWet)
            growth += (growSpeed * 4f);
        else
            growth += growSpeed;

        if (growth >= 100f)
        {
            growth = 0f;
            respawnAlarm = Random.Range(respawnDuration.x, respawnDuration.y);
            state = States.GROWN;
        }
    }

    // Respawn as a sapling after being chopped
    void Respawn()
    {
        respawnAlarm--;

        if (respawnAlarm <= 0f)
            state = States.SAPLING;
    }
}

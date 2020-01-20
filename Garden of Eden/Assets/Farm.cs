using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Farm : NetworkBehaviour
{
    // Initialize the public variables
    [SyncVar]
    public bool isWet;

    public float foodScore;
    public ParticleSystem buffEffect;
    public GameObject farmlands;

    // Initialize the private variables
    float baseFoodScore = 2f;
    Grab grab;

    // Start is called before the first frame update
    void Start()
    {
        foodScore = baseFoodScore;
        grab = GetComponent<Grab>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWet)
        {
            foodScore = baseFoodScore * 2f;

            if (!buffEffect.isPlaying)
                buffEffect.Play();
        }
            
        else
        {
            foodScore = baseFoodScore;

            if (buffEffect.isPlaying)
                buffEffect.Stop();
        }

        farmlands.SetActive(grab.isGrounded);
    }
}

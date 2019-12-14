using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    // Initialize the public variables
    public bool isWet;
    public float foodScore;
    public ParticleSystem buffEffect;

    // Initialize the private variables
    float baseFoodScore = 2f;

    // Start is called before the first frame update
    void Start()
    {
        foodScore = baseFoodScore;
        GameManager.Instance.teamOneFarms.Add(transform);
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
    }
}

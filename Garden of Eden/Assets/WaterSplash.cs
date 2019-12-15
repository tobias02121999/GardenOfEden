using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    // Initialize the public variables
    public Vector2 splashAlarmDuration;
    public ParticleSystem[] splashEffect;

    // Initialize the private variables
    int splashAlarm;

    // Start is called before the first frame update
    void Start()
    {
        splashAlarm = Mathf.RoundToInt(Random.Range(splashAlarmDuration.x, splashAlarmDuration.y));
    }

    // Update is called once per frame
    void Update()
    {
        if (splashAlarm <= 0)
        {
            var length = splashEffect.Length;
            for (var i = 0; i < length; i++)
                splashEffect[i].Play();

            splashAlarm = Mathf.RoundToInt(Random.Range(splashAlarmDuration.x, splashAlarmDuration.y));
        }

        splashAlarm--;
    }
}

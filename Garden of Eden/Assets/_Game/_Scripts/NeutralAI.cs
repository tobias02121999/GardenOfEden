﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralAI : HumanAI
{
    public override void Start()
    {
        base.Start();

        if (GameManager.Instance.TeamOneNeutralHumans.Contains(gameObject))
            switchShrine = false;

        if (GameManager.Instance.TeamTwoNeutralHumans.Contains(gameObject))
            switchShrine = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        // The neutral human prays at a different shrine every night. The first human to please it will convert it to their side.
        if ((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360))
        {
            // During the day the only thing it does is switch the shrine it will pray at during the upcoming night, and idle about.
            currentState = HumanState.IDLE;
            if (!shrineSwitched)
            {

                if (switchShrine == false)
                {
                    GameManager.Instance.TeamOneNeutralHumans.Remove(gameObject);
                    GameManager.Instance.TeamTwoNeutralHumans.Add(gameObject);
                    switchShrine = true;
                }
                else
                {
                    GameManager.Instance.TeamTwoNeutralHumans.Remove(gameObject);
                    GameManager.Instance.TeamOneNeutralHumans.Add(gameObject);
                    switchShrine = false;
                }

                shrineSwitched = true;

            }
        }

        if (Sun.Instance.rotation >= 90 && Sun.Instance.rotation <= 180)
        {
            // Then, at nighttime, the human will pray as normal together with the non-neutral humans, in an attempt to get what it desires.
            currentState = HumanState.PRAYING;
            shrineSwitched = false;
        }
    }
}

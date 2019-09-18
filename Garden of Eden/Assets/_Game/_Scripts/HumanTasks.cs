using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Panda;

public class HumanTasks : MonoBehaviour
{
    public RagdollAnimator humanAnimator;
    public float fearGauge, fearMultiplier, fearReductionSpeed;
    [HideInInspector] float baseSpeed;

    float closestObject = 10000f;
    Vector3 closestObjectPosition = Vector3.zero;

    // AI BEHAVIOR
    [Task]
    bool MeteorHasSpawned() 
    {
        if (DrawRecognition.returnShape == "Circle" || Input.GetMouseButtonDown(0))    // Checking if the player has drawn a circle
        {
            Task.current.Succeed();
            return true;
        }
        else
        {
            Task.current.Succeed();                        // Else fail this task
            return false;
        }
    }

    [Task]
    void GaugeFear()
    {
        if (GameManager.Instance.fearObjects.Count <= 0)
        {
            Task.current.Fail();
        }
        else
        {
            fearGauge = 0f;

            for (int i = 0; i < GameManager.Instance.fearObjects.Count; i++)
            {
                float newObjectDistance = Vector3.Distance(this.transform.position, GameManager.Instance.fearObjects[i].transform.position);

                if (newObjectDistance < closestObject)
                {
                    closestObject = newObjectDistance;
                    closestObjectPosition = GameManager.Instance.fearObjects[i].transform.position;
                }
            }

            fearGauge = Mathf.Clamp(fearGauge + 100 - Vector3.Distance(this.transform.position, closestObjectPosition) / 0.75f, 0, 100);
            SetSpeed();

            Task.current.Succeed();
        }
    }

    void SetSpeed()
    {
        if (fearGauge <= 100)
        {
            humanAnimator.speed = 0f;
            humanAnimator.speed = Mathf.Clamp(humanAnimator.speed + fearGauge / 4, 2, 12);
        }
    }

    [Task]
    void CheckForDanger()
    {
        if (GameManager.Instance.fearObjects.Count <= 0)
        {
            fearGauge = 0f;
            SetSpeed();
        }
        else
        {
            GaugeFear();
        }

        Task.current.Succeed();
    }
}

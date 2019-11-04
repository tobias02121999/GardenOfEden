﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public enum HumanState {RECOVER, IDLE, CHOPPING, BUILDING_HOUSE, BUILDING_ALTAR, GATHERING_RESOURCES, FIGHTING, PRAYING, SPREADING_RELIGION};

public class HumanAI : Singleton<HumanAI>
{
    [Header("Current AI State:")]
    public HumanState currentState;

    [Space]

    public RagdollAnimator humanAnimator;
    public Rigidbody hips;
    public Transform humanMesh, movementParent, rotationReference;
    public int secondsSinceLastBuild;
    public float speed, wanderDuration, turnSpeed, fear, faith, happiness, fearReductionSpeed;
    public float wanderAlarm, minDistanceFromBuildToCalamity, gatheredWood;
    [HideInInspector] float baseSpeed;
    [HideInInspector] bool _inRangeOfTree;

    float closestObject = Mathf.Infinity;
    float closestLingeringObject = Mathf.Infinity;
    Vector3 closestObjectPosition = Vector3.zero;
    Vector3 closestLingeringObjectPosition = Vector3.zero;
    bool wasInvoked = false;
    bool _wasInvoked = false; 
    bool checkForTree = false; 
    List<GameObject> houses = new List<GameObject>();
    List<GameObject> people = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (!wasInvoked)
        {
            StartCoroutine("BuildTimer");   // Continually runs the build timer.
            ReduceFearOverTime();
        }

        if (!_wasInvoked)
            StartCoroutine("IncreaseFaithOverTime");

        AddForce();

        #region State Declaration
        // Building has #1 priority, then comes chopping trees.
        if (humanAnimator.hasCollapsed)
            currentState = HumanState.RECOVER;
        else if (CheckForCalamitySites() && CheckResources() && secondsSinceLastBuild >= 5 && CheckForSufficientRoom() && gatheredWood >= 30)
            currentState = HumanState.BUILDING_HOUSE;
        else if (CheckResources() && gatheredWood < 30)
            currentState = HumanState.CHOPPING;
        else
            currentState = HumanState.IDLE;
        
        // Keep gauging fear over time.
        if (GameManager.Instance.fearObjects.Count > 0)
            GaugeFear();
        #endregion

        #region State Machine
        switch (currentState)
        {
            case HumanState.RECOVER:
                // Do nothing untill recovered.
                break;

            case HumanState.IDLE: // Human wanders about when idle.
                if (!humanAnimator.hasCollapsed)
                {
                    if (wanderAlarm <= 0f)
                    {
                        humanAnimator.targetRot = Random.Range(0f, 360f);
                        wanderAlarm = wanderDuration;

                        Debug.Log("Randomize");
                    }

                    wanderAlarm--;

                    if (humanAnimator.currentRot >= humanAnimator.targetRot + turnSpeed)
                        humanAnimator.currentRot -= turnSpeed;

                    if (humanAnimator.currentRot <= humanAnimator.targetRot - turnSpeed)
                        humanAnimator.currentRot += turnSpeed;

                    movementParent.rotation = Quaternion.Euler(0f, humanAnimator.currentRot, 0f);              
                }

                break;

            case HumanState.CHOPPING: // The human chops a tree.
                MoveToDestination(1);   // Move to the tree first.

                int layer = 17;
                int mask = 1 << layer;

                Collider[] trees = Physics.OverlapSphere(humanMesh.position, 1.5f, mask);
                foreach (Collider tree in trees)
                {
                    Destroy(tree.gameObject);   // Then destroy all nearby trees.
                    gatheredWood += 10;
                }

                break;

            case HumanState.BUILDING_HOUSE:   // The human builds a house. secondsSinceLastBuild value is a debug value, change when ready.
                if (houses.Count < people.Count)
                {
                    Vector3 inFront = transform.position + new Vector3(0, 0, 6);
                    var house = ObjectPooler.Instance.SpawnFromPool("House", inFront, Quaternion.identity);

                    houses.Add(house);

                    gatheredWood -= 30;
                    secondsSinceLastBuild = 0;
                }

                break;

            case HumanState.BUILDING_ALTAR:
                
                break;

            case HumanState.GATHERING_RESOURCES:
                // Obsolete State? See chopping.
                break;

            case HumanState.FIGHTING:
                break;

            case HumanState.PRAYING:
                MoveToDestination(2);

                break;

            case HumanState.SPREADING_RELIGION:
                break;
        }
        #endregion
    }

    Transform GetClosestUnit(Transform[] units)
    {
        Transform closestUnit = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform unit in units)
        {
            float dist = Vector3.Distance(unit.position, currentPosition);
            if (dist < minDistance)
            {
                closestUnit = unit;
                minDistance = dist;
            }
        }
        return closestUnit;
    }

    void GaugeFear()    // Add or remove fear based on the specified circumstances.
    {
        if (GameManager.Instance.fearObjects.Count <= 0)    // If there are no fear objects in the scene fail this task.
        {
            SetSpeed();
        }
        else
        {
            fear = 0f; // Reset the fear gauge to adjust it (otherwise it will just add onto the former amount.
            GameManager.Instance.fearObjects.Clear(); // Reset the list
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("FearObject").Length; i++)
            {
                GameManager.Instance.fearObjects.Add(GameObject.FindGameObjectsWithTag("FearObject")[i]);   // Add all the fear objects back into the scene.
            }

            for (int i = 0; i < GameManager.Instance.fearObjects.Count; i++)    // Scan through all objects that can instill fear.
            {
                // Determine the current objects distance.
                float currentObjectDistance = Vector3.Distance(humanMesh.position, GameManager.Instance.fearObjects[i].transform.position);

                if (currentObjectDistance < closestObject) // If an objects distance is smaller than the former distance, the current objects distance will be the new closest distance.
                {
                    closestObject = currentObjectDistance;
                    closestObjectPosition = GameManager.Instance.fearObjects[i].transform.position;
                }
            }

            // Big Math
            var distanceToFear = fear + 100 - Vector3.Distance(humanMesh.position, closestObjectPosition);
            fear = Mathf.Clamp(distanceToFear, 0, 100); // Set fear
            SetSpeed();
        }
    }

    void SetSpeed() // Sets the speed of this human based on the amount of fear.
    {
        if (fear <= 100)
        {
            speed = 0f;
            var adjustedFear = fear / 20;
            speed = Mathf.Clamp(speed + adjustedFear, 10, 20);
            Debug.Log("Speed adjusted to " + speed);
        }
    }

    void AddForce()
    {
        var length = humanAnimator.bones.Length;
        for (var i = 0; i < length; i++)
        {
            var bone = humanAnimator.bones[i].GetComponent<Rigidbody>();
            bone.AddForce(humanAnimator.movementParent.forward * speed);
        }
    }

    bool CheckResources()   // Check if there are enough resources (Phony mechanic).
    {
        Collider[] cols = Physics.OverlapSphere(humanMesh.position, 100f);  // Check in a radius of 100f (DONT FORGET TO ADD A LAYERMASK TO IGNORE HUMANS AND BUILDINGS).
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Tree"))
            {
                CheckForSufficientRoom();
                return true;
            }
        }

        return false;
    }

    bool CheckForCalamitySites()    // Check if any calamities recently took place nearby.
    {
        for (int i = 0; i < GameManager.Instance.lingeringFearObjects.Count; i++)   // Compare all the lingering fear object's distance from the human.
        {
            float newDistanceFromCalamity = Vector3.Distance(humanMesh.position, GameManager.Instance.lingeringFearObjects[i].transform.position);

            if (newDistanceFromCalamity < closestLingeringObject)
            {
                closestLingeringObject = newDistanceFromCalamity;
            }
        }

        if (closestLingeringObject >= minDistanceFromBuildToCalamity)   // The task succeeds if the minimal distance to a calamity is greater than or equal to the build distance.
        {
            CheckResources();
            return true;
        }
        else    // It fails if this distance is shorter.
        {
            return false;
        }
    }

    bool CheckForSufficientRoom()   // Check's if there is room to build the house.
    {
        int buildingLayer = 16;
        int resourceLayer = 17;
        int playerLayer = 23;
        int handsLayer = 14;
        int buildingMask = 1 << buildingLayer;
        int resourceMask = 1 << resourceLayer;
        int playerMask = 1 << playerLayer;
        int handsMask = 1 << handsLayer;

        Vector3 halfExtends = new Vector3(3.5f, 3f, 3.5f);
        Collider[] homes = Physics.OverlapBox(humanMesh.position, halfExtends, Quaternion.identity, buildingMask);
        Collider[] trees = Physics.OverlapBox(humanMesh.position, halfExtends, Quaternion.identity, resourceMask);
        Collider[] player = Physics.OverlapBox(humanMesh.position, halfExtends, Quaternion.identity, playerMask);
        Collider[] hands = Physics.OverlapBox(humanMesh.position, halfExtends, Quaternion.identity, handsMask);
        if (homes.Length > 0 || trees.Length > 0 || player.Length > 0 || hands.Length > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void MoveToDestination(int destinationIndex)
    {
        switch (destinationIndex)   // Goto...
        {
            case 0: // ...Nearest house.
                break;

            case 1: // ...Nearest tree.
                int layer = 17;
                int mask = 1 << layer;

                Collider[] trees = Physics.OverlapSphere(humanMesh.position, 50f, mask);
                List<Transform> colTransforms = new List<Transform>();
                foreach (Collider col in trees)
                {
                    colTransforms.Add(col.transform);
                }

                rotationReference.LookAt(GetClosestUnit(colTransforms.ToArray()));
                var rot = rotationReference.rotation;
                rot.x = 0f;
                rot.z = 0f;

                movementParent.rotation = rot;

                break;

            case 2: // ...Nearest altar.
                List<Transform> altars = new List<Transform>();

                int prayingLayer = 19;
                int prayingMask = 1 << prayingLayer;

                Collider[] prayingGrounds = Physics.OverlapSphere(humanMesh.position, 25f, prayingMask);
                foreach (Collider altar in prayingGrounds)
                {
                    altars.Add(altar.transform);
                }
                break;
        }
    }

    IEnumerator BuildTimer()    // To make sure humans don't go building houses the second they are born.
    {
        wasInvoked = true;

        yield return new WaitForSeconds(1); // Wait for one second.
        secondsSinceLastBuild++;

        wasInvoked = false;
    }

    IEnumerator IncreaseFaithOverTime()
    {
        _wasInvoked = true;

        if (faith >= 100)
        {
            // "Upgrade" to Prophet.
        }
        else
        {
            yield return new WaitForSeconds(5f);
            faith++;
            faith = Mathf.Clamp(faith, 0, 100);
        }

        _wasInvoked = false;
    }

    void ReduceFearOverTime()
    {
        if (GameManager.Instance.fearObjects.Count <= 0)
        {
            fear = Mathf.Lerp(fear, 0, Time.deltaTime * fearReductionSpeed);
            SetSpeed();
        }
    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public enum HumanState {IDLE, CHOPPING, BUILDING, GATHERING_RESOURCES, FIGHTING, PRAYING, SPREADING_RELIGION};

public class HumanAI : Singleton<HumanAI>
{
    [Header("Current AI State:")]
    public HumanState currentState;

    [Space]

    public RagdollAnimator humanAnimator;
    public Transform humanMesh, movementParent;
    public GameObject home;
    public int secondsSinceLastBuild;
    public float fearGauge, fearReductionSpeed, minDistanceFromBuildToCalamity, minResourceRequired, gatheredWood;
    [HideInInspector] float baseSpeed;
    [HideInInspector] bool _inRangeOfTree;


    float closestObject = 10000f;
    float closestLingeringObject = 10000f;
    Vector3 closestObjectPosition = Vector3.zero;
    Vector3 closestLingeringObjectPosition = Vector3.zero;
    bool wasInvoked = false;
    bool checkForTree = false;

    // Update is called once per frame
    void Update()
    {
        if (!wasInvoked)
        {
            StartCoroutine("BuildTimer");   // Continually runs the build timer.
            ReduceFearOverTime();
        }

        if (checkForTree)
        {
            Collider[] trees = Physics.OverlapSphere(humanMesh.position, 3f);
            Debug.Log(trees.Length);
            foreach (Collider collider in trees)
            {
                if (collider.tag == "Tree")
                {
                    _inRangeOfTree = true;
                    currentState = HumanState.IDLE;
                }
            }
        }

        #region State Declaration
        // Building has #1 priority, then comes chopping trees.
        if (CheckForCalamitySites() && CheckResources() && secondsSinceLastBuild >= 5 && CheckForSufficientRoom() && gatheredWood >= 30)
            currentState = HumanState.BUILDING;
        else if (CheckResources())
            currentState = HumanState.CHOPPING;
        else
            currentState = HumanState.PRAYING;
        
        // Keep gauging fear over time.
        if (GameManager.Instance.fearObjects.Count > 0)
            GaugeFear();
        #endregion

        #region State Machine
        switch (currentState)
        {
            case HumanState.IDLE: // Human wanders about when idle.
                if (!humanAnimator.hasCollapsed)
                {
                    if (humanAnimator.wanderAlarm <= 0f)
                    {
                        humanAnimator.targetRot = Random.Range(0f, 360f);
                        humanAnimator.wanderAlarm = humanAnimator.wanderDuration;

                        Debug.Log("Randomize");
                    }

                    humanAnimator.wanderAlarm--;

                    if (humanAnimator.currentRot >= humanAnimator.targetRot + humanAnimator.turnSpeed)
                        humanAnimator.currentRot -= humanAnimator.turnSpeed;

                    if (humanAnimator.currentRot <= humanAnimator.targetRot - humanAnimator.turnSpeed)
                        humanAnimator.currentRot += humanAnimator.turnSpeed;

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

            case HumanState.BUILDING:   // The human builds a house. secondsSinceLastBuild value is a debug value, change when ready.
                    gameObject.SetActive(false);
                    Instantiate(home, humanMesh.position, Quaternion.identity);

                    Destroy(gameObject);    // Remove the human from the game when the house is built.

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
            fearGauge = 0f; // Reset the fear gauge to adjust it (otherwise it will just add onto the former amount.
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
            var distanceToFear = fearGauge + 100 - Vector3.Distance(humanMesh.position, closestObjectPosition);
            fearGauge = Mathf.Clamp(distanceToFear, 0, 100); // Set fear
            SetSpeed();
        }
    }

    void SetSpeed() // Sets the speed of this human based on the amount of fear.
    {
        if (fearGauge <= 100)
        {
            humanAnimator.speed = 0f;
            var adjustedFear = fearGauge / 20;
            humanAnimator.speed = Mathf.Clamp(humanAnimator.speed + adjustedFear, 2, 5);
            Debug.Log("Speed adjusted to " + humanAnimator.speed);
        }
    }

    bool CheckResources()   // Check if there are enough resources (Phony mechanic).
    {
        Collider[] cols = Physics.OverlapSphere(humanMesh.position, 100f);  // Check in a radius of 100f (DONT FORGET TO ADD A LAYERMASK TO IGNORE HUMANS AND BUILDINGS).
        foreach (Collider col in cols)
        {
            if (col.tag == "Tree")
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
        int layer = 16;
        int mask = 1 << layer;

        Vector3 halfExtends = new Vector3(3.5f, 3f, 3.5f);
        Collider[] homes = Physics.OverlapBox(humanMesh.position, halfExtends, Quaternion.identity, mask);
        Debug.Log(homes.Length);
        if (homes.Length > 0)
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
                if (trees.Length > 0)
                {
                    checkForTree = true;
                }
                foreach (Collider col in trees)
                {
                    float distanceToTree = Vector3.Distance(humanMesh.position, col.transform.position);
                    if (distanceToTree < 20)
                    {
                        movementParent.LookAt(col.transform.position);
                    }
                }
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

                GetClosestUnit(altars.ToArray());
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

    void ReduceFearOverTime()
    {
        if (GameManager.Instance.fearObjects.Count <= 0)
        {
            fearGauge = Mathf.Lerp(fearGauge, 0, Time.deltaTime * fearReductionSpeed);
            SetSpeed();
        }
    }
}

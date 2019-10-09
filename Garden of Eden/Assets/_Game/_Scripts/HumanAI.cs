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
    public float fearGauge, fearReductionSpeed, minDistanceFromBuildToCalamity, minResourceRequired;
    [HideInInspector] float baseSpeed;
    [HideInInspector] bool _inRangeOfTree;


    float closestObject = 10000f;
    float closestLingeringObject = 10000f;
    Vector3 closestObjectPosition = Vector3.zero;
    Vector3 closestLingeringObjectPosition = Vector3.zero;
    bool wasInvoked = false;
    bool checkForTree = false;

    // Start is called before the first frame update
    void Start() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                int layer = 17;
                int mask = 1 << layer;

                Collider[] trees = Physics.OverlapSphere(humanMesh.position, 1.5f, mask);
                foreach (Collider tree in trees)
                {
                    Destroy(tree.gameObject);
                }

                break;

            case HumanState.BUILDING:   // The human builds a house. secondsSinceLastBuild value is a debug value, change when ready.
                if (CheckForCalamitySites() && CheckResources() && secondsSinceLastBuild >= 5 && CheckForSufficientRoom())
                {
                    gameObject.SetActive(false);
                    Instantiate(home, humanMesh.position, Quaternion.identity);

                    Destroy(gameObject);    // Remove the human from the game when the house is built.
                }

                break;

            case HumanState.GATHERING_RESOURCES:
                // Obsolete State? See chopping.
                break;

            case HumanState.FIGHTING:
                break;

            case HumanState.PRAYING:
                break;

            case HumanState.SPREADING_RELIGION:
                break;
        }
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
            var adjustedFear = fearGauge / 10;
            humanAnimator.speed = Mathf.Clamp(humanAnimator.speed + adjustedFear, 2, 12);
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

    IEnumerator BuildTimer()    // To make sure humans don't go building houses the second they are born.
    {
        wasInvoked = true;

        yield return new WaitForSeconds(1); // Wait for one second.
        secondsSinceLastBuild++;

        wasInvoked = false;
    } 
}

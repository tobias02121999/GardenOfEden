using System.Collections;
using UnityEngine;
using Panda;

public class HumanTasks : MonoBehaviour
{
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

    private void Update()
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
                }
            }
        }
    }

    //// AI BEHAVIOR
    [Task]
    void Wander() // Randomize the movement direction
    {
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
    }

    [Task]
    bool MeteorHasSpawned() // Check if a meteor has spawned.
    {
        if (DrawRecognition.returnShape == "Circle" || Input.GetMouseButtonDown(0))    // Checking if the player has drawn a circle
        {
            Task.current.Succeed();
            return true;
        }
        else{
            Task.current.Succeed();
            return false;
        }
    }

    [Task]
    void GaugeFear()    // Add or remove fear based on the specified circumstances.
    {
        if (GameManager.Instance.fearObjects.Count <= 0)    // If there are no fear objects in the scene fail this task.
        {
            Task.current.Succeed();
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

            Task.current.Succeed();
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

    [Task]
    void CheckForDanger() // Checks if there is a fear object in the scene.
    {
        if (GameManager.Instance.fearObjects.Count <= 0)
        {
            SetSpeed();
            Task.current.Succeed();
        }
        else
        {
            GaugeFear();
        }

        Task.current.Fail();
    }

    [Task]
    bool IsCalm()   // Checks if the human is fearful or not.
    {
        if (fearGauge <= 0)
        {
            Task.current.Succeed();
            return true;
        }
        else
        {
            Task.current.Fail();
            return false;
        }
    }

    [Task]
    void CheckForCalamitySites()    // Check if any calamities recently took place nearby.
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
            Task.current.Succeed();
        }
        else    // It fails if this distance is shorter.
        {
            Task.current.Fail();
        }
    }

    [Task]
    void CheckResources()   // Check if there are enough resources (Phony mechanic).
    {
        int treesNearby = 0;
        Collider[] cols = Physics.OverlapSphere(humanMesh.position, 100f);  // Check in a radius of 100f (DONT FORGET TO ADD A LAYERMASK TO IGNORE HUMANS AND BUILDINGS).
        foreach (Collider col in cols)
        {
            if (col.tag == "Tree")
            {
                treesNearby++;
            }
        }

        if (treesNearby >= minResourceRequired) // If there's enough trees nearby this task succeeds.
        {
            Task.current.Succeed();
        }
        else // If not, it fails.
        {
            Task.current.Fail();
        }
    }

    [Task]
    void CheckTimeSinceLastBuild()  // Checks if enough time has passed to actually build a house.
    {
        if (secondsSinceLastBuild > 5)  // Check the time since this human last built something (5 seconds is debug time. Increase for release).
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    bool CheckForSufficientRoom()   // Check's if there is room to build the house.
    {
        int layer = 16;
        int mask = 1 << layer;

        Vector3 halfExtends = new Vector3(3.5f, 3f, 3.5f);
        Collider[] homes = Physics.OverlapBox(humanMesh.position, halfExtends, Quaternion.identity, mask);
        Debug.Log(homes.Length);
        if (homes.Length > 0)
        {
            Task.current.Succeed();
            return false;
        }
        else{
            Task.current.Succeed();
            return true;
        }
    }

    [Task]
    void BuildHome()    // Build the house.
    {
        gameObject.SetActive(false);
        Instantiate(home, humanMesh.position, Quaternion.identity);

        Destroy(gameObject);    // Remove the human from the game when the house is built.
    }

    [Task]
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
                        Task.current.Succeed();
                    }
                }
                break;

            case 2:
                break;
        }
    }

    [Task]
    bool InRangeOfTree()
    {
        if (_inRangeOfTree)
        {
            Task.current.Succeed();
            return true;
        }
        else
        {
            Task.current.Fail();
            return false;
        }
    }

    [Task]
    void ChopTree()
    {
        int layer = 17;
        int mask = 1 << layer;

        Collider[] trees = Physics.OverlapSphere(humanMesh.position, 1.5f, mask);
        foreach (Collider tree in trees)
        {
            Destroy(tree.gameObject);
            Task.current.Succeed();
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
        }
    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum HumanState {RECOVER, HUNGRY, IDLE, BUILDING_HOUSE, BUILDING_CHURCH, GATHERING_RESOURCES, FIGHTING, PRAYING, SLEEPING, SPREADING_RELIGION};
public enum HumanDesire {HOUSING, FOOD, TO_ASCEND, NOTHING}

public class HumanAI : MonoBehaviour
{
    [Header("Current AI State:")]
    public HumanState currentState;

    [Header("This human currently desires:")]
    public HumanDesire currentDesire = HumanDesire.NOTHING;
    public UnityEngine.UI.Image[] desireClouds;

    [Header("Focus Vars")]
    public float fear;
    public float faith = 1;
    public GameObject halo;
    public float happiness = 100;
    public bool hasWood = false;
    public bool hungry = false;
    public bool isAscended = false;
    public GameObject house;
    public GameObject hologram;
    public bool isDayTime = false;

    [Space]

    public RagdollAnimator humanAnimator;
    public Rigidbody hips;
    public Transform humanMesh, movementParent, rotationReference;
    public int secondsSinceLastBuild, fearReductionSpeed;
    public float speed, wanderDuration, turnSpeed;
    public float wanderAlarm, minDistanceFromBuildToCalamity, gatheredWood;
    public bool atShrine;
    [HideInInspector] float baseSpeed;
    [HideInInspector] bool _inRangeOfTree;

    float closestObject = Mathf.Infinity;
    float closestLingeringObject = Mathf.Infinity;
    Vector3 closestObjectPosition = Vector3.zero;
    Vector3 closestLingeringObjectPosition = Vector3.zero;
    bool wasInvoked = false;
    bool _wasInvoked = false;
    bool buildingHouse = false;
    bool desireStated = false;
    bool happinessDecreasing = false;
    bool checkForTree = false; 
    bool readyToAscend = false;
    List<GameObject> houses = new List<GameObject>();
    List<GameObject> people = new List<GameObject>();

    private void Start()
    {
        currentDesire = HumanDesire.NOTHING;
    }
    // Update is called once per frame
    void Update() 
    {
        if (isAscended)
            halo.SetActive(true);
        else
            halo.SetActive(false);

        //if (fear >= 100f || happiness <= 0f) // Neutralize human's faith.
        //{
        //        // give player a chance to please the human before making him leave.
        //    faith = 0;

        //    if (GameManager.Instance.TeamOneHumans.Contains(gameObject)) GameManager.Instance.TeamOneHumans.Remove(gameObject);
        //    if (GameManager.Instance.TeamTwoHumans.Contains(gameObject)) GameManager.Instance.TeamTwoHumans.Remove(gameObject);

        //    GameManager.Instance.NeutralHumans.Add(gameObject);
        //}

        if (!wasInvoked)
        {
            StartCoroutine("BuildTimer");   // Continually runs the build timer.
            ReduceFearOverTime();
        }

        if (!_wasInvoked)
            StartCoroutine("IncreaseFaithOverTime");    // Continually gain levels of faith.

        if (!humanAnimator.hasCollapsed)
            AddForce();

        if (readyToAscend) 
            currentDesire = HumanDesire.TO_ASCEND;

        #region State Declaration
        if ((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360))
        {
            isDayTime = true;
            if (humanAnimator.hasCollapsed)
                currentState = HumanState.RECOVER;
            else if (hungry)
            {
                currentDesire = HumanDesire.FOOD;
                if (happinessDecreasing == false)
                    StartCoroutine("ReduceHappinessOverTime");
                currentState = HumanState.HUNGRY;
            }
            else if (CheckForCalamitySites() && secondsSinceLastBuild >= 5 && !buildingHouse)
                currentState = HumanState.BUILDING_HOUSE;
            else
                currentState = HumanState.IDLE;
        }

        if (Sun.Instance.rotation >= 90 && Sun.Instance.rotation <= 180)
        {
            isDayTime = false;
            currentState = HumanState.PRAYING;
        }
        else if (Sun.Instance.rotation > 180 && Sun.Instance.rotation <= 270)
        {
            isDayTime = false;
            currentState = HumanState.SLEEPING;
        }
            
        // Keep gauging fear over time.
        if (GameManager.Instance.fearObjects.Count > 0)
            GaugeFear();
        #endregion

        #region State Machine Day
        if (isDayTime)
        {
            Debug.Log("dayTime");

            switch (currentState)
            {
                case HumanState.RECOVER:
                    // Do nothing untill recovered.
                    break;

                case HumanState.HUNGRY:
                    MoveToDestination(0);

                    int berryLayer = 23;
                    int berryMask = 1 << berryLayer;

                    Collider[] berries = Physics.OverlapSphere(humanMesh.position, 1.5f, berryMask);
                    foreach (Collider berry in berries)
                    {
                        Destroy(berry.gameObject);
                        hungry = false;
                    }

                    break;

                case HumanState.IDLE: // Human wanders about when idle.
                    Idling();

                    break;

                case HumanState.BUILDING_HOUSE:   // The human builds a house. secondsSinceLastBuild value is a debug value, change when ready.
                    Debug.Log("Building");

                    if (CheckForSufficientRoom() && !buildingHouse)
                    {
                        buildingHouse = true;

                        Vector3 inFront = humanMesh.position + (humanMesh.transform.forward * 6f);
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.transform.position = inFront;
                        obj.tag = "House Blueprint";
                    }

                    if (gatheredWood < 30)
                    {
                        // Chopping
                        if (!hasWood)
                            MoveToDestination(1);   // Move to the tree first.
                        else
                            MoveToDestination(4);   // Then, if the player has chopped a tree, move to the house that he's building.
                        

                        int layer = 17;
                        int mask = 1 << layer;

                        Collider[] trees = Physics.OverlapSphere(humanMesh.position, 1.5f, mask);
                        foreach (Collider tree in trees)
                        {
                            Destroy(tree.gameObject);   // Then destroy all nearby trees.
                            gatheredWood += 10;
                            hasWood = true;
                        }
                    }
                    else
                    {
                        // Building
                        if (GameManager.Instance.emptyHomes.Count < GameManager.Instance.TeamOneHumans.Count)
                        {
                            if (!CheckForSufficientRoom())
                            {
                                Debug.Log("IK KAN NIETS VINDEN HELP ER IS GEEN PLEK WTF");
                                if (currentDesire != HumanDesire.FOOD)
                                    currentDesire = HumanDesire.HOUSING;

                                Idling();
                            }
                            else
                            {
                                Vector3 inFront = humanMesh.position + (humanMesh.transform.forward * 6f);
                                var obj = Instantiate(house, inFront, Quaternion.identity);

                                GameManager.Instance.emptyHomes.Add(obj);

                                gatheredWood -= 30;
                                secondsSinceLastBuild = 0;
                                buildingHouse = false;
                            }
                        }
                    }

                    break;

                case HumanState.BUILDING_CHURCH:

                    break;

                case HumanState.GATHERING_RESOURCES:
                    // Obsolete State? See chopping.
                    break;

                case HumanState.FIGHTING:
                    break;

                case HumanState.SPREADING_RELIGION:
                    break;
            }
        }
        #endregion

        #region State Machine Night
        if (!isDayTime)
        {
            switch (currentState)
            {
                case HumanState.PRAYING:
                    Debug.Log("Praying");

                    GameManager.Instance.CheckForFood();
                    MoveToDestination(2);

                    if (!desireStated && atShrine)
                        StateDesire();

                    break;

                case HumanState.SLEEPING:
                    Debug.Log("Sleeping");

                    desireStated = false;
                    foreach (UnityEngine.UI.Image cloud in desireClouds)
                        cloud.enabled = false;  // Deactivate all the desire clouds.

                    if ((GameManager.Instance.TeamOneHumans.Contains(gameObject) && GameManager.Instance.TeamOneHomes.Count > 0) ||
                         GameManager.Instance.TeamTwoHumans.Contains(gameObject) && GameManager.Instance.TeamTwoHomes.Count > 0)
                        MoveToDestination(3);
                    else
                        Idling();

                    break;
            }
        }
        #endregion
    }

    void Idling()
    {
        Debug.Log("Idling");
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

    void GaugeFear()    // Add or remove fear based on the specified circumstances.     OBSOLETE. SIMPLIFY BY ADDING OR SUBTRACTING STANDARD AMOUNT TO FEAR VAR.
    {
        var fear = 0f; // Reset the fear gauge to adjust it (otherwise it will just add onto the former amount.
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
            case 0: // ...Nearest food source.
                int foodLayer = 23;
                int foodMask = 1 << foodLayer;

                Collider[] berries = Physics.OverlapSphere(humanMesh.position, 50f, foodMask);
                List<Transform> berryTransforms = new List<Transform>();
                foreach (Collider col in berries)
                {
                    berryTransforms.Add(col.transform);
                }

                rotationReference.LookAt(GetClosestUnit(berryTransforms.ToArray()));
                var berryRot = rotationReference.rotation;
                berryRot.x = 0f;
                berryRot.z = 0f;

                movementParent.rotation = berryRot;

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
                var treeRot = rotationReference.rotation;
                treeRot.x = 0f;
                treeRot.z = 0f;

                movementParent.rotation = treeRot;

                break;


            case 2: // ... the Shrine
                if (GameManager.Instance.TeamOneHumans.Contains(gameObject))
                {
                    rotationReference.LookAt(GameManager.Instance.shrines[0].transform.position);

                    var shrineRot = rotationReference.rotation;
                    shrineRot.x = 0f;
                    shrineRot.z = 0f;

                    movementParent.rotation = shrineRot;
                }

                if (GameManager.Instance.TeamTwoHumans.Contains(gameObject))
                {
                    rotationReference.LookAt(GameManager.Instance.shrines[1].transform.position);

                    var shrineRot = rotationReference.rotation;
                    shrineRot.x = 0f;
                    shrineRot.y = 0f;

                    movementParent.rotation = shrineRot;
                }

                break;

            case 3: // ... nearest house
                if (GameManager.Instance.TeamOneHumans.Contains(gameObject))
                {
                    rotationReference.LookAt(GetClosestUnit(GameManager.Instance.TeamOneHomes.ToArray()));
                    var homeRot = rotationReference.rotation;
                    homeRot.x = 0f;
                    homeRot.z = 0f;

                    movementParent.rotation = homeRot;
                }

                if (GameManager.Instance.TeamTwoHumans.Contains(gameObject))
                {
                    rotationReference.LookAt(GetClosestUnit(GameManager.Instance.TeamTwoHomes.ToArray()));
                    var homeRot = rotationReference.rotation;
                    homeRot.x = 0f;
                    homeRot.z = 0f;

                    movementParent.rotation = homeRot;
                }

                break;

            case 4: // ... Nearest home being built.
                Collider[] homesBeingBuilt = Physics.OverlapSphere(humanMesh.position, 50f);
                List<Transform> builtHomes = new List<Transform>();
                foreach (Collider home in homesBeingBuilt)
                {
                    if (home.CompareTag("House Blueprint"))
                        builtHomes.Add(home.transform);
                }

                rotationReference.LookAt(GetClosestUnit(builtHomes.ToArray()));
                var builtHomeRot = rotationReference.rotation;
                builtHomeRot.x = 0f;
                builtHomeRot.z = 0f;

                movementParent.rotation = builtHomeRot;

                break;
        }
    }

    void StateDesire()
    {
        desireStated = true;

        if (currentDesire != HumanDesire.FOOD && houses.Count < people.Count)
            currentDesire = HumanDesire.HOUSING; Debug.Log("I NEED A HOME!");

        switch (currentDesire)
        {
            case HumanDesire.FOOD:  // This unit is hungry or out of food.
                desireClouds[0].enabled = true;
                break;

            case HumanDesire.HOUSING:   // This unit has no housing or doesn't have enough room to build one.
                desireClouds[1].gameObject.SetActive(true);
                break;

            case HumanDesire.TO_ASCEND:
                Debug.Log("I am ready to ascend, allmighty one.");
                // set ascension cloud active.
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
            readyToAscend = true;
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
        }
    }

    IEnumerator ReduceHappinessOverTime()
    {
        happinessDecreasing = true;

        yield return new WaitForSeconds(30f);
        happiness -= 10;

        happinessDecreasing = false;
    }
}

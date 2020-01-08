using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum HumanState {RECOVER, IDLE, BUILDING_HOUSE, BUILDING_CHURCH, GATHERING_RESOURCES, PRAYING, SLEEPING };
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
    public bool isDepressed = false;
    public GameObject house;
    public GameObject checkHouse;
    public GameObject hologram;
    public bool isDayTime = false;
    public bool isGrounded = false;

    [Space]

    public NetworkPlayers players;
    public RagdollAnimator humanAnimator;
    public Rigidbody hips;
    public Transform humanMesh, movementParent, rotationReference;
    public int fearReductionSpeed;
    public float speed, wanderDuration, turnSpeed;
    public float wanderAlarm, minDistanceFromBuildToCalamity, gatheredWood;
    public bool atShrine, statsTweaked;
    public bool enoughSpaceToBuild = true;

    // Private Variables
    float closestObject = Mathf.Infinity;
    float closestLingeringObject = Mathf.Infinity;
    Vector3 closestObjectPosition = Vector3.zero;
    Vector3 inFront;
    bool wasInvoked = false;
    bool _wasInvoked = false;
    public bool buildingHouse = false;
    public bool desireStated = false;
    public bool hasHome = false;
    bool readyToAscend = false;
    public GameObject _house;

    private void Start()
    {
        foreach (UnityEngine.UI.Image cloud in desireClouds)
            cloud.gameObject.SetActive(false);

        players = GameObject.Find("Network Manager").GetComponent<NetworkPlayers>();
    }

    // Update is called once per frame
    void Update()
    {
        inFront = humanMesh.position + (humanMesh.transform.forward * 6f);
        inFront.y = 33f;

        if (isAscended)
            halo.SetActive(true);
        else
            halo.SetActive(false);

        if (!wasInvoked)
            ReduceFearOverTime();

        if (!_wasInvoked)
            StartCoroutine("IncreaseFaithOverTime");    // Continually gain levels of faith.

        if (!humanAnimator.hasCollapsed && isGrounded)
            AddForce();

        if (readyToAscend)
            currentDesire = HumanDesire.TO_ASCEND;

        if (happiness < 15)
            isDepressed = true;

        #region State Declaration
        if ((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360) && !isDepressed)
        {
            isDayTime = true;
            if (!statsTweaked)
                TweakStats();

            if (humanAnimator.hasCollapsed)
                currentState = HumanState.RECOVER;

            else if (!hasHome)
                currentState = HumanState.BUILDING_HOUSE;

            else
                currentState = HumanState.IDLE;
        }

        if (Sun.Instance.rotation >= 90 && Sun.Instance.rotation <= 180 && !isDepressed)
        {
            isDayTime = false;
            currentState = HumanState.PRAYING;
        }
        else if (Sun.Instance.rotation > 180 && Sun.Instance.rotation <= 270 && !isDepressed)
        {
            isDayTime = false;
            currentState = HumanState.SLEEPING;
        }

        // Depressed states (Human becomes depressed if the happiness value drops below 15).
        if (humanAnimator.hasCollapsed && isDepressed)
            currentState = HumanState.RECOVER;

        else if (Sun.Instance.rotation >= 90 && Sun.Instance.rotation <= 180 && isDepressed)
            currentState = HumanState.PRAYING;

        else if (isDepressed)
            currentState = HumanState.IDLE;
            
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

                case HumanState.IDLE: // Human wanders about when idle.
                    Idling();

                    break;

                case HumanState.BUILDING_HOUSE:   // The human builds a house. secondsSinceLastBuild value is a debug value, change when ready.
                    int layer = 17;
                    int mask = 1 << layer;

                    Collider[] trees = Physics.OverlapSphere(humanMesh.position, 1.5f, mask);

                    if (!buildingHouse && isGrounded)
                    {
                        var newPos = humanMesh.position + (humanMesh.transform.forward * 6f);  // Instantiate the collision checker before checking if there is enough space.
                        newPos.y = 36.5f;
                        checkHouse.transform.position = newPos;
                        checkHouse.SetActive(true);

                        if (enoughSpaceToBuild)
                        {
                            buildingHouse = true;

                            newPos.y = 32.34402f;

                            GameObject obj = Instantiate(house, newPos, Quaternion.Euler(0, Random.Range(0, 359), 0));
                            //obj.transform.parent = transform;
                            obj.GetComponent<House>().humanBuilt = true;
                            obj.layer = 28;
                            _house = obj;   // Set this object as the house variable.
                            
                            checkHouse.SetActive(false);
                        }
                        else
                        {
                            Idling();
                        }
                    }


                    if (gatheredWood < 30 && buildingHouse)
                    {
                        // Chopping
                        if (!hasWood)
                        {
                            MoveToDestination(1);   // Move to the tree first.

                            foreach (Collider tree in trees)
                            {
                                tree.GetComponentInParent<Tree>().state = Tree.States.CHOPPED;   // Then destroy all nearby trees.
                                gatheredWood += 10;
                                hasWood = true;
                            }
                        }
                        else
                        {
                            rotationReference.LookAt(_house.transform.position);
                            var _houseRot = rotationReference.rotation;
                            _houseRot.x = 0f;
                            _houseRot.z = 0f;

                            movementParent.rotation = _houseRot;  // Then, if the player has chopped a tree, move (look at) to the house that he's building.

                            if (Vector3.Distance(humanMesh.position, _house.transform.position) <= 2f)
                            {
                                hasWood = false;
                                // Advance the building to the next stage.
                            }
                        }
                    }
                    else if (gatheredWood >= 30 && buildingHouse)
                    {
                        MoveToDestination(4);

                        if (Vector3.Distance(humanMesh.position, _house.transform.position) <= 2f)
                        {
                            _house.GetComponent<House>().constructionFinished = true;  // Spawn house with random rotation.
                            humanMesh.position = _house.GetComponentInParent<House>().doorPosition.position;  // Set human to entrance position.

                            // Reset the human's speed.
                            for (int i = 0; i < humanAnimator.bones.Length; i++)    
                                humanAnimator.bones[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                            
                            // Assign standard data & reset bools.
                            _house.layer = 16;
                            gatheredWood -= 30;
                            buildingHouse = false;
                            hasWood = false;
                            hasHome = true;

                            if (currentDesire == HumanDesire.HOUSING)
                                currentDesire = HumanDesire.NOTHING;
                        }
                    }

                    break;

                case HumanState.BUILDING_CHURCH:
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
                    MoveToDestination(2);

                    if (!desireStated && atShrine)
                        StateDesire();

                    break;

                case HumanState.SLEEPING:
                    Debug.Log("Sleeping");

                    desireStated = false;
                    statsTweaked = false;
                    foreach (UnityEngine.UI.Image cloud in desireClouds)
                        cloud.gameObject.SetActive(false);  // Deactivate all the desire clouds.

                    if (_house != null)
                        MoveToDestination(3);
                    else
                        Idling();

                    break;
            }
        }
        #endregion
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
                    shrineRot.z = 0f;

                    movementParent.rotation = shrineRot;
                }

                break;

            case 3: // ... nearest house
                rotationReference.LookAt(_house.transform.position);

                var homeRot = rotationReference.rotation;
                homeRot.x = 0f;
                homeRot.z = 0f;

                movementParent.rotation = homeRot;
                break;

            case 4: // ... Nearest home being built.
                rotationReference.LookAt(_house.transform.position);

                var builtHomeRot = rotationReference.rotation;
                builtHomeRot.x = 0f;
                builtHomeRot.z = 0f;

                movementParent.rotation = builtHomeRot;
                break;
        }
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

    void StateDesire()
    {
        desireStated = true;

        GameManager.Instance.CheckForFood();

        if (!hasHome)
            currentDesire = HumanDesire.HOUSING;

        switch (currentDesire)
        {
            case HumanDesire.FOOD:  // This unit is hungry or out of food.
                desireClouds[0].gameObject.SetActive(true);
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

    void ReduceFearOverTime()
    {
        if (GameManager.Instance.fearObjects.Count <= 0)
        {
            fear = Mathf.Lerp(fear, 0, Time.deltaTime * fearReductionSpeed);
        }
    }

    void TweakStats()
    {
        if (currentDesire == HumanDesire.HOUSING)
            happiness -= 20;
        else if (currentDesire == HumanDesire.FOOD)
            happiness -= 40;
        else if (currentDesire == HumanDesire.TO_ASCEND)
            happiness -= 10;
        else if (currentDesire == HumanDesire.NOTHING)
            players.localPlayer.GetComponent<PlayerInventory>().paint = Mathf.Clamp(players.localPlayer.GetComponent<PlayerInventory>().paint + faith, 0, 100);

        statsTweaked = true;
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
}

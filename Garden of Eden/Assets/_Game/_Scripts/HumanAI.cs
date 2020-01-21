using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;

public enum HumanState {RECOVER, IDLE, BUILDING_HOUSE, BUILDING_MONUMENT, GATHERING_RESOURCES, PRAYING, SLEEPING };
public enum HumanDesire {HOUSING, FOOD, TO_ASCEND, NOTHING}

public class HumanAI : NetworkBehaviour
{
    [SerializeField]
    bool hasFaith;

    [Header("Current AI State:")]
    public HumanState currentState;

    [Header("This human currently desires:")]
    public HumanDesire currentDesire = HumanDesire.NOTHING;
    public GameObject[] desireClouds;

    [Space]

    public Transform humanMesh;
    public Transform movementParent, rotationReference;

    public RagdollAnimator humanAnimator;

    [HideInInspector]
    public bool shrineSwitched, atShrine;

    public float speed, wanderDuration, turnSpeed;
    public bool enoughSpaceToBuild = true, switchShrine;

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
    [SyncVar] public bool isStork = false;
    [SyncVar] public NetworkInstanceId storkID;

    [Space]

    int fearReductionSpeed, timesSwitched;
    float wanderAlarm, gatheredWood;
    bool statsTweaked, convertedToNeutral;

    // Private Variables
    Vector3 inFront;
    bool wasInvoked = false;
    bool _wasInvoked = false;
    public bool buildingHouse = false;
    public bool desireStated = false;
    public bool hasHome = false;
    bool readyToAscend = false;
    public GameObject _house;

    public virtual void Start()
    {
        foreach (GameObject cloud in desireClouds)
            cloud.SetActive(false);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isStork)
        {
            var humanHips = transform.Find("mixamorig:Hips").GetComponent<Rigidbody>();
            humanHips.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            GetComponent<RagdollAnimator>().impactRecovery = 0f;
        }
        else
        {
            var humanHips = transform.Find("mixamorig:Hips").GetComponent<Rigidbody>();
            humanHips.constraints = RigidbodyConstraints.None;
            GetComponent<RagdollAnimator>().impactRecovery = 1f;

            transform.parent = null;
        }
        
        inFront = humanMesh.position + (humanMesh.transform.forward * 6f);
        inFront.y = 33f;

        if (isAscended)
            halo.SetActive(true);
        else
            halo.SetActive(false);

        if (!_wasInvoked)
            StartCoroutine("IncreaseFaithOverTime");    // Continually gain levels of faith.

        if (!humanAnimator.hasCollapsed && isGrounded)
            AddForce();

        if (readyToAscend)
            currentDesire = HumanDesire.TO_ASCEND;

        if (happiness < 15)
            isDepressed = true;

        if (humanAnimator.hasCollapsed)
            currentState = HumanState.RECOVER;

        if (currentState == HumanState.IDLE) // Human wanders about when idle.
            Idling();

        if (currentState == HumanState.PRAYING)
        {
            Debug.Log("Praying");
            MoveToDestination(2);

            if (!desireStated && atShrine)
                StateDesire();
        }

        #region Neutral AI
        if (!hasFaith)
        {
            ConvertToNeutral();
            // The neutral human prays at a different shrine every night. The first human to please it will convert it to their side.
            if ((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360))
            {
                foreach (GameObject cloud in desireClouds)
                    cloud.SetActive(false);

                if (timesSwitched >= 5)
                    gameObject.SetActive(false);

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

                    timesSwitched++;
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
        #endregion  // Idling doesnt work?

        #region Human AI
        else
        {
            #region State Declaration
            if ((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360) && !isDepressed)
            {
                isDayTime = true;
                if (!statsTweaked)
                    TweakStats();

                else if (!hasHome)
                    currentState = HumanState.BUILDING_HOUSE;

                else if (!isDepressed && hasHome)
                    currentState = HumanState.BUILDING_MONUMENT;

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

                                var humanID = GetComponent<NetworkIdentity>().netId;
                                CmdSpawnHouse(newPos, isServer, humanID); // Spawn a house through the server

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
                                    //tree.GetComponentInParent<Tree>().state = Tree.States.CHOPPED;   // Then destroy all nearby trees

                                    var obj = tree.GetComponentInParent<Tree>();
                                    var ID = obj.GetComponent<NetworkIdentity>().netId;
                                    CmdChopTreeServer(ID); // Send the networked variables to the server

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

                                movementParent.rotation = _houseRot;  // Then, if the player has chopped a tree, move (look at) to the house that he's building

                                if (Vector3.Distance(humanMesh.position, _house.transform.position) <= 2f)
                                {
                                    hasWood = false;
                                    // Advance the building to the next stage
                                }
                            }
                        }
                        else if (gatheredWood >= 30 && buildingHouse)
                        {
                            MoveToDestination(4);

                            if (Vector3.Distance(humanMesh.position, _house.transform.position) <= 2f)
                            {
                                var obj = _house.GetComponentInParent<House>();
                                var id = obj.GetComponent<NetworkIdentity>().netId;
                                obj.CmdSendVarServer(id, true);

                                humanMesh.position = _house.GetComponentInParent<House>().doorPosition.position;  // Set human to entrance position

                                // Reset the human's speed
                                for (int i = 0; i < humanAnimator.bones.Length; i++)
                                    humanAnimator.bones[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

                                // Assign standard data & reset bools
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

                    case HumanState.BUILDING_MONUMENT:
                        if (!isDepressed)
                            MoveToDestination(5);
                        break;
                }
            }
            #endregion

            #region "State Machine" Night
            if (!isDayTime)
            {
                if (currentState == HumanState.SLEEPING)
                {
                    Debug.Log("Sleeping");

                    desireStated = false;
                    statsTweaked = false;

                    foreach (GameObject cloud in desireClouds)
                        cloud.SetActive(false);  // Deactivate all the desire clouds.

                    if (_house != null)
                        MoveToDestination(3);
                    else
                        Idling();
                }
            }
            #endregion
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
                if (hasFaith)
                {
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
                }

                else
                {
                    if (switchShrine == false)
                    {
                        rotationReference.LookAt(GameManager.Instance.shrines[0].transform.position);

                        var shrineRot = rotationReference.rotation;
                        shrineRot.x = 0f;
                        shrineRot.z = 0f;

                        movementParent.rotation = shrineRot;
                    }

                    else
                    {
                        rotationReference.LookAt(GameManager.Instance.shrines[1].transform.position);

                        var shrineRot = rotationReference.rotation;
                        shrineRot.x = 0f;
                        shrineRot.z = 0f;

                        movementParent.rotation = shrineRot;
                    }
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

            case 5:
                if (GameManager.Instance.TeamOneHumans.Contains(gameObject))
                {
                    rotationReference.LookAt(GameManager.Instance.monuments[0].transform.position);

                    var monumentRot = rotationReference.rotation;
                    monumentRot.x = 0f;
                    monumentRot.z = 0f;

                    movementParent.rotation = monumentRot;
                }

                if (GameManager.Instance.TeamTwoHumans.Contains(gameObject))
                {
                    rotationReference.LookAt(GameManager.Instance.monuments[1].transform.position);

                    var monumentRot = rotationReference.rotation;
                    monumentRot.x = 0f;
                    monumentRot.z = 0f;

                    movementParent.rotation = monumentRot;
                }
                break;
        }
    }

    public void Idling()
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

    void AddForce()
    {
        var length = humanAnimator.bones.Length;
        for (var i = 0; i < length; i++)
        {
            var bone = humanAnimator.bones[i].GetComponent<Rigidbody>();
            bone.AddForce(humanAnimator.movementParent.forward * speed);
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
                desireClouds[0].SetActive(true);
                break;

            case HumanDesire.HOUSING:   // This unit has no housing or doesn't have enough room to build one.
                desireClouds[1].SetActive(true);
                break;

            case HumanDesire.TO_ASCEND:
                Debug.Log("I am ready to ascend, allmighty one.");
                // set ascension cloud active.
                break;
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
            NetworkPlayers.Instance.localPlayer.GetComponent<PlayerInventory>().paint = Mathf.Clamp(NetworkPlayers.Instance.localPlayer.GetComponent<PlayerInventory>().paint + faith, 0, 100);

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

    void ConvertToNeutral() //Adust so this works for both teams AND neutral conversion.
    {
        if (!convertedToNeutral)
        {
            if (GameManager.Instance.TeamOneHumans.Contains(gameObject))
                shrineSwitched = false;

            if (GameManager.Instance.TeamTwoHumans.Contains(gameObject))
                shrineSwitched = true;
        }

        convertedToNeutral = true;
    }

    public void IncreaseFear(float amount, float modifier) { fear += amount * modifier; }

    #region uNet Commands & RPC calls
    [Command] // Spawn a house through the server
    void CmdSpawnHouse(Vector3 pos, bool isHost, NetworkInstanceId humanID)
    {
        GameObject obj = Instantiate(house, pos, Quaternion.Euler(0, Random.Range(0, 359), 0));
        obj.layer = 28;
        obj.GetComponent<House>().humanBuilt = true;

        NetworkServer.Spawn(obj);

        // Assign the house to the host player
        if (isHost)
        {
            var human = NetworkServer.FindLocalObject(humanID);
            human.GetComponent<HumanAI>()._house = obj;
        }
        else
        {
            var houseID = obj.GetComponent<NetworkIdentity>().netId;
            RpcAssignHouse(humanID, houseID);
        }

        var teamID = GetComponent<RagdollSetup>().teamID;
        if (teamID == 0)
            GameManager.Instance.teamOneHomes.Add(obj);

        if (teamID == 1)
            GameManager.Instance.teamTwoHomes.Add(obj);
    }

    [ClientRpc] // Assign the spawned house to the client player
    void RpcAssignHouse(NetworkInstanceId humanID, NetworkInstanceId houseID)
    {
        var human = ClientScene.FindLocalObject(humanID);
        var newHouse = ClientScene.FindLocalObject(houseID);

        human.GetComponent<HumanAI>()._house = newHouse;
    }

    [Command] // Send the networked variables to the server
    public void CmdChopTreeServer(NetworkInstanceId ID)
    {
        var obj = NetworkServer.FindLocalObject(ID);
        obj.GetComponent<Tree>().state = Tree.States.CHOPPED;

        RpcChopTreeClient(ID);
    }

    [ClientRpc] // Send the networked variables to the client
    void RpcChopTreeClient(NetworkInstanceId ID)
    {
        var obj = ClientScene.FindLocalObject(ID);
        obj.GetComponent<Tree>().state = Tree.States.CHOPPED;
    }
    #endregion
}

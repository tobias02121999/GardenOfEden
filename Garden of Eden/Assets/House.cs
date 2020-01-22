using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class House : NetworkBehaviour
{
    // Initialize the public enums
    public enum States { CONSTRUCTION, FINISHED }

    // Initialize the public variables
    [SyncVar]
    public bool humanBuilt;

    [SyncVar]
    public bool constructionFinished;

    public States state = States.FINISHED;

    public Animator animator;
    public Transform doorPosition;
    public GameObject[] modelStates;
    public MonoBehaviour[] finishedScripts;

    // Initialize the private variables
    bool hasRun, humanConstructed;
    RagdollAnimator human;
    HumanAI AI;
    HouseSetup setup;

    void Start()
    {
        setup = GetComponent<HouseSetup>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sync up the variables
        var localPlayerID = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerSetup>().teamID;

        // Check if the house is under construction
        if (humanBuilt)
        {
            if (!constructionFinished)
            {
                state = States.CONSTRUCTION;

                modelStates[0].SetActive(true);
                modelStates[1].SetActive(false);

                var length = finishedScripts.Length;
                for (var i = 0; i < length; i++)
                    finishedScripts[i].enabled = false;

                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        else
        {
            if (setup.teamID == 0)
            {
                var length = GameManager.Instance.TeamOneHumans.Count;
                for (var i = 0; i < length; i++)
                {
                    var human = GameManager.Instance.TeamOneHumans[i].GetComponent<HumanAI>();
                    if (!human.hasHome && !human.buildingHouse)
                    {
                        human._house = this.gameObject;
                        human.hasHome = true;
                        break;
                    }
                }
            }

            if (setup.teamID == 1)
            {
                var length = GameManager.Instance.TeamTwoHumans.Count;
                for (var i = 0; i < length; i++)
                {
                    var human = GameManager.Instance.TeamTwoHumans[i].GetComponent<HumanAI>();
                    if (!human.hasHome && !human.buildingHouse)
                    {
                        human._house = this.gameObject;
                        human.hasHome = true;
                        break;
                    }
                }
            }

            if (((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360)) && !hasRun)
            {
                if (human != null)
                {
                    human.GetComponent<HumanAI>().humanHips.position = doorPosition.position;
                    human.gameObject.SetActive(true);

                    hasRun = true;
                }
            }
        }

        RunState(); // Run the current house state
    }

    // Run the current house state
    void RunState()
    {
        switch (state)
        {
            case States.CONSTRUCTION:
                CheckIfFinished(); // Check if the house is finished constructing
                break;

            case States.FINISHED:
                break;
        }
    }

    // Check if the house is finished constructing
    void CheckIfFinished()
    {
        if (constructionFinished)
        {
            modelStates[0].SetActive(false);
            modelStates[1].SetActive(true);

            if (humanBuilt)
                animator.Play("HouseBuild");

            var length = finishedScripts.Length;
            for (var i = 0; i < length; i++)
                finishedScripts[i].enabled = true;

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GetComponent<Rigidbody>().isKinematic = false;

            state = States.FINISHED;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        if (Sun.Instance.rotation >= 180 && Sun.Instance.rotation <= 270 && collision.transform.CompareTag("HumanBodypart")) // Check human layer during nighttime.
        {
            var _human = collision.gameObject.GetComponentInParent<HumanAI>();
            if (_human._house == this.gameObject)
            {
                hasRun = false;

                human = collision.gameObject.GetComponentInParent<RagdollAnimator>();
                AI = collision.gameObject.GetComponentInParent<HumanAI>();

                collision.gameObject.GetComponentInParent<RagdollAnimator>().gameObject.SetActive(false);
            }
        }
    }

    [Command] // Send the clients variable values to the server
    public void CmdSendVarServer(NetworkInstanceId ID, bool _constructionFinished)
    {
        var obj = NetworkServer.FindLocalObject(ID);
        var house = obj.GetComponent<House>();

        house.constructionFinished = _constructionFinished;
    }
}

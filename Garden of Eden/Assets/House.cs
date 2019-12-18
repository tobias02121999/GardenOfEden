using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    // Initialize the public enums
    public enum States { CONSTRUCTION, FINISHED }

    // Initialize the public variables
    public bool humanBuilt;
    public bool constructionFinished;
    public Animator animator;
    public Transform doorPosition;
    public States state = States.FINISHED;
    public GameObject[] modelStates;
    public MonoBehaviour[] finishedScripts;

    // Initialize the private variables
    bool hasRun, humanConstructed;
    RagdollAnimator human;
    HumanAI AI;

    void Start()
    {
        if (humanBuilt)
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
        else
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
    }

    // Update is called once per frame
    void Update()
    {
        if (((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360)) && !hasRun)
        {
            if (human != null)
            {
                human.GetComponent<HumanAI>().humanMesh.position = doorPosition.position;
                human.gameObject.SetActive(true);

                hasRun = true;
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
}

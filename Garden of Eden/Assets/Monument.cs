using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Monument : NetworkBehaviour
{
    // Initialize the public variables
    [SyncVar]
    public int teamID;
    public GameObject[] masks;
    public int buildState;
    public int buildTarget;

    // Initialize the private variables
    bool hasRun;
    int buildProgress;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("HumanBodypart"))
        {
            StartCoroutine("BuildTimer");

            var humanAI = collision.gameObject.GetComponentInParent<HumanAI>();

            if (humanAI.currentState == HumanState.BUILDING_MONUMENT)
            {
                hasRun = false;
                collision.gameObject.GetComponentInParent<HumanAI>().speed = 0;
                collision.gameObject.GetComponentInParent<RagdollAnimator>().enabled = false;
                collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("HumanBodypart"))
            StopCoroutine("BuildTimer");
    }

    // Update is called once per frame
    void Update()
    {
        if (Sun.Instance.rotation >= 90 && Sun.Instance.rotation <= 180)
        {
            hasRun = true;
            foreach (GameObject human in GameManager.Instance.TeamOneHumans)
            {
                human.gameObject.GetComponentInParent<HumanAI>().speed = 20;
                human.gameObject.GetComponentInParent<RagdollAnimator>().enabled = true;
            }

            foreach (GameObject human in GameManager.Instance.TeamTwoHumans)
            {
                human.gameObject.GetComponentInParent<HumanAI>().speed = 20;
                human.gameObject.GetComponentInParent<RagdollAnimator>().enabled = true;
            }
        }

        var length = masks.Length;
        for (var i = 0; i < length; i++)
            masks[i].SetActive(i == teamID);

        var chunk = buildProgress / 4;
        if (buildProgress >= chunk && buildProgress < (chunk * 2))
            buildState = 1;

        if (buildProgress >= (chunk * 2) && buildProgress < (chunk * 3))
            buildState = 2;

        if (buildProgress >= buildTarget)
            buildState = 3;

        animator.SetInteger("buildState", buildState);
    }

    IEnumerator BuildTimer()
    {
        yield return new WaitForSeconds(1);
        buildProgress++;
    }
}

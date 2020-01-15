using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour
{
    bool hasRun;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("HumanBodypart"))
        {
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
    }
}

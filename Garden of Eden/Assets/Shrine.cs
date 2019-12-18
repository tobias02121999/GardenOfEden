using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{

    bool hasRun;
    GameObject human;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("HumanBodypart"))
        {
            var humanAI = collision.gameObject.GetComponentInParent<HumanAI>();

            if (humanAI.currentState == HumanState.PRAYING)
            {
                hasRun = false;
                collision.gameObject.GetComponentInParent<HumanAI>().atShrine = true;
                collision.gameObject.GetComponentInParent<HumanAI>().speed = 0;
                collision.gameObject.GetComponentInParent<RagdollAnimator>().enabled = false;
                collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            }
        }
    }

    private void Update()
    {
        if ((Sun.Instance.rotation > 180 && Sun.Instance.rotation <= 270) && !hasRun)
        {
            hasRun = true;
            foreach (GameObject human in GameManager.Instance.TeamOneHumans)
            {
                human.gameObject.GetComponentInParent<HumanAI>().atShrine = false;
                human.gameObject.GetComponentInParent<HumanAI>().speed = 15;
                human.gameObject.GetComponentInParent<RagdollAnimator>().enabled = true;
            }

            foreach (GameObject human in GameManager.Instance.TeamTwoHumans)
            {
                human.gameObject.GetComponentInParent<HumanAI>().atShrine = false;
                human.gameObject.GetComponentInParent<HumanAI>().speed = 15;
                human.gameObject.GetComponentInParent<RagdollAnimator>().enabled = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{

    bool hasRun;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Human"))
        {
            hasRun = false;
            collision.gameObject.GetComponentInParent<HumanAI>().speed = 0;
            collision.gameObject.GetComponentInParent<RagdollAnimator>().enabled = false;
        }
    }

    private void Update()
    {
        if ((Sun.Instance.rotation > 180 && Sun.Instance.rotation <= 270) && !hasRun)
        {
            hasRun = true;
            foreach (GameObject human in GameManager.Instance.TeamOneHumans)
            {
                human.gameObject.GetComponentInParent<HumanAI>().speed = 15;
                human.gameObject.GetComponentInParent<RagdollAnimator>().enabled = true;
            }

            foreach (GameObject human in GameManager.Instance.TeamTwoHumans)
            {
                human.gameObject.GetComponentInParent<HumanAI>().speed = 15;
                human.gameObject.GetComponentInParent<RagdollAnimator>().enabled = true;
            }
        }
    }
}

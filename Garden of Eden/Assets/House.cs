using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    bool hasRun;

    RagdollAnimator human;
    HumanAI AI;

    private void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        if (Sun.Instance.rotation >= 180 && Sun.Instance.rotation <= 270 && collision.transform.CompareTag("Human")) // Check human layer during nighttime.
        {
            if (GameManager.Instance.emptyHomes.Contains(gameObject))
            {
                hasRun = false;
                GameManager.Instance.emptyHomes.Remove(gameObject);

                human = collision.gameObject.GetComponentInParent<RagdollAnimator>();
                AI = collision.gameObject.GetComponentInParent<HumanAI>();

                collision.gameObject.GetComponentInParent<RagdollAnimator>().gameObject.SetActive(false);

                if (GameManager.Instance.emptyHomes.Contains(gameObject))
                    GameManager.Instance.emptyHomes.Remove(gameObject);
            }
        }
    }

    private void Update()
    {
        if (((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360)) && !hasRun)
        {
            Debug.Log("Daytime");
            human.gameObject.SetActive(true);

            GameManager.Instance.emptyHomes.Add(gameObject);
            hasRun = true;
        }
    }
}

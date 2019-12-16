using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    // Initialize the public variables
    public bool humanBuilt;
    public Animator animator;
    public Transform doorPosition;

    // Initialize the private variables
    bool hasRun;
    RagdollAnimator human;
    HumanAI AI;

    void Start()
    {
        if (humanBuilt)
            animator.Play("HouseBuild");
    }

    // Update is called once per frame
    void Update()
    {
        if (((Sun.Instance.rotation < 90 && Sun.Instance.rotation >= 0) || (Sun.Instance.rotation > 270 && Sun.Instance.rotation <= 360)) && !hasRun)
        {
            Debug.Log("Daytime");
            human.gameObject.SetActive(true);

            hasRun = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        if (Sun.Instance.rotation >= 180 && Sun.Instance.rotation <= 270 && collision.transform.CompareTag("HunanBodypart")) // Check human layer during nighttime.
        {
            hasRun = false;

            human = collision.gameObject.GetComponentInParent<RagdollAnimator>();
            AI = collision.gameObject.GetComponentInParent<HumanAI>();

            collision.gameObject.GetComponentInParent<RagdollAnimator>().gameObject.SetActive(false);
        }
    }
}

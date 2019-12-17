using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        transform.GetComponentInParent<HumanAI>().enoughSpaceToBuild = false;
    }

    public void OnTriggerExit(Collider other)
    {
        transform.GetComponentInParent<HumanAI>().enoughSpaceToBuild = true;
    }
}

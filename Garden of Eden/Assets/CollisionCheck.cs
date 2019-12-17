using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        Debug.Log("Not enough space to build!");
        transform.GetComponentInParent<HumanAI>().enoughSpaceToBuild = false;
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Enough space to build here!");
        transform.GetComponentInParent<HumanAI>().enoughSpaceToBuild = true;
    }
}

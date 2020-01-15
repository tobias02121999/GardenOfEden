using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LookAt(); // Rotate to always face the local player
    }

    // Rotate to always face the local player
    void LookAt()
    {
        var head = NetworkPlayers.Instance.localPlayer.GetComponent<PlayerInventory>().headTransform;
        transform.LookAt(head);
    }
}

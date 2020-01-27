using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    // Initialize the public variables
    public int teamID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check for hand
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            if (teamID == 0)
                GameManager.Instance.playerOneReady = true;

            if (teamID == 1)
                GameManager.Instance.playerTwoReady = true;

            Destroy(this.gameObject);
        }
    }
}

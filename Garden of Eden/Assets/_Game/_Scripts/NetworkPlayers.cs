using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayers : MonoBehaviour
{
    // Initialize the singleton
    public static NetworkPlayers Instance { get; private set; }

    // Initialize the public variables
    public GameObject localPlayer, otherPlayer;

    // Manage the singleton instance
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

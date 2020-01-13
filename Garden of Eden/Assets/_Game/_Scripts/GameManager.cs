using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    // Initialize the singleton
    public static GameManager Instance { get; private set; }

    public GameObject[] shrines;
    //public Transform[] homes;
    public List<GameObject> teamOneHomes = new List<GameObject>();
    public List<GameObject> teamTwoHomes = new List<GameObject>();

    public List<GameObject> ballsTeam1 = new List<GameObject>();
    public List<GameObject> ballsTeam2 = new List<GameObject>();

    [Header("Humans")]
    public List<GameObject> TeamOneHumans = new List<GameObject>();
    public List<GameObject> TeamTwoHumans = new List<GameObject>();
    public List<GameObject> NeutralHumans = new List<GameObject>();

    [Header("Farms")]
    public List<Transform> teamOneFarms = new List<Transform>();
    public List<Transform> teamTwoFarms = new List<Transform>();

    public List<GameObject> sleepingHumans = new List<GameObject>();
    public List<GameObject> fearObjects = new List<GameObject>();
    public List<GameObject> lingeringFearObjects = new List<GameObject>();

    [Header("Globals")]
    public float teamOneFoodScore;
    public float teamTwoFoodScore;

    bool hungerDistributed = false;

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
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("FearObject").Length; i++)
        {
            fearObjects.Add(GameObject.FindGameObjectsWithTag("FearObject")[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var humanCount = TeamOneHumans.Count;
        var requiredScore = humanCount;

        float farmScore = 0f;
        var length = teamOneFarms.Count;

        for (var i = 0; i < length; i++)
            farmScore += teamOneFarms[i].GetComponent<Farm>().foodScore;
        
        if (humanCount > 2)
            teamOneFoodScore = Mathf.Clamp(farmScore / requiredScore, 0f, 1f);
        else
            teamOneFoodScore = 1;

        SetAuthority(); // Set the authorities of the balls
    }

    // Check if food is sufficient
    public void CheckForFood()
    {
        if (teamOneFoodScore < 0.5f)
        {
            foreach (GameObject human in TeamOneHumans)
            {
                human.GetComponent<HumanAI>().happiness -= 25;
                human.GetComponent<HumanAI>().currentDesire = HumanDesire.FOOD;
            }
        }

        if (teamTwoFoodScore < 0.5f)
        {
            foreach (GameObject human in TeamTwoHumans)
            {
                human.GetComponent<HumanAI>().happiness -= 25;
                human.GetComponent<HumanAI>().currentDesire = HumanDesire.FOOD;
            }
        }
    }

    // Set the authorities of the balls
    void SetAuthority()
    {
        // Humans
        var length = TeamOneHumans.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = TeamOneHumans[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;

            if (identity.clientAuthorityOwner != null)
                identity.RemoveClientAuthority(connection);

            TeamOneHumans[i].GetComponent<RagdollSetup>().teamID = 0;
        }

        length = TeamTwoHumans.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = TeamTwoHumans[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;
            identity.AssignClientAuthority(connection);

            TeamTwoHumans[i].GetComponent<RagdollSetup>().teamID = 1;
        }

        // Homes
        length = teamOneHomes.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamOneHomes[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;

            if (identity.clientAuthorityOwner != null)
                identity.RemoveClientAuthority(connection);

            teamOneHomes[i].GetComponent<HouseSetup>().teamID = 0;
        }

        length = teamTwoHomes.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamTwoHomes[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;
            identity.AssignClientAuthority(connection);

            teamTwoHomes[i].GetComponent<HouseSetup>().teamID = 1;
        }
    }
}

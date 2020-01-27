using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // Initialize the singleton
    public static GameManager Instance { get; private set; }
    public GameObject plateau;

    [SyncVar]
    public bool playerOneReady;
    public bool playerTwoReady;

    [SyncVar]
    public bool playersReady;

    public GameObject[] shrines;
    public GameObject[] monuments;

    //public Transform[] homes;
    public List<GameObject> teamOneHomes = new List<GameObject>();
    public List<GameObject> teamTwoHomes = new List<GameObject>();

    public List<GameObject> teamOneClouds = new List<GameObject>();
    public List<GameObject> teamTwoClouds = new List<GameObject>();

    public List<GameObject> teamOneStorks = new List<GameObject>();
    public List<GameObject> teamTwoStorks = new List<GameObject>();

    public List<GameObject> ballsTeam1 = new List<GameObject>();
    public List<GameObject> ballsTeam2 = new List<GameObject>();

    [Header("Humans")]
    public List<GameObject> TeamOneHumans = new List<GameObject>();
    public List<GameObject> TeamTwoHumans = new List<GameObject>();
    public List<GameObject> TeamOneNeutralHumans = new List<GameObject>();
    public List<GameObject> TeamTwoNeutralHumans = new List<GameObject>();

    [Header("Farms")]
    public List<GameObject> teamOneFarms = new List<GameObject>();
    public List<GameObject> teamTwoFarms = new List<GameObject>();

    public List<GameObject> sleepingHumans = new List<GameObject>();

    [Header("Globals")]
    [SyncVar] public float teamOneFoodScore;
    [SyncVar] public float teamTwoFoodScore;

    public List<GameObject> teamOneHolograms = new List<GameObject>();
    public List<GameObject> teamTwoHolograms = new List<GameObject>();

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playersReady)
            plateau.SetActive(true);

        if (isServer)
        {
            // Calculate food score team one
            var humanCount = TeamOneHumans.Count;
            var requiredScore = humanCount;

            var farmScore = 0f;
            var length = teamOneFarms.Count;

            for (var i = 0; i < length; i++)
                farmScore += teamOneFarms[i].GetComponent<Farm>().foodScore;

            if (humanCount > 2)
                teamOneFoodScore = Mathf.Clamp(farmScore / requiredScore, 0f, 1f);
            else
                teamOneFoodScore = 1;

            // Calculate food score team two
            humanCount = TeamTwoHumans.Count;
            requiredScore = humanCount;
            var score = 0f;

            farmScore = 0f;
            length = teamTwoFarms.Count;

            for (var i = 0; i < length; i++)
                farmScore += teamTwoFarms[i].GetComponent<Farm>().foodScore;

            if (humanCount > 2)
                score = Mathf.Clamp(farmScore / requiredScore, 0f, 1f);
            else
                score = 1;

            teamTwoFoodScore = score;

            if (playerOneReady && playerTwoReady)
                playersReady = true;
        }
        else
            CmdSyncClientReady(playerTwoReady);

        // Control the object authorities
        if (isServer)
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

        // Farms
        length = teamOneFarms.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamOneFarms[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;

            if (identity.clientAuthorityOwner != null)
                identity.RemoveClientAuthority(connection);

            teamOneFarms[i].GetComponent<HouseSetup>().teamID = 0;
        }

        length = teamTwoFarms.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamTwoFarms[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;
            identity.AssignClientAuthority(connection);

            teamTwoFarms[i].GetComponent<HouseSetup>().teamID = 1;
        }

        // Storks
        length = teamOneStorks.Count;
        for (var i = 0; i < length; i++)
        {
            if (teamOneStorks[i] != null)
            {
                var identity = teamOneStorks[i].GetComponent<NetworkIdentity>();
                var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;

                if (identity.clientAuthorityOwner != null)
                    identity.RemoveClientAuthority(connection);

                teamOneStorks[i].GetComponent<Stork>().teamID = 0;
            }
        }

        length = teamTwoStorks.Count;
        for (var i = 0; i < length; i++)
        {
            if (teamTwoStorks[i] != null)
            {
                var identity = teamTwoStorks[i].GetComponent<NetworkIdentity>();
                var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;
                identity.AssignClientAuthority(connection);

                teamTwoStorks[i].GetComponent<Stork>().teamID = 1;
            }
        }

        // Holograms
        length = teamOneHolograms.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamOneHolograms[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;

            if (identity.clientAuthorityOwner != null)
                identity.RemoveClientAuthority(connection);

            teamOneHolograms[i].GetComponent<HologramSetup>().teamID = 0;
        }

        length = teamTwoHolograms.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamTwoHolograms[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;
            identity.AssignClientAuthority(connection);

            teamTwoHolograms[i].GetComponent<HologramSetup>().teamID = 1;
        }

        // Clouds
        length = teamOneClouds.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamOneClouds[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;

            if (identity.clientAuthorityOwner != null)
                identity.RemoveClientAuthority(connection);

            teamOneClouds[i].GetComponent<CloudSetup>().teamID = 0;
        }

        length = teamTwoClouds.Count;
        for (var i = 0; i < length; i++)
        {
            var identity = teamTwoClouds[i].GetComponent<NetworkIdentity>();
            var connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;
            identity.AssignClientAuthority(connection);

            teamTwoClouds[i].GetComponent<CloudSetup>().teamID = 1;
        }

        // Monuments
        var identity1 = monuments[0].GetComponent<NetworkIdentity>();
        var _connection = NetworkPlayers.Instance.otherPlayer.GetComponent<NetworkIdentity>().connectionToClient;

        if (identity1.clientAuthorityOwner != null)
            identity1.RemoveClientAuthority(_connection);

        monuments[0].GetComponent<Monument>().teamID = 0;

        // -----------------------
        var identity2 = monuments[1].GetComponent<NetworkIdentity>();
        identity2.AssignClientAuthority(_connection);

        monuments[1].GetComponent<Monument>().teamID = 1;
    }

    [ClientRpc]
    public void RpcBounceHuman(NetworkInstanceId storkID, NetworkInstanceId humanID)
    {
        var obj = ClientScene.FindLocalObject(storkID);
        obj.GetComponent<Stork>().human = ClientScene.FindLocalObject(humanID);

        Debug.Log("Client received human bounce");

        if (ClientScene.FindLocalObject(humanID) == null)
            Debug.Log("Couldnt find bounced human");
    }

    [Command]
    void CmdSyncClientReady(bool ready)
    {
        playerTwoReady = ready;
    }
}

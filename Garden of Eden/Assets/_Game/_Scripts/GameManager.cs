using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] shrines;
    //public Transform[] homes;
    public List<Transform> homes = new List<Transform>();

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
    }

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
}

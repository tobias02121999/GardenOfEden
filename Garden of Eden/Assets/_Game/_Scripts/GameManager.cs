using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] shrines;
    public Transform[] homes;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(3))
        {
            Destroy(fearObjects[0]);
            fearObjects.RemoveAt(0);
        }

        if (Input.GetKeyDown("s"))
            ObjectPooler.Instance.SpawnFromPool("Humans", transform.position, Quaternion.identity);

        var humanCount = TeamOneHumans.Count;
        var requiredScore = humanCount;

        float farmScore = 0f;
        var length = teamOneFarms.Count;

        for (var i = 0; i < length; i++)
            farmScore += teamOneFarms[i].GetComponent<Farm>().foodScore;
        
        if (humanCount > 2)
            teamOneFoodScore = farmScore / requiredScore;
        else
            teamOneFoodScore = 1;
    }

    public void CheckForFood()
    {
        GameObject[] food = GameObject.FindGameObjectsWithTag("BerryBush");
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
        if (food.Length < humans.Length)
        {
            if (hungerDistributed == false)
            {
                var hungryHuman = humans[Random.Range(0, humans.Length + 1)];
                hungryHuman.GetComponent<HumanAI>().hungry = true;

                hungerDistributed = true;
            }
        }
        else if (food.Length >= humans.Length)
        {
            foreach (GameObject person in humans)
            {
                person.GetComponent<HumanAI>().hungry = false;
            }
        }
    }
}

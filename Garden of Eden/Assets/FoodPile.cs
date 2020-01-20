using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPile : MonoBehaviour
{
    // Initialize the public variables
    public GameObject[] foodLevels;
    public HouseSetup setup;

    // Initialize the private variables


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var teamID = setup.teamID;
        var score = 0f;

        if (teamID == 0)
            score = GameManager.Instance.teamOneFoodScore;

        if (teamID == 1)
            score = GameManager.Instance.teamTwoFoodScore;

        var maxChunkAmount = foodLevels.Length;
        var chunkAmount = Mathf.RoundToInt((score / 1f) * maxChunkAmount);

        for (var i = 0; i < maxChunkAmount; i++)
            foodLevels[i].SetActive(i < chunkAmount);
    }
}

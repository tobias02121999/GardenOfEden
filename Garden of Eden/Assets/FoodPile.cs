using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPile : MonoBehaviour
{
    // Initialize the public variables
    public GameObject[] foodLevels;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var maxChunkAmount = foodLevels.Length;
        var chunkAmount = Mathf.RoundToInt((GameManager.Instance.teamOneFoodScore / 1f) * maxChunkAmount);

        for (var i = 0; i < maxChunkAmount; i++)
            foodLevels[i].SetActive(i < chunkAmount);
    }
}

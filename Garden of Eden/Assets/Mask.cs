using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    // Initialize the public variables
    public GameObject[] masks;
    public GameObject[] leaves;
    public HumanAI humanAI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var maxChunkAmount = leaves.Length;
        var chunkAmount = Mathf.RoundToInt((humanAI.happiness / 100f) * maxChunkAmount);

        for (var i = 0; i < maxChunkAmount; i++)
            leaves[i].SetActive(i < chunkAmount);

        var level = Mathf.RoundToInt(humanAI.faith / 33f);
        var length = masks.Length;
        for (var j = 0; j < length; j++)
            masks[j].SetActive(j == level - 1);
    }
}

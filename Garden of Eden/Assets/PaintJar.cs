using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintJar : MonoBehaviour
{
    // Initialize the public variables
    public MeshRenderer[] paintChunks;
    public NetworkPlayers players;
    public Material paintMat, paintToSpendMat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var inventory = players.localPlayer.GetComponent<PlayerInventory>();
        var maxChunkAmount = paintChunks.Length;
        var chunkAmount = Mathf.RoundToInt((inventory.paint / inventory.maxPaint) * maxChunkAmount);
        var toSpendChunkAmount = Mathf.RoundToInt((inventory.paintToSpend / inventory.maxPaint) * maxChunkAmount);

        for (var i = 0; i < maxChunkAmount; i++)
        {
            paintChunks[i].enabled = (i < chunkAmount);

            if (i < (chunkAmount - toSpendChunkAmount))
                paintChunks[i].material = paintMat;
            else
                paintChunks[i].material = paintToSpendMat;
        }
    }
}

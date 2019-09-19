using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public GameObject cube;

    public List<GameObject> fearObjects = new List<GameObject>();
    public List<GameObject> lingeringFearObjects = new List<GameObject>();

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
        if (Input.GetMouseButtonDown(0))
        {
            var go = Instantiate(cube, new Vector3(3, 0, 0), Quaternion.identity);
            fearObjects.Add(go);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(fearObjects[0]);
            fearObjects.RemoveAt(0);
        }
    }
}

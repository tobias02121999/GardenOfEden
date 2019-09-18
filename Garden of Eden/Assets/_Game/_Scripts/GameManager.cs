using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
    }
    #endregion

    public GameObject cube;

    public static List<GameObject> fearObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var go = Instantiate(cube, new Vector3(3, 0, 0), Quaternion.identity);
            fearObjects.Add(go);
        }
    }
}

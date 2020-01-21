using UnityEngine;

public class NeutralAI : HumanAI
{
    private int timesSwitched;
    private bool shrineSwitched;

    public override void Start()
    {
        foreach (GameObject cloud in desireClouds)
            cloud.SetActive(false);

    }

    // Update is called once per frame
    public override void Update()
    {

    }
}

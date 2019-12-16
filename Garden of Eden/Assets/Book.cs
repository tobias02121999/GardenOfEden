using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    //public List
    public List<Material> Pages = new List<Material>();

    public Animator anim;
    public bool leftCollider;
    public bool rightCollider;

    private bool doOnce;
    private float fadeInDuration;
    private float fadeOutDelay;
    private int pageNumber;
    private int startPage;
    private int maxPages;

    private MeshRenderer render;
    private Material material;
    private Color32 col;


    // Start is called before the first frame update
    void Start()
    {
        fadeInDuration = .5f;
        fadeOutDelay = .15f;
        for (int i = 0; i < Pages.Count; i++)
        {
            col = Pages[i].GetColor("_Color");
            col.a = 0;
            Pages[i].SetColor("_Color", col);
        }        
        startPage = 0;
        pageNumber = startPage;
        maxPages = Pages.Count;
        col = Pages[startPage].GetColor("_Color");
        col.a = 255;
        Pages[startPage].SetColor("_Color", col);
        
        doOnce = true;
        leftCollider = false;
        rightCollider = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (rightCollider && doOnce && pageNumber < (maxPages-1))
        {
            rightCollider = false;
            doOnce = false;
            StartCoroutine(SwitchPageCycle(true));
        }
        else if(leftCollider && doOnce && pageNumber > 0)
        {
            leftCollider = false;
            doOnce = false;
            StartCoroutine(SwitchPageCycle(false));          
        }
    }

    IEnumerator SwitchPageCycle(bool forward)
    {
        if (forward)
        {
            anim.Play("NextPage_Anim");
            StartCoroutine(SwitchPage(1));
        }
        else
        {
            anim.Play("PreviousPage_Anim");
            StartCoroutine(SwitchPage(-1));
        }
        yield return new WaitForSeconds(fadeInDuration);
    }

    private IEnumerator SwitchPage(int forward)
    {
        col = Pages[pageNumber].GetColor("_Color");
        col.a = 0;
        yield return new WaitForSeconds(fadeOutDelay);
        Pages[pageNumber].SetColor("_Color", col);
        pageNumber = pageNumber + forward;
        col = Pages[pageNumber].GetColor("_Color");
        col.a = 255;
        yield return new WaitForSeconds(fadeInDuration);
        Pages[pageNumber].SetColor("_Color", col);
        doOnce = true;
    }
}

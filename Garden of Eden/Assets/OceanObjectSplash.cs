using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanObjectSplash : MonoBehaviour
{
    // Initialize the public variables
    public ParticleSystem[] waterSplashEffect;
    public Transform waterSplashTransform;
    public string[] tagsToCheck;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        var length = tagsToCheck.Length;
        for (var i = 0; i < length; i++)
        {
            var tag = tagsToCheck[i];
            if (other.CompareTag(tag))
            {
                var comp = other.GetComponentInParent<Floatable>();
                if (comp != null)
                {
                    if (comp.isFloating)
                        break;
                }

                var pos = other.transform.position;
                waterSplashTransform.position = pos;

                var _length = waterSplashEffect.Length;
                for (var j = 0; i < _length; i++)
                    waterSplashEffect[i].Play();

                break;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    // Initialize the public variables
    public float dayNightSpeed, densityDay, densityNight, ambientDay, ambientNight;
    public Color waterColorDay, waterColorNight;
    public Color auraColorDay, auraColorNight;
    public Color mistColorDay, mistColorNight;
    public Color treeColorDay, treeColorNight;
    public Color grassColorDay, grassColorNight;
    public Color plateauColorDay, plateauColorNight;
    public Color sandColorDay, sandColorNight;
    public Material waterMaterial, skyMaterial, treeMaterial, grassMaterial, plateauMaterial, sandMaterial;
    public ParticleSystem mist;
    public AuraAPI.Aura aura;

    // Initialize the private variables
    float rotation;
    ParticleSystem.Particle[] grassParticles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DoDayNightCycle(); // Rotate the sun around the world pivot
    }

    // Rotate the sun around the world pivot
    void DoDayNightCycle()
    {
        transform.Rotate(dayNightSpeed, 0f, 0f);
        rotation += dayNightSpeed;

        if (rotation >= 360f)
            rotation = 0f;

        Color waterColor, auraColor, mistColor, treeColor, grassColor, plateauColor, sandColor;
        float density, ambient;

        if (rotation <= 180f)
        {
            var index = rotation / 180f;
            waterColor = Color.Lerp(waterColorDay, waterColorNight, index);
            auraColor = Color.Lerp(auraColorDay, auraColorNight, index);
            density = Mathf.Lerp(densityDay, densityNight, index);
            ambient = Mathf.Lerp(ambientDay, ambientNight, index);
            mistColor = Color.Lerp(mistColorDay, mistColorNight, index);
            treeColor = Color.Lerp(treeColorDay, treeColorNight, index);
            grassColor = Color.Lerp(grassColorDay, grassColorNight, index);
            plateauColor = Color.Lerp(plateauColorDay, plateauColorNight, index);
            sandColor = Color.Lerp(sandColorDay, sandColorNight, index);
        }
        else
        {
            var index = (rotation - 180f) / 180f;
            waterColor = Color.Lerp(waterColorNight, waterColorDay, index);
            auraColor = Color.Lerp(auraColorNight, auraColorDay, index);
            density = Mathf.Lerp(densityNight, densityDay, index);
            ambient = Mathf.Lerp(ambientNight, ambientDay, index);
            mistColor = Color.Lerp(mistColorNight, mistColorDay, index);
            treeColor = Color.Lerp(treeColorNight, treeColorDay, index);
            grassColor = Color.Lerp(grassColorNight, grassColorDay, index);
            plateauColor = Color.Lerp(plateauColorNight, plateauColorDay, index);
            sandColor = Color.Lerp(sandColorNight, sandColorDay, index);
        }

        waterMaterial.SetColor("_DepthGradientDeep", waterColor);
        skyMaterial.color = waterColor;
        treeMaterial.color = treeColor;
        grassMaterial.color = grassColor;
        plateauMaterial.color = plateauColor;
        sandMaterial.SetColor("_EmissionColor", sandColor);

        aura.frustum.settings.color = auraColor;
        aura.frustum.settings.density = density;
        aura.frustum.settings.colorStrength = ambient;

        var _mist = mist.main;
        _mist.startColor = mistColor;
    }
}

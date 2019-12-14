using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : Singleton<Sun>
{
    // Initialize the public variables
    public float dayNightSpeed, densityDay, densityNight, ambientDay, ambientNight;
    public Color waterColorDay, waterColorNight;
    public Color auraColorDay, auraColorNight;
    public Color mistColorDay, mistColorNight;
    public Color birchColorDay, birchColorNight;
    public Color oakColorDay, oakColorNight;
    public Color grassColorDay, grassColorNight;
    public Color plateauColorDay, plateauColorNight;
    public Color rockColorDay, rockColorNight;
    public Color sandColorDay, sandColorNight;
    public Material waterMaterial, skyMaterial, birchMaterial, oakMaterial, grassMaterial, plateauMaterial, rockMaterial, sandMaterial;
    public ParticleSystem mist;
    //public AuraAPI.Aura aura;
    public Light directionalLight;
    public float lightIntensityDay, lightIntensityNight;

    // Initialize the private variables
    public float rotation;
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

        Color waterColor, auraColor, mistColor, birchColor, oakColor, grassColor, plateauColor, rockColor, sandColor;
        float density, ambient, lightIntensity;

        if (rotation <= 180f)
        {
            var index = rotation / 180f;
            waterColor = Color.Lerp(waterColorDay, waterColorNight, index);
            auraColor = Color.Lerp(auraColorDay, auraColorNight, index);
            density = Mathf.Lerp(densityDay, densityNight, index);
            ambient = Mathf.Lerp(ambientDay, ambientNight, index);
            mistColor = Color.Lerp(mistColorDay, mistColorNight, index);
            birchColor = Color.Lerp(birchColorDay, birchColorNight, index);
            oakColor = Color.Lerp(oakColorDay, oakColorNight, index);
            grassColor = Color.Lerp(grassColorDay, grassColorNight, index);
            plateauColor = Color.Lerp(plateauColorDay, plateauColorNight, index);
            rockColor = Color.Lerp(rockColorDay, rockColorNight, index);
            sandColor = Color.Lerp(sandColorDay, sandColorNight, index);
            lightIntensity = Mathf.Lerp(lightIntensityDay, lightIntensityNight, index);
        }
        else
        {
            var index = (rotation - 180f) / 180f;
            waterColor = Color.Lerp(waterColorNight, waterColorDay, index);
            auraColor = Color.Lerp(auraColorNight, auraColorDay, index);
            density = Mathf.Lerp(densityNight, densityDay, index);
            ambient = Mathf.Lerp(ambientNight, ambientDay, index);
            mistColor = Color.Lerp(mistColorNight, mistColorDay, index);
            birchColor = Color.Lerp(birchColorNight, birchColorDay, index);
            oakColor = Color.Lerp(oakColorNight, oakColorDay, index);
            grassColor = Color.Lerp(grassColorNight, grassColorDay, index);
            plateauColor = Color.Lerp(plateauColorNight, plateauColorDay, index);
            rockColor = Color.Lerp(rockColorNight, rockColorDay, index);
            sandColor = Color.Lerp(sandColorNight, sandColorDay, index);
            lightIntensity = Mathf.Lerp(lightIntensityNight, lightIntensityDay, index);
        }

        waterMaterial.SetColor("_DepthGradientDeep", waterColor);
        skyMaterial.color = waterColor;
        grassMaterial.color = grassColor;
        plateauMaterial.color = plateauColor;
        rockMaterial.color = rockColor;
        sandMaterial.SetColor("_EmissionColor", sandColor);
        birchMaterial.SetColor("_EmissionColor", birchColor);
        oakMaterial.SetColor("_EmissionColor", oakColor);

        /*
        aura.frustum.settings.color = auraColor;
        aura.frustum.settings.density = density;
        aura.frustum.settings.colorStrength = ambient;
        */

        directionalLight.intensity = lightIntensity;

        var _mist = mist.main;
        _mist.startColor = mistColor;
    }
}

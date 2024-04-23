using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    public Slider lucidityBar;

    public bool inNightmare = false;
    public bool inNeutral   = true;
    public bool inLucid     = false;

    public Color nightmareColor;
    public Color lucidColor;

    public PlayerController player;
    public float clearHUD = 50f;

    public ParticleSystem fogEffects;

    private void TrackState()
    {
        // we can replace Report() with whatever functions change the environment

        // going from neutral to nightmare
        if (inNeutral && lucidityBar.value < 25)
        {
            inNeutral = false;
            inNightmare = true;
            player.speedScalar = 2.5f;

            fogEffects.startColor = nightmareColor;

            //Report();
        }

        // going from nightmare to neutral
        else if (inNightmare && lucidityBar.value >= 25 && lucidityBar.value < 75)
        {
            inNightmare = false;
            inNeutral = true;
            player.speedScalar = 5f;

            fogEffects.startColor = Color.white;

            //Report();
        }

        // going from neutral to lucid
        else if (inNeutral && lucidityBar.value >= 75)
        {
            inNeutral = false;
            inLucid = true;
            player.speedScalar = 6.5f;

            fogEffects.startColor = lucidColor;

            //Report();
        }

        // going from lucid to neutral
        else if (inLucid && lucidityBar.value < 75 && lucidityBar.value > 25)
        {
            inLucid = false;
            inNeutral = true;
            player.speedScalar = 5f;

            fogEffects.startColor = Color.white;

            //Report();
        }

        // going from nightmare to lucid (completely fill bar, for example)
        else if (inNightmare && lucidityBar.value >= 75)
        {
            inNightmare = false;
            inLucid = true;
            player.speedScalar = 6.5f;

            fogEffects.startColor = lucidColor;

            //Report();
        }

        // going from lucid to nightmare (large drop from injury, for example)
        else if (inLucid && lucidityBar.value < 25)
        {
            inLucid = false;
            inNightmare = true;
            player.speedScalar = 2.5f;

            fogEffects.startColor = nightmareColor;

            //Report();
        }
    }
    
    public int Report()
    {

        if (inNightmare) {
            //Debug.Log("In nightmare mode");
            return 1;
        }

        if (inNeutral) { 
            //Debug.Log("In neutral mode");
            return 2;
        }

        if (inLucid) {
            //Debug.Log("In lucid mode");
            return 3;
        }

        return 0;
    }

    private void FixedUpdate()
    {
        TrackState();
    }
}

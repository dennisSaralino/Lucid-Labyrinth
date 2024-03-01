using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    public Slider lucidityBar;

    private bool inNightmare = false;
    private bool inNeutral   = true;
    private bool inLucid     = false;

    private bool screenNightmare  = false;
    private bool screenLucid      = false;
    private bool nightmareToLucid = false;
    private bool lucidToNightmare = false;
    public Color nightmareColor;
    public Color lucidColor;

    public PlayerController controlSpeed;
    public Image lucidHUD;
    public float clearHUD = 50f;

    private void TrackState()
    {
        // we can replace Report() with whatever functions change the environment

        // going from neutral to nightmare
        if (inNeutral && lucidityBar.value < 25)
        {
            inNeutral = false;
            inNightmare = true;
            screenNightmare = true;
            //lucidHUD.color = nightmareColor;
            //controlSpeed.speedScalar = 2.5f;

            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.1f;
            RenderSettings.fogColor = nightmareColor;

            Report();
        }

        // going from nightmare to neutral
        else if (inNightmare && lucidityBar.value >= 25 && lucidityBar.value < 75)
        {
            inNightmare = false;
            inNeutral = true;
            screenNightmare = false;
            //controlSpeed.speedScalar = 5f;

            RenderSettings.fog = false;

            Report();
        }

        // going from neutral to lucid
        else if (inNeutral && lucidityBar.value >= 75)
        {
            inNeutral = false;
            inLucid = true;
            screenLucid = true;
            //lucidHUD.color = lucidColor;
            //controlSpeed.speedScalar = 7.5f;

            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.025f;
            RenderSettings.fogColor = lucidColor;

            Report();
        }

        // going from lucid to neutral
        else if (inLucid && lucidityBar.value < 75 && lucidityBar.value > 25)
        {
            inLucid = false;
            inNeutral = true;
            screenLucid = false;
            //controlSpeed.speedScalar = 5f;

            RenderSettings.fog = false;

            Report();
        }

        // going from nightmare to lucid (completely fill bar, for example)
        else if (inNightmare && lucidityBar.value >= 75)
        {
            inNightmare = false;
            inLucid = true;
            nightmareToLucid = true;
            //controlSpeed.speedScalar = 7.5f;

            Report();
        }

        // going from lucid to nightmare (large drop from injury, for example)
        else if (inLucid && lucidityBar.value < 25)
        {
            inLucid = false;
            inNightmare = true;
            lucidToNightmare = true;
            //controlSpeed.speedScalar = 2.5f;
            Report();
        }
    }

    private void Report()
    {
        
        if (inNightmare)
            Debug.Log("In nightmare mode");

        if (inNeutral)
            Debug.Log("In neutral mode");

        if (inLucid)
            Debug.Log("In lucid mode");
        
    }

    private void FixedUpdate()
    {
        TrackState();
        if (screenNightmare == false && screenLucid == false)
        {
            RenderSettings.fogColor = Color.Lerp(lucidHUD.color, Color.clear, clearHUD * Time.deltaTime);
            //lucidHUD.color = Color.Lerp(lucidHUD.color, Color.clear, clearHUD * Time.deltaTime);
        }
        else if (nightmareToLucid == true)
        {
            RenderSettings.fogColor = Color.Lerp(lucidHUD.color, lucidColor, clearHUD * Time.deltaTime);
            //lucidHUD.color = Color.Lerp(lucidHUD.color, lucidColor, clearHUD * Time.deltaTime);
        }
        else if (lucidToNightmare == true)
        {
            RenderSettings.fogColor = Color.Lerp(lucidHUD.color, nightmareColor, clearHUD * Time.deltaTime);
            //lucidHUD.color = Color.Lerp(lucidHUD.color, nightmareColor, clearHUD * Time.deltaTime);
        }
    }
}
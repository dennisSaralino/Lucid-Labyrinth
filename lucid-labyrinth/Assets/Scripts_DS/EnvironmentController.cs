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

    public BadController_PW controlSpeed;
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
            lucidHUD.color = nightmareColor;
            controlSpeed.speed = 2.5f;
            Report();
        }

        // going from nightmare to neutral
        else if (inNightmare && lucidityBar.value >= 25 && lucidityBar.value < 75)
        {
            inNightmare = false;
            inNeutral = true;
            screenNightmare = false;
            controlSpeed.speed = 5f;
            Report();
        }

        // going from neutral to lucid
        else if (inNeutral && lucidityBar.value >= 75)
        {
            inNeutral = false;
            inLucid = true;
            screenLucid = true;
            lucidHUD.color = lucidColor;
            controlSpeed.speed = 10f;
            Report();
        }

        // going from lucid to neutral
        else if (inLucid && lucidityBar.value < 75 && lucidityBar.value > 25)
        {
            inLucid = false;
            inNeutral = true;
            screenLucid = false;
            controlSpeed.speed = 5;
            Report();
        }

        // going from nightmare to lucid (completely fill bar, for example)
        else if (inNightmare && lucidityBar.value >= 75)
        {
            inNightmare = false;
            inLucid = true;
            nightmareToLucid = true;
            controlSpeed.speed = 10;
            Report();
        }

        // going from lucid to nightmare (large drop from injury, for example)
        else if (inLucid && lucidityBar.value < 25)
        {
            inLucid = false;
            inNightmare = true;
            lucidToNightmare = true;
            controlSpeed.speed = 2.5f;
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
            lucidHUD.color = Color.Lerp(lucidHUD.color, Color.clear, clearHUD * Time.deltaTime);
        }
        else if (nightmareToLucid == true)
        {
            lucidHUD.color = Color.Lerp(lucidHUD.color, lucidColor, clearHUD * Time.deltaTime);
        } else if (lucidToNightmare == true)
        {
            lucidHUD.color = Color.Lerp(lucidHUD.color, nightmareColor, clearHUD * Time.deltaTime);
        }
    }
}

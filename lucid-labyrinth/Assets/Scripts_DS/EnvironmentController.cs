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

    private void TrackState()
    {
        // we can replace Report() with whatever functions change the environment

        // going from neutral to nightmare
        if (inNeutral && lucidityBar.value < 25)
        {
            inNeutral = false;
            inNightmare = true;
            Report();
        }

        // going from nightmare to neutral
        else if (inNightmare && lucidityBar.value >= 25 && lucidityBar.value < 75)
        {
            inNightmare = false;
            inNeutral = true;
            Report();
        }

        // going from neutral to lucid
        else if (inNeutral && lucidityBar.value >= 75)
        {
            inNeutral = false;
            inLucid = true;
            Report();
        }

        // going from lucid to neutral
        else if (inLucid && lucidityBar.value < 75 && lucidityBar.value > 25)
        {
            inLucid = false;
            inNeutral = true;
            Report();
        }

        // going from nightmare to lucid (completely fill bar, for example)
        else if (inNightmare && lucidityBar.value >= 75)
        {
            inNightmare = false;
            inLucid = true;
            Report();
        }

        // going from lucid to nightmare (large drop from injury, for example)
        else if (inLucid && lucidityBar.value < 25)
        {
            inLucid = false;
            inNightmare = true;
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
    }
}

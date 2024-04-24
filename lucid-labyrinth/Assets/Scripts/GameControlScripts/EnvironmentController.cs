using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    public Slider lucidityBar;

    public bool inNightmare = false;
    public bool inLucid     = false;
    public bool inNeutral   = true;

    public bool nightmareLevel1 = false;
    public bool nightmareLevel2 = false;
    public bool nightmarelevel3 = false;
    public bool lucidLevel1 = false;
    public bool lucidLevel2 = false;
    public bool lucidLevel3 = false;

    public Color nightmareColor;
    public Color lucidColor;

    public PlayerController player;
    public ParticleSystem fogEffects;

    private void TrackState()
    {
        // Going from neutral to Nightmare Level 1
        if (inNeutral && lucidityBar.value < 45)
        {
            inNeutral = false;
            inNightmare = true;
            nightmareLevel1 = true;
            player.speedScalar = 3.75f;
        }

        // Going from Nightmare Level 1 to Nightmare Level 2
        else if (nightmareLevel1 && lucidityBar.value < 20)
        {
            nightmareLevel1 = false;
            nightmareLevel2 = true;
            player.speedScalar = 2.5f;

            fogEffects.startColor = nightmareColor;
        }

        // Going from Nightmare Level 2 to Nightmare Level 3
        else if (nightmareLevel2 && lucidityBar.value < 5)
        {
            nightmareLevel2 = false;
            nightmarelevel3 = true;
            player.speedScalar = 0f;
        }

        // Going from Nightmare Level 3 to Nightmare Level 2
        else if (nightmarelevel3 && lucidityBar.value > 5)
        {
            nightmarelevel3 = false;
            nightmareLevel2 = true;
            player.speedScalar = 2.5f;
        }

        // Going from Nightmare Level 2 to Nightmare Level 1
        else if (nightmareLevel2 && lucidityBar.value > 20)
        {
            nightmareLevel2 = false;
            nightmareLevel1 = true;
            player.speedScalar = 3.75f;

            fogEffects.startColor = Color.white;
        }

        // Going from Nightmare Level 1 to Neutral
        else if (nightmareLevel1 && lucidityBar.value > 45)
        {
            nightmareLevel1 = false;
            inNightmare = false;
            inNeutral = true;
            player.speedScalar = 5f;
        }

        // Going from Neutral to Lucid Level 1
        else if (inNeutral && lucidityBar.value > 55)
        {
            inNeutral = false;
            inLucid = true;
            lucidLevel1 = true;
            player.speedScalar = 5.5f;
        }

        // Going from Lucid Level 1 to Lucid Level 2
        else if (lucidLevel1 && lucidityBar.value > 70)
        {
            lucidLevel1 = false;
            lucidLevel2 = true;
            player.speedScalar = 6f;

            fogEffects.startColor = lucidColor;
        }

        // Going from Lucid Level 2 to Lucid Level 3
        else if (lucidLevel2 && lucidityBar.value > 95)
        {
            lucidLevel2 = false;
            lucidLevel3 = true;
            player.speedScalar = 6.5f;
        }

        // Going from Lucid Level 3 to Lucid Level 2
        else if (lucidLevel3 && lucidityBar.value < 95)
        {
            lucidLevel3 = false;
            lucidLevel2 = true;
            player.speedScalar = 6f;
        }

        // Going from Lucid Level 2 to Lucid Level 1
        else if (lucidLevel2 && lucidityBar.value < 70)
        {
            lucidLevel2 = false;
            lucidLevel1 = true;
            player.speedScalar = 5.5f;

            fogEffects.startColor = Color.white;
        }

        // Going from Lucid Level 1 to Neutral
        else if (lucidLevel1 && lucidityBar.value < 55)
        {
            lucidLevel1 = false;
            inLucid = false;
            inNeutral = true;
            player.speedScalar = 5f;
        }

        // Going from Nightmare to Lucid Level 1
        else if (inNightmare && lucidityBar.value > 45)
        {
            inNightmare = false;
            inLucid = true;
            lucidLevel1 = true;

            fogEffects.startColor = lucidColor;
        }

        // Going from Lucid to Nightmare Level 1
        else if (inLucid && lucidityBar.value > 55)
        {
            inLucid = false;
            inNightmare = true;
            nightmareLevel1 = true;

            fogEffects.startColor = nightmareColor;
        }
    }

    public int Report()
    {
        if (inLucid)
        {
            return 3;
        }
        else if (inNightmare)
        {
            return 1;
        }
        else { return 2; }
    }

    //Levels of Lucidity:
    //Lucid Level 3 (95-100), Lucid Level 2 (70-95), Lucid Level 1 (55-70)
    //Neutral/Dreaming (45-55)
    //Nightmare Level 1 (20-45), Nightmare Level 2 (5-20), Nightmare Level 3 (0-5)
    private void FixedUpdate()
    {
        TrackState();
    }
}

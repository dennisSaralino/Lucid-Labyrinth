using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
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
    public bool nightmareLevel3 = false;
    public bool lucidLevel1 = false;
    public bool lucidLevel2 = false;
    public bool lucidLevel3 = false;

    public Color nightmareColor;
    public Color lucidColor;

    public PlayerController player;
    public ParticleSystem fogEffects;

    private void TrackState()
    {
        lucidLevel1 = lucidLevel2 = lucidLevel3 = false;
        nightmareLevel1 = nightmareLevel2 = nightmareLevel3 = false;

        var main = fogEffects.main;

        //In Lucid State
        if (lucidityBar.value >= 55)
        {
            inLucid = true;
            inNightmare = false;
            inNeutral = false;
            // Lucid Level 3
            if (lucidityBar.value >= 95)
            {
                lucidLevel3 = true;
                player.speedScalar = 6.5f;
            }
            // Lucid Level 2
            else if (lucidityBar.value >= 70)
            {
                lucidLevel2 = true;
                player.speedScalar = 6f;
                main.startColor = lucidColor;
            }
            // Lucid Level 1
            else
            {
                lucidLevel1 = true;
                player.speedScalar = 5.5f;
                main.startColor = Color.white;
            }
        }
        //In Nightmare State
        if (lucidityBar.value <= 45)
        {
            inNightmare = true;
            inLucid = false;
            inNeutral = false;
            // Nightmare Level 3
            if (lucidityBar.value <= 5)
            {
                nightmareLevel3 = true;
                player.speedScalar = 0f;
            }
            // Nightmare Level 2
            else if (lucidityBar.value <= 20)
            {
                nightmareLevel2 = true;
                player.speedScalar = 2.5f;
                main.startColor = nightmareColor;
            }
            // Nightmare Level 1
            else
            {
                nightmareLevel1 = true;
                player.speedScalar = 3.75f;
                main.startColor = Color.white;
            }
        }
        //In Neutral State
        else
        {
            inNeutral = true;
            inLucid = false;
            inNightmare = false;
            player.speedScalar = 5f;
        }
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
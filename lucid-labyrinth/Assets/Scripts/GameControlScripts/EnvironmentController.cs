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

        //In Lucid State
        if (lucidityBar.value >= 55)
        {
            Debug.Log("In Lucid");
            inLucid = true;
            inNightmare = false;
            inNeutral = false;
            // Lucid Level 3
            if (lucidityBar.value >= 95)
            {
                Debug.Log("In Lucid 3");
                lucidLevel3 = true;
                player.speedScalar = 6.5f;
            }
            // Lucid Level 2
            else if (lucidityBar.value >= 70)
            {
                Debug.Log("In Lucid 2");
                lucidLevel2 = true;
                player.speedScalar = 6f;
                fogEffects.startColor = lucidColor;
            }
            // Lucid Level 1
            else
            {
                Debug.Log("In Lucid 1");
                lucidLevel1 = true;
                player.speedScalar = 5.5f;
            }
        }
        //In Nightmare State
        if (lucidityBar.value <= 45)
        {
            Debug.Log("In Nightmare");
            inNightmare = true;
            inLucid = false;
            inNeutral = false;
            // Nightmare Level 3
            if (lucidityBar.value <= 5)
            {
                Debug.Log("In Nightmare 3");
                nightmareLevel3 = true;
                player.speedScalar = 0f;
            }
            // Nightmare Level 2
            else if (lucidityBar.value <= 20)
            {
                Debug.Log("In Nightmare 2");
                nightmareLevel2 = true;
                player.speedScalar = 2.5f;
                fogEffects.startColor = nightmareColor;
            }
            // Nightmare Level 1
            else
            {
                Debug.Log("In Nightmare 1");
                nightmareLevel1 = true;
                player.speedScalar = 3.75f;
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
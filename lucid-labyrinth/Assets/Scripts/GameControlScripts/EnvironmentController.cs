using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
//using UnityEditor.Experimental.GraphView;
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

    private bool monsterInPlay = false;
    public float luciditySpeedModifier;

    public Color nightmareColor;
    public Color lucidColor;
    public Color nightmareTint;
    public Color nightmareTintRed;

    public ParticleSystem fogEffects;
    public GameObject monsterPrefab;
    public BreadcrumbSpawner breadcrumbs;

    public Image playerBlindfold;

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
            if (lucidityBar.value >= 85)
            {
                lucidLevel3 = true;
            }
            // Lucid Level 2
            else if (lucidityBar.value >= 70)
            {
                lucidLevel2 = true;
                main.startColor = lucidColor;
            }
            // Lucid Level 1
            else
            {
                lucidLevel1 = true;
                main.startColor = Color.white;
                breadcrumbs.maxCrumbs = 20;
            }
        }
        //In Nightmare State
        if (lucidityBar.value <= 45)
        {
            inNightmare = true;
            inLucid = false;
            inNeutral = false;

            if (!monsterInPlay)
            {
                Instantiate(monsterPrefab, MazeController.i.mazeData.getEnemySpawnPoints()[0] + new Vector3(0, 1.51f, 0), Quaternion.identity);
                monsterInPlay = true;
            }

            // Nightmare Level 3
            if (lucidityBar.value <= 15)
            {
                playerBlindfold.color = Color.Lerp(playerBlindfold.color, nightmareTintRed, 0 + Time.deltaTime / 5f);
                nightmareLevel3 = true;
            }
            // Nightmare Level 2
            else if (lucidityBar.value <= 30)
            {
                playerBlindfold.color = Color.Lerp(playerBlindfold.color, nightmareTint, 0 + Time.deltaTime / 5f);
                nightmareLevel2 = true;
                main.startColor = nightmareColor;
            }
            // Nightmare Level 1
            else
            {
                playerBlindfold.color = Color.Lerp(playerBlindfold.color, Color.clear, 0 + Time.deltaTime / 5f);
                nightmareLevel1 = true;
                main.startColor = Color.white;
                breadcrumbs.maxCrumbs = 5;
            }
        }
        //In Neutral State
        else
        {
            inNeutral = true;
            inLucid = false;
            inNightmare = false;
        }
    }

    //Levels of Lucidity:
    //Lucid Level 1 (55 - 70), Lucid Level 2 (70 - 85), Lucid Level 3 (85 - 100)
    //Neutral (45-55)
    //Nightmare Level 1 (45 - 30), Nightmare Level 2 (30 - 15), Nightmare Level 3 (15 - 0)
    private void FixedUpdate()
    {
        TrackState();
        luciditySpeedModifier = (Mathf.Floor(Mathf.Abs(lucidityBar.value - 50f)/10f))/2f;
    }
}
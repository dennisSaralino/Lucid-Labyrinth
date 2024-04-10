using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LucidityBar : MonoBehaviour
{

    public Slider slider;
    public PlayerController player;
    public EnvironmentController state;
    // public TMP_Text gameOver;

    private float sprintModifier;

    public void SetStartingLucidity(int health)
    {
        slider.maxValue = health;
        slider.value = health / 2;
    }

    private void Awake()
    {
        SetStartingLucidity(100);
    }

    private void FixedUpdate()
    {
        if (player.isSprinting) { sprintModifier = 2.0f; } else { sprintModifier = 0; }

        if (state.inLucid == true) { slider.value -= Time.deltaTime * (3.25f + sprintModifier); }
        else if (state.inNightmare == true) { slider.value -= Time.deltaTime * (1.5f + sprintModifier); }
        else { slider.value -= Time.deltaTime * (3 + sprintModifier); }

        if (slider.value == 0)
        {
            player.input.Disable();
            SceneManager.LoadScene(3);
        }
    }
}
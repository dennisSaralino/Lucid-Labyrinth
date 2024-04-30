using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;

public class LucidityBar : MonoBehaviour
{

    public Slider slider;
    public PlayerController player;
    public EnvironmentController state;
    public bool debugging = false;
    // public TMP_Text gameOver;

    public Image fillBar;
    public Image fillBorder;

    private float sprintModifier;
    public float monsterModifier;

    public void SetStartingLucidity(int health)
    {
        slider.maxValue = health;
        slider.value = health / 2;
    }

    private void Awake()
    {
        SetStartingLucidity(100);

        if (PlayerPrefs.GetInt("showUI") == 0)
        {
            fillBar.gameObject.SetActive(false);
            fillBorder.gameObject.SetActive(false);
        }
        else
        {
            fillBar.gameObject.SetActive(true);
            fillBorder.gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        
        if (!debugging) {
            if (player.isSprinting) { sprintModifier = 2.0f; } else { sprintModifier = 0; }

            if (state.inLucid == true) { slider.value -= Time.deltaTime * (1f + sprintModifier + monsterModifier); }
            else if (state.inNightmare == true) { slider.value -= Time.deltaTime * (1f + sprintModifier + monsterModifier); }
            else { slider.value -= Time.deltaTime * (1 + sprintModifier + monsterModifier); }
            if (slider.value == 0)
            {
                if (player.isSprinting) { sprintModifier = 2.0f; } else { sprintModifier = 0; }

                if (state.inLucid == true) { slider.value -= Time.deltaTime * (3.25f + sprintModifier); }
                else if (state.inNightmare == true) { slider.value -= Time.deltaTime * (1.5f + sprintModifier); }
                else { slider.value -= Time.deltaTime * (3 + sprintModifier); }

                if (slider.value == 0)
                {
                    player.input.Disable();
                    SceneManager.LoadScene(2);
                }
            }
        }
    }
}
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
    public PauseMenu pauseMenu;
    
    // public TMP_Text gameOver;

    public Image fillBar;
    public Image fillBorder;

    public float baseLucidDrain = 0.5f;
    private float sprintModifier;
    public float monsterModifier;

    public bool debugging = false;

    // Sets starting value at the halfway point
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
        // Option for player to hide bar UI in game
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

        if (!pauseMenu.paused)
        {
            if (player.isSprinting) { sprintModifier = 2.0f; } else { sprintModifier = 0; }
            
            slider.value -= Time.deltaTime * (baseLucidDrain + sprintModifier + monsterModifier);
        }


        if (slider.value == 0)
        {
            player.input.Disable();
            SceneManager.LoadScene(2);
        }
    }
}
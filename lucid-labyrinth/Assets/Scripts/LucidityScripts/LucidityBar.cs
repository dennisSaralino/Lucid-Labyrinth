using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LucidityBar : MonoBehaviour
{

    public Slider slider;
    public PlayerController player;
    
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
        slider.value -= Time.deltaTime;
        if (player.isSprinting == true) { slider.value -= Time.deltaTime * 10; }
    }
}
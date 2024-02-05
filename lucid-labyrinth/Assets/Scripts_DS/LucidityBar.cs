using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LucidityBar : MonoBehaviour
{

    public Slider slider;
    
    public void SetStartingLucidity(int health)
    {
        slider.maxValue = health;
        slider.value = health / 2;
    }

    private void Awake()
    {
        SetStartingLucidity(100);
    }
}

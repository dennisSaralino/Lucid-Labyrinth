using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    private Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (PlayerPrefs.HasKey("volume"))
            slider.value = PlayerPrefs.GetFloat("volume");
        else
            slider.value = 0;
    }
}

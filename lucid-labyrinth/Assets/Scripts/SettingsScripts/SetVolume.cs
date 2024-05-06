using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public Slider slider;
    private void Awake()
    {
        slider.value = PlayerPrefs.GetFloat("volume");
    }
}

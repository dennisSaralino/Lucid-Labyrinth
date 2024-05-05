using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TextMeshProUGUI currentDifficulty;
    // public enum Difficulty { easy, normal, hard };

    private void Awake()
    {
        switch (PlayerPrefs.GetInt("mazeSize"))
        {
            case 6:
                SetDifficulty(0);
                break;

            case 9:
                SetDifficulty(1); 
                break;

            case 12:
                SetDifficulty(2);
                break;

            default:
                SetDifficulty(1);
                break;
        }
        
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetDifficulty(int diff)
    {
        if (diff == 0)
        {
            PlayerPrefs.SetInt("mazeSize", 6);
            PlayerPrefs.SetFloat("pickupGain", 25f);
            currentDifficulty.text = "Easy";
        }
        else if (diff == 1)
        {
            PlayerPrefs.SetInt("mazeSize", 9);
            PlayerPrefs.SetFloat("pickupGain", 15f);
            currentDifficulty.text = "Normal";
        } else if (diff == 2)
        {
            PlayerPrefs.SetInt("mazeSize", 12);
            PlayerPrefs.SetFloat("pickupGain", 20f);
            currentDifficulty.text = "Hard";
        }
    }
}

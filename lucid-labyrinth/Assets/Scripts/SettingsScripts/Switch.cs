using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    public Image On;
    public Image Off;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("showUI"))
        {
            if (PlayerPrefs.GetInt("showUI") == 0)
            {
                Off.gameObject.SetActive(true);
                On.gameObject.SetActive(false);
            }

            if (PlayerPrefs.GetInt("showUI") == 1)
            {
                On.gameObject.SetActive(true);
                Off.gameObject.SetActive(false);
            }
        }     
    }

    public void ON()
    {
        Off.gameObject.SetActive(true);
        On.gameObject.SetActive(false);
        // deactivate UI visibility
        PlayerPrefs.SetInt("showUI", 0);
    }

    public void OFF()
    {
        On.gameObject.SetActive(true);
        Off.gameObject.SetActive(false);
        // activate UI visibility
        PlayerPrefs.SetInt("showUI", 1);
    }
}

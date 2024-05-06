using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Canvas main;
    public Canvas controls;

    private void Awake()
    {
        controls.gameObject.SetActive(false);
    }
    public void Controls()
    {
        controls.gameObject.SetActive(true);
        main.gameObject.SetActive(false);
    }
    public void Return()
    {
        controls.gameObject.SetActive(false);
        main.gameObject.SetActive(true);
    }
}

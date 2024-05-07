using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Canvas pause;
    public Canvas options;
    public Canvas controls;

    public bool paused = false;

    private void Awake()
    {
        pause.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        paused = false;
        this.gameObject.SetActive(false);
    }

    public void Options()
    {
        pause.gameObject.SetActive(false);
        options.gameObject.SetActive(true);
    }

    public void Controls()
    {
        pause.gameObject.SetActive(false);
        controls.gameObject.SetActive(true);
    }

    public void Return()
    {
        options.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        pause.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool paused = false;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        paused = false;
        this.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCursor : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentDifficulty : MonoBehaviour
{
    public TextMeshProUGUI current;
    private void FixedUpdate()
    {
        switch (PlayerPrefs.GetString("difficulty"))
        {
            case "easy":
                current.text = "Easy";
                break;

            case "normal":
                current.text = "Normal";
                break;

            case "hard":
                current.text = "Hard";
                break;
        }
    }
}

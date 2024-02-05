using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    public Slider lucidityBar;
    private enum state
    {
        Nightmare,
        Neutral,
        Lucid
    }
    private state currentState;
    

    private void Report()
    {
        switch(currentState)
        {
            case state.Nightmare:
                Debug.Log("In nightmare mode"); break;

            case state.Neutral:
                Debug.Log("In neutral mode"); break;

            case state.Lucid:
                Debug.Log("In lucid mode"); break;
        }
    }

    private void FixedUpdate()
    {
        if (lucidityBar.value <= 25 && lucidityBar.value > 0)
        {
            currentState = state.Nightmare;
        }
        else if (lucidityBar.value > 25 && lucidityBar.value <= 75)
        {
            currentState = state.Neutral;
        }
        else if (lucidityBar.value > 75)
        {
            currentState = state.Lucid;
        }
        Report();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    public Slider lucidityBar;

    /*
    private void ChangeState()
    {
        switch(lucidityBar.value)
        {
            case 0:

        }
    }
    */
}

public class State
{
    public virtual void ReportChange(EnvironmentController env) { }
}

public class Nightmare : State
{
    public override void ReportChange(EnvironmentController env) { }
}
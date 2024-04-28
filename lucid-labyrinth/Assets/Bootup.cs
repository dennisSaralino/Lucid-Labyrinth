using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootup : MonoBehaviour
{
    private void Awake()
    {
        PlayerPrefs.SetInt("showUI", 1);
    }
}

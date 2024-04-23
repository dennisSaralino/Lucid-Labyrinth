using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Log : MonoBehaviour
{
    public float rotateSpeed = 200f;
    public GameObject log;

    void FixedUpdate()
    {
        //spin the log
        //works for both orientations
        if(log != null)
        log.transform.Rotate(0,rotateSpeed * Time.deltaTime,0);
    }
}

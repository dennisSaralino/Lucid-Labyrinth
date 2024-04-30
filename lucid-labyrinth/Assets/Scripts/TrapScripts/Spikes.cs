using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private float maxHeight = 1f;
    private float startingHeight;
    public float spikeSpeed = 1f;
    public float spikeResetDelay = 2f;
    private float  speedMultiplier = 0f;
    private float interpolator = 0f;


    // At current state
    // after spikes reset, player must collide twice to activate spikes again
    //
    //
    public void Start()
    {
        StartCoroutine(repeatActive());
    }
    IEnumerator repeatActive()
    {
        while (true)
        {
            maxHeight = 0.4f;
            startingHeight = -0.75f;
            yield return StartCoroutine(moveSpike());
            maxHeight = -0.75f;
            startingHeight = 0.4f;
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(moveSpike());
            yield return new WaitForSeconds(2);
        }
    }
    IEnumerator moveSpike()
    {
        speedMultiplier = spikeSpeed;
        interpolator = 0;
        while (interpolator != 1.0f)
        {
            interpolator += speedMultiplier * Time.deltaTime;
            interpolator = Mathf.Min(interpolator, 1.0f);
            transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    // adds smoothness to movement
                    Mathf.Lerp(startingHeight, maxHeight, interpolator),
                    transform.localPosition.z);
            yield return null;
        }
        

    }

}

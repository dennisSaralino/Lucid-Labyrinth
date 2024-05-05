using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBreadcrumbs : MonoBehaviour
{
    public EnvironmentController lucidityState;
    private int willRoam;
    private bool willRotate = false;
    private float randomAngle;

    private void Awake()
    {
        lucidityState = GameObject.FindGameObjectWithTag("EnvironmentController").GetComponent<EnvironmentController>();
        willRoam = Random.Range(0, 3);
        randomAngle = Random.Range(0f, 360f);

        if (lucidityState.nightmareLevel1)
        {
            if (willRoam == 0) { willRotate = true; }
        }
        else if (lucidityState.nightmareLevel2)
        {
            if (willRoam == 0 || willRoam == 1) { willRotate = true; }
        }
        else { willRotate = true; }
    }

    // Update is called once per frame
    void Update()
    {
        if (lucidityState.nightmareLevel1)
        {
            if (willRoam == 0)
            {
                if (willRotate)
                {
                    this.transform.Rotate(this.transform.up, randomAngle);
                    willRotate = false;
                }
                this.transform.position += this.transform.forward * Time.deltaTime;
            }
        }
        else if (lucidityState.nightmareLevel2)
        {
            if (willRoam == 0 || willRoam == 1)
            {
                if (willRotate)
                {
                    this.transform.Rotate(this.transform.up, randomAngle);
                    willRotate = false;
                }
                this.transform.position += this.transform.forward * Time.deltaTime;
            }
        }
        else {
            if (willRotate)
            {
                this.transform.Rotate(this.transform.up, randomAngle);
                willRotate = false;
            }
            this.transform.position += this.transform.forward * Time.deltaTime;
        }
    }
}

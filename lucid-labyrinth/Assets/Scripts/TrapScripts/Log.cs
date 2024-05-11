using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class Log : Trap
{
    public float rotateSpeed = 200f;
    public GameObject log;
    
    public AudioClip audioClip;

    float startPoint;
    float endPoint;
    static float interpolator=0f;

    [SerializeField]
    float riseSpeed = 0.1f;

    private bool isPlaying = false;
    public override void Start()
    {
        base.Start();
        audioSource.loop = true;
        startPoint= log.transform.position.y;

        // how high log will raise
        endPoint= startPoint +1.5f; //<- modify number
    }
  

    void FixedUpdate()
    {

        //modify y value to raise/lower log
        log.transform.position = new Vector3(log.transform.position.x,
                                            Mathf.Lerp(startPoint, endPoint,interpolator),
                                            log.transform.position.z);
        // gives mathf.lerp() functionality
        interpolator += riseSpeed * Time.deltaTime;

        // change direction when log reached top
        if(interpolator >= 1f){
            float temp = endPoint;
            endPoint = startPoint;
            startPoint = temp;
            interpolator = 0f;
        }

        // plays soundfx
        if(!isPlaying && audioClip != null){
            audioSource.PlayOneShot(audioClip);
        }

        //spin the log
        //works for both orientations
        if(log != null)
        log.transform.Rotate(rotateSpeed * Time.deltaTime,0,0);
    }
}

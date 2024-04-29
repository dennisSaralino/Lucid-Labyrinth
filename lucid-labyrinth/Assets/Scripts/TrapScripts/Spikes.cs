using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spikes : MonoBehaviour
{
   
    
    // bools
    private bool activated = false;
    private bool resetSpikes = false;
    private bool isMoving = false;
    // variables
    public float maxHeight = 1f;
    private float startingHeight;
    public float spikeSpeed = 1f;
    public float spikeResetDelay = 2f;
    //utility
    private float  speedMultiplier = 0f;
    private float interpolator = 0f;
    private float bottomHeight;

    //Audio
    private AudioSource audioSource;
    public AudioClip shootAudio;
    public AudioClip resetAudio;

 
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //startingHeight will change with movment
        startingHeight = transform.position.y;
        //bottomHeight is a reference to resting pos
        bottomHeight = transform.position.y;
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            activated = true;   
        }
    }

    void FixedUpdate()
    {
        // initiates movement
        if(activated && !isMoving){
            speedMultiplier = spikeSpeed;
            if(audioSource != null)
                audioSource.PlayOneShot(shootAudio);
            activated = false;
            isMoving = true;
        }

        // updates position of spikes 
        interpolator += speedMultiplier * Time.deltaTime;

        transform.position= new Vector3(
                transform.position.x,
                // adds smoothness to movement
                Mathf.Lerp(startingHeight, maxHeight, interpolator),
                transform.position.z);   

        // reverses values to descent , also resets 
        if(interpolator >= spikeResetDelay ){
            ResetLerp();
            resetSpikes = true;
        }

        //sets values back to normal
        if(transform.position.y <= bottomHeight && resetSpikes && isMoving){
            speedMultiplier = 0f;
            resetSpikes = false;
            activated = false;
            isMoving = false;
            ResetLerp();
            if(audioSource!=null)
                audioSource.PlayOneShot(resetAudio);
        }
    }

    void ResetLerp(){
        float temp = maxHeight;
        maxHeight = startingHeight;
        startingHeight = temp;

        interpolator = 0f;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    private bool activated = false;
    private bool resetSpikes = false;
    public float maxHeight = 1f;
    private float startingHeight;
    public float spikeSpeed = 1f;
    public float spikeResetDelay = 2f;
    private float  speedMultiplier = 0f;
    private float interpolator = 0f;

    private float bottomHeight;
    

   // At current state
   // after spikes reset, player must collide twice to activate spikes again
   //
   //

    
    void Start()
    {
       
        startingHeight = transform.position.y;
        bottomHeight = transform.position.y;
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            activated = true;

            Debug.Log($"In OnTriggerEnter: activated is {activated}");
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       if(activated){  
            speedMultiplier = spikeSpeed;
            
        }

        // updates position of spikes 
        interpolator += speedMultiplier * Time.deltaTime;

        transform.position= new Vector3(
                transform.position.x,
                // adds smoothness to movement
                Mathf.Lerp(startingHeight, maxHeight, interpolator),
                transform.position.z);   


       // Debug.Log("in update; activated = " + activated);
       // Debug.Log("in update; speedMultiplier = " + speedMultiplier);
 
        
        // reverses values to descent , also resets 
        if(interpolator >= spikeResetDelay){
            
            float temp = maxHeight;
            maxHeight = startingHeight;
            startingHeight = temp;

            interpolator = 0f;
            resetSpikes = true;
        }

        //sets values back to normal
        if(transform.position.y <= bottomHeight && resetSpikes){
            speedMultiplier = 0f;
            resetSpikes = false;
            activated = false;
            Debug.Log("in resetting; activated = " + activated);
           
        }

    }

}

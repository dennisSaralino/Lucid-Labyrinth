using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
//using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.UIElements;

public class Pendulum : Trap
{
    
    
    // swing behavior
    public float swingSpeed = 10f;
    public float limit = 35f;

    // prevents multiple sound plays
    private bool soundPlayed = false;
    
    //audio 
    public AudioClip audioClip;

    public override void Start(){
        base.Start();
      
    }
    public float orient = 0;
    public void setOrientation(float f)
    {
        orient = f;
    }
    
    void FixedUpdate(){
        //provides swinging for pendulum
        float angle = limit * Mathf.Sin(Time.time * swingSpeed);
        transform.rotation = Quaternion.Euler(angle, orient, 0);
    
        //play sound each swing
        if((Mathf.Abs(angle) > limit/2f) && soundPlayed == false ){
            if(audioClip != null)
                audioSource.PlayOneShot(audioClip);
            soundPlayed = true;  
        }
        // resets soundPlayed
        if(Mathf.Abs(angle) < limit /2f) soundPlayed = false;
    }
}

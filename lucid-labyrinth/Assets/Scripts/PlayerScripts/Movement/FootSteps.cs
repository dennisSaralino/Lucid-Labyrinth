
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FootSteps : MonoBehaviour
//For footsteps, ground must be in "Ground" Layer 
// with a corresponding tag (rock, water, etc.)

{
   
    // holds sounds
    public AudioSource audioSource;
    public AudioClip rock;
    public AudioClip water;

    //used to detect movement
    private Vector3 previousPosition;

    //values for Raycast 
    private RaycastHit hit;
    public Transform rayStart;
    public float rayRange;
    public LayerMask layerMask; 

    //no delay = step every frame
    public float maxWalkDelay = 0.7f;
   // public float maxSprintDelay = 0.4f;
    public float currentDelay;

    void Start()
    {
     audioSource = GetComponent<AudioSource>();   
     previousPosition = transform.position;
     currentDelay = maxWalkDelay;
    }

   
    // Update is called once per frame
    void FixedUpdate()
    {
       

        //if theres a difference in position, then player is walking
        
        
        if(transform.position != previousPosition && currentDelay >= maxWalkDelay){
            //Debug.DrawRay(rayStart.position,rayStart.up*-1* rayRange, Color.green);   
            StartWalk();
            currentDelay = 0f;
        }
        
        
        
        previousPosition = transform.position;
        currentDelay += Time.deltaTime;
    }

    void StartWalk(){
                    // from rayStart.position, raycast downward about 1.5m on layerMask
        if(Physics.Raycast(rayStart.position,rayStart.up * -1, out hit, rayRange, layerMask)){
            //hit holds collision info
            if(hit.collider.CompareTag("Rock"))
                TakeStep(rock);
            if(hit.collider.CompareTag("Water"))
                TakeStep(water);
        }
    }

    void TakeStep(AudioClip audio){
        //add variation
        audioSource.pitch = Random.Range(0.7f, 1.2f);
        audioSource.PlayOneShot(audio);
    }
}

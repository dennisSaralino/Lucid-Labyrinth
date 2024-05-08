
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class FootSteps : MonoBehaviour
//For footsteps, ground must be in "Ground" Layer 
// with a corresponding tag (rock, water, etc.)

{
   
    // holds sounds
    public AudioSource audioSource;
    public AudioClip rock;
    public AudioClip water;
    public AudioClip jumpStart;
    public AudioClip jumpEnd;
    public AudioClip takeDamage;
    public AudioClip lucidityPickup;

    //used to detect movement
    private Vector3 previousPosition;

    //values for Raycast 
    private RaycastHit hit;
    public Transform rayStart;
    public float rayRange;
    public LayerMask layerMask;

    private PlayerController playerController; 

    //no delay = step every frame
    public float walkDelay = 0.7f;
    public float sprintDelay = 0.4f;
   private float delay = 0f;
    public float currentDelay;

    void Start()
    {
     playerController = GetComponentInParent<PlayerController>();
     audioSource = GetComponent<AudioSource>();   
     previousPosition = transform.position;
     currentDelay = walkDelay;
     float delay = playerController.isSprinting ? sprintDelay : walkDelay;
    }

    void FixedUpdate()
    {

        // Check if the player is moving
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        // Reduce the delay based on the current time
        delay -= Time.deltaTime;

        // If the player is moving and the delay has elapsed, play footstep sounds
        if (isMoving && delay <= 0f)
        {
            StartWalk();
            delay = playerController.isSprinting ? sprintDelay : walkDelay;
        }


    }

    

    void StartWalk()
    {
                    // from rayStart.position, raycast downward about 1.5m on layerMask
        if(Physics.Raycast(rayStart.position,rayStart.up * -1, out hit, rayRange, layerMask))
        {
            //hit holds collision info
            if(hit.collider.CompareTag("Rock"))
                audioSource.volume = 0.1f; //0.03f;
                PlaySound(rock);
            if(hit.collider.CompareTag("Water")){
                audioSource.volume = 0.3f; //0.1f;
                PlaySound(water);
                audioSource.volume = 0.1f; //0.03f;
            }
        }
    }
    // used for walking/running
    void PlaySound(AudioClip audio)
    {
        //add variation
        if(audio != null){
            audioSource.pitch = Random.Range(0.7f, 1.2f);
            audioSource.PlayOneShot(audio);
        }
    }


    // methods below invoked by PlayerController script
    //
    ///////

    //plays sound for jumping (rising))
    public void PlayJumpStart(){
        if(jumpStart != null)
            audioSource.PlayOneShot(jumpStart);
    }

    //plays sound for jumping (landing)
    public void PlayJumpEnd(){
        if(jumpEnd != null)
            audioSource.PlayOneShot(jumpEnd);
    }

    // plays sound when taking damage (for traps)
    public void PlayDamage(){
        if(takeDamage != null){
            audioSource.volume = 0.6f;
            audioSource.PlayOneShot(takeDamage);
            audioSource.volume = 0.1f;
        }
    }

    //play sound when picking up lucidity object
    public void PlayLucidityPickup(){
        if (lucidityPickup != null)
            audioSource.PlayOneShot(lucidityPickup, 0.6f); //0.3f);
    }
}

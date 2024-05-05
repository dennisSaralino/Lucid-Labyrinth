
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
     
    }

    void FixedUpdate()
    {
        // swaps frequency of sound plays
        delay = playerController.isSprinting ? sprintDelay : walkDelay;
        
        //if theres a difference in x || z position, then player is walking
        if((transform.position.x != previousPosition.x ||
           transform.position.z != previousPosition.z) &&
           currentDelay >= delay)
        {
            StartWalk();
            currentDelay = 0f;
        }

        // always plays sound at beginning of walk
        if(transform.position == previousPosition && !playerController.isJumping)
            currentDelay = walkDelay;
        

        //continuously updates
        previousPosition = transform.position;
        currentDelay += Time.deltaTime;
    }

    void StartWalk()
    {
                    // from rayStart.position, raycast downward about 1.5m on layerMask
        if(Physics.Raycast(rayStart.position,rayStart.up * -1, out hit, rayRange, layerMask))
        {
            //hit holds collision info
            if(hit.collider.CompareTag("Rock"))
                audioSource.volume = 0.03f;
                Debug.Log("Walking");
                PlaySound(rock);
            if(hit.collider.CompareTag("Water")){
                audioSource.volume = 0.1f;
                PlaySound(water);
                audioSource.volume = 0.03f;
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
}

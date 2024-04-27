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
    
    private AudioSource audioSource;
    public AudioClip audioClip;

    private bool isPlaying = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        
    }
  

    void FixedUpdate()
    {
        if(!isPlaying && audioClip != null){
            audioSource.PlayOneShot(audioClip);
            
        }



        //spin the log
        //works for both orientations
        if(log != null)
        log.transform.Rotate(rotateSpeed * Time.deltaTime,0,0);
    }
}

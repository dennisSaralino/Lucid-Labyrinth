using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Audio
    private AudioSource audioSource;
    public AudioClip arrowImpactAudio;
    
    // functionality
    Rigidbody rigidbody;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, 10f);
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            Destroy(gameObject);
        }
        else if(col.gameObject.CompareTag("ArrowPlate"))
        {
            if(audioSource != null)
                audioSource.PlayOneShot(arrowImpactAudio);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        
    }
}

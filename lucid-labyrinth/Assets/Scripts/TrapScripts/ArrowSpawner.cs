using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrowSpawner : MonoBehaviour
{
    
    public GameObject arrowPrefab;
    public Transform arrowPos;

    private AudioSource audioSource;
    public AudioClip arrowShootAudio;
    public AudioClip pressurePlateAudio;

    public float arrowSpeed = 1f;


    // Works for wall with 0 and 90 rotation

    void Start(){
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("Player")){
            if(audioSource != null)
                audioSource.PlayOneShot(pressurePlateAudio);
            ShootArrow();
        }
    }
    
    void ShootArrow(){
        Debug.Log("in ShootArrow");
        
        //get set rotation of arrow based on rotation of wall
        float yRotation; yRotation = transform.rotation.y == 0f ? 0f : 90f;

        //spawn arrow
        Quaternion arrowRot = Quaternion.Euler(0f,yRotation, -90f);
        Vector3 newPos = new Vector3(arrowPos.position.x,arrowPos.position.y,arrowPos.position.z);
        GameObject newArrow = Instantiate(arrowPrefab, newPos, arrowRot);
        aSource.clip = arrowSoundClip;
        aSource.Play();
        Rigidbody newRigid = newArrow.GetComponent<Rigidbody>();

        //give movement
        Vector3 arrowVelocity;
        if(audioSource !=null)
            audioSource.PlayOneShot(arrowShootAudio);
        if(transform.rotation.y == 0f){
            arrowVelocity = new Vector3(arrowSpeed,0,0);
            newRigid.velocity= arrowVelocity;
        }
        else if(transform.rotation.y == 0.7071068f){ //<- quaternion value for 90 degrees
            arrowVelocity = new Vector3(0,0,-arrowSpeed);
            newRigid.velocity= arrowVelocity;
        }
       
       
    }
}

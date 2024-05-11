using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrowSpawner : Trap
{
    public GameObject arrowPrefab;
    public List<Transform> arrowPosL;
    public AudioSource aSource;
    public AudioClip arrowSoundClip;
    // Works for wall with 0 and 90 rotation

    public AudioClip arrowShootAudio;
    public AudioClip pressurePlateAudio;

    public float arrowSpeed = 1f;
    public void Awake()
    {
        StartCoroutine(shootAllArrow());
    }
    public override void Start()
    {
        base.Start();
    }
    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("Player")){
            if(audioSource != null)
                audioSource.PlayOneShot(pressurePlateAudio);
        }
    }
    
    IEnumerator shootAllArrow()
    {
        while(true)
        {
            int current = 0;
            int waitI = 9;
            for (int i = 0; i < arrowPosL.Count; i++)
            {
                ShootArrow(i);
                current++;
                if (current == waitI)
                {
                    current = 0;
                }
            }

            yield return new WaitForSeconds(2.5f);
        }
    }
    void ShootArrow(int index){
        Debug.Log("in ShootArrow");
        Transform arrowPos = arrowPosL[index];
        //get set rotation of arrow based on rotation of wall
        Debug.Log(transform.parent.rotation.eulerAngles.y);
        float yRotation = transform.parent.rotation.y == 0f ? -180f : 90f;
       
        //spawn arrow
        Quaternion arrowRot = Quaternion.Euler(0f,-1 * yRotation, -90f);
        Vector3 newPos = new Vector3(arrowPos.position.x,arrowPos.position.y,arrowPos.position.z);
        GameObject newArrow = Instantiate(arrowPrefab, newPos, arrowRot);       
    }
}

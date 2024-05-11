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
    
    // Start is called before the first frame update
    float spd = 15;
    void Start()
    {
         audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, 1f);
        StartCoroutine(selfDestroy());
    }
    IEnumerator selfDestroy()
    {
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position += -1 * Vector3.Cross(transform.forward, Vector3.up) * spd * Time.deltaTime;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            Destroy(gameObject);
        }
        
    }
}

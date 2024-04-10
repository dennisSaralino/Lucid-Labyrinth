using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1f);
        
    }

    // Update is called once per frame
    
    
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            Destroy(gameObject);
        }
        
    }
}

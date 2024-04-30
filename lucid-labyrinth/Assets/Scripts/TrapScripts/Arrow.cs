using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    float spd = 15;
    void Start()
    {
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
        //else if(col.gameObject.CompareTag("ArrowPlate"))
        //{
    
        //    rigidbody.velocity = Vector3.zero;
        //    rigidbody.angularVelocity = Vector3.zero;
        //}
        
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupObjScript : MonoBehaviour
{
    private bool isHeld = false;
    private bool isAirborne = false;
    private bool hitGround = false;
    private Vector3 throwAngle = Vector3.zero;
    private GameObject playerHoldPos;
    private BoxCollider objCollider;
    private GameObject[] monsters;
    public string throwableType;

    private void Start()
    {
        playerHoldPos = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().holdPos;
        objCollider = GetComponent<BoxCollider>();
        monsters = GameObject.FindGameObjectsWithTag("Monster");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isHeld) { transform.position = playerHoldPos.transform.position; }
        if (hitGround)
        {
            foreach (GameObject x in monsters)
            {
                Debug.Log(transform.position);
                x.GetComponent<basicAI>().alert(transform.position);
            }
            hitGround = false;
        }
        if (gameObject.CompareTag("Key"))
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        
    }

    public void Hold()
    {
        isHeld = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ThrowObj(Vector3 thVec)
    {
        isHeld = false;
        GetComponent<Rigidbody>().isKinematic = false;
        isAirborne = true;
        GetComponent<Rigidbody>().AddForce(thVec);
    }

    public void Drop()
    {
        isHeld = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public bool isKey()
    {
        return gameObject.CompareTag("Key");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) {
            if (isAirborne)
            {
                hitGround = true;
                isAirborne = false;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BadController_PW : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;

    public Slider lucidityBar;

    // private float lucidityMeter = 100;

    // Start is called before the first frame update

    private int damagePool = 5;
    private int damageProjectile = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            lucidityBar.value += 20;
        }
        if (other.gameObject.CompareTag("DamagePool"))
        {
            lucidityBar.value -= damagePool;
        }
        if (other.gameObject.CompareTag("DamageProjectile"))
        {
            lucidityBar.value -= damageProjectile;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("DamagePool"))
        {
            lucidityBar.value -= damagePool;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);

        /*
        lucidityMeter -= Time.deltaTime * 2;
        if (lucidityMeter > 100) { lucidityMeter = 100; }
        //Debug.Log(Math.Ceiling(lucidityMeter));
        */
    }
}

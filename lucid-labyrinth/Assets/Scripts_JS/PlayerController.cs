using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Camera mainCam;
    private PlayerControls input = null;
    private Rigidbody playerBody;
    private Vector3 moveVector = Vector3.zero;
    private Vector2 cameraVector = Vector2.zero;
    private Quaternion playerRot = Quaternion.identity;
    public float speedScalar = 10.0f;
    int lookSensitivity = 6;
    public Slider lucidityBar;

    // Start is called before the first frame update
    private void Awake()
    {
        input = new PlayerControls();
        playerBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.player.move.performed += OnMovementPerformed;
        input.player.camera.performed += OnCameraMovementPerformed;
        input.player.move.canceled += OnMovementCancelled;
        input.player.camera.canceled += OnCameraMovementCancelled;
    }
    private void OnDisable()
    {
        input.Disable();
        input.player.move.performed -= OnMovementPerformed;
        input.player.camera.performed -= OnCameraMovementPerformed;
        input.player.move.canceled -= OnMovementCancelled;
        input.player.camera.canceled -= OnCameraMovementCancelled;
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector3>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector3.zero;
    }

    private void OnCameraMovementPerformed(InputAction.CallbackContext value)
    {
        cameraVector = value.ReadValue<Vector2>();
    }

    private void OnCameraMovementCancelled(InputAction.CallbackContext value)
    {
        cameraVector = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector3 currentVelocity = new Vector3(moveVector.x * speedScalar, 0, moveVector.z * speedScalar);
        playerBody.velocity = transform.TransformDirection(currentVelocity);

        var playerQuat = transform.rotation.eulerAngles;
        var camQuat = mainCam.transform.rotation.eulerAngles;
        playerQuat.y += reduceNum(cameraVector.x) * lookSensitivity;
        camQuat.y = playerQuat.y;
        if (mainCam.transform.rotation.x > -80 && mainCam.transform.rotation.x < 90)
        {
            if (mainCam.transform.rotation.x + (reduceNum(cameraVector.y) * lookSensitivity) > 90)
            {
                //float tmp = 80 - mainCam.transform.rotation.x;
                camQuat.x = 90;
            }
            else if (mainCam.transform.rotation.x + (reduceNum(cameraVector.y) * lookSensitivity) < -80)
            {
                //float tmp = -90 - mainCam.transform.rotation.x;
                camQuat.x = -80;
            }
            else
            {
                camQuat.x -= reduceNum(cameraVector.y) * lookSensitivity;
            }
            
            //if (cameraVector != new Vector2(0.0f, 0.0f)) { Debug.Log(cameraVector); }
            if (moveVector != new Vector3(0.0f, 0.0f, 0.0f)) { 
                //Debug.Log((float)Math.Cos(transform.rotation.y));
                Debug.Log((float)Math.Sin(transform.rotation.y));
            }
        }
        playerQuat.z = 0;
        camQuat.z = 0;
        transform.rotation = Quaternion.Euler(playerQuat);
        mainCam.transform.rotation = Quaternion.Euler(camQuat);
    }

    float reduceNum(float x)
    {
        if (-1 <= x && x <= 1) {
            return x;
        }
        else
        {
            float tmp = reduceNum(x / 10);
            return tmp;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            lucidityBar.value += 20;
        }
        /*
        if (other.gameObject.CompareTag("DamagePool"))
        {
            lucidityBar.value -= damagePool;
        }
        if (other.gameObject.CompareTag("DamageProjectile"))
        {
            lucidityBar.value -= damageProjectile;
        }
        */

    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor.UIElements;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    // global GameObject variables
    public NoiseSettings weakShake;
    public NoiseSettings strongShake;
    public NoiseSettings extremeShake;
    public CinemachineVirtualCamera mainCam;
    private CinemachineBasicMultiChannelPerlin camEffect;
    private EnvironmentController env;
    public GameObject environmentCont;
    private PlayerControls input = null;
    private CharacterController playerController;
    private bool isGrappling = false;
    private float yVelocity = -9.8f;

    // global vectors for storing input values
    private Vector3 moveVector = Vector3.zero;
    private Vector2 cameraVector = Vector2.zero;
    public Slider lucidityBar;

    // Scalable values for speed and look sensitivity.
    [Range(1.0f, 20.0f)]
    public float speedScalar = 5.0f;
    [Range(1.0f, 10.0f)]
    public float xLookSensitivity = 4.5f;
    [Range(1.0f, 10.0f)]
    public float yLookSensitivity = 3.0f;


    // Private GameObject variables inititalized
    private void Awake()
    {
        input = new PlayerControls();
        playerController = GetComponent<CharacterController>();
        camEffect = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        env = environmentCont.GetComponent<EnvironmentController>();
        //Cursor.lockState = CursorLockMode.Locked;
    }


    // The following functions exist for enabling and disabling player movement
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

    private void Update() // Camera Controls are in Update for smoothness
    {
        // set current player/camera rotations equal to temporary quaternions
        var playerQuat = transform.rotation.eulerAngles;
        var camQuat = mainCam.transform.rotation.eulerAngles;

        // update temp player/camera Quaternions based on mouse delta/right stick position (depending on input method)
        playerQuat.y += reduceNum(cameraVector.x) * yLookSensitivity;
        camQuat.y = playerQuat.y;
        camQuat.x -= Mathf.Clamp(reduceNum(cameraVector.y) * xLookSensitivity, -80, 90);

        // these two lines exist for the sole fact that moving the mouse was rotating the 
        // player/camera on the z-axis even though these values were never changed
        playerQuat.z = 0;
        camQuat.z = 0;

        // set the player/camera rotation equal to the updated temp player/camera quaternions
        transform.rotation = Quaternion.Euler(playerQuat);
        mainCam.transform.rotation = Quaternion.Euler(camQuat);

        
    }

    private void FixedUpdate()
    {
        // update velocity based on current input
        Vector3 currentVelocity = new Vector3(moveVector.x * 0.75f, 0, moveVector.z);
        if (isGrappling)
        {
            currentVelocity.y = 0;
        }
        else
        {
            currentVelocity.y = yVelocity;
        }
        Vector3 scaledVelocity = currentVelocity * Time.deltaTime * speedScalar;
        if (currentVelocity.x != 0 || currentVelocity.z != 0)
        {
            Debug.Log(scaledVelocity);
        }
        playerController.Move(transform.TransformDirection(scaledVelocity));

        if (isGrappling) {
            currentVelocity.y = 0;
        }
        else {
            currentVelocity.y = -9.8f;
        }

        if (env.Report() == 1)
        {
            camEffect.m_NoiseProfile = extremeShake;
        }
        else if (env.Report() == 2)
        {
            camEffect.m_NoiseProfile = strongShake;
        }
        else if (env.Report() == 3)
        {
            camEffect.m_NoiseProfile = weakShake;
        }

        
    }


    void enableGrapple()
    {
        isGrappling = true;
    }

    void disableGrapple()
    {
        isGrappling = false;
    }

    // A little recursive function that reduces a value to be between -1 and 1.
    // Exists to turn mouse deltas into values closer resembling values from stick inputs
    // so that we don't have to check for input method when moving the player/camera.
    float reduceNum(float x)
    {
        if (-1 <= x && x <= 1) {
            return x;
        }
        else
        {
            return reduceNum(x / 10);
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

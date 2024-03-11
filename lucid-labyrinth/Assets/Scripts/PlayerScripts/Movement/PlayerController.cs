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
    public GameObject environmentCont;
    private CinemachineBasicMultiChannelPerlin camEffect;
    private EnvironmentController env;
    private PlayerControls input = null;
    private CharacterController playerController;
    private pickupHitboxScript pickupHitbox;
    private GameObject currentPickup;

    // global movement bools
    public bool isGrappling = false;
    public bool isSprinting = false;
    private bool doFalling = true;
    private bool holdingObj = false;
    private float jumpTimer = 0.0f;

    // global gravity variable
    private float gravity = -9.81f;

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
        pickupHitbox = mainCam.GetComponentInChildren<pickupHitboxScript>();
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

        if (input.player.sprint.WasPerformedThisFrame())
        {
            isSprinting = true;
            speedScalar += 2.0f;
            camEffect.m_FrequencyGain += 0.5f;
        }
        if (input.player.sprint.WasReleasedThisFrame())
        {
            isSprinting = false;
            speedScalar -= 2.0f;
            camEffect.m_FrequencyGain -= 0.5f;
        }

        if (input.player.interact.WasPerformedThisFrame())
        {
            if (pickupHitbox.grabableObj() != null)
            {
                currentPickup = pickupHitbox.grabableObj();
                currentPickup.GetComponent<pickupObjScript>().Hold();
                holdingObj = true;
            }
        }
        else if (holdingObj)
        {
            if (input.player.interact.WasPerformedThisFrame())
            {
                currentPickup.GetComponent<pickupObjScript>().Drop();
                currentPickup = null;
                holdingObj = false;
                Debug.Log("Tried to drop");
            }
        }
    }

    private void FixedUpdate()
    {
        // update velocity based on current input
        
        Vector3 playerMoveDelta = new Vector3(moveVector.x * 0.75f, 0, moveVector.z);
        if (playerController.isGrounded && input.player.jump.WasPerformedThisFrame()) { jumpTimer = 0.5f; }
        else if (!playerController.isGrounded && jumpTimer <= 0.0f) { playerMoveDelta.y -= 1.0f; }
        else if (playerController.isGrounded) { playerMoveDelta.y = 0; }

        if (jumpTimer != 0.0f)
        {
            playerMoveDelta.y += 0.5f;
            jumpTimer -= Time.deltaTime;
            if (jumpTimer < 0.0f) { jumpTimer = 0.0f; }
        }

        Vector3 scaledVelocity = playerMoveDelta * Time.deltaTime * speedScalar;
        playerController.Move(transform.TransformDirection(scaledVelocity));

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

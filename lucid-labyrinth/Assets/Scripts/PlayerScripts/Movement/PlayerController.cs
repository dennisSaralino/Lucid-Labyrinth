using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using UnityEditor.UIElements;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    // global GameObject variables
    public NoiseSettings weakShake;
    public NoiseSettings strongShake;
    public NoiseSettings extremeShake;
    public CinemachineVirtualCamera mainCam;
    public GameObject environmentCont;
    //public GameObject pickupHitBox;
    private CinemachineBasicMultiChannelPerlin camEffect;
    private EnvironmentController env;
    public PlayerControls input = null;
    private CharacterController playerController;
    //private pickupHitboxScript pickupHitboxScript;
<<<<<<< Updated upstream
    private GameObject currentPickup;
    public GameObject pauseMenu;
=======
    public GameObject currentPickup { get; set; }
    public PauseMenu pauseMenu;
>>>>>>> Stashed changes

    private float xRot;
    private float yRot;

    // global movement bools
    public bool isGrappling = false;
    public bool isSprinting = false;
    //private bool holdingObj = false;
    public bool paused = false;

    // timer ints
    private float jumpTimer = 0.0f;
    private float pickupCooldown = 0.0f;

    // layer mask

    private int pickupLayerMask = 1 << 3;

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

    // values for decrementing after taking damage
    private float damagePool = 10f;
    private float damageProjectile = 12f;

    // For walking audio


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yRot = mainCam.transform.rotation.y;
        xRot = mainCam.transform.rotation.x;
    }

    // Private GameObject variables inititalized
    private void Awake()
    {
        input = new PlayerControls();
        playerController = GetComponent<CharacterController>();
        camEffect = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        env = environmentCont.GetComponent<EnvironmentController>();
        //pickupHitboxScript = pickupHitBox.GetComponent<pickupHitboxScript>();\
        pauseMenu.gameObject.SetActive(false);
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
        // update velocity based on current input
        yRot += cameraVector.x * Time.deltaTime * xLookSensitivity;
        xRot -= cameraVector.y * Time.deltaTime * yLookSensitivity;
        xRot = Mathf.Clamp(xRot, -90f, 90f); // Clamp the x rotation of the camera to limit how far up/down the player can look

        // set the player/camera rotation equal to the the new x and y rotation values
        mainCam.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        transform.rotation = Quaternion.Euler(0f, yRot, 0);

        Vector3 playerMoveDelta = new Vector3(moveVector.x * 0.75f, 0, moveVector.z);
        if (playerController.isGrounded && input.player.jump.WasPerformedThisFrame()) { jumpTimer = 0.4f; }
        else if (!playerController.isGrounded && jumpTimer <= 0.0f) { playerMoveDelta.y -= 0.7f; }
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

        if (input.player.pause.WasPerformedThisFrame())
        {
<<<<<<< Updated upstream
            if (paused)
            {
                paused = false;
                pauseMenu.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                paused = true;
                pauseMenu.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
=======
            if (pauseMenu.paused == false)
            {
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.paused = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.paused = false;
                pauseMenu.gameObject.SetActive(false);
>>>>>>> Stashed changes
            }
        }
    }
    public int solutionIndex;
    private void FixedUpdate()
    {
        // add mouse deltas to current camera rotation
        yRot += cameraVector.x * Time.deltaTime * xLookSensitivity;
        xRot -= cameraVector.y *Time.deltaTime * yLookSensitivity;
        xRot = Mathf.Clamp(xRot, -90f, 90f); // Clamp the x rotation of the camera to limit how far up/down the player can look

        // set the player/camera rotation equal to the the new x and y rotation values
        mainCam.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        transform.rotation = Quaternion.Euler(0f, yRot, 0);

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

        if (input.player.interact.WasPerformedThisFrame() && currentPickup == null)
        {
            RaycastHit pickupHit;
            Physics.SphereCast(mainCam.transform.position, 1.0f, mainCam.transform.forward, out pickupHit, 5f, pickupLayerMask);
            if (pickupHit.collider != null)
            {
                currentPickup = pickupHit.collider.gameObject;
                currentPickup.GetComponent<pickupObjScript>().Hold();
                pickupCooldown = 0.5f;
            }

        }
        else if (input.player.interact.WasPerformedThisFrame() && currentPickup != null)
        {
            if (pickupCooldown == 0.0f)
            {
                currentPickup.GetComponent<pickupObjScript>().Drop();
                currentPickup = null;
            }
        }
        else
        {
            if (pickupCooldown > 0) { pickupCooldown -= Time.deltaTime; }
            else { pickupCooldown = 0.0f; }
        }


        if (input.player.throwObj.WasPerformedThisFrame())
        {
            if (currentPickup != null)
            {
                currentPickup.GetComponent<Rigidbody>().isKinematic = false;
                Vector3 thVec = mainCam.transform.forward * 1000;
                Debug.Log(thVec);
                thVec.y = xRot;
                //Debug.DrawRay(transform.position + new Vector3(0, 0.6f, 0.1f), thVec, Color.white, 120f);
                currentPickup.GetComponent<pickupObjScript>().Drop();
                currentPickup.GetComponent<Rigidbody>().AddForce(thVec);
                currentPickup = null;
            }
        }

    }

    public bool isJumping()
    {
        return jumpTimer == 0.0f;
    }


    public void enableGrapple()
    {
        isGrappling = true;
    }

    public void disableGrapple()
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
            lucidityBar.value += 15;
        }
        
        if (other.gameObject.CompareTag("Fire"))
        {
            lucidityBar.value -= damagePool;
        }
        if (other.gameObject.CompareTag("Arrow"))
        {
            lucidityBar.value -= damageProjectile;
        }
       
    }

    
}

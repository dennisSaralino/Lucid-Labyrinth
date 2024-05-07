using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using UnityEditor.UIElements;
using Cinemachine;
using UnityEngine.SceneManagement;

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
    public PlayerControls input = null;
    private CharacterController playerController;
    private AudioSource SFX;
    private GameObject[] monsters;

    //private pickupHitboxScript pickupHitboxScript;
    public GameObject holdPos;

    public GameObject currentPickup { get; set; }
    public PauseMenu pauseMenu;
    public Canvas pickupControl;

    public FootSteps footSteps;
    public Transform footPos;
    private float xRot;
    private float yRot;

    // global movement bools
    public bool isSprinting = false;
    public bool isJumping = false;
    //private bool holdingObj = false
    //public bool paused = false;


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
    [SerializeField]
    private float fireEnter = 10f;
    [SerializeField]
    private float fireStay = 0.5f;
    [SerializeField]
    private float arrowDamage = 12f;
    [SerializeField]
    private float logDamage = 5f;
    [SerializeField]
    private float spikeDamage = 10f;

    // For walking audio


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yRot = mainCam.transform.rotation.y;
        xRot = mainCam.transform.rotation.x;
        Application.targetFrameRate = 80;
        monsters = GameObject.FindGameObjectsWithTag("Monster");
    }

    // Private GameObject variables inititalized
    private void Awake()
    {
        mainCam = GetComponentInChildren<CinemachineVirtualCamera>();
        input = new PlayerControls();
        playerController = GetComponent<CharacterController>();
        camEffect = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        env = environmentCont.GetComponent<EnvironmentController>();
        SFX = GetComponent<AudioSource>();
        //pickupHitboxScript = pickupHitBox.GetComponent<pickupHitboxScript>();
        StartCoroutine(waitForMaze());
        camEffect.enabled = false;
        footSteps = GetComponentInChildren<FootSteps>();
        pickupControl.gameObject.SetActive(false);
        currentPickup = null;
    }
    IEnumerator waitForMaze()
    {
        while (MazeController.i == null) yield return null;
        while (!MazeController.i.isReady) yield return null;
        float i = 2;
        while (i > 0)
        {
            i -= Time.deltaTime;

            transform.position = MazeController.i.mazeData.startPos - footPos.localPosition;
            transform.eulerAngles = new Vector3(0, MazeController.i.mazeData.startRotation, 0);
            yield return null;
        }
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
        if (!pauseMenu.paused)
        {
            Vector3 playerMoveDelta = new Vector3(moveVector.x * 0.75f, 0, moveVector.z);
            if (playerController.isGrounded && input.player.jump.WasPerformedThisFrame()){
                jumpTimer = 0.4f;
                footSteps.PlayJumpStart();
                isJumping = true;
            }
            else if (!playerController.isGrounded && jumpTimer <= 0.0f) { playerMoveDelta.y -= 0.7f; }
            else if (playerController.isGrounded) { playerMoveDelta.y = 0; }

            if (jumpTimer != 0.0f)
            {
                playerMoveDelta.y += 0.5f;
                jumpTimer -= Time.deltaTime;
                if (jumpTimer < 0.0f) { jumpTimer = 0.0f; footSteps.PlayJumpEnd(); isJumping = false;};
            }

            Vector3 scaledVelocity;
            if(isSprinting) {                 
                scaledVelocity = playerMoveDelta * Time.deltaTime * (speedScalar + 2f + env.luciditySpeedModifier);
            } else
                scaledVelocity = playerMoveDelta * Time.deltaTime * (speedScalar + env.luciditySpeedModifier);



            playerController.Move(transform.TransformDirection(scaledVelocity));

            if (env.inNightmare)
            {
                camEffect.m_NoiseProfile = extremeShake;
                monsters = GameObject.FindGameObjectsWithTag("Monster");
            }
            else if (env.inNeutral)
            {
                camEffect.m_NoiseProfile = strongShake;
            }
            else if (env.inLucid)
            {
                camEffect.m_NoiseProfile = weakShake;
            }
        }
        
        if (input.player.pause.WasPerformedThisFrame())
        { 
            if (pauseMenu.paused == false)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.options.gameObject.SetActive(false);
                pauseMenu.paused = true;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.paused = false;
                pauseMenu.gameObject.SetActive(false);
            }
        }
    }
    public int solutionIndex;
    private void FixedUpdate()
    {
        if (!pauseMenu.paused)
        {
            // add mouse deltas to current camera rotation
            yRot += cameraVector.x * Time.deltaTime * xLookSensitivity;
            xRot -= cameraVector.y * Time.deltaTime * yLookSensitivity;
            xRot = Mathf.Clamp(xRot, -90f, 90f); // Clamp the x rotation of the camera to limit how far up/down the player can look

            // set the player/camera rotation equal to the the new x and y rotation values
            //mainCam.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
            mainCam.transform.localEulerAngles = new Vector3(xRot, 0, 0);  // smoother turns
            transform.rotation = Quaternion.Euler(0f, yRot, 0);

            // Handles Sprinting
            if (input.player.sprint.WasPerformedThisFrame()) //WasPressedThisFrame())
            {
                isSprinting = true;
                //speedScalar += 2.0f;
                //camEffect.m_FrequencyGain += 0.5f;
            }
            if (input.player.sprint.WasReleasedThisFrame())
            {
                isSprinting = false;
                //speedScalar -= 2.0f;
                //camEffect.m_FrequencyGain -= 0.5f;
            }

            // Picking up an object
            if (input.player.interact.WasPerformedThisFrame() && currentPickup == null)
            {
                RaycastHit pickupHit;
                Physics.SphereCast(mainCam.transform.position, 1.0f, mainCam.transform.forward, out pickupHit, 5f, pickupLayerMask);
                if (pickupHit.collider != null)
                {
                    currentPickup = pickupHit.collider.gameObject;
                    currentPickup.GetComponent<pickupObjScript>().Hold();
                    currentPickup.GetComponent<pickupObjScript>().PlayJingle();
                    pickupCooldown = 0.5f;
                }
                pickupControl.gameObject.SetActive(false);

            }
            // Dropping an object
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

            // Throwing an object
            if (input.player.throwObj.WasPerformedThisFrame())
            {
                if (currentPickup != null)
                {
                    currentPickup.GetComponent<Rigidbody>().isKinematic = false;
                    Vector3 thVec = mainCam.transform.forward * 1200;
                    thVec.y += xRot * 10;
                    currentPickup.GetComponent<pickupObjScript>().ThrowObj(thVec);
                    currentPickup = null;
                }
            }
        }
        
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
            footSteps.PlayLucidityPickup();
            lucidityBar.value += PlayerPrefs.GetFloat("pickupGain");
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.CompareTag("Fire"))
        {
            lucidityBar.value -= fireEnter;
            footSteps.PlayDamage();

        }
        if (other.gameObject.CompareTag("Arrow"))
        {
            lucidityBar.value -= arrowDamage;
            footSteps.PlayDamage();
        }
        if(other.gameObject.CompareTag("Log"))
        {
            lucidityBar.value -= logDamage;
            footSteps.PlayDamage();
        }
        if(other.gameObject.CompareTag("Spikes"))
        {
            lucidityBar.value -= spikeDamage;
            footSteps.PlayDamage();
        }

        if (other.gameObject.CompareTag("Water"))
        {
            SFX.enabled = true;
            foreach (GameObject x in monsters)
            {
                Debug.Log(transform.position);
                x.GetComponent<basicAI>().alert(transform.position);
            }
        }
        
        if (other.gameObject.CompareTag("Key") && currentPickup == null)
        {
            pickupControl.gameObject.SetActive(true);
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            SceneManager.LoadSceneAsync(5);
        }
    }

    private void OnTriggerStay(Collider other){
        if(other.gameObject.CompareTag("Fire"))
            lucidityBar.value -= fireStay;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            SFX.enabled = false;
        }

        if (other.gameObject.CompareTag("Key"))
        {
            pickupControl.gameObject.SetActive(false);
        }
    }

    public bool MovedThisFrame()
    {
        return input.player.move.WasPerformedThisFrame();
    }
}

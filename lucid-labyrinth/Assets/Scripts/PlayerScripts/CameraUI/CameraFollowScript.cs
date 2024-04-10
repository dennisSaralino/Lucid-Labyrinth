using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//public Transform player;

public class CameraFollowScript : MonoBehaviour
{
    public Transform playerCamPos;
    //private PlayerControls input = null;
    //private Vector2 cameraVector = Vector2.zero;

    [Range(1.0f, 20.0f)]
    public float xLookSensitivity = 4.5f;
    [Range(1.0f, 50.0f)]
    public float yLookSensitivity = 3.0f;

    

    //private void Awake()
    //{
    //    input = new PlayerControls();
    //}

    //private void OnEnable()
    //{
    //    input.Enable();
    //    input.player.camera.performed += OnCameraMovementPerformed;
    //    input.player.camera.canceled += OnCameraMovementCancelled;
    //}
    //private void OnDisable()
    //{
    //    input.Disable();
    //    input.player.camera.performed -= OnCameraMovementPerformed;
    //    input.player.camera.canceled -= OnCameraMovementCancelled;
    //}

    //private void OnCameraMovementPerformed(InputAction.CallbackContext value)
    //{
    //    cameraVector = value.ReadValue<Vector2>();
    //}

    //private void OnCameraMovementCancelled(InputAction.CallbackContext value)
    //{
    //    cameraVector = Vector2.zero;
    //}

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = playerCamPos.position;

        //transform.rotation = yRot * transform.rotation * xRot;
        //yRot = Quaternion.Euler(0f, rotVec.x * yLookSensitivity, 0f);
        //xRot = Quaternion.Euler(-rotVec.y * xLookSensitivity, 0f, 0f);
    }
}

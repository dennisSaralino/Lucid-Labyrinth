using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//public Transform player;

public class CameraFollowScript : MonoBehaviour
{
    public Transform playerCamPos;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = playerCamPos.position;
    }
}

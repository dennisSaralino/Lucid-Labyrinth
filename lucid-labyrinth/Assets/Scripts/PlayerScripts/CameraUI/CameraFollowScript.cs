using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public Transform player;

public class CameraFollowScript : MonoBehaviour
{
    public Transform playerCamPos;
    
    // Update is called once per frame
    void Update()
    {
        transform.position = playerCamPos.position;
        //transform.Rotate(Vector3.forward, -transform.rotation.z);
    }
}

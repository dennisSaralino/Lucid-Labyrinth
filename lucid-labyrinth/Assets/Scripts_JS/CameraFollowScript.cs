using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public Transform player;

public class CameraFollowScript : MonoBehaviour
{
    public Transform player;
    
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        transform.position = player.position + new Vector3(-4.0f, 4.0f, 0.0f); 
    }
}

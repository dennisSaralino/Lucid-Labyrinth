using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject arrow;
    public Vector3 position;
    Quaternion rotation;
    // Update is called once per frame

    void Start()
    {
        position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rotation = Quaternion.identity;
    }

    void OnEnable()
    {
        Instantiate(arrow, position,rotation);
        
    }

    void FixedUpdate()
    {

    }
}

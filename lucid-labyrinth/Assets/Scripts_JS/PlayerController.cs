using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    float speedScalar = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("left"))
        {
            transform.Rotate(0.0f, -2.0f, 0.0f);
        }
        if (Input.GetKey("right"))
        {
            transform.Rotate(0.0f, 2.0f, 0.0f);
        }
        if (Input.GetKey("w"))
        {
            transform.position = transform.position + (speedScalar * transform.forward);
        }
        if (Input.GetKey("a"))
        {
            transform.position += (speedScalar * -transform.right);
        }
        if (Input.GetKey("s"))
        {
            transform.position += (speedScalar * -transform.forward);
        }
        if (Input.GetKey("d"))
        {
            transform.position += (speedScalar * transform.right);
        }
    }

    

}

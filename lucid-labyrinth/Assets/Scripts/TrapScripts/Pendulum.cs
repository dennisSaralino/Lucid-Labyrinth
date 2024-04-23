using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pendulum : MonoBehaviour
{
    // Start is called before the first frame update
    
    
    public float swingSpeed = 10f;
    public float limit = 35f;

    
    void FixedUpdate(){

        float angle = limit * Mathf.Sin(Time.time * swingSpeed);
        transform.rotation = Quaternion.Euler(angle, 90, 0);
    }
}

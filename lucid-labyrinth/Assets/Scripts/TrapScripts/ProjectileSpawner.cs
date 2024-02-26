using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject arrow;
    private float arrowDelay = 0f;
    private float arrowMaxDelay = 3f;
    // Update is called once per frame

    void Start()
    {
        Instantiate(arrow);
        arrowDelay = 0f;
    }

    void FixedUpdate()
    {

        arrowDelay += Time.deltaTime;
        if(arrowDelay >= arrowMaxDelay)
        {
            arrowDelay = 0f;
            Instantiate(arrow);
        }
    }
}

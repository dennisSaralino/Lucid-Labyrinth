using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    public GameObject fire;

    // Start is called before the first frame update
    void Start()
    {
        //get random int between 1-4
        int spawnChange = Random.Range(1,2);

        // 1 in 3 chance to spawn fire
        if(spawnChange == 1)
        {
            Instantiate(fire, transform.position, Quaternion.identity, transform);
        }
    }

   
}

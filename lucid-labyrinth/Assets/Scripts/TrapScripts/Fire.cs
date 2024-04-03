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
        int spawnChange = Random.Range(1,4);

        // 1 in 4 chance to spawn fire
        if(spawnChange == 1){

            Instantiate(fire, transform.position, Quaternion.identity);
        }
    }

   
}

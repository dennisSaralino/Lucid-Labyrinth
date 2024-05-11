using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Trap
{
    public List<Vector3> spawnList = new List<Vector3>() { new Vector3(5.8f, 0.5f, 3.3f), new Vector3(3, 0.5f, 5.9f), new Vector3(5.84f, 0.5f, 8.87f), new Vector3(9, 0.5f, 6)};

    public GameObject fire;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        float spawnChange = Random.Range(1,2);
        

        // 50% chance to spawn fire
        if(spawnChange == 1)
        {
            Instantiate(fire, transform.position, Quaternion.identity);
            
        }
    }

   
}

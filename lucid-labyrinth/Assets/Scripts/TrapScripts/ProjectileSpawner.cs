using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject arrowPrefab;
    private float arrowSpeed = 1f;
    //Holds 4 walls around pressure plate
    public GameObject[] walls = new GameObject[4];
    //will have transforms of enabled walls, plus rotation for arrow
    public Dictionary<Transform, float> spawnList = new Dictionary<Transform, float>();
    

    void Start()
    {
        //start with four potential walls
        foreach(GameObject w in walls  ){
            //check for each active wall
            if(w.activeInHierarchy){

                //populate map with spawnpoint and rot of arrow
                spawnList.Add(w.transform, GetRotation(w));
            }
        }
    }


    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("Player")){
            Debug.Log("Pressure Plate stepped on");

            foreach(KeyValuePair<Transform, float> pair in spawnList){
                ShootArrow(pair);
            }
            
        }
    }

    void ShootArrow(KeyValuePair<Transform,float> pair){
        //create rotation with current transform of respective wall
        Quaternion arrowRot = Quaternion.Euler(pair.Value,0f, 0f);//still testing
        //Create Vector3 using transform of respective wall
        Vector3 arrowPos = pair.Key.transform.position;

        //spawned arrow
        GameObject newArrow = Instantiate(arrowPrefab, arrowPos,arrowRot);
        

        //arrow movement
        Rigidbody newArrowRigid = newArrow.GetComponent<Rigidbody>();
        newArrowRigid.velocity = transform.forward * arrowSpeed;
        
    }
    // Looks at tag of each wall to determine the rotation towards center
    // Float to generate a Quaternion
    float GetRotation(GameObject wallObject){
        // these return numbers are placeholders
        if (gameObject.CompareTag("UpWall"))
            return 0f;
        if (gameObject.CompareTag("DownWall"))
           return 90f;
        if (gameObject.CompareTag("LeftWall"))
            return 180f;
        if (gameObject.CompareTag("RightWall"))
            return 270f;
        
        return 0;
                
    }
    
}

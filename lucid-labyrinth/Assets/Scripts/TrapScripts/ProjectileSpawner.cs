using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject arrowPrefab;
    public float arrowSpeed = 3f;
    //Holds 4 walls around pressure plate
    public GameObject[] walls = new GameObject[4];
    //will have transforms of enabled walls, plus rotation for arrow
    public Dictionary<Vector3, float> spawnList = new Dictionary<Vector3, float>();
    

    void Start()
    {
        //start with four potential walls
        foreach(GameObject w in walls  ){
            //check for each active wall
            if(w.activeInHierarchy){

                //populate map with spawnpoint and rot of arrow
                spawnList.Add(w.transform.position, GetRotation(w));
            }
        }
    }


    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("Player")){
            Debug.Log("Pressure Plate stepped on");

            foreach(KeyValuePair<Vector3, float> pair in spawnList){
                ShootArrow(pair);
            }
            
        }
    }

    void ShootArrow(KeyValuePair<Vector3,float> pair){
        //create rotation with current transform of respective wall
        Quaternion arrowRot = Quaternion.Euler(0f,pair.Value, -90f);//still testing
        //Create Vector3 using transform of respective wall
        Vector3 arrowPos = pair.Key;

        //spawned arrow
        GameObject newArrow = Instantiate(arrowPrefab, arrowPos,arrowRot);
        
       

        //arrow movement
        Rigidbody newArrowRigid = newArrow.GetComponent<Rigidbody>();

        /*
        Vector3 arrowDirection = arrowRot * Vector3.forward;
        Vector3 arrowVelocity = arrowDirection * arrowSpeed;
        newArrowRigid.velocity = arrowVelocity;
        */
        Vector3 arrowVelocity;
        switch(pair.Value)
        {
            case 0f:
                arrowVelocity = new Vector3(arrowSpeed,0,0);
                newArrowRigid.velocity= arrowVelocity;
                //arrowVelocity.x = arrowSpeed;
                break;
            case 90f:
                arrowVelocity = new Vector3(0,0,-arrowSpeed);
                newArrowRigid.velocity= arrowVelocity;
                //arrowVelocity.x =  arrowSpeed;
                break;
            case 180f:
                arrowVelocity = new Vector3(-arrowSpeed,0,0);
                newArrowRigid.velocity= arrowVelocity;
               // arrowVelocity.z =  arrowSpeed;
                break;
            case 270f:
                arrowVelocity = new Vector3(0,0,arrowSpeed);
                newArrowRigid.velocity= arrowVelocity;
               // arrowVelocity.z = arrowSpeed;
                break;
            default:
            break;
            
        }
      //  newArrowRigid.velocity = new Vector3(arrowVelocity);
        
    }
    // Looks at tag of each wall to determine the rotation towards center
    // Float to generate a Quaternion
    float GetRotation(GameObject wall ){
        // these return numbers are placeholders
        if (wall.transform.parent.CompareTag("UpWall"))
            return 90f;
        if (wall.transform.parent.CompareTag("DownWall"))
           return 270f;
        if (wall.transform.parent.CompareTag("LeftWall"))
            return 0f;
        if (wall.transform.parent.CompareTag("RightWall"))
            return 180f;
        
        return 0;
                
    }
    
}

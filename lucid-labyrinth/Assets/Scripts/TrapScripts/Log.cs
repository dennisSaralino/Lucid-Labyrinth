using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Log : MonoBehaviour
{
    public GameObject logPrefab;
    public Transform logPosition;
    public float rotateSpeed = 5f;
    private GameObject logInstance;
     public GameObject[] walls = new GameObject[4];
    //will have transforms of enabled walls,
    public List<GameObject> spawnList = new List<GameObject>();
    private bool up, down, left, right = false;

    void Start()
    {
        //start with four potential walls
        foreach(GameObject w in walls  ){
            //check for each active wall
            if(w.activeInHierarchy){
                //add active wall to list
                spawnList.Add(w);
            }
        }

        // mark active walls
        foreach(GameObject w in spawnList){
            if(w.CompareTag("UpWall")){
                up = true;
            }
            if(w.CompareTag("DownWall")){
                down = true;
            }
            if(w.CompareTag("LeftWall")){
                left = true;
            }
            if(w.CompareTag("RightWall")){
                right = true;
            }
        }

        //log attached to two parallel walls
        if(up && down){
            Quaternion logRot = Quaternion.Euler(0,90,-90);
            Vector3 newVector3 = new Vector3(logPosition.position.x, logPosition.position.y, logPosition.position.z);
            Instantiate(logPrefab,newVector3, logRot);
            logInstance = Instantiate(logPrefab,newVector3, logRot);
        }

        else if(left && right){
            Quaternion logRot = Quaternion.Euler(0,0,-90);
            Vector3 newVector3 = new Vector3(logPosition.position.x, logPosition.position.y, logPosition.position.z);
            logInstance = Instantiate(logPrefab,newVector3, logRot);
            
        }
        
        // spin the log

    }

    void Update(){
        if(up && down){
        
        logInstance.transform.Rotate(0,0,rotateSpeed * Time.deltaTime);
        }
        if(left && right){
        
        logInstance.transform.Rotate(rotateSpeed * Time.deltaTime,0,0);
        }
    }
}

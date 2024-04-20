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
    public float rotateSpeed = 200f;
    private GameObject logInstance;
    
    
    // 
    // prefab not working out of the box 
    // code needs editing

    void Start()
    {
        // reference parent to reference other children
        Transform parentTransform = transform.parent;
        Transform wallTransform = parentTransform.Find("Wall");

   
        //Debug.Log($"wall rotation x: {wallTransform.rotation.x}");
        //Debug.Log($"wall rotation y: {wallTransform.rotation.y}");
        //Debug.Log($"wall rotation z: {wallTransform.rotation.z}");
        
        //log attached to two parallel walls
        //change rotation of log upon active parallel walls
        //***  inactive walls still seen and cause instantiation ***

        
        if(wallTransform.rotation.y == 0.7071068f){//<-- quaternion value for 90 degrees
            Debug.Log("in 90");
            Quaternion logRot = Quaternion.Euler(0,90,-90);
            Vector3 newVector3 = new Vector3(logPosition.position.x, logPosition.position.y + 1.0f, logPosition.position.z);
            logInstance = Instantiate(logPrefab,newVector3, logRot);
        }

        else if(parentTransform.rotation.y == 0f){
            Debug.Log("In 0");
            Quaternion logRot = Quaternion.Euler(0,0,-90);
            Vector3 newVector3 = new Vector3(logPosition.position.x, logPosition.position.y + 1.0f, logPosition.position.z);
            logInstance = Instantiate(logPrefab,newVector3, logRot);
            
        }
    }

    void Update(){

        //spin the log
        //works for both orientations
        if(logInstance != null)
        logInstance.transform.Rotate(0,rotateSpeed * Time.deltaTime,0);
      
    }
}

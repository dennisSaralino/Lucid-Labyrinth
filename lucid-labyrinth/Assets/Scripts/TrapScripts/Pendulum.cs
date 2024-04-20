using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PendulumPrefab;
    public Transform PendulumPosition;
    public float rotateSpeed = 200f;
    private GameObject PendulumInstance;
    
    private Quaternion targetRotation0;
    private Quaternion targetRotation90;


    // Code Is incomplete
    //using same logic as log script to set rotation  based on walls


    void Start()
    {
        targetRotation90 = Quaternion.Euler(0f,90f,0f); //xyz 
        targetRotation0 = Quaternion.Euler(0f,0f,0f);

        Debug.Log($"targetRotation0 y: {targetRotation0.y}");
        Debug.Log($"targetRotation90 y: {targetRotation90.y}");
        // compare with walls




        // reference parent to reference other children
        Transform parentTransform = transform.parent.transform;
        Transform wallTransform = parentTransform.Find("Wall");
        
        if (wallTransform != null)
        Debug.Log($"Parent Found: {wallTransform.rotation}");
        

   
        //Debug.Log($"wall rotation x: {wallTransform.rotation.x}");
        //Debug.Log($"wall rotation y: {wallTransform.rotation.y}");
        //Debug.Log($"wall rotation z: {wallTransform.rotation.z}");
        
        //log attached to two parallel walls
        //change rotation of log upon active parallel walls
        //*** if no wall, then must be deleted, not inactive ***

        
       // if(wallTransform.rotation.y == 1.0f){//<-- quaternion value for 90 degrees
        if(wallTransform.rotation.y == 0.7071068f){
            Debug.Log("in Pen 90");
            Quaternion logRot = Quaternion.Euler(0,90,-90);
            Vector3 newVector3 = new Vector3(PendulumPosition.position.x, PendulumPosition.position.y + 1.0f, PendulumPosition.position.z);
            PendulumInstance = Instantiate(PendulumPrefab,newVector3, logRot);
        }

        //else if(parentTransform.rotation.y == 0f){
            else if(wallTransform.rotation == targetRotation0){
            Debug.Log("In Pen 0");
            Quaternion logRot = Quaternion.Euler(0,0,-90);
            Vector3 newVector3 = new Vector3(PendulumPosition.position.x, PendulumPosition.position.y + 1.0f, PendulumPosition.position.z);
            PendulumInstance = Instantiate(PendulumPrefab,newVector3, logRot);
            
        }
    }

    void Update(){

        //spin the log
        //works for both orientations
        if(PendulumInstance != null)
        PendulumInstance.transform.Rotate(0,rotateSpeed * Time.deltaTime,0);
    
    }
}

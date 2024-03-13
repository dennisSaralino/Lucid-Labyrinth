using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject projectileSpawner;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter()
    {
        projectileSpawner.SetActive(true);
        Debug.Log("Plate stepped on");
    }

    private void OnTriggerExit()
    {
        projectileSpawner.SetActive(false);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject damageProjectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter()
    {
        damageProjectile.SetActive(true);
    }
}

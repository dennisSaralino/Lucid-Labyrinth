using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LucidityPickup_PW : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float spawnPosX = UnityEngine.Random.Range(-9f, 9f);
            float spawnPosZ = UnityEngine.Random.Range(-9f, 9f);
            this.transform.position = new Vector3(spawnPosX, 1.0f, spawnPosZ);
            //this.gameObject.SetActive(false);
        }
    }
}

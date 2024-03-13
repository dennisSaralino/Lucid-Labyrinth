using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LucidityPickup_PW : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float spawnPosX = UnityEngine.Random.Range(-4f, 4f);
            float spawnPosZ = UnityEngine.Random.Range(20f, 27f);
            //float spawnPosX = UnityEngine.Random.Range(1f, 28f);
            //float spawnPosZ = UnityEngine.Random.Range(1f, 28f);
            this.transform.position = new Vector3(spawnPosX, 2.0f, spawnPosZ);
            //this.gameObject.SetActive(false);
        }
    }
}

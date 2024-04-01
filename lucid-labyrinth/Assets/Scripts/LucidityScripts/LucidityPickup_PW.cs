using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LucidityPickup_PW : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float spawnPosX = UnityEngine.Random.Range(-8f, 8f);
            float spawnPosZ = UnityEngine.Random.Range(-8f, 8f);
            this.transform.position = new Vector3(spawnPosX, 0.0f, spawnPosZ);
        }
    }
}

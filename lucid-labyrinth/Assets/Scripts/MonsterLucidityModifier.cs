using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLucidityModifier : MonoBehaviour
{
    public LucidityBar lucidity;
    public float modifier = 2;

    private void Start()
    {
        lucidity = GameObject.FindGameObjectWithTag("LucidityBar").GetComponent<LucidityBar>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lucidity.monsterModifier += modifier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lucidity.monsterModifier -= modifier;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    protected AudioSource audioSource;
    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}

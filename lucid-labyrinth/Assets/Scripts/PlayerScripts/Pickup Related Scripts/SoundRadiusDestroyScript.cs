using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRadiusDestroyScript : MonoBehaviour
{
    GameObject soundRadius;
    // Start is called before the first frame update
    void Start()
    {
        soundRadius = GameObject.FindGameObjectWithTag("Sound");
        if (soundRadius != null) { Destroy(soundRadius); }
        Destroy(this.gameObject, 30f);
    }
}

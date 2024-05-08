using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.ParticleSystemJobs;

public class AwakenSeeker : MonoBehaviour
{
    public Light[] lightEffects;
    public ParticleSystem[] partEffects;
    public float startingLight = 0.0f;
    public float awakeLight = 3.8f;

    private AudioSource audioSource;
    public AudioClip monsterAlerted;

    public GameObject head;
    public float viewRange = 12f;
    public GameObject player;
    public GameObject soundRadius;
    private GameObject monster;
    private RaycastHit[] sawPlayer = new RaycastHit[10];
    private RaycastHit playerCheck;
    private int layerMask = 384;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        audioSource = GetComponent<AudioSource>();

        foreach(Light lightEffect in lightEffects)
            lightEffect.intensity = startingLight;
        foreach(ParticleSystem partEffect in partEffects) 
            partEffect.Stop();
    }

    void FixedUpdate()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        if (monster != null)
        {
            Physics.BoxCastNonAlloc(head.transform.position, new Vector3(5f, 5f, 0.01f), head.transform.forward, sawPlayer, Quaternion.identity, viewRange, layerMask);
            foreach (RaycastHit x in sawPlayer)
            {
                if (x.collider != null)
                {
                    if (x.collider.gameObject.CompareTag("Player"))
                    {
                        Physics.Linecast(head.transform.position, player.gameObject.transform.position, out playerCheck, layerMask);
                        if (!playerCheck.collider.gameObject.CompareTag("Player"))
                        {
                            sawPlayer = new RaycastHit[10];
                            break;
                        }
                        else if (x.collider.gameObject.CompareTag("Player"))
                        {
                            // sees player and awakens
                            foreach (Light lightEffect in lightEffects)
                                lightEffect.intensity = awakeLight;
                            foreach (ParticleSystem partEffect in partEffects)
                                partEffect.Play();

                            if (audioSource != null)
                                audioSource.PlayOneShot(monsterAlerted);

                            // notify main monster
                            monster.GetComponent<basicAI>().alert(x.collider.gameObject.transform.position);
                            Instantiate(soundRadius, x.collider.gameObject.transform.position, Quaternion.identity);
                            Debug.DrawRay(head.transform.position + new Vector3(0f, 0f, 2.0f), head.transform.forward * 60, Color.red, 6.0f);
                            sawPlayer = new RaycastHit[10];
                            break;
                        }
                    }
                }
            }
        }
    }
}

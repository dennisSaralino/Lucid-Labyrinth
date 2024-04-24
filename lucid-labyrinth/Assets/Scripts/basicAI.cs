using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicAI : MonoBehaviour
{
    Transform player;
    NavMeshAgent nav;

    private bool isDistracted = false;
    private float seenTimer = 0f;
    Transform soundPos;
    private int playerLayerMask = 1 << 7;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit sawPlayer;
        if (!isDistracted && seenTimer <= 0f) {
            Debug.DrawRay(transform.position + new Vector3(0f, 1.0f, 1.0f), transform.forward, Color.blue, 1.0f);
            Physics.SphereCast(transform.position + new Vector3(0f, 1.0f, 1.0f), 10f, transform.forward, out sawPlayer, 30f);
            if (sawPlayer.collider != null)
            {
                if (sawPlayer.collider.gameObject.tag == "Player")
                {
                    seenTimer = 6.0f;
                    Debug.DrawRay(transform.position + new Vector3(0f, 1.0f, 1.0f), transform.forward, Color.red, 6.0f);
                }
                else if (sawPlayer.collider.gameObject.tag != "Player")
                {
                    nav.ResetPath();
                    Debug.Log("Reset");
                }
            }
        }
        else if (seenTimer > 0.0f)
        {
            seenTimer -= Time.deltaTime;
            transform.LookAt(player.position);
            nav.SetDestination(player.position);
            if (seenTimer <= 0.0f)
            {
                seenTimer = 0.0f;
            }
        }
        else if (isDistracted)
        {
            nav.destination = soundPos.position;
            if (transform.position == soundPos.position)
            {
                isDistracted = false;
            }
        }
        
    }

    public void alert(Transform position)
    {
        soundPos = position;
        isDistracted = true;
    }
}

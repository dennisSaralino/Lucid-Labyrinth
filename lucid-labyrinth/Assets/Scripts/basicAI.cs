using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicAI : MonoBehaviour
{
    Transform player;
    NavMeshAgent nav;

    public GameObject head;
    private bool isDistracted = false;
    private bool rightTurn = true;
    private bool focused = false;
    private float seenTimer = 0f;
    private float yTurn = 0f;
    //private float netTurn = 0f;
    Transform soundPos;
    private int layerMask = 1 << 7;
    private RaycastHit sawPlayer;
    static private float turnSpeed = 15;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = 0.75f;
        yTurn = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!focused)
        {
            if (rightTurn)
            {
                yTurn += Time.deltaTime * turnSpeed;
                if (yTurn >= 60f)
                {
                    rightTurn = false;
                }
                yTurn = Mathf.Clamp(yTurn, -60f, 60f);
                head.transform.rotation = Quaternion.Euler(0, yTurn, 0);
            }
            else
            {
                yTurn -= Time.deltaTime * turnSpeed;
                if (yTurn <= -60f)
                {
                    rightTurn = true;
                }
                yTurn = Mathf.Clamp(yTurn, -60f, 60f);
                head.transform.rotation = Quaternion.Euler(0, yTurn, 0);
            }
        }
        if (!isDistracted && seenTimer <= 0f) {
            Debug.DrawRay(head.transform.position , head.transform.forward * 60, Color.blue, 0.2f);
            Physics.SphereCast(head.transform.position, 5f, head.transform.forward, out sawPlayer, layerMask);
            if (sawPlayer.collider != null)
            {
                //Debug.Log("saw something");            
                if (sawPlayer.collider.gameObject.CompareTag("Player"))
                {
                    Physics.Linecast(head.transform.position, player.position, out sawPlayer, layerMask);
                    Debug.DrawRay(head.transform.position, -(head.transform.position - player.position) * 60, Color.green, 2.0f);
                    if (!sawPlayer.collider.gameObject.CompareTag("Player"))
                    {
                        nav.ResetPath();
                        Debug.Log("Reset");
                        return;
                    }
                    else if (sawPlayer.collider.gameObject.CompareTag("Player"))
                    {
                        seenTimer = 6.0f;
                        focused = true;
                        Debug.Log("Saw player");
                        Debug.DrawRay(head.transform.position + new Vector3(0f, 0f, 2.0f), head.transform.forward * 60, Color.red, 6.0f);
                    }
                }
            }
        }
        else if (seenTimer > 0.0f)
        {
            seenTimer -= Time.deltaTime;
            //transform.LookAt(player.position);
            head.transform.LookAt(player.position);
            transform.rotation = Quaternion.Euler(0, head.transform.rotation.eulerAngles.y, 0);
            nav.SetDestination(player.position);
            if (seenTimer <= 0.0f)
            {
                seenTimer = 0.0f;
                nav.ResetPath();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicAI : MonoBehaviour
{
    Transform player;
    NavMeshAgent nav;

    public GameObject head;
    public float viewRange = 30f;
    private bool isDistracted = false;
    private bool rightTurn = true;
    private bool focused = false;
    private float seenTimer = 0f;
    private float yTurn = 0f;
    private float netTurn = 0f;
    Transform soundPos;
    private int layerMask = 384;
    private RaycastHit[] sawPlayer = new RaycastHit[10];
    private RaycastHit playerCheck;
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
                netTurn += Time.deltaTime * turnSpeed;
                if (netTurn >= 60f)
                {
                    rightTurn = false;
                }
                //yTurn = Mathf.Clamp(yTurn, -60f, 60f);
                head.transform.rotation = Quaternion.Euler(0, yTurn, 0);
            }
            else
            {
                yTurn -= Time.deltaTime * turnSpeed;
                netTurn -= Time.deltaTime * turnSpeed;
                if (netTurn <= -60f)
                {
                    rightTurn = true;
                }
                //yTurn = Mathf.Clamp(yTurn, -60f, 60f);
                head.transform.rotation = Quaternion.Euler(0, yTurn, 0);
            }
        }
        if (!isDistracted && seenTimer <= 0f) {
            Debug.DrawRay(head.transform.position , head.transform.forward * 60, Color.blue, 0.2f);
            Physics.SphereCastNonAlloc(head.transform.position, 5f, head.transform.forward, sawPlayer, viewRange, layerMask);
            foreach (RaycastHit x in sawPlayer)
            {
                if (x.collider != null)
                {
                    //Debug.Log("saw something");            
                    if (x.collider.gameObject.CompareTag("Player"))
                    {
                        Physics.Linecast(head.transform.position, player.position, out playerCheck, layerMask);
                        Debug.DrawRay(head.transform.position, -(head.transform.position - player.position) * 60, Color.green, 2.0f);
                        if (!playerCheck.collider.gameObject.CompareTag("Player"))
                        {
                            nav.ResetPath();
                            Debug.Log("Reset");
                            sawPlayer = new RaycastHit[10];
                            break;
                        }
                        else if (x.collider.gameObject.CompareTag("Player"))
                        {
                            seenTimer = 6.0f;
                            focused = true;
                            Debug.Log("Saw player");
                            Debug.DrawRay(head.transform.position + new Vector3(0f, 0f, 2.0f), head.transform.forward * 60, Color.red, 6.0f);
                            sawPlayer = new RaycastHit[10];
                            break;
                        }
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
                yTurn = transform.rotation.eulerAngles.y;
                seenTimer = 0.0f;
                nav.ResetPath();
                focused = false;
            }
        }
        else if (isDistracted)
        {
            nav.SetDestination(soundPos.position);
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

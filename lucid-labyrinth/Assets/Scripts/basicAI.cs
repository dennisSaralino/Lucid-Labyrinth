using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class basicAI : MonoBehaviour
{
    Transform player;
    NavMeshAgent nav;

    public GameObject head;
    public float viewRange = 30f;
    public bool debugging = false;
    public Vector3 debugDestination = Vector3.zero;
    private bool isDistracted = false;
    private bool rightTurn = true;
    private bool focused = false;
    private bool hasDestination = false;
    private bool atSound = false;
    private float seenTimer = 0f;
    private float distractedTimer = 0f;
    private float yTurn = 0f;
    private float netTurn = 0f;
    private Vector3 soundPos;
    private int layerMask = 384;
    private RaycastHit[] sawPlayer = new RaycastHit[10];
    private RaycastHit playerCheck;
    static private float turnSpeed = 15;
    private List<Vector3> wanderPoints;
    private Vector3 currentWanderDestination;
    private int randIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = 1.5f;
        yTurn = head.transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!debugging)
        {
            if (!hasDestination)
            {
                wanderPoints = MazeController.i.mazeData.getEnemySpawnPoints();
                randIndex = Random.Range(0, wanderPoints.Count);
                currentWanderDestination = wanderPoints[randIndex];
                nav.SetDestination(currentWanderDestination);
                hasDestination = true;
            }
            if (hasDestination && nav.destination.x == currentWanderDestination.x && nav.destination.z == currentWanderDestination.z)
            {
                if (transform.position == nav.destination)
                {
                    nav.ResetPath();
                    hasDestination = false;
                }
            }
        }
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
                head.transform.rotation = Quaternion.Euler(0, yTurn, 0);
            }
        }
        if (!isDistracted && seenTimer <= 0f)
        {
            Debug.DrawRay(head.transform.position, head.transform.right * 60, Color.blue, 0.2f);
            Physics.SphereCastNonAlloc(head.transform.position, 5f, head.transform.right, sawPlayer, viewRange, layerMask);
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
                            //Debug.Log("Reset");
                            sawPlayer = new RaycastHit[10];
                            break;
                        }
                        else if (x.collider.gameObject.CompareTag("Player"))
                        {
                            seenTimer = 6.0f;
                            hasDestination = true;
                            focused = true;
                            //Debug.Log("Saw player");
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
            //head.transform.LookAt(player.position);
            transform.LookAt(player.position);
            head.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90f, 0);
            nav.SetDestination(player.position);
            if (seenTimer <= 0.0f)
            {
                yTurn = head.transform.rotation.eulerAngles.y;
                netTurn = 0f;
                seenTimer = 0.0f;
                nav.ResetPath();
                focused = false;
                hasDestination = false;
            }
        }
        else if (isDistracted)
        {
            nav.ResetPath();
            nav.SetDestination(soundPos);
            hasDestination = true;
            focused = true;
            head.transform.LookAt(soundPos);
            if (distractedTimer <= 0f && atSound)
            {
                Debug.Log("reached destination");
                distractedTimer = 0f;
                isDistracted = false;
                hasDestination = false;
                focused = false;
                atSound = false;
            }
            else if (distractedTimer > 0f)
            {
                distractedTimer -= Time.deltaTime;
            }
        }
        if (Input.GetKeyDown("m"))
        {
            nav.ResetPath();
            isDistracted = false;
            focused = false;
            atSound = false;
            nav.SetDestination(debugDestination);
            hasDestination = true;
        }
    }

    public void alert(Vector3 position)
    {
        Physics.Linecast(head.transform.position, player.position, out playerCheck, layerMask);
        if (!playerCheck.collider.gameObject.CompareTag("Player"))
        {
            seenTimer = 0f;
            soundPos = position;
            isDistracted = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sound"))
        {
            atSound = true;
            distractedTimer = 0.65f;
            Destroy(other.gameObject);
        }
    }
}

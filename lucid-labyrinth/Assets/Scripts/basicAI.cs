using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class basicAI : MonoBehaviour
{
    PlayerController player;
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
    public bool test = true;
    private float seenTimer = 0f;
    private float distractedTimer = 0f;
    private float idleTimer = 10f;

    private float testTimer = 0f;

    private float yTurn = 0f;
    private float netTurn = 0f;
    private Vector3 soundPos;
    private int layerMask = 384;
    private RaycastHit[] sawPlayer = new RaycastHit[10];
    private RaycastHit playerCheck;
    static private float turnSpeed = 15;
    private List<Vector3> wanderPoints;
    private Vector3 currentWanderDestination;
    private int wanderIndex = 1;
    private Vector3 currPos = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = 1.5f;
        yTurn = head.transform.rotation.eulerAngles.y + 90;
        currPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
        {
            if (currPos == transform.position)
            {
                nav.ResetPath();
                hasDestination = false;
            }
            idleTimer = 10f;
            currPos = transform.position;
        }
        if (!focused && !debugging)
        {
            if (rightTurn)
            {
                yTurn += Time.deltaTime * turnSpeed;
                netTurn += Time.deltaTime * turnSpeed;
                if (netTurn >= 60f)
                {
                    rightTurn = false;
                }
                head.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90 + yTurn, 0);
            }
            else
            {
                yTurn -= Time.deltaTime * turnSpeed;
                netTurn -= Time.deltaTime * turnSpeed;
                if (netTurn <= -60f)
                {
                    rightTurn = true;
                }
                head.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90 + yTurn, 0);
            }
        }
        else if (debugging) { head.transform.rotation = transform.rotation; }

        if (!hasDestination)
        {
            wanderPoints = MazeController.i.mazeData.getEnemySpawnPoints();
            currentWanderDestination = wanderPoints[wanderIndex];
            nav.SetDestination(currentWanderDestination);
            hasDestination = true;
        }
        else if (hasDestination && !focused && !isDistracted)
        {
            if (transform.position.x == nav.destination.x && transform.position.z == nav.destination.z)
            {
                nav.ResetPath();
                hasDestination = false;
                if (wanderIndex == 1) { --wanderIndex; }
                else if (wanderIndex == 0) { ++wanderIndex; }
            }
        }

        if (!isDistracted && seenTimer <= 0f)
        {
            Debug.DrawRay(head.transform.position, head.transform.right * 60, Color.blue, 0.2f);
            Physics.BoxCastNonAlloc(head.transform.position, new Vector3(5f, 5f, 0.01f), head.transform.forward, sawPlayer, Quaternion.identity, viewRange, layerMask);
            foreach (RaycastHit x in sawPlayer)
            {
                if (x.collider != null)
                {
                    if (x.collider.gameObject.CompareTag("Player"))
                    {
                        Physics.Linecast(head.transform.position, player.gameObject.transform.position, out playerCheck, layerMask);
                        Debug.DrawRay(head.transform.position, -(head.transform.position - player.gameObject.transform.position) * 60, Color.green, 2.0f);
                        if (!playerCheck.collider.gameObject.CompareTag("Player"))
                        {
                            nav.ResetPath();
                            sawPlayer = new RaycastHit[10];
                            break;
                        }
                        else if (x.collider.gameObject.CompareTag("Player"))
                        {
                            nav.ResetPath();
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
        if (seenTimer > 0.0f)
        {
            seenTimer -= Time.deltaTime;
            //head.transform.LookAt(player.gameObject.transform.position);
            //head.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90f, 0);
            //nav.SetDestination(GameObject.FindGameObjectWithTag("Test").transform.position);
            nav.ResetPath();
            nav.SetDestination(player.groundPos());
            if (seenTimer <= 0.0f)
            {
                Debug.Log("seenTimer over");
                netTurn = 0f;
                seenTimer = 0.0f;
                nav.ResetPath();
                focused = false;
                hasDestination = false;
            }
        }
        else if (isDistracted)
        {
            Debug.Log("Distracted");
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
            test = !test;
        }
    }

    public void alert(Vector3 position)
    {
        if (seenTimer <= 0f)
        {
            seenTimer = 0f;
            soundPos = position;
            isDistracted = true;
            nav.ResetPath();
            hasDestination = true;
            focused = true;
            nav.SetDestination(soundPos);
            head.transform.LookAt(soundPos);
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

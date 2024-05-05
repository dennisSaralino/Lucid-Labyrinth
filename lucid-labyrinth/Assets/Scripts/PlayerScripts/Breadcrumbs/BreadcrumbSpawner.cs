using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BreadcrumbSpawner : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject breadcrumb;
    public EnvironmentController lucidityState;
    public int maxCrumbs;
    private int numOfCrumbs = 0;
    private bool beginSpawning = false;
    private bool isGrounded = false;
    private bool spawnLeft = true;
    private bool spawnRight = false;
    public float timer;
    private float resetTimer;
    private RaycastHit hit;

    private void Awake()
    {
        resetTimer = timer;
        beginSpawning = true;
    }

    private void Update()
    {
        if (beginSpawning == true)
        {
            if (Physics.Raycast(playerTransform.transform.position, new Vector3(0, -1, 0), out hit, 1.1f))
            {
                isGrounded = true;
            }
            else { isGrounded = false; }

            if (isGrounded == true)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    numOfCrumbs++;
                    if (numOfCrumbs > maxCrumbs)
                    {
                        Destroy(gameObject.transform.GetChild(0).gameObject);
                        numOfCrumbs--;
                    }

                    Vector3 playerHorizontals;
                    Vector3 playerActual = new Vector3(playerTransform.transform.position.x, playerTransform.transform.position.y - 1f, playerTransform.transform.position.z);
                    
                    if (spawnLeft) { playerHorizontals = new Vector3(playerTransform.transform.position.x - 0.5f, playerTransform.transform.position.y - 1f, playerTransform.transform.position.z); }
                    else { playerHorizontals = new Vector3(playerTransform.transform.position.x + 0.5f, playerTransform.transform.position.y - 1f, playerTransform.transform.position.z); }
                    
                    GameObject newBreadcrumb = Instantiate(breadcrumb, playerHorizontals, Quaternion.identity);
                    newBreadcrumb.transform.RotateAround(playerActual, new Vector3(0, 1, 0), playerTransform.rotation.eulerAngles.y);

                    if (lucidityState.lucidLevel3) { newBreadcrumb.transform.GetChild(0).GameObject().SetActive(true); }

                    if (lucidityState.inNightmare) { newBreadcrumb.GetComponent<MovingBreadcrumbs>().enabled = true; }
                    
                    if (spawnLeft == true)
                    {
                        //left foot
                        newBreadcrumb.transform.localScale = new Vector3(0.075f, 1f, -0.075f);
                        spawnLeft = !spawnLeft;
                        spawnRight = !spawnRight;
                    }
                    else
                    {
                        //right foot
                        newBreadcrumb.transform.localScale = new Vector3(-0.075f, 1f, -0.075f);
                        spawnRight = !spawnRight;
                        spawnLeft = !spawnLeft;
                    }
                    newBreadcrumb.transform.SetParent(gameObject.transform);

                    if (numOfCrumbs > maxCrumbs)
                    {
                        Destroy(gameObject.transform.GetChild(0).gameObject);
                        numOfCrumbs--;
                    }

                    timer = resetTimer;
                }
            }
        }
    }
}
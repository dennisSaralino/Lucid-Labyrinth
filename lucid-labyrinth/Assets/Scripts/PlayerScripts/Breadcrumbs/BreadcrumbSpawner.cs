using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BreadcrumbSpawner : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject breadcrumb;
    private int numOfCrumbs = 0;

    private void Start()
    {
        StartCoroutine(spawnBreadcrumb());
    }

    IEnumerator spawnBreadcrumb()
    {
        while (true)
        {
            Vector3 playerHorizontals = new Vector3(playerTransform.transform.position.x, playerTransform.transform.position.y - 1f, playerTransform.transform.position.z);
            GameObject newBreadcrumb = Instantiate(breadcrumb, playerHorizontals, playerTransform.rotation);
            numOfCrumbs++;
            if (numOfCrumbs % 2 == 1)
            {
                newBreadcrumb.transform.localScale = new Vector3(0.075f, 1f, -0.075f);

            } else
            {
                newBreadcrumb.transform.localScale = new Vector3(-0.075f, 1f, -0.075f);
            }
            newBreadcrumb.transform.SetParent(gameObject.transform);
            yield return new WaitForSeconds(1);
        }
    }
}
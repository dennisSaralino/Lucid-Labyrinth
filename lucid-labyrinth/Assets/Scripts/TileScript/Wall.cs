using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Wall : MonoBehaviour
{
    List<Collider> colliderList;
    private void Awake()
    {
        colliderList = gameObject.GetComponentsInChildren<Collider>().ToList();
    }
    private void OnBecameInvisible()
    {

        colliderList.ForEach(x =>
        {
            x.enabled = false;
        });
    }
    private void OnBecameVisible()
    {
        colliderList.ForEach(x =>
        {
            x.enabled = true;
        });
    }
}

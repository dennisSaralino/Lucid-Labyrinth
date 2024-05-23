using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using System.Linq;
public class navigationBaker : MonoBehaviour
{
    public NavMeshSurface surface;
    public bool active;
    public static navigationBaker baker;
    public static List<GameObject> activeAfterBakedOb;
    public void Awake()
    {
        if (baker == null) baker = this;
        else Destroy(this.gameObject);
    }
    public void bakeMap()
    {
        surface.BuildNavMesh();
    }
    public IEnumerator bakeMap(List<NavMeshSurface> l)
    {
        yield return StartCoroutine(bakeMapSlowly(l));
    }
    IEnumerator bakeMapSlowly(List<NavMeshSurface> l)
    {
        GameObject navParent = GameObject.Find("ALLSURFACES");
        l.ForEach(i =>
        {
            i.transform.SetParent(navParent.transform);
        });
        yield return null;
        navParent.GetComponent<NavMeshSurface>().BuildNavMesh();
        //if (!active) yield break;
        //int j = 20;
        //foreach (NavMeshSurface i in l)
        //{
        //    if (j == 0)
        //    {
        //        yield return null;
        //        j = 20;
        //    }
        //    i.BuildNavMesh();
        //    j--;   
        //}

        yield return null;
        if (activeAfterBakedOb != null && activeAfterBakedOb.Count != 0)
            activeAfterBakedOb.ForEach(x => x.SetActive(true));

    }
}
